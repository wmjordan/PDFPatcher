using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PDFPatcher.Processor.Imaging;

public static class WuQuantizer
{
	private const int __MaxColor = 256;
	private const int __Red = 2;
	private const int __Green = 1;
	private const int __Blue = 0;
	private const int __SideSize = 33;
	private const int __MaxSideIndex = 32;

	public static Bitmap QuantizeImage(Bitmap image) {
		int colorCount = __MaxColor;
		ColorData data = BuildHistogram(image);
		CalculateMoments(data);
		IEnumerable<Box> cubes = SplitData(ref colorCount, data);
		QuantizedPalette palette = GetQuantizedPalette(colorCount, data, cubes);
		return ProcessImagePixels(image, palette);
	}

	private static Bitmap ProcessImagePixels(Image sourceImage, QuantizedPalette palette) {
		Bitmap result = new(sourceImage.Width, sourceImage.Height, PixelFormat.Format8bppIndexed);
		ColorPalette newPalette = result.Palette;
		palette.Colors.CopyTo(newPalette.Entries, 0);
		result.Palette = newPalette;

		BitmapData targetData = null;
		try {
			targetData = result.LockBits(Rectangle.FromLTRB(0, 0, result.Width, result.Height), ImageLockMode.WriteOnly,
				result.PixelFormat);
			int targetByteLength = targetData.Stride < 0 ? -targetData.Stride : targetData.Stride;
			int targetSize = targetByteLength * result.Height;
			int targetOffset = 0;
			byte[] targetBuffer = new byte[targetSize];
			int pixelIndex = 0;
			int[] pil = palette.PixelIndex;
			int rw = result.Width, rh = result.Height;
			int empty = palette.Colors.Count - 1;
			for (int y = 0; y < rh; y++) {
				for (int x = 0; x < rw; x++) {
					int pv = pil[pixelIndex];
					targetBuffer[targetOffset + x] = (byte)(pv == -1 ? empty : pv);
					pixelIndex++;
				}

				targetOffset += targetByteLength;
			}

			Marshal.Copy(targetBuffer, 0, targetData.Scan0, targetSize);
		}
		finally {
			if (targetData != null) {
				result.UnlockBits(targetData);
			}
		}

		return result;
	}

	private static ColorData BuildHistogram(Bitmap sourceImage) {
		int bitmapWidth = sourceImage.Width;
		int bitmapHeight = sourceImage.Height;

		BitmapData data = sourceImage.LockBits(false);
		ColorData colorData = new(__MaxSideIndex, bitmapWidth, bitmapHeight);

		try {
			int bitDepth = Image.GetPixelFormatSize(sourceImage.PixelFormat);
			if (bitDepth != 32 && bitDepth != 24) {
				throw new QuantizationException(
					$"The image you are attempting to quantize does not contain a 32 bit ARGB palette. This image has a bit depth of {bitDepth} with {sourceImage.Palette.Entries.Length} colors.");
			}

			int byteLength = data.Stride < 0 ? -data.Stride : data.Stride;
			int offset = 0;
			byte[] buffer = new byte[byteLength * sourceImage.Height];

			Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
			int[] t = new int[__MaxColor];
			for (int i = 0; i < __MaxColor; ++i) {
				t[i] = i * i;
			}

			long[] w = colorData.Weights;
			long[] mr = colorData.MomentsRed;
			long[] mg = colorData.MomentsGreen;
			long[] mb = colorData.MomentsBlue;
			float[] m = colorData.Moments;
			for (int y = 0; y < bitmapHeight; y++) {
				int index = 0;
				for (int x = 0; x < bitmapWidth; x++) {
					int indexOffset = offset + (index >> 3);

					byte vr = buffer[indexOffset + __Red];
					byte vg = buffer[indexOffset + __Green];
					byte vb = buffer[indexOffset + __Blue];
					byte r = (byte)((vr >> 3) + 1);
					byte g = (byte)((vg >> 3) + 1);
					byte b = (byte)((vb >> 3) + 1);
					int pos = (r << 10) + (r << 6) + r + (g << 5) + g + b;
					w[pos]++;
					mr[pos] += vr;
					mg[pos] += vg;
					mb[pos] += vb;
					m[pos] += t[vr] + t[vg] + t[vb];

					colorData.AddPixel(
						new Pixel(vr, vg, vb),
						(r << 16) + (g << 8) + b
					);
					index += bitDepth;
				}

				offset += byteLength;
			}
		}
		finally {
			sourceImage.UnlockBits(data);
		}

		return colorData;
	}

