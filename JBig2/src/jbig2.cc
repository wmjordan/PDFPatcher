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

#include <vector>

#include <sys/types.h>
#include <stdio.h>
#include <stdlib.h>
#include <fcntl.h>
#include <string.h>
#include <unistd.h>

#include <allheaders.h>
#include <pix.h>

#include "jbig2enc.h"

#if defined(WIN32)
#define WINBINARY O_BINARY
#else
#define WINBINARY 0
#endif

static void
usage(const char *argv0) {
  fprintf(stderr, "Usage: %s [options] <input filenames...>\n", argv0);
  fprintf(stderr, "Options:\n");
  fprintf(stderr, "  -b <basename>: output file root name when using symbol coding\n");
  fprintf(stderr, "  -d --duplicate-line-removal: use TPGD in generic region coder\n");
  fprintf(stderr, "  -p --pdf: produce PDF ready data\n");
  fprintf(stderr, "  -s --symbol-mode: use text region, not generic coder\n");
  fprintf(stderr, "  -t <threshold>: set classification threshold for symbol coder (def: 0.85)\n");
  fprintf(stderr, "  -T <bw threshold>: set 1 bpp threshold (def: 188)\n");
  fprintf(stderr, "  -r --refine: use refinement (requires -s: lossless)\n");
  fprintf(stderr, "  -O <outfile>: dump thresholded image as PNG\n");
  fprintf(stderr, "  -2: upsample 2x before thresholding\n");
  fprintf(stderr, "  -4: upsample 4x before thresholding\n");
  fprintf(stderr, "  -S: remove images from mixed input and save separately\n");
  fprintf(stderr, "  -j --jpeg-output: write images from mixed input as JPEG\n");
  fprintf(stderr, "  -v: be verbose\n");
}

static bool verbose = false;


static void
pixInfo(PIX *pix, const char *msg) {
  if (msg != NULL) fprintf(stderr, "%s ", msg);
  if (pix == NULL) {
    fprintf(stderr, "NULL pointer!\n");
    return;
  }
  fprintf(stderr, "%u x %u (%d bits) %udpi x %udpi, refcount = %u\n",
          pix->w, pix->h, pix->d, pix->xres, pix->yres, pix->refcount);
}

#ifdef _MSC_VER
// -----------------------------------------------------------------------------
// Windows, sadly, lacks asprintf
// -----------------------------------------------------------------------------
#include <stdarg.h>
int
asprintf(char **strp, const char *fmt, ...) {
    va_list va;
    va_start(va, fmt);

    const int required = vsnprintf(NULL, 0, fmt, va);
    char *const buffer = (char *) malloc(required + 1);
    const int ret = vsnprintf(buffer, required + 1, fmt, va);
    *strp = buffer;

    va_end(va);

    return ret;
}
#endif

// -----------------------------------------------------------------------------
// Morphological operations for segmenting an image into text regions
// -----------------------------------------------------------------------------
static const char *segment_mask_sequence = "r11";
static const char *segment_seed_sequence = "r1143 + o4.4 + x4"; /* maybe o6.6 */
static const char *segment_dilation_sequence = "d3.3";

// -----------------------------------------------------------------------------
// Takes two pix as input, generated from the same original image:
//   1. pixb   - a binary thresholded image
//   2. piximg - a full color or grayscale image
// and segments them by finding the areas that contain color or grayscale
// graphics, removing those areas from the binary image, and doing the
// opposite for the full color/grayscale image.  The upshot is that after
// this routine has been run, the binary image contains only text and the
// full color image contains only the graphics.
//
// Both input images are modified by this procedure.  If no text is found,
// pixb is set to NULL.  If no graphics is found, piximg is set to NULL.
//
// Thanks to Dan Bloomberg for this
// -----------------------------------------------------------------------------

