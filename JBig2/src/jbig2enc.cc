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

#include <allheaders.h>
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
#include "jbig2sym.h"
#include "jbig2structs.h"
#include "jbig2segments.h"

// -----------------------------------------------------------------------------
// Removes spots which are less than size x size pixels
//
// Note, this has a side-effect of removing a few pixels
// that from components you want to keep.
//
// If that's a problem, you do a binary reconstruction
// (from seedfill.c):
// -----------------------------------------------------------------------------
static PIX *
remove_flyspecks(PIX *const source, const int size) {
  Sel *sel_5h = selCreateBrick(1, size, 0, 2, SEL_HIT);
  Sel *sel_5v = selCreateBrick(size, 1, 2, 0, SEL_HIT);

  Pix *pixt = pixOpen(NULL, source, sel_5h);
  Pix *pixd = pixOpen(NULL, source, sel_5v);
  pixOr(pixd, pixd, pixt);
  pixDestroy(&pixt);
  selDestroy(&sel_5h);
  selDestroy(&sel_5v);

  return pixd;
}

// -----------------------------------------------------------------------------
// Returns the number of bits needed to encode v symbols
// -----------------------------------------------------------------------------
static unsigned
log2up(int v) {
  unsigned r = 0;
  const bool is_pow_of_2 = (v & (v - 1)) == 0;

  while (v >>= 1) r++;
  if (is_pow_of_2) return r;

  return r + 1;
}

// -----------------------------------------------------------------------------
// This is the context for a multi-page JBIG2 document.
// -----------------------------------------------------------------------------
struct jbig2ctx {
  struct JbClasser *classer;  // the leptonica classifier
  int xres, yres;  // ppi for the X and Y direction
  bool full_headers;  // true if we are producing a full JBIG2 file
  bool pdf_page_numbering;  // true if all text pages are page "1" (pdf mode)
  int segnum;  // current segment number
  int symtab_segment;  // the segment number of the symbol table
  // a map from page number a list of components for that page
  std::map<int, std::vector<int> > pagecomps;
  // for each page, the list of symbols which are only used on that page
  std::map<int, std::vector<unsigned> > single_use_symbols;
  // the number of symbols in the global symbol table
  int num_global_symbols;
  std::vector<int> page_xres, page_yres;
  std::vector<int> page_width, page_height;
  // Used to store the mapping from symbol number to the index in the global
  // symbol dictionary.
  std::map<int, int> symmap;
  bool refinement;
  PIXA *avg_templates;  // grayed templates
  int refine_level;
  // only used when using refinement
    // the number of the first symbol of each page
    std::vector<int> baseindexes;
};

// see comments in .h file
struct jbig2ctx *
jbig2_init(float thresh, float weight, int xres, int yres, bool full_headers,
           int refine_level) {
  struct jbig2ctx *ctx = new jbig2ctx;
  ctx->xres = xres;
  ctx->yres = yres;
  ctx->full_headers = full_headers;
  ctx->pdf_page_numbering = !full_headers;
  ctx->segnum = 0;
  ctx->symtab_segment = -1;
  ctx->refinement = refine_level >= 0;
  ctx->refine_level = refine_level;
  ctx->avg_templates = NULL;

  ctx->classer = jbCorrelationInitWithoutComponents(JB_CONN_COMPS, 9999, 9999,
                                                    thresh, weight);

  return ctx;
}

// see comments in .h file
void
jbig2_destroy(struct jbig2ctx *ctx) {
  if (ctx->avg_templates) pixaDestroy(&ctx->avg_templates);
  jbClasserDestroy(&ctx->classer);
  delete ctx;
}

// see comments in .h file
void
jbig2_add_page(struct jbig2ctx *ctx, struct Pix *input) {
  PIX *bw;

  if (false /*ctx->xres >= 300*/) {
    bw = remove_flyspecks(input, (int) (0.0084*ctx->xres));
  } else {
    bw = pixClone(input);
  }

  if (ctx->refinement) {
    ctx->baseindexes.push_back(ctx->classer->baseindex);
  }

  jbAddPage(ctx->classer, bw);
  ctx->page_width.push_back(bw->w);
  ctx->page_height.push_back(bw->h);
  ctx->page_xres.push_back(bw->xres);
  ctx->page_yres.push_back(bw->yres);

  if (ctx->refinement) {
    // This code is broken by (my) recent changes to Leptonica. Needs to be
    // fixed at some point, but not too important at the moment since we don't
    // use refinement.

    /*BOXA *boxes = boxaCopy(ctx->classer->boxas, L_CLONE);
    ctx->boxes.push_back(boxes);
    PIXA *comps = pixaCopy(ctx->classer->pixas, L_CLONE);
    ctx->comps.push_back(comps);*/
  }

  pixDestroy(&bw);
}

