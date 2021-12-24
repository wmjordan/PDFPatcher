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

#ifndef JBIG2ENC_JBIG2_H__
#define JBIG2ENC_JBIG2_H__

// -----------------------------------------------------------------------------
// Welcome gentle reader,
//
// This is an encoder for JBIG2:
// www.jpeg.org/public/fcd14492.pdf
//
// JBIG2 encodes bi-level (1 bpp) images using a number of clever tricks to get
// better compression than G4. This encoder can:
//    * Generate JBIG2 files, or fragments for embedding in PDFs
//    * Generic region encoding
//    * Symbol extraction, classification and text region coding
//
// It uses the (Apache-ish licensed) Leptonica library:
//   http://www.leptonica.com/
// -----------------------------------------------------------------------------

#if defined(sun)
#include <sys/types.h>
#else
#include <stdint.h>
#endif

struct Pix;
// This is the (opaque) structure which handles multi-page compression.
struct jbig2ctx;

// -----------------------------------------------------------------------------
// Multipage compression.
//
// First call jbig2_init to setup the structure. This structure must be free'ed
// by calling jbig2_destroy when you are finished.
//
// First, add all the pages with jbig2_add_page. This will collect all the
// information required. If refinement is on, it will also save all the
// component images, so this may take large amounts of memory.
//
// Then call jbig2_pages_complete. This returns a malloced buffer with the
// symbol table encoded.
//
// Then call jbig2_produce_page for each page. You must call it with pages
// numbered from zero, and for every page.
// -----------------------------------------------------------------------------

// -----------------------------------------------------------------------------
// Create a multi-page compression context structure
//
// thresh: The threshold for the classifier. The larger the number the larger
//         the number of different symbols, the more bits used and the closer
//         the resulting image is to the original. (0.85 is a good value)
// weight: Use 0.5
// xres: the ppi in the X direction. If 0, the ppi is taken from bw
// yres: see xres
// full_headers: if true a full JBIG2 file is produced, otherwise the data is
//               only good for embedding in PDFs
// refine: If < 0, disable refinement. Otherwise, the number of incorrect
//         pixels which will be accepted per symbol. Enabling refinement
//         increases memory use.
// -----------------------------------------------------------------------------
struct jbig2ctx *jbig2_init(float thresh, float weight, int xres, int yres,
                            bool full_headers, int refine_level);

// -----------------------------------------------------------------------------
// Delete a context returned by jbig2_init
// -----------------------------------------------------------------------------
void jbig2_destroy(struct jbig2ctx *);
// -----------------------------------------------------------------------------
// Classify and record information about a page.
//
// bw: A 1-bpp image
// -----------------------------------------------------------------------------
void jbig2_add_page(struct jbig2ctx *ctx, struct Pix *bw);
// -----------------------------------------------------------------------------
// Finalise information about the document and encode the symbol table.
//
// WARNING: returns a malloced buffer which the caller must free
// -----------------------------------------------------------------------------
uint8_t *jbig2_pages_complete(struct jbig2ctx *ctx, int *const length);
// -----------------------------------------------------------------------------
// Encode a page.
//
// page_no: number of this page, indexed from 0. This *must* match the order of
//          pages presented to jbig2_add_page.
// xres, yres: if -1, use values given in _init. Otherwise, set the resolution
//             for this page only
//
// WARNING: returns a malloced buffer which the caller must free
// -----------------------------------------------------------------------------
uint8_t *jbig2_produce_page(struct jbig2ctx *ctx, int page_no, int xres,
                            int yres, int *const length);

// WARNING: returns a malloced buffer which the caller must free
// -----------------------------------------------------------------------------


// -----------------------------------------------------------------------------
// Single page compression
// -----------------------------------------------------------------------------

// -----------------------------------------------------------------------------
// Encode an image as a single generic region. This is lossless. It should not
// be used for images as half-tone coding is not implemented.
//
// see argument comments for jbig2_init
// duplicate_line_removal: turning this on
//    * Breaks ghostscript
//    * Takes ever so slightly more bytes to encode
//    * Cuts the encode time by half
//
// WARNING: returns a malloced buffer which the caller must free
// -----------------------------------------------------------------------------
uint8_t *
jbig2_encode_generic(struct Pix *const bw, const bool full_headers,
                     const int xres, const int yres,
                     const bool duplicate_line_removal,
                     int *const length);

#endif  // JBIG2ENC_JBIG2_H__
