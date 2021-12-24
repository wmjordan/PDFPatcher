#include <stdint.h>
struct Pix
{
    uint32_t             w;           /* width in pixels                   */
    uint32_t             h;           /* height in pixels                  */
    int32_t              xres;        /* image res (ppi) in x direction    */
                                      /* (use 0 if unknown)                */
    int32_t              yres;        /* image res (ppi) in y direction    */
                                      /* (use 0 if unknown)                */
    uint32_t            *data;        /* the image data                    */
};
typedef struct Pix PIX;