	private static void CalculateMoments(ColorData data) {
		long[] w = data.Weights;
		long[] mr = data.MomentsRed;
		long[] mg = data.MomentsGreen;
		long[] mb = data.MomentsBlue;
		float[] m = data.Moments;
		for (int r = 1; r <= __MaxSideIndex; ++r) {
			long[] area = new long[__SideSize];
			long[] areaRed = new long[__SideSize];
			long[] areaGreen = new long[__SideSize];
			long[] areaBlue = new long[__SideSize];
			float[] area2 = new float[__SideSize];
			for (int g = 1; g <= __MaxSideIndex; ++g) {
				long line = 0;
				long lineRed = 0;
				long lineGreen = 0;
				long lineBlue = 0;
				float line2 = 0.0f;
				for (int b = 1; b <= __MaxSideIndex; ++b) {
					int pos = (r << 10) + (r << 6) + r + (g << 5) + g + b; // [r,g,b]
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

					int pos2 = pos - (__MaxSideIndex * __MaxSideIndex); // [r-1,g,b]
					w[pos] = w[pos2] + area[b];
					mr[pos] = mr[pos2] + areaRed[b];
					mg[pos] = mg[pos2] + areaGreen[b];
					mb[pos] = mb[pos2] + areaBlue[b];
					m[pos] = m[pos2] + area2[b];
				}
			}
		}
	}

