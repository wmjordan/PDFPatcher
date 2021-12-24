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

#ifndef JBIG2ENC_JBIG2STRUCTS_H__
#define JBIG2ENC_JBIG2STRUCTS_H__

// GCC packs bit fields in a different order on big endian machines

enum {
  segment_symbol_table = 0,
  segment_imm_generic_region = 38,
  segment_page_information = 48,
  segment_imm_text_region =  6,
  segment_end_of_page = 49,
  segment_end_of_file = 51
};

// note that the < 1 byte fields are packed from the LSB upwards - unless
// you're bigendian, in which case they are packed MSB downwards. Joy.

#define JBIG2_FILE_MAGIC "\x97\x4a\x42\x32\x0d\x0a\x1a\x0a"

#if defined(WIN32)
#pragma pack(1)
#define PACKED
#else
#define PACKED __attribute__((packed))
#endif

struct jbig2_file_header {
  u8 id[8];
#ifndef _BIG_ENDIAN
  u8 organisation_type : 1;
  u8 unknown_n_pages : 1;
  u8 reserved : 6;
#else
  u8 reserved : 6;
  u8 unknown_n_pages : 1;
  u8 organisation_type : 1;
#endif
  u32 n_pages;
} PACKED;

struct jbig2_page_info {
  u32 width;
  u32 height;
  u32 xres;
  u32 yres;
#ifndef _BIG_ENDIAN
  u8 is_lossless : 1;
  u8 contains_refinements : 1;
  u8 default_pixel : 1;
  u8 default_operator : 2;
  u8 aux_buffers : 1;
  u8 operator_override : 1;
  u8 reserved : 1;
#else
  u8 reserved : 1;
  u8 operator_override : 1;
  u8 aux_buffers : 1;
  u8 default_operator : 2;
  u8 default_pixel : 1;
  u8 contains_refinements : 1;
  u8 is_lossless : 1;
#endif
  u16 segment_flags;
} PACKED;

struct jbig2_generic_region {
  u32 width;
  u32 height;
  u32 x;
  u32 y;
  u8 comb_operator;

#ifndef _BIG_ENDIAN
  u8 mmr : 1;
  u8 gbtemplate : 2;
  u8 tpgdon : 1;
  u8 reserved : 4;
#else
  u8 reserved : 4;
  u8 tpgdon : 1;
  u8 gbtemplate : 2;
  u8 mmr : 1;
#endif

  // generic region segment here. You may not need to write all 8 bytes here.
  // If the template is 1..3 only the first two are needed.
  signed char a1x, a1y, a2x, a2y, a3x, a3y, a4x, a4y;
} PACKED ;

struct jbig2_symbol_dict {
#ifndef _BIG_ENDIAN
  u8 sdhuff:1;
  u8 sdrefagg:1;
  u8 sdhuffdh:2;
  u8 sdhuffdw:2;
  u8 sdhuffbmsize:1;
  u8 sdhuffagginst:1;
  u8 bmcontext:1;
  u8 bmcontextretained:1;
  u8 sdtemplate:2;
  u8 sdrtemplate:1;
  u8 reserved:3;
#else
  u8 reserved:3;
  u8 sdrtemplate:1;
  u8 sdtemplate:2;
  u8 bmcontextretained:1;
  u8 bmcontext:1;
  u8 sdhuffagginst:1;
  u8 sdhuffbmsize:1;
  u8 sdhuffdw:2;
  u8 sdhuffdh:2;
  u8 sdrefagg:1;
  u8 sdhuff:1;
#endif

  signed char a1x, a1y, a2x, a2y, a3x, a3y, a4x, a4y;

  // refinement AT flags omitted

  u32 exsyms;
  u32 newsyms;
} PACKED;

struct jbig2_text_region {
  u32 width;
  u32 height;
  u32 x;
  u32 y;
  u8 comb_operator;

#ifndef _BIG_ENDIAN
  u8 sbcombop2:1;
  u8 sbdefpixel:1;
  u8 sbdsoffset:5;
  u8 sbrtemplate:1;
  u8 sbhuff:1;
  u8 sbrefine:1;
  u8 logsbstrips:2;
  u8 refcorner:2;
  u8 transposed:1;
  u8 sbcombop1:1;
#else
  u8 sbcombop1:1;
  u8 transposed:1;
  u8 refcorner:2;
  u8 logsbstrips:2;
  u8 sbrefine:1;
  u8 sbhuff:1;
  u8 sbrtemplate:1;
  u8 sbdsoffset:5;
  u8 sbdefpixel:1;
  u8 sbcombop2:1;
#endif

  // huffman flags omitted
} PACKED;


struct jbig2_text_region_atflags {
  signed char a1x, a1y, a2x, a2y;
} PACKED;

struct jbig2_text_region_syminsts {
  u32 sbnuminstances;
  // huffman decoding table omitted
} PACKED;

#if defined(WIN32)
#pragma pack()
#endif

#endif  // JBIG2ENC_JBIG2STRUCTS_H__