static PIX*
segment_image(PIX *pixb, PIX *piximg) {
  // Make seed and mask, and fill seed into mask
  PIX *pixmask4 = pixMorphSequence(pixb, (char *) segment_mask_sequence, 0);
  PIX *pixseed4 = pixMorphSequence(pixb, (char *) segment_seed_sequence, 0);
  PIX *pixsf4 = pixSeedfillBinary(NULL, pixseed4, pixmask4, 8);
  PIX *pixd4 = pixMorphSequence(pixsf4, (char *) segment_dilation_sequence, 0);

  // we want to force the binary mask to be the same size as the
  // input color image, so we have to do it this way...
  // is there a better way?
  // PIX *pixd = pixExpandBinary(pixd4, 4);
  PIX *pixd = pixCreate(piximg->w, piximg->h, 1);
  pixCopyResolution(pixd, piximg);
  if (verbose) pixInfo(pixd, "mask image: ");
  expandBinaryPower2Low(pixd->data, pixd->w, pixd->h, pixd->wpl,
                        pixd4->data, pixd4->w, pixd4->h, pixd4->wpl, 4);

  pixDestroy(&pixd4);
  pixDestroy(&pixsf4);
  pixDestroy(&pixseed4);
  pixDestroy(&pixmask4);

  pixSubtract(pixb, pixb, pixd);

  // now see what we got from the segmentation
  static l_int32 *tab = NULL;
  if (tab == NULL) tab = makePixelSumTab8();

  // if no image portion was found, set the image pointer to NULL and return
  l_int32  pcount;
  pixCountPixels(pixd, &pcount, tab);
  if (verbose) fprintf(stderr, "pixel count of graphics image: %u\n", pcount);
  if (pcount < 100) {
    pixDestroy(&pixd);
    return NULL;
  }

  // if no text portion found, set the binary pointer to NULL
  pixCountPixels(pixb, &pcount, tab);
  if (verbose) fprintf(stderr, "pixel count of binary image: %u\n", pcount);
  if (pcount < 100) {
    pixDestroy(&pixb);
  }

  PIX *piximg1;
  if (piximg->d == 1 || piximg->d == 8 || piximg->d == 32) {
    piximg1 = pixClone(piximg);
  } else if (piximg->d > 8) {
    piximg1 = pixConvertTo32(piximg);
  } else {
    piximg1 = pixConvertTo8(piximg, FALSE);
  }

  PIX *pixd1;
  if (piximg1->d == 32) {
    pixd1 = pixConvertTo32(pixd);
  } else if (piximg1->d == 8) {
    pixd1 = pixConvertTo8(pixd, FALSE);
  } else {
    pixd1 = pixClone(pixd);
  }
  pixDestroy(&pixd);

  if (verbose) {
    pixInfo(pixd1, "binary mask image:");
    pixInfo(piximg1, "graphics image:");
  }
  pixRasteropFullImage(pixd1, piximg1, PIX_SRC | PIX_DST);

  pixDestroy(&piximg1);
  if (verbose) {
    pixInfo(pixb, "segmented binary text image:");
    pixInfo(pixd1, "segmented graphics image:");
  }

  return pixd1;
}

