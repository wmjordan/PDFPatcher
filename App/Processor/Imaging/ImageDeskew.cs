using System;
using System.Drawing;
using FreeImageAPI;
public class ImageDeskew
{
	// Representation of a line in the image.  
	private class HougLine
	{
		// Count of points in the line.
		public int Count;
		// Index in Matrix.
		public int Index;
		// The line is represented as all x,y that solve y*cos(alpha)-x*sin(alpha)=d
		public double Alpha;
	}


	// The range of angles to search for lines
	const double ALPHA_START = -20;
	const double ALPHA_STEP = 0.2;
	const int STEPS = 40 * 5;
	const double STEP = 1;

	// Precalculation of sin and cos.
	double[] _sinA;
	double[] _cosA;

	// Range of d
	double _min;


	int _count;
	// Count of points that fit in a line.
	int[] _hMatrix;

	// Calculate the skew angle of the image cBmp.
	public double GetSkewAngle (FreeImageBitmap image) {
		if (image.PixelFormat != System.Drawing.Imaging.PixelFormat.Format1bppIndexed) {
			image = image.GetColorConvertedInstance (FREE_IMAGE_COLOR_DEPTH.FICD_FORCE_GREYSCALE | FREE_IMAGE_COLOR_DEPTH.FICD_01_BPP_DITHER);
		}
		image.Save ("m:\\1.png");
		// Hough Transformation
		Calc (image);

		// Top 20 of the detected lines in the image.
		HougLine[] hl = GetTop (20);

		// Average angle of the lines
		double sum = 0;
		int count = 0;
		for (int i = 0; i <= 19; i++) {
			sum += hl[i].Alpha;
			count += 1;
		}
		return sum / count;
	}

	// Calculate the Count lines in the image with most points.
	private HougLine[] GetTop (int count) {
		HougLine[] hl = new HougLine[count];

		for (int i = 0; i <= count - 1; i++) {
			hl[i] = new HougLine ();
		}
		for (int i = 0; i <= _hMatrix.Length - 1; i++) {
			if (_hMatrix[i] > hl[count - 1].Count) {
				hl[count - 1].Count = _hMatrix[i];
				hl[count - 1].Index = i;
				int j = count - 1;
				while (j > 0 && hl[j].Count > hl[j - 1].Count) {
					HougLine tmp = hl[j];
					hl[j] = hl[j - 1];
					hl[j - 1] = tmp;
					j -= 1;
				}
			}
		}

		for (int i = 0; i <= count - 1; i++) {
			int dIndex = hl[i].Index / STEPS;
			int alphaIndex = hl[i].Index - dIndex * STEPS;
			hl[i].Alpha = GetAlpha (alphaIndex);
			//hl[i].D = dIndex + _min;
		}

		return hl;
	}


	// Hough Transforamtion:
	private void Calc (FreeImageBitmap image) {
		int hMin = image.Height / 4;
		int hMax = image.Height * 3 / 4;

		Init (image);
		Scanline<FI1BIT> l1, l2;

		for (int y = hMin; y <= hMax; y++) {
			l1 = image.GetScanline<FI1BIT> (y - 1);
			l2 = image.GetScanline<FI1BIT> (y);
			for (int x = 1; x <= image.Width - 2; x++) {
				// Only lower edges are considered.
				if (l1[x] == FI1BIT.MinValue) {
					if (l2[x] == FI1BIT.MaxValue) {
						Calc (x, y);
					}
				}
			}
		}
	}

	// Calculate all lines through the point (x,y).
	private void Calc (int x, int y) {
		int alpha;

		for (alpha = 0; alpha <= STEPS - 1; alpha++) {
			double d = y * _cosA[alpha] - x * _sinA[alpha];
			int calculatedIndex = (int)CalcDIndex (d);
			int index = calculatedIndex * STEPS + alpha;
			try {
				_hMatrix[index] += 1;
			}
			catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine (ex.ToString ());
			}
		}
	}
	private double CalcDIndex (double d) {
		return Convert.ToInt32 (d - _min);
	}

	private void Init (FreeImageBitmap image) {
		// Precalculation of sin and cos.
		_cosA = new double[STEPS];
		_sinA = new double[STEPS];

		for (int i = 0; i < STEPS; i++) {
			double angle = GetAlpha (i) * Math.PI / 180.0;
			_sinA[i] = Math.Sin (angle);
			_cosA[i] = Math.Cos (angle);
		}

		// Range of d:            
		_min = -image.Width;
		_count = (int)(2 * (image.Width + image.Height) / STEP);
		_hMatrix = new int[_count * STEPS];


	}

	private static double GetAlpha (int index) {
		return ALPHA_START + index * ALPHA_STEP;
	}
}


