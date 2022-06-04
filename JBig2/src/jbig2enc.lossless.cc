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

#include <map>
#include <vector>
#include <algorithm>

#include <stdio.h>
#include <string.h>

#include <pix.h>

#include <math.h>
#if defined(sun)
#include <sys/types.h>
#else
#include <stdint.h>
#endif

#define u64 uint64_t
#define u32 uint32_t
#define u16 uint16_t
#define u8  uint8_t

#include "jbig2arith.h"
#include "jbig2structs.h"
#include "jbig2segments.h"

#define SEGMENT(x) x.write(ret + offset); offset += x.size();


static const uint32_t rmask32[] = {0x0,
    0x00000001, 0x00000003, 0x00000007, 0x0000000f,
    0x0000001f, 0x0000003f, 0x0000007f, 0x000000ff,
    0x000001ff, 0x000003ff, 0x000007ff, 0x00000fff,
    0x00001fff, 0x00003fff, 0x00007fff, 0x0000ffff,
    0x0001ffff, 0x0003ffff, 0x0007ffff, 0x000fffff,
    0x001fffff, 0x003fffff, 0x007fffff, 0x00ffffff,
    0x01ffffff, 0x03ffffff, 0x07ffffff, 0x0fffffff,
    0x1fffffff, 0x3fffffff, 0x7fffffff, 0xffffffff};

int32_t
pixSetPadBits(PIX     *pix,
              int32_t  val)
{
int32_t    i, w, h, d, wpl, endbits, fullwords;
uint32_t   mask;
uint32_t  *data, *pword;

	w = pix->w;
	h = pix->h;
	d = 1;

	data = pix->data;
    wpl = (w + 31) /32;
    endbits = 32 - ((w * d) % 32);
    if (endbits == 32)  /* no partial word */
        return 0;
    fullwords = w * d / 32;

    mask = rmask32[endbits];
    if (val == 0)
        mask = ~mask;

    for (i = 0; i < h; i++) {
        pword = data + i * wpl + fullwords;
        if (val == 0) /* clear */
            *pword = *pword & mask;
        else  /* set */
            *pword = *pword | mask;
    }

    return 0;
}


// see comments in .h file
u8 *
jbig2_encode_generic(struct Pix *const bw, const bool full_headers, const int xres,
                     const int yres, const bool duplicate_line_removal,
                     int *const length) {
  int segnum = 0;

  if (!bw) return NULL;
  pixSetPadBits(bw, 0);

  struct jbig2_file_header header;
  if (full_headers) {
    memset(&header, 0, sizeof(header));
    header.n_pages = htonl(1);
    header.organisation_type = 1;
    memcpy(&header.id, JBIG2_FILE_MAGIC, 8);
  }

  // setup compression
  struct jbig2enc_ctx ctx;
  jbig2enc_init(&ctx);

  Segment seg, seg2, endseg;
  jbig2_page_info pageinfo;
  memset(&pageinfo, 0, sizeof(pageinfo));
  jbig2_generic_region genreg;
  memset(&genreg, 0, sizeof(genreg));

  seg.number = segnum;
  segnum++;
  seg.type = segment_page_information;
  seg.page = 1;
  seg.len = sizeof(struct jbig2_page_info);
  pageinfo.width = htonl(bw->w);
  pageinfo.height = htonl(bw->h);
  pageinfo.xres = htonl(xres ? xres : bw->xres);
  pageinfo.yres = htonl(yres ? yres : bw->yres);
  pageinfo.is_lossless = 1;

#ifdef SURPRISE_MAP
  dprintf(3, "P5\n%d %d 255\n", bw->w, bw->h);
#endif

  jbig2enc_bitimage(&ctx, (u8 *) bw->data, bw->w, bw->h, duplicate_line_removal);
  jbig2enc_final(&ctx);
  const int datasize = jbig2enc_datasize(&ctx);

  seg2.number = segnum;
  segnum++;
  seg2.type = segment_imm_generic_region;
  seg2.page = 1;
  seg2.len = sizeof(genreg) + datasize;

  endseg.number = segnum;
  segnum++;
  endseg.page = 1;

  genreg.width = htonl(bw->w);
  genreg.height = htonl(bw->h);
  if (duplicate_line_removal) {
    genreg.tpgdon = true;
  }
  genreg.a1x = 3;
  genreg.a1y = -1;
  genreg.a2x = -3;
  genreg.a2y = -1;
  genreg.a3x = 2;
  genreg.a3y = -2;
  genreg.a4x = -2;
  genreg.a4y = -2;

  const int totalsize = seg.size() + sizeof(pageinfo) + seg2.size() +
                        sizeof(genreg) + datasize +
                        (full_headers ? (sizeof(header) + 2*endseg.size()) : 0);
  u8 *const ret = (u8 *) malloc(totalsize);
  int offset = 0;

#define F(x) memcpy(ret + offset, &x, sizeof(x)) ; offset += sizeof(x)
  if (full_headers) {
    F(header);
  }
  SEGMENT(seg);
  F(pageinfo);
  SEGMENT(seg2);
  F(genreg);
  jbig2enc_tobuffer(&ctx, ret + offset);
  offset += datasize;

  if (full_headers) {
    endseg.type = segment_end_of_page;
    SEGMENT(endseg);
    endseg.number += 1;
    endseg.type = segment_end_of_file;
    SEGMENT(endseg);
  }

  if (totalsize != offset) abort();

  jbig2enc_dealloc(&ctx);

  *length = offset;

  return ret;
}

// see comments in .h file
u8 *
jbig2_encode (int width, int height, int stride, bool zeroIsWhite, uint8_t * const source, int *const length) {
	u8 * ret;
	u32 mask = zeroIsWhite ? 0x0 : 0xffffffff;
	u8* pl = source;
	u8* p;
	u32* pw;
	char a, b;
	for (int h = 0; h < height; h++) {
		p = pl;
		for (int w = 0; w < stride; w += 4) {
			pw = (u32*)p;
			*pw = *pw ^ mask;
			a = *p;
			*p = p[3];
			p++;
			b = *p;
			*p = p[1];
			p++;
			*p = b;
			p++;
			*p = a;
			p++;
		}
		pl += stride;
	}

	Pix * pix = (Pix*)malloc(sizeof(Pix));
	pix->w = width;
	pix->h = height;
	pix->data = (u32*)source;
	ret = jbig2_encode_generic (pix, false, 0, 0, false, length);
	free(pix);
	return ret;
}

// see comments in .h file
void jbig2_freemem (uint8_t * const memblock) {
	free (memblock);
};