#define F(x) memcpy(ret + offset, &x, sizeof(x)) ; offset += sizeof(x)
#define G(x, y) memcpy(ret + offset, x, y); offset += y;
#define SEGMENT(x) x.write(ret + offset); offset += x.size();

// see comments in .h file
uint8_t *
jbig2_pages_complete(struct jbig2ctx *ctx, int *const length) {
  /*
     Graying support - disabled.
     It's not very clear that graying actaully buys you much extra quality
     above pick-the-first. Also, aligning the gray glyphs requires the
     original source image.

     Remember that you need the Init without WithoutComponents to use this */


  /*NUMA *samples_per_composition;
  PTA *grayed_centroids;
  PIXA *grayed;

  grayed = jbAccumulateComposites(ctx->classer->pixaa, &samples_per_composition,
                                  &grayed_centroids);

  if (!grayed || grayed->n != ctx->classer->pixaa->n) {
    fprintf(stderr, "Graying failed\n");
    return NULL;
  }

  ctx->avg_templates = pixaCreate(0);
  for (int i = 0; i < grayed->n; ++i) {
    int samples;
    numaGetIValue(samples_per_composition, i, &samples);
    PIX *avg = pixFinalAccumulateThreshold(grayed->pix[i], 0,
                                           (samples + 1) >> 1);
    pixaAddPix(ctx->avg_templates, avg, L_INSERT);
    //char b[512];
    //sprintf(b, "gray-%d/th.png", i);
    //pixWrite(b, avg, IFF_PNG);
  }

  pixaDestroy(&grayed);
  numaDestroy(&samples_per_composition);*/

  // We find the symbols which only appear on a single page and encode them in
  // a symbol dictionary just for that page. This is because we want to keep
  // the size of the global dictionary down as some PDF readers appear to
  // decode it for every page (!)

  // (as a short cut, we just pick the symbols which are only used once since,
  // in testing, all the symbols which appear on only one page appear only once
  // on that page)

  const bool single_page = ctx->classer->npages == 1;

  // maps symbol number to the number of times it has been used
  // pixat->n is the number of symbols
  // naclass->n is the number of connected components

  std::vector<unsigned> symbol_used(ctx->classer->pixat->n);
  for (int i = 0; i < ctx->classer->naclass->n; ++i) {
    int n;
    numaGetIValue(ctx->classer->naclass, i, &n);
    symbol_used[n]++;
  }

  // the multiuse symbols are the ones which go into the global dictionary
  std::vector<unsigned> multiuse_symbols;
  for (int i = 0; i < ctx->classer->pixat->n; ++i) {
    if (symbol_used[i] == 0) abort();
    if (symbol_used[i] > 1 || single_page) multiuse_symbols.push_back(i);
  }
  ctx->num_global_symbols = multiuse_symbols.size();

  // build the pagecomps map: a map from page number to the list of connected
  // components for that page. The classer gives us an array from connected
  // component number to page number - we just have to reverse it
  for (int i = 0; i < ctx->classer->napage->n; ++i) {
    int page_num;
    numaGetIValue(ctx->classer->napage, i, &page_num);
    ctx->pagecomps[page_num].push_back(i);
    int symbol;
    numaGetIValue(ctx->classer->naclass, i, &symbol);
    if (symbol_used[symbol] == 1 && !single_page) {
      ctx->single_use_symbols[page_num].push_back(symbol);
    }
  }

#ifdef DUMP_SYMBOL_GRAPH
  for (int p = 0; p < ctx->classer->npages; ++p) {
    for (std::vector<int>::const_iterator i = ctx->pagecomps[p].begin();
         i != ctx->pagecomps[p].end(); ++i) {
      const int sym = (int) ctx->classer->naclass->array[*i];
      fprintf(stderr, "S: %d %d\n", p, sym);
    }
  }
#endif

#ifdef SYMBOL_COMPRESSION_DEBUGGING
  std::map<int, int> usecount;
  for (int i = 0; i < ctx->classer->naclass->n; ++i) {
    usecount[(int)ctx->classer->naclass->array[i]]++;
  }

  for (int p = 0; p < ctx->classer->npages; ++p) {
    const int numcomps = ctx->pagecomps[p].size();
    int unique_in_doc = 0;
    std::map<int, int> symcount;
    for (std::vector<int>::const_iterator i = ctx->pagecomps[p].begin();
         i != ctx->pagecomps[p].end(); ++i) {
      const int sym = (int) ctx->classer->naclass->array[*i];
      symcount[sym]++;
      if (usecount[sym] == 1) unique_in_doc++;
    }
    int unique_this_page = 0;
    for (std::map<int, int>::const_iterator i = symcount.begin();
         i != symcount.end(); ++i) {
      if (i->second == 1) unique_this_page++;
    }

    fprintf(stderr, "Page %d %d/%d/%d\n", p, numcomps, unique_this_page, unique_in_doc);
  }
#endif

#ifdef DUMP_ALL_SYMBOLS
  char filenamebuf[128];
  for (int i = 0; i < ctx->classer->pixat->n; ++i) {
    sprintf(filenamebuf, "sym-%d.png", i);
    pixWrite(filenamebuf, ctx->classer->pixat->pix[i], IFF_PNG);
  }
#endif
  fprintf(stderr, "JBIG2 compression complete. pages:%d symbols:%d log2:%d\n",
          ctx->classer->npages, ctx->classer->pixat->n,
          log2up(ctx->classer->pixat->n));

  jbGetLLCorners(ctx->classer);

  struct jbig2enc_ctx ectx;
  jbig2enc_init(&ectx);

  struct jbig2_file_header header;
  if (ctx->full_headers) {
    memset(&header, 0, sizeof(header));
    header.n_pages = htonl(ctx->classer->npages);
    header.organisation_type = 1;
    memcpy(&header.id, JBIG2_FILE_MAGIC, 8);
  }

  Segment seg;
  struct jbig2_symbol_dict symtab;
  memset(&symtab, 0, sizeof(symtab));

  jbig2enc_symboltable
    (&ectx, ctx->avg_templates ? ctx->avg_templates : ctx->classer->pixat,
     &multiuse_symbols, &ctx->symmap, ctx->avg_templates == NULL);
  const int symdatasize = jbig2enc_datasize(&ectx);

  symtab.a1x = 3;
  symtab.a1y = -1;
  symtab.a2x = -3;
  symtab.a2y = -1;
  symtab.a3x = 2;
  symtab.a3y = -2;
  symtab.a4x = -2;
  symtab.a4y = -2;
  symtab.exsyms = symtab.newsyms = htonl(multiuse_symbols.size());

  ctx->symtab_segment = ctx->segnum;
  seg.number = ctx->segnum;
  ctx->segnum++;
  seg.type = segment_symbol_table;
  seg.len = sizeof(symtab) + symdatasize;
  seg.page = 0;
  seg.retain_bits = 1;

  u8 *const ret = (u8 *) malloc((ctx->full_headers ? sizeof(header) : 0) +
                                seg.size() + sizeof(symtab) + symdatasize);
  int offset = 0;
  if (ctx->full_headers) {
    F(header);
  }
  SEGMENT(seg);
  F(symtab);
  jbig2enc_tobuffer(&ectx, ret + offset);
  jbig2enc_dealloc(&ectx);
  offset += symdatasize;

  *length = offset;

  return ret;
}