//// AForge Image Processing Library
//// AForge.NET framework
//// http://www.aforgenet.com/framework/
////
//// Copyright © Andrew Kirillov, 2005-2010
//// andrew.kirillov@aforgenet.com
////
//// Alejandro Pirola, 2008
//// alejamp@gmail.com
////

//namespace PDFPatcher.Processor
//{
//    using System;
//    using System.Collections;
//    using System.Drawing;
//    using System.Drawing.Imaging;
//    using FreeImageAPI;

//    /// <summary>
//    /// Hough line.
//    /// </summary>
//    /// 
//    /// <remarks><para>Represents line of Hough Line transformation using
//    /// <a href="http://en.wikipedia.org/wiki/Polar_coordinate_system">polar coordinates</a>.
//    /// See <a href="http://en.wikipedia.org/wiki/Polar_coordinate_system#Converting_between_polar_and_Cartesian_coordinates">Wikipedia</a>
//    /// for information on how to convert polar coordinates to Cartesian coordinates.
//    /// </para>
//    /// 
//    /// <para><note><see cref="HoughLineTransformation">Hough Line transformation</see> does not provide
//    /// information about lines start and end points, only slope and distance from image's center. Using
//    /// only provided information it is not possible to draw the detected line as it exactly appears on
//    /// the source image. But it is possible to draw a line through the entire image, which contains the
//    /// source line (see sample code below).
//    /// </note></para>
//    /// 
//    /// <para>Sample code to draw detected Hough lines:</para>
//    /// <code>
//    /// HoughLineTransformation lineTransform = new HoughLineTransformation( );
//    /// // apply Hough line transofrm
//    /// lineTransform.ProcessImage( sourceImage );
//    /// Bitmap houghLineImage = lineTransform.ToBitmap( );
//    /// // get lines using relative intensity
//    /// HoughLine[] lines = lineTransform.GetLinesByRelativeIntensity( 0.5 );
//    /// 
//    /// foreach ( HoughLine line in lines )
//    /// {
//    ///     // get line's radius and theta values
//    ///     int    r = line.Radius;
//    ///     double t = line.Theta;
//    ///     
//    ///     // check if line is in lower part of the image
//    ///     if ( r &lt; 0 )
//    ///     {
//    ///         t += 180;
//    ///         r = -r;
//    ///     }
//    ///     
//    ///     // convert degrees to radians
//    ///     t = ( t / 180 ) * Math.PI;
//    ///     
//    ///     // get image centers (all coordinate are measured relative
//    ///     // to center)
//    ///     int w2 = image.Width /2;
//    ///     int h2 = image.Height / 2;
//    ///     
//    ///     double x0 = 0, x1 = 0, y0 = 0, y1 = 0;
//    ///     
//    ///     if ( line.Theta != 0 )
//    ///     {
//    ///         // none-vertical line
//    ///         x0 = -w2; // most left point
//    ///         x1 = w2;  // most right point
//    ///     
//    ///         // calculate corresponding y values
//    ///         y0 = ( -Math.Cos( t ) * x0 + r ) / Math.Sin( t );
//    ///         y1 = ( -Math.Cos( t ) * x1 + r ) / Math.Sin( t );
//    ///     }
//    ///     else
//    ///     {
//    ///         // vertical line
//    ///         x0 = line.Radius;
//    ///         x1 = line.Radius;
//    ///     
//    ///         y0 = h2;
//    ///         y1 = -h2;
//    ///     }
//    ///     
//    ///     // draw line on the image
//    ///     Drawing.Line( sourceData,
//    ///         new IntPoint( (int) x0 + w2, h2 - (int) y0 ),
//    ///         new IntPoint( (int) x1 + w2, h2 - (int) y1 ),
//    ///         Color.Red );
//    /// }
//    /// </code>
//    /// 
//    /// <para>To clarify meaning of <see cref="Radius"/> and <see cref="Theta"/> values
//    /// of detected Hough lines, let's take a look at the below sample image and
//    /// corresponding values of radius and theta for the lines on the image:
//    /// </para>
//    /// 
//    /// <img src="img/imaging/sample15.png" width="400" height="300" />
//    /// 
//    /// <para>Detected radius and theta values (color in corresponding colors):
//    /// <list type="bullet">
//    /// <item><font color="#FF0000">Theta = 90, R = 125, I = 249</font>;</item>
//    /// <item><font color="#00FF00">Theta = 0, R = -170, I = 187</font> (converts to Theta = 180, R = 170);</item>
//    /// <item><font color="#0000FF">Theta = 90, R = -58, I = 163</font> (converts to Theta = 270, R = 58);</item>
//    /// <item><font color="#FFFF00">Theta = 101, R = -101, I = 130</font> (converts to Theta = 281, R = 101);</item>
//    /// <item><font color="#FF8000">Theta = 0, R = 43, I = 112</font>;</item>
//    /// <item><font color="#FF80FF">Theta = 45, R = 127, I = 82</font>.</item>
//    /// </list>
//    /// </para>
//    /// 
//    /// </remarks>
//    /// 
//    /// <seealso cref="HoughLineTransformation"/>
//    /// 
//    public class HoughLine : IComparable
//    {
//        /// <summary>
//        /// Line's slope - angle between polar axis and line's radius (normal going
//        /// from pole to the line). Measured in degrees, [0, 180).
//        /// </summary>
//        public readonly double Theta;

