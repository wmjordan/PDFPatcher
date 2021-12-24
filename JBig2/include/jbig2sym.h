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

#ifndef JBIG2ENC_JBIG2SYM_H__
#define JBIG2ENC_JBIG2SYM_H__

struct jbig2enc_ctx;

// -----------------------------------------------------------------------------
// Write a symbol table.
//
// symbols: A 2d array. The first dimention is of different classes of symbols.
//          Then, for each class, there are all the examples of that class. The
//          first member of the class is taken as the exemplar.
// symbol_list: a list of symbols to encode
// symmap: an empty map which is filled. The symbols are written to the file in
//         a different order than they are given in symbols. The maps an index
//         into the symbols array to a symbol number in the file
// unborder_symbols: if true, remove a border from every element of symbols
// -----------------------------------------------------------------------------
void jbig2enc_symboltable(struct jbig2enc_ctx *__restrict__ ctx,
                          PIXA *__restrict__ const symbols,
                          std::vector<unsigned> *__restrict__ symbol_list,
                          std::map<int, int> *symmap,
                          bool unborder_symbols);

// -----------------------------------------------------------------------------
// Write a text region.
//
// A text region is a list of placements of symbols. The symbols must already
// have been coded.
//
// symmap: This maps class numbers to symbol numbers. Only symbol numbers
//         appear in the JBIG2 data stream
// symmap2: If not found in the first symmap, try this one
// comps: a list of connected-component numbers for this page
// ll: This is an array of the lower-left corners of the boxes for each symbol
// assignments: an array, of the same length as boxes, mapping each box to a
//              symbol
// stripwidth: 1 is a safe default (one of [1, 2, 4, 8])
// symbits: number of bits needed to code the symbol number (log2(number of
//          symbols) - rounded up)
// source: an array of the original images for all the connected components.
//         If NULL, refinement is disabled. (page indexed)
// boxes: if source is non-NULL, this is page based list of boxes of symbols on
//        the page
// baseindex: if source is non-NULL, this is the component number of the first
//            component on this page
// refine_level: the number of incorrect pixels allowed before refining.
// unborder_symbols: if true, symbols have a 6px border around them
// -----------------------------------------------------------------------------
void jbig2enc_textregion(struct jbig2enc_ctx *__restrict__ ctx,
                         /*const*/ std::map<int, int> &symmap,
                         /*const*/ std::map<int, int> &symmap2,
                         const std::vector<int> &comps,
                         PTA *const ll, PIXA *const symbols,
                         NUMA *assignments,
                         int stripwidth, int symbits,
                         PIXA *const source, BOXA *boxes, int baseindex,
                         int refine_level, bool unborder_symbols);

#endif  // JBIG2ENC_JBIG2SYM_H__
