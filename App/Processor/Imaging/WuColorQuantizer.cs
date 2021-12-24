using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace PDFPatcher.Processor.Imaging
{
	public static class WuQuantizer
	{
		const int __MaxColor = 256;
		const int __Red = 2;
		const int __Green = 1;
		const int __Blue = 0;
		const int __SideSize = 33;
		const int __MaxSideIndex = 32;

		public static Bitmap QuantizeImage (Bitmap image) {
			var colorCount = __MaxColor;
			var data = BuildHistogram (image);
			CalculateMoments (data);
			var cubes = SplitData (ref colorCount, data);
			var palette = GetQuantizedPalette (colorCount, data, cubes);
			return ProcessImagePixels (image, palette);
		}

		private static Bitmap ProcessImagePixels (Image sourceImage, QuantizedPalette palette) {
			var result = new Bitmap (sourceImage.Width, sourceImage.Height, PixelFormat.Format8bppIndexed);
			var newPalette = result.Palette;
			palette.Colors.CopyTo (newPalette.Entries, 0);
			result.Palette = newPalette;

			BitmapData targetData = null;
			try {
				targetData = result.LockBits (Rectangle.FromLTRB (0, 0, result.Width, result.Height), ImageLockMode.WriteOnly, result.PixelFormat);
				var targetByteLength = targetData.Stride < 0 ? -targetData.Stride : targetData.Stride;
				var targetSize = targetByteLength * result.Height;
				var targetOffset = 0;
				var targetBuffer = new byte[targetSize];
				var pixelIndex = 0;
				var pil = palette.PixelIndex;
				int rw = result.Width, rh = result.Height;
				var empty = palette.Colors.Count - 1;
				for (var y = 0; y < rh; y++) {
					for (var x = 0; x < rw; x++) {
						var pv = pil[pixelIndex];
						targetBuffer[targetOffset + x] = (byte)(pv == -1 ? empty : pv);
						pixelIndex++;
					}

					targetOffset += targetByteLength;
				}

				System.Runtime.InteropServices.Marshal.Copy (targetBuffer, 0, targetData.Scan0, targetSize);
			}
			finally {
				if (targetData != null)
					result.UnlockBits (targetData);
			}

			return result;
		}

		private static ColorData BuildHistogram (Bitmap sourceImage) {
			int bitmapWidth = sourceImage.Width;
			int bitmapHeight = sourceImage.Height;

			var data = sourceImage.LockBits (false);
			var colorData = new ColorData (__MaxSideIndex, bitmapWidth, bitmapHeight);

			try {
				var bitDepth = Image.GetPixelFormatSize (sourceImage.PixelFormat);
				if (bitDepth != 32 && bitDepth != 24)
					throw new QuantizationException (string.Format ("The image you are attempting to quantize does not contain a 32 bit ARGB palette. This image has a bit depth of {0} with {1} colors.", bitDepth, sourceImage.Palette.Entries.Length));
				var byteLength = data.Stride < 0 ? -data.Stride : data.Stride;
				var byteCount = Math.Max (1, bitDepth >> 3);
				var offset = 0;
				var buffer = new Byte[byteLength * sourceImage.Height];

				System.Runtime.InteropServices.Marshal.Copy (data.Scan0, buffer, 0, buffer.Length);
				var t = new int[__MaxColor];
				for (var i = 0; i < __MaxColor; ++i) {
					t[i] = i * i;
				}
				int pos;
				byte vr, vg, vb, r, g, b;
				var w = colorData.Weights;
				var mr = colorData.MomentsRed;
				var mg = colorData.MomentsGreen;
				var mb = colorData.MomentsBlue;
				var m = colorData.Moments;
				for (int y = 0; y < bitmapHeight; y++) {
					var index = 0;
					for (int x = 0; x < bitmapWidth; x++) {
						var indexOffset = offset + (index >> 3);

						vr = buffer[indexOffset + __Red];
						vg = buffer[indexOffset + __Green];
						vb = buffer[indexOffset + __Blue];
						r = (byte)((vr >> 3) + 1);
						g = (byte)((vg >> 3) + 1);
						b = (byte)((vb >> 3) + 1);
						pos = (r << 10) + (r << 6) + r + (g << 5) + g + b; // [r,g,b]
						w[pos]++;
						mr[pos] += vr;
						mg[pos] += vg;
						mb[pos] += vb;
						m[pos] += (float)(t[vr] + t[vg] + t[vb]);

						colorData.AddPixel (
							new Pixel (vr, vg, vb),
							((r << 16) + (g << 8) + b)
							);
						index += bitDepth;

					}

					offset += byteLength;
				}
			}
			finally {
				sourceImage.UnlockBits (data);
			}
			return colorData;
		}

		private static void CalculateMoments (ColorData data) {
			var w = data.Weights;
			var mr = data.MomentsRed;
			var mg = data.MomentsGreen;
			var mb = data.MomentsBlue;
			var m = data.Moments;
			for (var r = 1; r <= __MaxSideIndex; ++r) {
				var area = new long[__SideSize];
				var areaRed = new long[__SideSize];
				var areaGreen = new long[__SideSize];
				var areaBlue = new long[__SideSize];
				var area2 = new float[__SideSize];
				for (var g = 1; g <= __MaxSideIndex; ++g) {
					long line = 0;
					long lineRed = 0;
					long lineGreen = 0;
					long lineBlue = 0;
					var line2 = 0.0f;
					for (var b = 1; b <= __MaxSideIndex; ++b) {
						var pos = (r << 10) + (r << 6) + r + (g << 5) + g + b; // [r,g,b]
						line += w[pos];
						lineRed += mr[pos];
						lineGreen += mg[pos];
						lineBlue += mb[pos];
						line2 += m[pos];

						area[b] += line;
						areaRed[b] += lineRed;
						areaGreen[b] += lineGreen;
						areaBlue[b] += lineBlue;
						area2[b] += line2;

						var pos2 = pos - __MaxSideIndex * __MaxSideIndex; // [r-1,g,b]
						w[pos] = w[pos2] + area[b];
						mr[pos] = mr[pos2] + areaRed[b];
						mg[pos] = mg[pos2] + areaGreen[b];
						mb[pos] = mb[pos2] + areaBlue[b];
						m[pos] = m[pos2] + area2[b];
					}
				}
			}
		}

		private static long Top (Box cube, int direction, int position, long[] moment) {
			int r0 = cube.RedMinimum,
				r1 = cube.RedMaximum,
				g0 = cube.GreenMinimum,
				g1 = cube.GreenMaximum,
				b0 = cube.BlueMinimum,
				b1 = cube.BlueMaximum;
			switch (direction) {
				case __Red:
					position = (position << 10) + (position << 6) + position;
					g0 = (g0 << 5) + g0;
					g1 = (g1 << 5) + g1;
					return
						-moment[position + g1 + b1]
						+ moment[position + g1 + b0]
						+ moment[position + g0 + b1]
						- moment[position + g0 + b0];
				case __Green:
					r0 = (r0 << 10) + (r0 << 6) + r0;
					r1 = (r1 << 10) + (r1 << 6) + r1;
					position = (position << 5) + position;
					return
						-moment[position + r1 + b1]
						+ moment[position + r1 + b0]
						+ moment[position + r0 + b1]
						- moment[position + r0 + b0];
				case __Blue:
					r0 = (r0 << 10) + (r0 << 6) + r0;
					r1 = (r1 << 10) + (r1 << 6) + r1;
					g0 = (g0 << 5) + g0;
					g1 = (g1 << 5) + g1;
					return
						-moment[position + r1 + g1]
						+ moment[position + r1 + g0]
						+ moment[position + r0 + g1]
						- moment[position + r0 + g0];
				default:
					return 0;
			}
		}

		private static long Bottom (Box cube, int direction, long[] moment) {
			int r0 = cube.RedMinimum, r1 = cube.RedMaximum,
				g0 = cube.GreenMinimum, g1 = cube.GreenMaximum,
				b0 = cube.BlueMinimum, b1 = cube.BlueMaximum;
			r0 = (r0 << 10) + (r0 << 6) + r0;
			r1 = (r1 << 10) + (r1 << 6) + r1;
			g0 = (g0 << 5) + g0;
			g1 = (g1 << 5) + g1;
			switch (direction) {
				case __Red:
					return
						-moment[r0 + g1 + b1]
						+ moment[r0 + g1 + b0]
						+ moment[r0 + g0 + b1]
						- moment[r0 + g0 + b0];
				case __Green:
					return
						-moment[r1+ g0 + b1]
						+ moment[r1 + g0 + b0]
						+ moment[r0 + g0 + b1]
						- moment[r0 + g0 + b0];
				case __Blue:
					return
						-moment[r1 + g1 + b0]
						+ moment[r1 + g0 + b0]
						+ moment[r0 + g1 + b0]
						- moment[r0 + g0 + b0];
				default:
					return 0;
			}
		}

		private static CubeCut Maximize (ColorData data, Box cube, int direction, byte first, byte last, long wholeRed, long wholeGreen, long wholeBlue, long wholeWeight) {
			var bottomRed = Bottom (cube, direction, data.MomentsRed);
			var bottomGreen = Bottom (cube, direction, data.MomentsGreen);
			var bottomBlue = Bottom (cube, direction, data.MomentsBlue);
			var bottomWeight = Bottom (cube, direction, data.Weights);

			var result = 0.0f;
			bool canSplit = false;
			byte cutPoint = 0;

			for (var position = first; position < last; ++position) {
				var halfRed = bottomRed + Top (cube, direction, position, data.MomentsRed);
				var halfGreen = bottomGreen + Top (cube, direction, position, data.MomentsGreen);
				var halfBlue = bottomBlue + Top (cube, direction, position, data.MomentsBlue);
				var halfWeight = bottomWeight + Top (cube, direction, position, data.Weights);

				if (halfWeight == 0) continue;

				var halfDistance = halfRed * halfRed + halfGreen * halfGreen + halfBlue * halfBlue;
				var temp = halfDistance / halfWeight;

				halfRed = wholeRed - halfRed;
				halfGreen = wholeGreen - halfGreen;
				halfBlue = wholeBlue - halfBlue;
				halfWeight = wholeWeight - halfWeight;

				if (halfWeight != 0) {
					halfDistance = halfRed * halfRed + halfGreen * halfGreen + halfBlue * halfBlue;
					temp += halfDistance / halfWeight;

					if (temp > result) {
						result = temp;
						canSplit = true;
						cutPoint = position;
					}
				}
			}

			return new CubeCut (canSplit, cutPoint, result);
		}

		private static bool Cut (ColorData data, ref Box first, ref Box second) {
			int direction;
			var wholeRed = Volume (first, data.MomentsRed);
			var wholeGreen = Volume (first, data.MomentsGreen);
			var wholeBlue = Volume (first, data.MomentsBlue);
			var wholeWeight = Volume (first, data.Weights);

			var maxRed = Maximize (data, first, __Red, (byte)(first.RedMinimum + 1), first.RedMaximum, wholeRed, wholeGreen, wholeBlue, wholeWeight);
			var maxGreen = Maximize (data, first, __Green, (byte)(first.GreenMinimum + 1), first.GreenMaximum, wholeRed, wholeGreen, wholeBlue, wholeWeight);
			var maxBlue = Maximize (data, first, __Blue, (byte)(first.BlueMinimum + 1), first.BlueMaximum, wholeRed, wholeGreen, wholeBlue, wholeWeight);

			if ((maxRed.Value >= maxGreen.Value) && (maxRed.Value >= maxBlue.Value)) {
				direction = __Red;
				if (maxRed.CanSplit == false) {
					return false;
				}
			}
			else {
				if ((maxGreen.Value >= maxRed.Value) && (maxGreen.Value >= maxBlue.Value))
					direction = __Green;
				else
					direction = __Blue;
			}

			second.RedMaximum = first.RedMaximum;
			second.GreenMaximum = first.GreenMaximum;
			second.BlueMaximum = first.BlueMaximum;

			switch (direction) {
				case __Red:
					second.RedMinimum = first.RedMaximum = maxRed.Position;
					second.GreenMinimum = first.GreenMinimum;
					second.BlueMinimum = first.BlueMinimum;
					break;

				case __Green:
					second.GreenMinimum = first.GreenMaximum = maxGreen.Position;
					second.RedMinimum = first.RedMinimum;
					second.BlueMinimum = first.BlueMinimum;
					break;

				case __Blue:
					second.BlueMinimum = first.BlueMaximum = maxBlue.Position;
					second.RedMinimum = first.RedMinimum;
					second.GreenMinimum = first.GreenMinimum;
					break;
			}

			first.Volume = (first.RedMaximum - first.RedMinimum) * (first.GreenMaximum - first.GreenMinimum) * (first.BlueMaximum - first.BlueMinimum);
			second.Volume = (second.RedMaximum - second.RedMinimum) * (second.GreenMaximum - second.GreenMinimum) * (second.BlueMaximum - second.BlueMinimum);

			return true;
		}

		private static float CalculateVariance (ColorData data, Box cube) {
			float volumeRed = Volume (cube, data.MomentsRed);
			float volumeGreen = Volume (cube, data.MomentsGreen);
			float volumeBlue = Volume (cube, data.MomentsBlue);
			float volumeMoment = VolumeFloat (cube, data.Moments);
			float volumeWeight = Volume (cube, data.Weights);

			float distance = volumeRed * volumeRed + volumeGreen * volumeGreen + volumeBlue * volumeBlue;

			var result = volumeMoment - distance / volumeWeight;
			return double.IsNaN (result) ? 0.0f : result;
		}

		private static long Volume (Box cube, long[] moment) {
			int r0 = cube.RedMinimum, r1 = cube.RedMaximum,
				g0 = cube.GreenMinimum, g1 = cube.GreenMaximum,
				b0 = cube.BlueMinimum, b1 = cube.BlueMaximum;
			r0 = (r0 << 10) + (r0 << 6) + r0;
			r1 = (r1 << 10) + (r1 << 6) + r1;
			g0 = (g0 << 5) + g0;
			g1 = (g1 << 5) + g1;
			return moment[r1 + g1 + b1]
					- moment[r1 + g1 + b0]
					- moment[r1 + g0 + b1]
					+ moment[r1 + g0 + b0]
					- moment[r0 + g1 + b1]
					+ moment[r0 + g1 + b0]
					+ moment[r0 + g0 + b1]
					- moment[r0 + g0 + b0];
		}

		private static float VolumeFloat (Box cube, float[] moment) {
			int r0 = cube.RedMinimum, r1 = cube.RedMaximum,
				g0 = cube.GreenMinimum, g1 = cube.GreenMaximum,
				b0 = cube.BlueMinimum, b1 = cube.BlueMaximum;
			r0 = (r0 << 10) + (r0 << 6) + r0;
			r1 = (r1 << 10) + (r1 << 6) + r1;
			g0 = (g0 << 5) + g0;
			g1 = (g1 << 5) + g1;
			return moment[r1 + g1 + b1]
					- moment[r1 + g1 + b0]
					- moment[r1 + g0 + b1]
					+ moment[r1 + g0 + b0]
					- moment[r0 + g1 + b1]
					+ moment[r0 + g1 + b0]
					+ moment[r0 + g0 + b1]
					- moment[r0 + g0 + b0];
		}

		private static Box[] SplitData (ref int colorCount, ColorData data) {
			--colorCount;
			var next = 0;
			var volumeVariance = new float[__MaxColor];
			var cubes = new Box[__MaxColor];
			cubes[0].RedMaximum = __MaxSideIndex;
			cubes[0].GreenMaximum = __MaxSideIndex;
			cubes[0].BlueMaximum = __MaxSideIndex;
			for (var cubeIndex = 1; cubeIndex < colorCount; ++cubeIndex) {
				if (Cut (data, ref cubes[next], ref cubes[cubeIndex])) {
					volumeVariance[next] = cubes[next].Volume > 1 ? CalculateVariance (data, cubes[next]) : 0.0f;
					volumeVariance[cubeIndex] = cubes[cubeIndex].Volume > 1 ? CalculateVariance (data, cubes[cubeIndex]) : 0.0f;
				}
				else {
					volumeVariance[next] = 0.0f;
					cubeIndex--;
				}

				next = 0;
				var temp = volumeVariance[0];

				for (var index = 1; index <= cubeIndex; ++index) {
					if (volumeVariance[index] <= temp)
						continue;
					temp = volumeVariance[index];
					next = index;
				}

				if (temp > 0.0)
					continue;
				colorCount = cubeIndex + 1;
				break;
			}
			Array.Resize(ref cubes, colorCount);
			return cubes;
		}

		static LookupData BuildLookups (IEnumerable<Box> cubes, ColorData data) {
			var lookupData = new LookupData (__SideSize);
			var lookups = lookupData.Lookups;
			int lookupsCount = lookupData.Lookups.Count;
			var tags = lookupData.Tags;
			int ri, rgi;
			foreach (var cube in cubes) {
				byte r1 = cube.RedMaximum,
					g1 = cube.GreenMaximum,
					b1 = cube.BlueMaximum,
					r0 = (byte)(cube.RedMinimum + 1),
					g0 = (byte)(cube.GreenMinimum + 1),
					b0 = (byte)(cube.BlueMinimum + 1);
				for (var redIndex = r0; redIndex <= r1; ++redIndex) {
					ri = (redIndex << 10) + (redIndex << 6) + redIndex; // redIndex * 33 * 33
					for (var greenIndex = g0; greenIndex <= g1; ++greenIndex) {
						rgi = ri + (greenIndex << 5) + greenIndex;
						for (var blueIndex = b0; blueIndex <= b1; ++blueIndex) {
							tags[rgi + blueIndex] = lookupsCount; // [redIndex,greenIndex,blueIndex]
						}
					}
				}

				var weight = Volume (cube, data.Weights);

				if (weight <= 0) continue;

				var lookup = new Pixel (
					(byte)(Volume (cube, data.MomentsRed) / weight),
					(byte)(Volume (cube, data.MomentsGreen) / weight),
					(byte)(Volume (cube, data.MomentsBlue) / weight)
				);
				lookups.Add (lookup);
			}
			return lookupData;
		}

		static QuantizedPalette GetQuantizedPalette (int colorCount, ColorData data, IEnumerable<Box> cubes) {
			int imageSize = data.PixelsCount;
			var lookups = BuildLookups (cubes, data);

			var quantizedPixels = data.QuantizedPixels;
			for (int index = 0, pixel = 0, red = 0, green = 0; index < imageSize; ++index) {
				pixel = quantizedPixels[index];
				red = pixel >> 16;
				green = 0xFF & (pixel >> 8);
				quantizedPixels[index] = lookups.Tags[(red << 10) + (red << 6) + red + (green << 5) + green + (0xFF & pixel)];// red*33*33 + green*33 + blue
			}

			var reds = new int[colorCount + 1];
			var greens = new int[colorCount + 1];
			var blues = new int[colorCount + 1];
			var sums = new int[colorCount + 1];
			var palette = new QuantizedPalette (imageSize);

			IList<Pixel> pixels = data.Pixels;
			var pixelIndexes = palette.PixelIndex;
			int pixelsCount = data.PixelsCount;
			var lookupsList = lookups.Lookups;
			int lookupsCount = lookupsList.Count;

			var cachedMatches = new Dictionary<int, int> ();

			for (int pixelIndex = 0; pixelIndex < pixelsCount; pixelIndex++) {
				var pixel = pixels[pixelIndex];
				pixelIndexes[pixelIndex] = -1;
				int bestMatch;
				int pr = pixel.Red, pg = pixel.Green, pb = pixel.Blue;
				int argb = pr << 16 | pg << 8 | pb;

				if (!cachedMatches.TryGetValue (argb, out bestMatch)) {
					int match = quantizedPixels[pixelIndex];
					bestMatch = match;
					int bestDistance = Int32.MaxValue;

					for (int lookupIndex = 0; lookupIndex < lookupsCount; lookupIndex++) {
						var lookup = lookupsList[lookupIndex];
						var deltaRed = pr - lookup.Red;
						var deltaGreen = pg - lookup.Green;
						var deltaBlue = pb - lookup.Blue;

						int distance = deltaRed * deltaRed + deltaGreen * deltaGreen + deltaBlue * deltaBlue;

						if (distance >= bestDistance)
							continue;

						bestDistance = distance;
						bestMatch = lookupIndex;
					}

					cachedMatches[argb] = bestMatch;
				}

				reds[bestMatch] += pr;
				greens[bestMatch] += pg;
				blues[bestMatch] += pb;
				sums[bestMatch]++;

				pixelIndexes[pixelIndex] = bestMatch;
			}

			for (var paletteIndex = 0; paletteIndex < colorCount; paletteIndex++) {
				var s = sums[paletteIndex];
				if (s > 0) {
					reds[paletteIndex] /= s;
					greens[paletteIndex] /= s;
					blues[paletteIndex] /= s;
				}

				var color = Color.FromArgb (reds[paletteIndex], greens[paletteIndex], blues[paletteIndex]);
				palette.Colors.Add (color);
			}

			palette.Colors.Add (Color.FromArgb (0, 0, 0, 0));

			return palette;
		}

		struct Box
		{
			public byte RedMinimum; // exclusive
			public byte RedMaximum; // inclusive
			public byte GreenMinimum;
			public byte GreenMaximum;
			public byte BlueMinimum;
			public byte BlueMaximum;
			public int Volume;
		}
		sealed class ColorData
		{
			public ColorData (int dataGranularity, int bitmapWidth, int bitmapHeight) {
				dataGranularity++;
				var s = dataGranularity * dataGranularity * dataGranularity;
				Weights = new long[s];
				MomentsRed = new long[s];
				MomentsGreen = new long[s];
				MomentsBlue = new long[s];
				Moments = new float[s];

				pixelsCount = bitmapWidth * bitmapHeight;
				pixels = new Pixel[pixelsCount];
				quantizedPixels = new int[pixelsCount];
			}

			internal long[] Weights { get; private set; }
			internal long[] MomentsRed { get; private set; }
			internal long[] MomentsGreen { get; private set; }
			internal long[] MomentsBlue { get; private set; }
			internal float[] Moments { get; private set; }

			internal int[] QuantizedPixels { get { return quantizedPixels; } }
			internal Pixel[] Pixels { get { return pixels; } }

			public int PixelsCount { get { return pixels.Length; } }
			internal void AddPixel (Pixel pixel, int quantizedPixel) {
				pixels[pixelFillingCounter] = pixel;
				quantizedPixels[pixelFillingCounter++] = quantizedPixel;
			}

			private Pixel[] pixels;
			private int[] quantizedPixels;
			private int pixelsCount;
			private int pixelFillingCounter;
		}
		struct CubeCut
		{
			public readonly bool CanSplit;
			public readonly byte Position;
			public readonly float Value;

			public CubeCut (bool canSplit, byte cutPoint, float result) {
				CanSplit = canSplit;
				Position = cutPoint;
				Value = result;
			}
		}
		sealed class LookupData
		{
			public LookupData (int granularity) {
				Lookups = new List<Pixel> ();
				Tags = new int[granularity * granularity * granularity];
			}

			public IList<Pixel> Lookups { get; private set; }
			public int[] Tags { get; private set; }
		}
		struct Pixel
		{
			public Pixel (byte red, byte green, byte blue) {
				Red = red;
				Green = green;
				Blue = blue;
			}

			public readonly byte Red;
			public readonly byte Green;
			public readonly byte Blue;
		}
		sealed class QuantizedPalette
		{
			public QuantizedPalette (int size) {
				Colors = new List<Color> ();
				PixelIndex = new int[size];
			}
			public IList<Color> Colors { get; private set; }
			public int[] PixelIndex { get; private set; }
		}

		sealed class QuantizationException : ApplicationException
		{
			public QuantizationException (string message)
				: base (message) {

			}
		}
	}

}