//        /// <summary>
//        /// Line's distance from image center, (−∞, +∞).
//        /// </summary>
//        /// 
//        /// <remarks><note>Negative line's radius means, that the line resides in lower
//        /// part of the polar coordinates system. This means that <see cref="Theta"/> value
//        /// should be increased by 180 degrees and radius should be made positive.
//        /// </note></remarks>
//        /// 
//        public readonly short Radius;

//        /// <summary>
//        /// Line's absolute intensity, (0, +∞).
//        /// </summary>
//        /// 
//        /// <remarks><para>Line's absolute intensity is a measure, which equals
//        /// to number of pixels detected on the line. This value is bigger for longer
//        /// lines.</para>
//        /// 
//        /// <para><note>The value may not be 100% reliable to measure exact number of pixels
//        /// on the line. Although these value correlate a lot (which means they are very close
//        /// in most cases), the intensity value may slightly vary.</note></para>
//        /// </remarks>
//        /// 
//        public readonly short Intensity;

//        /// <summary>
//        /// Line's relative intensity, (0, 1].
//        /// </summary>
//        /// 
//        /// <remarks><para>Line's relative intensity is relation of line's <see cref="Intensity"/>
//        /// value to maximum found intensity. For the longest line (line with highest intesity) the
//        /// relative intensity is set to 1. If line's relative is set 0.5, for example, this means
//        /// its intensity is half of maximum found intensity.</para>
//        /// </remarks>
//        /// 
//        public readonly double RelativeIntensity;

//        /// <summary>
//        /// Initializes a new instance of the <see cref="HoughLine"/> class.
//        /// </summary>
//        /// 
//        /// <param name="theta">Line's slope.</param>
//        /// <param name="radius">Line's distance from image center.</param>
//        /// <param name="intensity">Line's absolute intensity.</param>
//        /// <param name="relativeIntensity">Line's relative intensity.</param>
//        /// 
//        public HoughLine (double theta, short radius, short intensity, double relativeIntensity) {
//            Theta = theta;
//            Radius = radius;
//            Intensity = intensity;
//            RelativeIntensity = relativeIntensity;
//        }