int
main(int argc, char **argv) {
  bool duplicate_line_removal = false;
  bool pdfmode = false;
  float threshold = 0.85;
  int bw_threshold = 188;
  bool symbol_mode = false;
  bool refine = false;
  bool up2 = false, up4 = false;
  const char *output_threshold = NULL;
  const char *basename = "output";
  l_int32 img_fmt = IFF_PNG;
  const char *img_ext = "png";
  bool segment = false;
  int i;

  for (i = 1; i < argc; ++i) {
    if (strcmp(argv[i], "-h") == 0 ||
        strcmp(argv[i], "--help") == 0) {
      usage(argv[0]);
      return 0;
      continue;
    }

    if (strcmp(argv[i], "-b") == 0 ||
        strcmp(argv[i], "--basename") == 0) {
      basename = argv[i+1];
      i++;
      continue;
    }

    if (strcmp(argv[i], "-d") == 0 ||
        strcmp(argv[i], "--duplicate-line-removal") == 0) {
      duplicate_line_removal = true;
      continue;
    }

    if (strcmp(argv[i], "-p") == 0 ||
        strcmp(argv[i], "--pdf") == 0) {
      pdfmode = true;
      continue;
    }

    if (strcmp(argv[i], "-s") == 0 ||
        strcmp(argv[i], "--symbol-mode") == 0) {
      symbol_mode = true;
      continue;
    }

    if (strcmp(argv[i], "-r") == 0 ||
        strcmp(argv[i], "--refine") == 0) {
      fprintf(stderr, "Refinement broke in recent releases since it's "
                      "rarely used. If you need it you should bug "
                      "agl@imperialviolet.org to fix it\n");
      return 1;
      refine = true;
      continue;
    }

    if (strcmp(argv[i], "-2") == 0) {
      up2 = true;
      continue;
    }
    if (strcmp(argv[i], "-4") == 0) {
      up4 = true;
      continue;
    }

    if (strcmp(argv[i], "-O") == 0) {
      output_threshold = argv[i+1];
      i++;
      continue;
    }

    if (strcmp(argv[i], "-S") == 0) {
      segment = true;
      continue;
    }

    if (strcmp(argv[i], "-j") == 0 ||
        strcmp(argv[i], "--jpeg-output") == 0) {
      img_ext = "jpg";
      img_fmt = IFF_JFIF_JPEG;
      continue;
    }

    if (strcmp(argv[i], "-t") == 0) {
      char *endptr;
      threshold = strtod(argv[i+1], &endptr);
      if (*endptr) {
        fprintf(stderr, "Cannot parse float value: %s\n", argv[i+1]);
        usage(argv[0]);
        return 1;
      }

      if (threshold > 0.9 || threshold < 0.4) {
        fprintf(stderr, "Invalid value for threshold\n");
        fprintf(stderr, "(must be between 0.4 and 0.9)\n");
        return 10;
      }
      i++;
      continue;
    }

    if (strcmp(argv[i], "-T") == 0) {
      char *endptr;
      bw_threshold = strtol(argv[i+1], &endptr, 10);
      if (*endptr) {
        fprintf(stderr, "Cannot parse int value: %s\n", argv[i+1]);
        usage(argv[0]);
        return 1;
      }
      if (bw_threshold < 0 || bw_threshold > 255) {
        fprintf(stderr, "Invalid bw threshold: (0..255)\n");
        return 11;
      }
      i++;
      continue;
    }

    if (strcmp(argv[i], "-v") == 0) {
      verbose = true;
      continue;
    }

    break;
  }

  if (i == argc) {
    fprintf(stderr, "No filename given\n\n");
    usage(argv[0]);
    return 4;
  }

  if (refine && !symbol_mode) {
    fprintf(stderr, "Refinement makes not sense unless in symbol mode!\n");
    fprintf(stderr, "(if you have -r, you must have -s)\n");
    return 5;
  }

  if (up2 && up4) {
    fprintf(stderr, "Can't have both -2 and -4!\n");
    return 6;
  }

  struct jbig2ctx *ctx = jbig2_init(threshold, 0.5, 0, 0, !pdfmode, refine ? 10 : -1);
  int pageno = -1;

  int numsubimages=0, subimage=0, num_pages = 0;
  while (i < argc) {
    if (subimage==numsubimages) {
      subimage = numsubimages = 0;
      FILE *fp;
      if ((fp=fopen(argv[i], "r"))==NULL) {
        fprintf(stderr, "Unable to open \"%s\"", argv[i]);
        return 1;
      }
      l_int32 filetype;
      findFileFormatStream(fp, &filetype);
      if (filetype==IFF_TIFF && tiffGetCount(fp, &numsubimages)) {
        return 1;
      }
      fclose(fp);
    }

    PIX *source;
    if (numsubimages<=1) {
      source = pixRead(argv[i]);
    } else {
      source = pixReadTiff(argv[i], subimage++);
    }

    if (!source) return 3;
    if (verbose)
      pixInfo(source, "source image:");

    PIX *pixl, *gray, *pixt;
    if ((pixl = pixRemoveColormap(source, REMOVE_CMAP_BASED_ON_SRC)) == NULL) {
      fprintf(stderr, "Failed to remove colormap from %s\n", argv[i]);
      return 1;
    }
    pixDestroy(&source);
    pageno++;

    if (pixl->d > 1) {
      if (pixl->d > 8) {
        gray = pixConvertRGBToGrayFast(pixl);
        if (!gray) return 1;
      } else {
        gray = pixClone(pixl);
      }
      if (up2) {
        pixt = pixScaleGray2xLIThresh(gray, bw_threshold);
      } else if (up4) {
        pixt = pixScaleGray4xLIThresh(gray, bw_threshold);
      } else {
        pixt = pixThresholdToBinary(gray, bw_threshold);
      }
      pixDestroy(&gray);
    } else {
      pixt = pixClone(pixl);
    }
    if (verbose)
      pixInfo(pixt, "thresholded image:");

    if (output_threshold) {
      pixWrite(output_threshold, pixt, IFF_PNG);
    }

    if (segment && pixl->d > 1) {
      PIX *graphics = segment_image(pixt, pixl);
      if (graphics) {
        if (verbose)
          pixInfo(graphics, "graphics image:");
        char *filename;
        asprintf(&filename, "%s.%04d.%s", basename, pageno, img_ext);
        pixWrite(filename, graphics, img_fmt);
        free(filename);
      } else if (verbose) {
        fprintf(stderr, "%s: no graphics found in input image\n", argv[i]);
      }
      if (! pixt) {
        fprintf(stderr, "%s: no text portion found in input image\n", argv[i]);
        i++;
        continue;
      }
    }

    pixDestroy(&pixl);

    if (!symbol_mode) {
      int length;
      uint8_t *ret;
      ret = jbig2_encode_generic(pixt, !pdfmode, 0, 0, duplicate_line_removal,
                                 &length);
	  _setmode (1, O_BINARY);
      write(1, ret, length);
      return 0;
    }

    jbig2_add_page(ctx, pixt);
    pixDestroy(&pixt);
    num_pages++;
    if (subimage==numsubimages) {
      i++;
    }
  }

  uint8_t *ret;
  int length;
  ret = jbig2_pages_complete(ctx, &length);
  if (pdfmode) {
    char *filename;
    asprintf(&filename, "%s.sym", basename);
    const int fd = open(filename, O_WRONLY | O_TRUNC | O_CREAT | WINBINARY, 0600);
    free(filename);
    if (fd < 0) abort();
    write(fd, ret, length);
    close(fd);
  } else {
    write(1, ret, length);
  }
  free(ret);

  for (int i = 0; i < num_pages; ++i) {
    ret = jbig2_produce_page(ctx, i, -1, -1, &length);
    if (pdfmode) {
      char *filename;
      asprintf(&filename, "%s.%04d", basename, i);
      const int fd = open(filename, O_WRONLY | O_CREAT | O_TRUNC | WINBINARY, 0600);
      free(filename);
      if (fd < 0) abort();
      write(fd, ret, length);
      close(fd);
    } else {
      write(1, ret, length);
    }
    free(ret);
  }

  jbig2_destroy(ctx);
}