// see comments in .h file
uint8_t *
jbig2_produce_page(struct jbig2ctx *ctx, int page_no,
                   int xres, int yres, int *const length) {
  const bool last_page = page_no == ctx->classer->npages;
  const bool include_trailer = last_page && ctx->full_headers;

  struct jbig2enc_ctx ectx;
  jbig2enc_init(&ectx);

  Segment seg, symseg;
  Segment endseg, trailerseg;
  struct jbig2_page_info pageinfo;
  memset(&pageinfo, 0, sizeof(pageinfo));
  struct jbig2_text_region textreg;
  memset(&textreg, 0, sizeof(textreg));
  struct jbig2_text_region_syminsts textreg_syminsts;
  memset(&textreg_syminsts, 0, sizeof(textreg_syminsts));
  struct jbig2_text_region_atflags textreg_atflags;
  memset(&textreg_atflags, 0, sizeof(textreg_atflags));
  Segment segr;

  // page information segment
  seg.number = ctx->segnum;
  ctx->segnum++;
  seg.type = segment_page_information;
  seg.page = ctx->pdf_page_numbering ? 1 : 1 + page_no;
  seg.len = sizeof(struct jbig2_page_info);
  pageinfo.width = htonl(ctx->page_width[page_no]);
  pageinfo.height = htonl(ctx->page_height[page_no]);
  pageinfo.xres = htonl(xres == -1 ? ctx->page_xres[page_no] : xres );
  pageinfo.yres = htonl(yres == -1 ? ctx->page_yres[page_no] : yres );
  pageinfo.is_lossless = ctx->refinement;

  std::map<int, int> second_symbol_map;
  // If we have single-use symbols on this page we make a new symbol table
  // containing just them.
  const bool extrasymtab = ctx->single_use_symbols[page_no].size() > 0;
  struct jbig2enc_ctx extrasymtab_ctx;

  struct jbig2_symbol_dict symtab;
  memset(&symtab, 0, sizeof(symtab));

  if (extrasymtab) {
    jbig2enc_init(&extrasymtab_ctx);
    symseg.number = ctx->segnum++;
    symseg.type = segment_symbol_table;
    symseg.page = ctx->pdf_page_numbering ? 1 : 1 + page_no;

    jbig2enc_symboltable
      (&extrasymtab_ctx,
       ctx->avg_templates ? ctx->avg_templates : ctx->classer->pixat,
       &ctx->single_use_symbols[page_no], &second_symbol_map,
       ctx->avg_templates == NULL);
    symtab.a1x = 3;
    symtab.a1y = -1;
    symtab.a2x = -3;
    symtab.a2y = -1;
    symtab.a3x = 2;
    symtab.a3y = -2;
    symtab.a4x = -2;
    symtab.a4y = -2;
    symtab.exsyms = symtab.newsyms =
      htonl(ctx->single_use_symbols[page_no].size());

    symseg.len = jbig2enc_datasize(&extrasymtab_ctx) + sizeof(symtab);
  }

  const int numsyms = ctx->num_global_symbols +
                      ctx->single_use_symbols[page_no].size();
  //BOXA *const boxes = ctx->refinement ? ctx->boxes[page_no] : NULL;
  int baseindex = ctx->refinement ? ctx->baseindexes[page_no] : 0;
  jbig2enc_textregion(&ectx, ctx->symmap, second_symbol_map,
                      ctx->pagecomps[page_no],
                      ctx->classer->ptall,
                      ctx->avg_templates ? ctx->avg_templates : ctx->classer->pixat,
                      ctx->classer->naclass, 1,
                      log2up(numsyms),
                      //ctx->refinement ? ctx->comps[page_no] : NULL,
                      NULL,
                      /* boxes */ NULL, baseindex, ctx->refine_level,
                      ctx->avg_templates == NULL);
  const int textdatasize = jbig2enc_datasize(&ectx);
  textreg.width = htonl(ctx->page_width[page_no]);
  textreg.height = htonl(ctx->page_height[page_no]);
  textreg.logsbstrips = 0;
  textreg.sbrefine = ctx->refinement;
  // refcorner = 0 -> bot left
  textreg_syminsts.sbnuminstances = htonl(ctx->pagecomps[page_no].size());

  textreg_atflags.a1x = -1;
  textreg_atflags.a1y = -1;
  textreg_atflags.a2x = -1;
  textreg_atflags.a2y = -1;

  segr.number = ctx->segnum;
  ctx->segnum++;
  segr.type = segment_imm_text_region;
  segr.referred_to.push_back(ctx->symtab_segment);
  if (extrasymtab) segr.referred_to.push_back(symseg.number);
  if (ctx->refinement) {
    segr.len = sizeof(textreg) + sizeof(textreg_syminsts) +
               sizeof(textreg_atflags) + textdatasize;
  } else {
    segr.len = sizeof(textreg) + sizeof(textreg_syminsts) + textdatasize;
  }

  segr.retain_bits = 2;
  segr.page = ctx->pdf_page_numbering ? 1 : 1 + page_no;

  const int extrasymtab_size = extrasymtab ?
    jbig2enc_datasize(&extrasymtab_ctx) : 0;

  if (ctx->full_headers) {
    endseg.number = ctx->segnum;
    ctx->segnum++;
    endseg.type = segment_end_of_page;
    endseg.page = ctx->pdf_page_numbering ? 1 : 1 + page_no;
  }

  if (include_trailer) {
    trailerseg.number = ctx->segnum;
    ctx->segnum++;
    trailerseg.type = segment_end_of_file;
    trailerseg.page = 0;
  }

  const int totalsize = seg.size() + sizeof(pageinfo) +
                        (extrasymtab ? (extrasymtab_size + symseg.size() +
                                        sizeof(symtab)) : 0) +
                        segr.size() +
                        sizeof(textreg) + sizeof(textreg_syminsts) +
                        (ctx->refinement ? sizeof(textreg_atflags) : 0) +
                        textdatasize +
                        (ctx->full_headers ? endseg.size() : 0) +
                        (include_trailer ? trailerseg.size() : 0);
  u8 *ret = (u8 *) malloc(totalsize);
  int offset = 0;

  SEGMENT(seg);
  F(pageinfo);
  if (extrasymtab) {
    SEGMENT(symseg);
    F(symtab);
    jbig2enc_tobuffer(&extrasymtab_ctx, ret + offset);
    offset += extrasymtab_size;
  }
  SEGMENT(segr);
  F(textreg);
  if (ctx->refinement) {
    F(textreg_atflags);
  }
  F(textreg_syminsts);
  jbig2enc_tobuffer(&ectx, ret + offset); offset += textdatasize;
  if (ctx->full_headers) {
    SEGMENT(endseg);
  }
  if (include_trailer) {
    SEGMENT(trailerseg);
  }

  if (totalsize != offset) abort();

  jbig2enc_dealloc(&ectx);
  if (extrasymtab) jbig2enc_dealloc(&extrasymtab_ctx);

  *length = offset;
  return ret;
}

#undef F
#undef G

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