//        /// <summary>
//        /// Compare the object with another instance of this class.
//        /// </summary>
//        /// 
//        /// <param name="value">Object to compare with.</param>
//        /// 
//        /// <returns><para>A signed number indicating the relative values of this instance and <b>value</b>: 1) greater than zero - 
//        /// this instance is greater than <b>value</b>; 2) zero - this instance is equal to <b>value</b>;
//        /// 3) greater than zero - this instance is less than <b>value</b>.</para>
//        /// 
//        /// <para><note>The sort order is descending.</note></para></returns>
//        /// 
//        /// <remarks>
//        /// <para><note>Object are compared using their <see cref="Intensity">intensity</see> value.</note></para>
//        /// </remarks>
//        /// 
//        public int CompareTo (object value) {
//            return (-Intensity.CompareTo (((HoughLine)value).Intensity));
//        }
//    }

//    /// <summary>
//    /// Skew angle checker for scanned documents.
//    /// </summary>
//    ///
//    /// <remarks><para>The class implements document's skew checking algorithm, which is based
//    /// on <see cref="HoughLineTransformation">Hough line transformation</see>. The algorithm
//    /// is based on searching for text base lines - black line of text bottoms' followed
//    /// by white line below.</para>
//    /// 
//    /// <para><note>The routine supposes that a white-background document is provided
//    /// with black letters. The algorithm is not supposed for any type of objects, but for
//    /// document images with text.</note></para>
//    /// 
//    /// <para>The range of angles to detect is controlled by <see cref="MaxSkewToDetect"/> property.</para>
//    /// 
//    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
//    /// 
//    /// <para>Sample usage:</para>
//    /// <code>
//    /// // create instance of skew checker
//    /// DocumentSkewChecker skewChecker = new DocumentSkewChecker( );
//    /// // get documents skew angle
//    /// double angle = skewChecker.GetSkewAngle( documentImage );
//    /// // create rotation filter
//    /// RotateBilinear rotationFilter = new RotateBilinear( -angle );
//    /// rotationFilter.FillColor = Color.White;
//    /// // rotate image applying the filter
//    /// Bitmap rotatedImage = rotationFilter.Apply( documentImage );
//    /// </code>
//    /// 
//    /// <para><b>Initial image:</b></para>
//    /// <img src="img/imaging/sample10.png" width="300" height="184" />
//    /// <para><b>Deskewed image:</b></para>
//    /// <img src="img/imaging/deskew.png" width="335" height="250" /> 
//    /// </remarks>
//    /// 
//    /// <seealso cref="HoughLineTransformation"/>
//    ///
//    public class ImageDeskew
//    {
//        // Hough transformation: quality settings
//        private int stepsPerDegree;
//        private int houghHeight;
//        private double thetaStep;
//        private double maxSkewToDetect;

//        // Hough transformation: precalculated Sine and Cosine values
//        private double[] sinMap;
//        private double[] cosMap;
//        private bool needToInitialize = true;

//        // Hough transformation: Hough map
//        private short[,] houghMap;
//        private short maxMapIntensity = 0;

//        private int localPeakRadius = 4;
//        private ArrayList lines = new ArrayList ();

//        /// <summary>
//        /// Steps per degree, [1, 10].
//        /// </summary>
//        /// 
//        /// <remarks><para>The value defines quality of Hough transform and its ability to detect
//        /// line slope precisely.</para>
//        /// 
//        /// <para>Default value is set to <b>1</b>.</para>
//        /// </remarks>
//        /// 
//        public int StepsPerDegree {
//            get { return stepsPerDegree; }
//            set {
//                stepsPerDegree = Math.Max (1, Math.Min (10, value));
//                needToInitialize = true;
//            }
//        }

//        /// <summary>
//        /// Maximum skew angle to detect, [0, 45] degrees.
//        /// </summary>
//        /// 
//        /// <remarks><para>The value sets maximum document's skew angle to detect.
//        /// Document's skew angle can be as positive (rotated counter clockwise), as negative
//        /// (rotated clockwise). So setting this value to 25, for example, will lead to
//        /// [-25, 25] degrees detection range.</para>
//        ///
//        /// <para>Scanned documents usually have skew in the [-20, 20] degrees range.</para>
//        /// 
//        /// <para>Default value is set to <b>30</b>.</para>
//        /// </remarks>
//        /// 
//        public double MaxSkewToDetect {
//            get { return maxSkewToDetect; }
//            set {
//                maxSkewToDetect = Math.Max (0, Math.Min (45, value));
//                needToInitialize = true;
//            }
//        }