	private static long Top(Box cube, int direction, int position, IList<long> moment) {
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

	private static long Bottom(Box cube, int direction, IList<long> moment) {
		int r0 = cube.RedMinimum,
			r1 = cube.RedMaximum,
			g0 = cube.GreenMinimum,
			g1 = cube.GreenMaximum,
			b0 = cube.BlueMinimum,
			b1 = cube.BlueMaximum;
		r0 = (r0 << 10) + (r0 << 6) + r0;
		r1 = (r1 << 10) + (r1 << 6) + r1;
		g0 = (g0 << 5) + g0;
		g1 = (g1 << 5) + g1;
		return direction switch {
			__Red => -moment[r0 + g1 + b1] + moment[r0 + g1 + b0] + moment[r0 + g0 + b1] - moment[r0 + g0 + b0],
			__Green => -moment[r1 + g0 + b1] + moment[r1 + g0 + b0] + moment[r0 + g0 + b1] - moment[r0 + g0 + b0],
			__Blue => -moment[r1 + g1 + b0] + moment[r1 + g0 + b0] + moment[r0 + g1 + b0] - moment[r0 + g0 + b0],
			_ => 0
		};
	}

	private static CubeCut Maximize(ColorData data, Box cube, int direction, byte first, byte last, long wholeRed,
		long wholeGreen, long wholeBlue, long wholeWeight) {
		long bottomRed = Bottom(cube, direction, data.MomentsRed);
		long bottomGreen = Bottom(cube, direction, data.MomentsGreen);
		long bottomBlue = Bottom(cube, direction, data.MomentsBlue);
		long bottomWeight = Bottom(cube, direction, data.Weights);

		float result = 0.0f;
		bool canSplit = false;
		byte cutPoint = 0;

		for (byte position = first; position < last; ++position) {
			long halfRed = bottomRed + Top(cube, direction, position, data.MomentsRed);
			long halfGreen = bottomGreen + Top(cube, direction, position, data.MomentsGreen);
			long halfBlue = bottomBlue + Top(cube, direction, position, data.MomentsBlue);
			long halfWeight = bottomWeight + Top(cube, direction, position, data.Weights);

			if (halfWeight == 0) {
				continue;
			}

			long halfDistance = (halfRed * halfRed) + (halfGreen * halfGreen) + (halfBlue * halfBlue);
			long temp = halfDistance / halfWeight;

			halfRed = wholeRed - halfRed;
			halfGreen = wholeGreen - halfGreen;
			halfBlue = wholeBlue - halfBlue;
			halfWeight = wholeWeight - halfWeight;

			if (halfWeight == 0) {
				continue;
			}

			halfDistance = (halfRed * halfRed) + (halfGreen * halfGreen) + (halfBlue * halfBlue);
			temp += halfDistance / halfWeight;

			if (!(temp > result)) {
				continue;
			}

			result = temp;
			canSplit = true;
			cutPoint = position;
		}

		return new CubeCut(canSplit, cutPoint, result);
	}

	private static bool Cut(ColorData data, ref Box first, ref Box second) {
		int direction;
		long wholeRed = Volume(first, data.MomentsRed);
		long wholeGreen = Volume(first, data.MomentsGreen);
		long wholeBlue = Volume(first, data.MomentsBlue);
		long wholeWeight = Volume(first, data.Weights);

		CubeCut maxRed = Maximize(data, first, __Red, (byte)(first.RedMinimum + 1), first.RedMaximum, wholeRed,
			wholeGreen, wholeBlue, wholeWeight);
		CubeCut maxGreen = Maximize(data, first, __Green, (byte)(first.GreenMinimum + 1), first.GreenMaximum, wholeRed,
			wholeGreen, wholeBlue, wholeWeight);
		CubeCut maxBlue = Maximize(data, first, __Blue, (byte)(first.BlueMinimum + 1), first.BlueMaximum, wholeRed,
			wholeGreen, wholeBlue, wholeWeight);

		if (maxRed.Value >= maxGreen.Value && maxRed.Value >= maxBlue.Value) {
			direction = __Red;
			if (maxRed.CanSplit == false) {
				return false;
			}
		}
		else {
			if (maxGreen.Value >= maxRed.Value && maxGreen.Value >= maxBlue.Value) {
				direction = __Green;
			}
			else {
				direction = __Blue;
			}
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

		first.Volume = (first.RedMaximum - first.RedMinimum) * (first.GreenMaximum - first.GreenMinimum) *
					   (first.BlueMaximum - first.BlueMinimum);
		second.Volume = (second.RedMaximum - second.RedMinimum) * (second.GreenMaximum - second.GreenMinimum) *
						(second.BlueMaximum - second.BlueMinimum);

		return true;
	}

	private static float CalculateVariance(ColorData data, Box cube) {
		float volumeRed = Volume(cube, data.MomentsRed);
		float volumeGreen = Volume(cube, data.MomentsGreen);
		float volumeBlue = Volume(cube, data.MomentsBlue);
		float volumeMoment = VolumeFloat(cube, data.Moments);
		float volumeWeight = Volume(cube, data.Weights);

		float distance = (volumeRed * volumeRed) + (volumeGreen * volumeGreen) + (volumeBlue * volumeBlue);

		float result = volumeMoment - (distance / volumeWeight);
		return double.IsNaN(result) ? 0.0f : result;
	}

	private static long Volume(Box cube, IList<long> moment) {
		int r0 = cube.RedMinimum,
			r1 = cube.RedMaximum,
			g0 = cube.GreenMinimum,
			g1 = cube.GreenMaximum,
			b0 = cube.BlueMinimum,
			b1 = cube.BlueMaximum;
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

	private static float VolumeFloat(Box cube, IList<float> moment) {
		int r0 = cube.RedMinimum,
			r1 = cube.RedMaximum,
			g0 = cube.GreenMinimum,
			g1 = cube.GreenMaximum,
			b0 = cube.BlueMinimum,
			b1 = cube.BlueMaximum;
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

	private static IEnumerable<Box> SplitData(ref int colorCount, ColorData data) {
		--colorCount;
		int next = 0;
		float[] volumeVariance = new float[__MaxColor];
		Box[] cubes = new Box[__MaxColor];
		cubes[0].RedMaximum = __MaxSideIndex;
		cubes[0].GreenMaximum = __MaxSideIndex;
		cubes[0].BlueMaximum = __MaxSideIndex;
		for (int cubeIndex = 1; cubeIndex < colorCount; ++cubeIndex) {
			if (Cut(data, ref cubes[next], ref cubes[cubeIndex])) {
				volumeVariance[next] = cubes[next].Volume > 1 ? CalculateVariance(data, cubes[next]) : 0.0f;
				volumeVariance[cubeIndex] =
					cubes[cubeIndex].Volume > 1 ? CalculateVariance(data, cubes[cubeIndex]) : 0.0f;
			}
			else {
				volumeVariance[next] = 0.0f;
				cubeIndex--;
			}

			next = 0;
			float temp = volumeVariance[0];

			for (int index = 1; index <= cubeIndex; ++index) {
				if (volumeVariance[index] <= temp) {
					continue;
				}

				temp = volumeVariance[index];
				next = index;
			}

			if (temp > 0.0) {
				continue;
			}

			colorCount = cubeIndex + 1;
			break;
		}

		Array.Resize(ref cubes, colorCount);
		return cubes;
	}

	private static LookupData BuildLookups(IEnumerable<Box> cubes, ColorData data) {
		LookupData lookupData = new(__SideSize);
		IList<Pixel> lookups = lookupData.Lookups;
		int lookupsCount = lookupData.Lookups.Count;
		int[] tags = lookupData.Tags;
		foreach (Box cube in cubes) {
			byte r1 = cube.RedMaximum,
				g1 = cube.GreenMaximum,
				b1 = cube.BlueMaximum,
				r0 = (byte)(cube.RedMinimum + 1),
				g0 = (byte)(cube.GreenMinimum + 1),
				b0 = (byte)(cube.BlueMinimum + 1);
			for (byte redIndex = r0; redIndex <= r1; ++redIndex) {
				int ri = (redIndex << 10) + (redIndex << 6) + redIndex;
				for (byte greenIndex = g0; greenIndex <= g1; ++greenIndex) {
					int rgi = ri + (greenIndex << 5) + greenIndex;
					for (byte blueIndex = b0; blueIndex <= b1; ++blueIndex) {
						tags[rgi + blueIndex] = lookupsCount; // [redIndex,greenIndex,blueIndex]
					}
				}
			}

			long weight = Volume(cube, data.Weights);

			if (weight <= 0) {
				continue;
			}

			Pixel lookup = new(
				(byte)(Volume(cube, data.MomentsRed) / weight),
				(byte)(Volume(cube, data.MomentsGreen) / weight),
				(byte)(Volume(cube, data.MomentsBlue) / weight)
			);
			lookups.Add(lookup);
		}

		return lookupData;
	}

	private static QuantizedPalette GetQuantizedPalette(int colorCount, ColorData data, IEnumerable<Box> cubes) {
		int imageSize = data.PixelsCount;
		LookupData lookups = BuildLookups(cubes, data);

		int[] quantizedPixels = data.QuantizedPixels;
		for (int index = 0; index < imageSize; ++index) {
			int pixel = quantizedPixels[index];
			int red = pixel >> 16;
			int green = 0xFF & (pixel >> 8);
			quantizedPixels[index] =
				lookups.Tags
					[(red << 10) + (red << 6) + red + (green << 5) + green + (0xFF & pixel)]; // red*33*33 + green*33 + blue
		}

		int[] reds = new int[colorCount + 1];
		int[] greens = new int[colorCount + 1];
		int[] blues = new int[colorCount + 1];
		int[] sums = new int[colorCount + 1];
		QuantizedPalette palette = new(imageSize);

		IList<Pixel> pixels = data.Pixels;
		int[] pixelIndexes = palette.PixelIndex;
		int pixelsCount = data.PixelsCount;
		IList<Pixel> lookupsList = lookups.Lookups;
		int lookupsCount = lookupsList.Count;

		Dictionary<int, int> cachedMatches = new();

		for (int pixelIndex = 0; pixelIndex < pixelsCount; pixelIndex++) {
			Pixel pixel = pixels[pixelIndex];
			pixelIndexes[pixelIndex] = -1;
			int pr = pixel.Red, pg = pixel.Green, pb = pixel.Blue;
			int argb = (pr << 16) | (pg << 8) | pb;

			if (!cachedMatches.TryGetValue(argb, out int bestMatch)) {
				int match = quantizedPixels[pixelIndex];
				bestMatch = match;
				int bestDistance = int.MaxValue;

				for (int lookupIndex = 0; lookupIndex < lookupsCount; lookupIndex++) {
					Pixel lookup = lookupsList[lookupIndex];
					int deltaRed = pr - lookup.Red;
					int deltaGreen = pg - lookup.Green;
					int deltaBlue = pb - lookup.Blue;

					int distance = (deltaRed * deltaRed) + (deltaGreen * deltaGreen) + (deltaBlue * deltaBlue);

					if (distance >= bestDistance) {
						continue;
					}

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

		for (int paletteIndex = 0; paletteIndex < colorCount; paletteIndex++) {
			int s = sums[paletteIndex];
			if (s > 0) {
				reds[paletteIndex] /= s;
				greens[paletteIndex] /= s;
				blues[paletteIndex] /= s;
			}

			Color color = Color.FromArgb(reds[paletteIndex], greens[paletteIndex], blues[paletteIndex]);
			palette.Colors.Add(color);
		}

		palette.Colors.Add(Color.FromArgb(0, 0, 0, 0));

		return palette;
	}

	private struct Box
	{
		public byte RedMinimum; // exclusive
		public byte RedMaximum; // inclusive
		public byte GreenMinimum;
		public byte GreenMaximum;
		public byte BlueMinimum;
		public byte BlueMaximum;
		public int Volume;
	}

	private sealed class ColorData
	{
		private int pixelFillingCounter;

		public ColorData(int dataGranularity, int bitmapWidth, int bitmapHeight) {
			dataGranularity++;
			int s = dataGranularity * dataGranularity * dataGranularity;
			Weights = new long[s];
			MomentsRed = new long[s];
			MomentsGreen = new long[s];
			MomentsBlue = new long[s];
			Moments = new float[s];

			int pixelsCount = bitmapWidth * bitmapHeight;
			Pixels = new Pixel[pixelsCount];
			QuantizedPixels = new int[pixelsCount];
		}

		internal long[] Weights { get; }
		internal long[] MomentsRed { get; }
		internal long[] MomentsGreen { get; }
		internal long[] MomentsBlue { get; }
		internal float[] Moments { get; }

		internal int[] QuantizedPixels { get; }

		internal Pixel[] Pixels { get; }

		public int PixelsCount => Pixels.Length;

		internal void AddPixel(Pixel pixel, int quantizedPixel) {
			Pixels[pixelFillingCounter] = pixel;
			QuantizedPixels[pixelFillingCounter++] = quantizedPixel;
		}
	}

	private struct CubeCut
	{
		public readonly bool CanSplit;
		public readonly byte Position;
		public readonly float Value;

		public CubeCut(bool canSplit, byte cutPoint, float result) {
			CanSplit = canSplit;
			Position = cutPoint;
			Value = result;
		}
	}

	private sealed class LookupData
	{
		public LookupData(int granularity) {
			Lookups = new List<Pixel>();
			Tags = new int[granularity * granularity * granularity];
		}

		public IList<Pixel> Lookups { get; }
		public int[] Tags { get; }
	}

	private struct Pixel
	{
		public Pixel(byte red, byte green, byte blue) {
			Red = red;
			Green = green;
			Blue = blue;
		}

		public readonly byte Red;
		public readonly byte Green;
		public readonly byte Blue;
	}

	private sealed class QuantizedPalette
	{
		public QuantizedPalette(int size) {
			Colors = new List<Color>();
			PixelIndex = new int[size];
		}

		public IList<Color> Colors { get; }
		public int[] PixelIndex { get; }
	}

	private sealed class QuantizationException : ApplicationException
	{
		public QuantizationException(string message)
			: base(message) {
		}
	}
}