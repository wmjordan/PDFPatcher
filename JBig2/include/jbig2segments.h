// Copyright 2006 Google Inc. All Rights Reserved.
// Author: agl@imperialviolet.org (Adam Langley)
//
// Copyright (C) 2006 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#ifndef THIRD_PARTY_JBIG2ENC_JBIG2SEGMENTS_H__
#define THIRD_PARTY_JBIG2ENC_JBIG2SEGMENTS_H__

#include <vector>
#ifdef WIN32
#include <winsock2.h>
#else
#include <netinet/in.h>
#endif

// -----------------------------------------------------------------------------
// See comments in jbig2structs.h about the bit packing in this structure.
// -----------------------------------------------------------------------------
#if defined(WIN32)
#pragma pack(1)
#endif
struct jbig2_segment {
  u32 number;
#ifndef _BIG_ENDIAN
  unsigned char type : 6;
  unsigned char page_assoc_size : 1;
  unsigned char deferred_non_retain : 1;
#else
  unsigned char deferred_non_retain : 1;
  unsigned char page_assoc_size : 1;
  unsigned char type : 6;
#endif

#ifndef _BIG_ENDIAN
  unsigned char retain_bits : 5;
  unsigned char segment_count : 3;
#else
  unsigned char segment_count : 3;
  unsigned char retain_bits : 5;
#endif
}
#if defined(WIN32)
;
#pragma pack()
#else
__attribute__((packed));
#endif
;

// -----------------------------------------------------------------------------
// This structure represents a JBIG2 segment header because they have too many
// variable length fields (number of referred to segments, page length etc).
// You should access and set the members directly. Endian swapping is carried
// out internally.
// -----------------------------------------------------------------------------
struct Segment {
  unsigned number;  // segment number
  int type;  // segment type (see enum in jbig2structs.h)
  int deferred_non_retain;  // see JBIG2 spec
  int retain_bits;
  std::vector<unsigned> referred_to;  // list of segment numbers referred to
  unsigned page;  // page number
  unsigned len;   // length of trailing data

  Segment()
      : number(0),
        type(0),
        deferred_non_retain(0),
        retain_bits(0),
        page(0),
        len(0) {}

  // ---------------------------------------------------------------------------
  // Return the size of the segment reference for this segment. Segments can
  // only refer to previous segments, so the bits needed is determined by the
  // number of this segment. (7.2.5)
  // ---------------------------------------------------------------------------
  unsigned reference_size() const {
    int refsize;
    if (number <= 256) {
      refsize = 1;
    } else if (number <= 65536) {
      refsize = 2;
    } else {
      refsize = 4;
    }

    return refsize;
  }

  // ---------------------------------------------------------------------------
  // Return the size of the segment page association field for this segment.
  // (7.2.6)
  // ---------------------------------------------------------------------------
  unsigned page_size() const {
      return page <= 255 ? 1 : 4;
  }

  // ---------------------------------------------------------------------------
  // Return the number of bytes that this segment header will take up
  // ---------------------------------------------------------------------------
  unsigned size() const {
    const int refsize = reference_size();
    const int pagesize = page_size();

    return sizeof(struct jbig2_segment) + refsize * referred_to.size() +
           pagesize + sizeof(u32);
  }

  // ---------------------------------------------------------------------------
  // Serialise this segment header into the memory pointed to by buf, which
  // must be at least long enough to contain it (e.g. size() bytes)
  // ---------------------------------------------------------------------------
  void write(u8 *buf) {
    struct jbig2_segment s;
    memset(&s, 0, sizeof(s));
#define F(x) s.x = x;
    s.number = htonl(number);
    s.type = type;
    s.deferred_non_retain = deferred_non_retain;
    s.retain_bits = retain_bits;
#undef F
    s.segment_count = referred_to.size();

    const int pagesize = page_size();
    const int refsize = reference_size();
    if (pagesize == 4) s.page_assoc_size = 1;

    unsigned j = 0;

    memcpy(buf, &s, sizeof(s));
    j += sizeof(s);
#define APPEND(type, val) type __i; __i = val; \
    memcpy(&buf[j], &__i, sizeof(type)); \
    j += sizeof(type)

    for (std::vector<unsigned>::const_iterator i = referred_to.begin();
         i != referred_to.end(); ++i) {
      if (refsize == 4) {
        APPEND(u32, htonl(*i));
      } else if (refsize == 2) {
        APPEND(u16, htons(*i));
      } else {
        APPEND(u8, *i);
      }
    }

    if (pagesize == 4) {
      APPEND(u32, htonl(page));
    } else {
      APPEND(u8, page);
    }

    APPEND(u32, htonl(len));

    if (j != size()) abort();
  }
};

#endif  // THIRD_PARTY_JBIG2ENC_JBIG2SEGMENTS_H__