//        /// <summary>
//        /// Minimum angle to detect skew in degrees.
//        /// </summary>
//        ///
//        /// <remarks><para><note>The property is deprecated and setting it has not any effect.
//        /// Use <see cref="MaxSkewToDetect"/> property instead.</note></para></remarks>
//        ///
//        [Obsolete ("The property is deprecated and setting it has not any effect. Use MaxSkewToDetect property instead.")]
//        public double MinBeta {
//            get { return (-maxSkewToDetect); }
//            set { }
//        }

//        /// <summary>
//        /// Maximum angle to detect skew in degrees.
//        /// </summary>
//        ///
//        /// <remarks><para><note>The property is deprecated and setting it has not any effect.
//        /// Use <see cref="MaxSkewToDetect"/> property instead.</note></para></remarks>
//        ///
//        [Obsolete ("The property is deprecated and setting it has not any effect. Use MaxSkewToDetect property instead.")]
//        public double MaxBeta {
//            get { return (maxSkewToDetect); }
//            set { }
//        }

//        /// <summary>
//        /// Radius for searching local peak value, [1, 10].
//        /// </summary>
//        /// 
//        /// <remarks><para>The value determines radius around a map's value, which is analyzed to determine
//        /// if the map's value is a local maximum in specified area.</para>
//        /// 
//        /// <para>Default value is set to <b>4</b>.</para></remarks>
//        /// 
//        public int LocalPeakRadius {
//            get { return localPeakRadius; }
//            set { localPeakRadius = Math.Max (1, Math.Min (10, value)); }
//        }

//        /// <summary>
//        /// Initializes a new instance of the <see cref="DocumentSkewChecker"/> class.
//        /// </summary>
//        public ImageDeskew () {
//            StepsPerDegree = 10;
//            MaxSkewToDetect = 30;
//        }

//        /// <summary>
//        /// Get skew angle of the provided document image.
//        /// </summary>
//        /// 
//        /// <param name="image">Document's image to get skew angle of.</param>
//        /// 
//        /// <returns>Returns document's skew angle. If the returned angle equals to -90,
//        /// then document skew detection has failed.</returns>
//        /// 
//        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
//        /// 
//        public double GetSkewAngle (FreeImageBitmap image) {
//            return GetSkewAngle (image, new Rectangle (0, 0, image.Width, image.Height));
//        }

//        /// <summary>
//        /// Get skew angle of the provided document image.
//        /// </summary>
//        /// 
//        /// <param name="image">Document's image to get skew angle of.</param>
//        /// <param name="rect">Image's rectangle to process (used to exclude processing of
//        /// regions, which are not relevant to skew detection).</param>
//        /// 
//        /// <returns>Returns document's skew angle. If the returned angle equals to -90,
//        /// then document skew detection has failed.</returns>
//        /// 
//        /// <exception cref="UnsupportedImageFormatException">Unsupported pixel format of the source image.</exception>
//        /// 
//        public double GetSkewAngle (FreeImageBitmap image, Rectangle rect) {
//            //image = image.GetColorConvertedInstance (FREE_IMAGE_COLOR_DEPTH.FICD_FORCE_GREYSCALE | FREE_IMAGE_COLOR_DEPTH.FICD_08_BPP);

//            // init hough transformation settings
//            InitHoughMap ();

//            // get source image size
//            int width = image.Width;
//            int height = image.Height;
//            int halfWidth = width / 2;
//            int halfHeight = height / 2;

//            // make sure the specified rectangle recides with the source image
//            rect.Intersect (new Rectangle (0, 0, width, height));

//            int startX = -halfWidth + rect.Left;
//            int startY = -halfHeight + rect.Top;
//            int stopX = width - halfWidth - (width - rect.Right);
//            int stopY = height - halfHeight - (height - rect.Bottom) - 1;

//            int offset = image.Stride - rect.Width;

//            // calculate Hough map's width
//            int halfHoughWidth = (int)Math.Sqrt (halfWidth * halfWidth + halfHeight * halfHeight);
//            int houghWidth = halfHoughWidth * 2;

//            houghMap = new short[houghHeight, houghWidth];

//            #region do the job
//            //using (Bitmap bmp = image.ToBitmap ()) {
//            //    BitmapData d = bmp.LockBits (new Rectangle (0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
//            //    unsafe {
//            //        byte* src = (byte*)d.Scan0.ToPointer () +
//            //            rect.Top * d.Stride + rect.Left;
//            //        byte* srcBelow = src + d.Stride;

//            //        // for each row
//            //        for (int y = startY; y < stopY; y++) {
//            //            // for each pixel
//            //            for (int x = startX; x < stopX; x++, src++, srcBelow++) {
//            //                // if current pixel is more black
//            //                // and pixel below is more white
//            //                if ((*src < 128) && (*srcBelow >= 128)) {
//            //                    // for each Theta value
//            //                    for (int theta = 0; theta < houghHeight; theta++) {
//            //                        int radius = (int)(cosMap[theta] * x - sinMap[theta] * y) + halfHoughWidth;

//            //                        if ((radius < 0) || (radius >= houghWidth))
//            //                            continue;

//            //                        houghMap[theta, radius]++;
//            //                    }
//            //                }
//            //            }
//            //            src += offset;
//            //            srcBelow += offset;
//            //        }
//            //    }
//            //}
//            #endregion
//            Scanline<byte> src, srcBelow;

//            // for each row
//            for (int y = startY, l = height - 1; y < stopY; y++, l--) {
//                src = image.GetScanline<byte> (l - 1);
//                srcBelow = image.GetScanline<byte> (l);
//                // for each pixel
//                for (int x = startX, i = 0; x < stopX; x++, i++) {
//                    // if current pixel is more black
//                    // and pixel below is more white
//                    if ((src[i] < 128) && (srcBelow[i] >= 128)) {
//                        // for each Theta value
//                        for (int theta = 0; theta < houghHeight; theta++) {
//                            int radius = (int)(cosMap[theta] * x - sinMap[theta] * y) + halfHoughWidth;

//                            if ((radius < 0) || (radius >= houghWidth))
//                                continue;

//                            houghMap[theta, radius]++;
//                        }
//                    }
//                }
//            }

//            // find max value in Hough map
//            maxMapIntensity = 0;
//            for (int i = 0; i < houghHeight; i++) {
//                for (int j = 0; j < houghWidth; j++) {
//                    if (houghMap[i, j] > maxMapIntensity) {
//                        maxMapIntensity = houghMap[i, j];
//                    }
//                }
//            }

//            CollectLines ((short)(width / 10));

//            // get skew angle
//            HoughLine[] hls = this.GetMostIntensiveLines (5);

//            double skewAngle = 0;
//            double sumIntensity = 0;

//            foreach (HoughLine hl in hls) {
//                if (hl.RelativeIntensity > 0.5) {
//                    skewAngle += (hl.Theta * hl.RelativeIntensity);
//                    sumIntensity += hl.RelativeIntensity;
//                }
//            }
//            if (hls.Length > 0) skewAngle = skewAngle / sumIntensity;

//            return skewAngle - 90.0;
//        }

//        // Get specified amount of lines with highest intensity
//        private HoughLine[] GetMostIntensiveLines (int count) {
//            // lines count
//            int n = Math.Min (count, lines.Count);

//            // result array
//            HoughLine[] dst = new HoughLine[n];
//            lines.CopyTo (0, dst, 0, n);

//            return dst;
//        }

//        // Collect lines with intesities greater or equal then specified
//        private void CollectLines (short minLineIntensity) {
//            int maxTheta = houghMap.GetLength (0);
//            int maxRadius = houghMap.GetLength (1);

//            short intensity;
//            bool foundGreater;

//            int halfHoughWidth = maxRadius >> 1;

//            // clean lines collection
//            lines.Clear ();

//            // for each Theta value
//            for (int theta = 0; theta < maxTheta; theta++) {
//                // for each Radius value
//                for (int radius = 0; radius < maxRadius; radius++) {
//                    // get current value
//                    intensity = houghMap[theta, radius];

//                    if (intensity < minLineIntensity)
//                        continue;

//                    foundGreater = false;

//                    // check neighboors
//                    for (int tt = theta - localPeakRadius, ttMax = theta + localPeakRadius; tt < ttMax; tt++) {
//                        // skip out of map values
//                        if (tt < 0)
//                            continue;
//                        if (tt >= maxTheta)
//                            break;

//                        // break if it is not local maximum
//                        if (foundGreater == true)
//                            break;

//                        for (int tr = radius - localPeakRadius, trMax = radius + localPeakRadius; tr < trMax; tr++) {
//                            // skip out of map values
//                            if (tr < 0)
//                                continue;
//                            if (tr >= maxRadius)
//                                break;

//                            // compare the neighboor with current value
//                            if (houghMap[tt, tr] > intensity) {
//                                foundGreater = true;
//                                break;
//                            }
//                        }
//                    }

//                    // was it local maximum ?
//                    if (!foundGreater) {
//                        // we have local maximum
//                        lines.Add (new HoughLine (90.0 - maxSkewToDetect + (double)theta / stepsPerDegree, (short)(radius - halfHoughWidth), intensity, (double)intensity / maxMapIntensity));
//                    }
//                }
//            }

//            lines.Sort ();
//        }

//        // Init Hough settings and map
//        private void InitHoughMap () {
//            if (needToInitialize) {
//                needToInitialize = false;

//                houghHeight = (int)(2 * maxSkewToDetect * stepsPerDegree);
//                thetaStep = (2 * maxSkewToDetect * Math.PI / 180) / houghHeight;

//                // precalculate Sine and Cosine values
//                sinMap = new double[houghHeight];
//                cosMap = new double[houghHeight];

//                double minTheta = 90.0 - maxSkewToDetect;

//                for (int i = 0; i < houghHeight; i++) {
//                    sinMap[i] = Math.Sin ((minTheta * Math.PI / 180) + (i * thetaStep));
//                    cosMap[i] = Math.Cos ((minTheta * Math.PI / 180) + (i * thetaStep));
//                }
//            }
//        }
//    }
//}

/*namespace PDFPatcher.Processor
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Diagnostics;
	using System.Drawing;
	using System.Drawing.Imaging;
	using FreeImageAPI;

	//public class main
	//{
	//    public static void Main () {
	//        string fnIn = "d:\\skewsample_in.tif";
	//        string fnOut = "d:\\skewsample_out.tif";
	//        Bitmap bmpIn = new Bitmap (fnIn);
	//        ImageDeskew sk = new ImageDeskew (bmpIn);
	//        double skewangle = sk.GetSkewAngle ();
	//        Bitmap bmpOut = RotateImage (bmpIn, -skewangle);
	//        bmpOut.Save (fnOut, ImageFormat.Tiff);
	//        Interaction.MsgBox ("Skewangle: " + skewangle);
	//    }

	//    private static Bitmap RotateImage (Bitmap bmp, double angle) {
	//        Graphics g = null;
	//        Bitmap tmp = new Bitmap (bmp.Width, bmp.Height, PixelFormat.Format32bppRgb);

	//        tmp.SetResolution (bmp.HorizontalResolution, bmp.VerticalResolution);
	//        g = Graphics.FromImage (tmp);
	//        try {
	//            g.FillRectangle (Brushes.White, 0, 0, bmp.Width, bmp.Height);
	//            g.RotateTransform ((float)angle);
	//            g.DrawImage (bmp, 0, 0);
	//        }
	//        finally {
	//            g.Dispose ();
	//        }
	//        return tmp;
	//    }
	//}

	public class ImageDeskew
	{
		// Representation of a line in the image.
		public class HougLine
		{
			// Count of points in the line.
			public int Count;
			// Index in Matrix.
			public int Index;
			// The line is represented as all x,y that solve y*cos(alpha)-x*sin(alpha)=d
			public double Alpha;
			public double d;
		}
		// The Bitmap
		FreeImageBitmap cBmp;
		// The range of angles to search for lines
		double cAlphaStart = -20;
		double cAlphaStep = 0.2;
		int cSteps = 40 * 5;
		// Precalculation of sin and cos.
		double[] cSinA;
		double[] cCosA;
		// Range of d
		double cDMin;
		double cDStep = 1;
		int cDCount;
		// Count of points that fit in a line.

		int[] cHMatrix;
		public static double GetSkewAngle (FreeImageBitmap bmp) {
			return new ImageDeskew (bmp).GetSkewAngle ();
		}

		// Calculate the skew angle of the image cBmp.
		private double GetSkewAngle () {
			ImageDeskew.HougLine[] hl = null;
			int i = 0;
			double sum = 0;
			int count = 0;

			// Hough Transformation
			Calc ();
			// Top 20 of the detected lines in the image.
			hl = GetTop (20);
			// Average angle of the lines
			for (i = 0; i <= 19; i++) {
				sum += hl[i].Alpha;
				count += 1;
			}
			return sum / count;
		}

		// Calculate the Count lines in the image with most points.
		private HougLine[] GetTop (int Count) {
			HougLine[] hl = null;
			int i = 0;
			int j = 0;
			HougLine tmp = null;
			int AlphaIndex = 0;
			int dIndex = 0;

			hl = new HougLine[Count + 1];
			for (i = 0; i <= Count - 1; i++) {
				hl[i] = new HougLine ();
			}
			for (i = 0; i <= cHMatrix.Length - 1; i++) {
				if (cHMatrix[i] > hl[Count - 1].Count) {
					hl[Count - 1].Count = cHMatrix[i];
					hl[Count - 1].Index = i;
					j = Count - 1;
					while (j > 0 && hl[j].Count > hl[j - 1].Count) {
						tmp = hl[j];
						hl[j] = hl[j - 1];
						hl[j - 1] = tmp;
						j -= 1;
					}
				}
			}
			for (i = 0; i <= Count - 1; i++) {
				dIndex = hl[i].Index / cSteps;
				AlphaIndex = hl[i].Index - dIndex * cSteps;
				hl[i].Alpha = GetAlpha (AlphaIndex);
				hl[i].d = dIndex + cDMin;
			}
			return hl;
		}
		private ImageDeskew (FreeImageBitmap bmp) {
			cBmp = bmp;
		}
		// Hough Transforamtion:
		private void Calc () {
			int x = 0;
			int y = 0;
			int hMin = cBmp.Height / 4;
			int hMax = cBmp.Height * 3 / 4;

			Init ();
			for (y = hMin; y <= hMax; y++) {
				for (x = 1; x <= cBmp.Width - 2; x++) {
					// Only lower edges are considered.
					if (IsBlack (x, y)) {
						if (!IsBlack (x, y + 1)) {
							Calc (x, y);
						}
					}
				}
			}
		}
		// Calculate all lines through the point (x,y).
		private void Calc (int x, int y) {
			int alpha = 0;
			double d = 0;
			int dIndex = 0;
			int Index = 0;

			for (alpha = 0; alpha <= cSteps - 1; alpha++) {
				d = y * cCosA[alpha] - x * cSinA[alpha];
				dIndex = (int)CalcDIndex (d);
				Index = dIndex * cSteps + alpha;
				try {
					cHMatrix[Index] += 1;
				}
				catch (Exception ex) {
					Debug.WriteLine (ex.ToString ());
				}
			}
		}
		private double CalcDIndex (double d) {
			return Convert.ToInt32 (d - cDMin);
		}
		private bool IsBlack (int x, int y) {
			Color c = cBmp.GetPixel (x, y);
			double luminance = (c.R * 0.299) + (c.G * 0.587) + (c.B * 0.114);
			return luminance < 140;
		}
		private void Init () {
			int i = 0;
			double angle = 0;

			// Precalculation of sin and cos.
			cSinA = new double[cSteps];
			cCosA = new double[cSteps];
			for (i = 0; i <= cSteps - 1; i++) {
				angle = GetAlpha (i) * Math.PI / 180.0;
				cSinA[i] = Math.Sin (angle);
				cCosA[i] = Math.Cos (angle);
			}
			// Range of d:
			cDMin = -cBmp.Width;
			cDCount = (int)(2 * (cBmp.Width + cBmp.Height) / cDStep);
			cHMatrix = new int[cDCount * cSteps + 1];
		}

		private double GetAlpha (int Index) {
			return cAlphaStart + Index * cAlphaStep;
		}
	}
}
*/