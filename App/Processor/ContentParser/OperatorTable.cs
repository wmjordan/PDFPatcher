using System;
using System.Text;
using Cat = PDFPatcher.Processor.ContentParser.OperatorCategory;
using Sem = PDFPatcher.Processor.ContentParser.OperatorSemantic;
using Kind = PDFPatcher.Processor.ContentParser.RenderCommandKind;

namespace PDFPatcher.Processor.ContentParser;

static class OperatorTable
{
	const byte Arg0 = 1 << 0,
		Arg1 = 1 << 1,
		Arg2 = 1 << 2,
		Arg3 = 1 << 3,
		Arg4 = 1 << 4,
		Arg5 = 1 << 5,
		Arg6 = 1 << 6,
		Variable = 0xFF;

	// ============ 文本相关 ============
	// 输出类
	public static readonly OperatorInfo Tj = new(Kind.ShowText, "Tj", Cat.Text, Sem.ContentOutput, Arg1);
	public static readonly OperatorInfo TJ = new(Kind.ShowTextWithSpacing, "TJ", Cat.Text, Sem.ContentOutput, Arg1);
	public static readonly OperatorInfo Quote = new(Kind.NextLineShowText, "'", Cat.Text, Sem.ContentOutput, Arg1);
	public static readonly OperatorInfo DoubleQuote = new(Kind.MoveToNextLineAndShowText, "\"", Cat.Text, Sem.ContentOutput, Arg3);

	// 状态设置类
	public static readonly OperatorInfo T_Star = new(Kind.NextLine, "T*", Cat.Text, Sem.StateSetup, Arg0);
	public static readonly OperatorInfo Tc = new(Kind.SetCharSpacing, "Tc", Cat.Text, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo Tw = new(Kind.SetWordSpacing, "Tw", Cat.Text, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo Tz = new(Kind.SetHorizontalScaling, "Tz", Cat.Text, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo TL = new(Kind.SetTextLeading, "TL", Cat.Text, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo Tf = new(Kind.SetFont, "Tf", Cat.Text, Sem.StateSetup, Arg2);
	public static readonly OperatorInfo Tr = new(Kind.SetTextRenderMode, "Tr", Cat.Text, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo Ts = new(Kind.SetTextRise, "Ts", Cat.Text, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo Td = new(Kind.MoveText, "Td", Cat.Text, Sem.StateSetup, Arg2);
	public static readonly OperatorInfo TD = new(Kind.MoveTextSetLeading, "TD", Cat.Text, Sem.StateSetup, Arg2);
	public static readonly OperatorInfo Tm = new(Kind.SetTextMatrix, "Tm", Cat.Text, Sem.StateSetup, Arg6);
	public static readonly OperatorInfo d0 = new(Kind.SetGlyphWidthInType3Font, "d0", Cat.Text, Sem.StateSetup, Arg2);
	public static readonly OperatorInfo d1 = new(Kind.SetGlyphWidthAndBoundingBoxInType3Font, "d1", Cat.Text, Sem.StateSetup, Arg6);

	// 结构分组类
	public static readonly OperatorInfo BT = new(Kind.BeginText, "BT", Cat.Text, Sem.GroupStructure, Arg0, isBeginScope: true);
	public static readonly OperatorInfo ET = new(Kind.EndText, "ET", Cat.Text, Sem.GroupStructure, Arg0, isEndScope: true, pairedBeginKind: Kind.BeginText);

	// ============ 图形状态 ============
	// 结构分组类
	public static readonly OperatorInfo q = new(Kind.GSave, "q", Cat.GraphicsState, Sem.GroupStructure, Arg0, isBeginScope: true);
	public static readonly OperatorInfo Q = new(Kind.GRestore, "Q", Cat.GraphicsState, Sem.GroupStructure, Arg0, isEndScope: true, pairedBeginKind: Kind.GSave);

	// 状态设置类
	public static readonly OperatorInfo cm = new(Kind.ConcatMatrix, "cm", Cat.GraphicsState, Sem.StateSetup, Arg6);
	public static readonly OperatorInfo w = new(Kind.SetLineWidth, "w", Cat.GraphicsState, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo J = new(Kind.SetLineCap, "J", Cat.GraphicsState, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo j = new(Kind.SetLineJoin, "j", Cat.GraphicsState, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo M = new(Kind.SetMiterLimit, "M", Cat.GraphicsState, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo d = new(Kind.SetDashPattern, "d", Cat.GraphicsState, Sem.StateSetup, Arg2);
	public static readonly OperatorInfo ri = new(Kind.SetRenderingIntent, "ri", Cat.GraphicsState, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo i = new(Kind.SetFlatness, "i", Cat.GraphicsState, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo gs = new(Kind.SetGraphicsState, "gs", Cat.GraphicsState, Sem.StateSetup, Arg1);

	// ============ 颜色 ============
	// 颜色操作符均为设置颜色状态
	public static readonly OperatorInfo CS = new(Kind.SetColorSpaceStroking, "CS", Cat.Color, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo cs = new(Kind.SetColorSpaceNonStroking, "cs", Cat.Color, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo SC = new(Kind.SetColorStroking, "SC", Cat.Color, Sem.StateSetup, Variable);
	public static readonly OperatorInfo SCN = new(Kind.SetColorPatternStroking, "SCN", Cat.Color, Sem.StateSetup, Variable);
	public static readonly OperatorInfo sc = new(Kind.SetColorNonStroking, "sc", Cat.Color, Sem.StateSetup, Variable);
	public static readonly OperatorInfo scn = new(Kind.SetColorPatternNonStroking, "scn", Cat.Color, Sem.StateSetup, Variable);
	public static readonly OperatorInfo G = new(Kind.SetGrayStroking, "G", Cat.Color, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo g = new(Kind.SetGrayNonStroking, "g", Cat.Color, Sem.StateSetup, Arg1);
	public static readonly OperatorInfo RG = new(Kind.SetRGBStroking, "RG", Cat.Color, Sem.StateSetup, Arg3);
	public static readonly OperatorInfo rg = new(Kind.SetRGBNonStroking, "rg", Cat.Color, Sem.StateSetup, Arg3);
	public static readonly OperatorInfo K = new(Kind.SetCMYKStroking, "K", Cat.Color, Sem.StateSetup, Arg4);
	public static readonly OperatorInfo k = new(Kind.SetCMYKNonStroking, "k", Cat.Color, Sem.StateSetup, Arg4);

	// ============ 路径 ============
	// 路径构造均为状态设置
	public static readonly OperatorInfo m = new(Kind.MoveTo, "m", Cat.Path, Sem.StateSetup, Arg2);
	public static readonly OperatorInfo l = new(Kind.LineTo, "l", Cat.Path, Sem.StateSetup, Arg2);
	public static readonly OperatorInfo c = new(Kind.CurveTo, "c", Cat.Path, Sem.StateSetup, Arg6);
	public static readonly OperatorInfo v = new(Kind.CurveToNoFirstControl, "v", Cat.Path, Sem.StateSetup, Arg4);
	public static readonly OperatorInfo y = new(Kind.CurveToNoSecondControl, "y", Cat.Path, Sem.StateSetup, Arg4);
	public static readonly OperatorInfo h = new(Kind.ClosePath, "h", Cat.Path, Sem.StateSetup, Arg0);
	public static readonly OperatorInfo re = new(Kind.Rectangle, "re", Cat.Path, Sem.StateSetup, Arg4);

	// ============ 路径绘制 ============
	// 路径绘制均为内容输出
	public static readonly OperatorInfo S = new(Kind.StrokePath, "S", Cat.Painting, Sem.ContentOutput, Arg0);
	public static readonly OperatorInfo s = new(Kind.CloseAndStrokePath, "s", Cat.Painting, Sem.ContentOutput, Arg0);
	public static readonly OperatorInfo f = new(Kind.FillPathNonZero, "f", Cat.Painting, Sem.ContentOutput, Arg0);
	public static readonly OperatorInfo F = new(Kind.FillPathNonZero, "F", Cat.Painting, Sem.ContentOutput, Arg0);
	public static readonly OperatorInfo f_Star = new(Kind.FillPathEvenOdd, "f*", Cat.Painting, Sem.ContentOutput, Arg0);
	public static readonly OperatorInfo B = new(Kind.FillStrokePathNonZero, "B", Cat.Painting, Sem.ContentOutput, Arg0);
	public static readonly OperatorInfo B_Star = new(Kind.FillStrokePathEvenOdd, "B*", Cat.Painting, Sem.ContentOutput, Arg0);
	public static readonly OperatorInfo b = new(Kind.CloseFillStrokePathNonZero, "b", Cat.Painting, Sem.ContentOutput, Arg0);
	public static readonly OperatorInfo b_Star = new(Kind.CloseFillStrokePathEvenOdd, "b*", Cat.Painting, Sem.ContentOutput, Arg0);
	public static readonly OperatorInfo n = new(Kind.EndPath, "n", Cat.Painting, Sem.StateSetup, Arg0); // n 仅结束路径，无输出
	public static readonly OperatorInfo W = new(Kind.SetClipPath, "W", Cat.Painting, Sem.StateSetup, Arg0);
	public static readonly OperatorInfo W_Star = new(Kind.SetClipPathEvenOdd, "W*", Cat.Painting, Sem.StateSetup, Arg0);

	// ============ 兼容性 ============
	public static readonly OperatorInfo BX = new(Kind.BeginCompatibilitySection, "BX", Cat.Compatibility, Sem.GroupStructure, Arg0, isBeginScope: true);
	public static readonly OperatorInfo EX = new(Kind.EndCompatibilitySection, "EX", Cat.Compatibility, Sem.GroupStructure, Arg0, isEndScope: true, pairedBeginKind: Kind.BeginCompatibilitySection);

	// ============ 标记内容 ============
	public static readonly OperatorInfo BMC = new(Kind.BeginMarkedContent, "BMC", Cat.MarkedContent, Sem.GroupStructure, Arg1, isBeginScope: true);
	public static readonly OperatorInfo BDC = new(Kind.BeginMarkedContentWithProps, "BDC", Cat.MarkedContent, Sem.GroupStructure, Arg2, isBeginScope: true);
	public static readonly OperatorInfo EMC = new(Kind.EndMarkedContent, "EMC", Cat.MarkedContent, Sem.GroupStructure, Arg0, isEndScope: true, pairedBeginKind: Kind.BeginMarkedContent);
	public static readonly OperatorInfo MP = new(Kind.DefineMarkedContentPoint, "MP", Cat.MarkedContent, Sem.GroupStructure, Arg1);
	public static readonly OperatorInfo DP = new(Kind.DefineMarkedContentPointWithProps, "DP", Cat.MarkedContent, Sem.GroupStructure, Arg2);

	// ============ 内联图像 ============
	public static readonly OperatorInfo BI = new(Kind.BeginInlineImage, "BI", Cat.Image, Sem.GroupStructure, Arg0, isBeginScope: true);
	public static readonly OperatorInfo EI = new(Kind.EndInlineImage, "EI", Cat.Image, Sem.GroupStructure, Arg1, isEndScope: true, pairedBeginKind: Kind.BeginInlineImage);
	public static readonly OperatorInfo ID = new(Kind.BeginInlineImageData, "ID", Cat.Image, Sem.StateSetup, Arg0);

	// ============ XObject / Shading ============
	// Do 和 sh 均为内容输出
	public static readonly OperatorInfo Do = new(Kind.PaintXObject, "Do", Cat.XObject, Sem.ContentOutput, Arg1);
	public static readonly OperatorInfo sh = new(Kind.PaintShading, "sh", Cat.XObject, Sem.ContentOutput, Arg1);

	// ============ 兜底 ============
	static readonly OperatorInfo _Unknown = new(Kind.Unknown, "", Cat.Unknown, Sem.StateSetup, Variable);

	public static OperatorInfo Resolve(byte[] buffer, int offset, int length) {
		if (length <= 0) return _Unknown;

		switch (length) {
			case 1:
				switch (buffer[offset]) {
					case (byte)'q': return q;
					case (byte)'Q': return Q;
					case (byte)'B': return B;
					case (byte)'b': return b;
					case (byte)'f': return f;
					case (byte)'F': return F;
					case (byte)'c': return c;
					case (byte)'d': return d;
					case (byte)'m': return m;
					case (byte)'l': return l;
					case (byte)'v': return v;
					case (byte)'y': return y;
					case (byte)'h': return h;
					case (byte)'n': return n;
					case (byte)'S': return S;
					case (byte)'s': return s;
					case (byte)'i': return i;
					case (byte)'w': return w;
					case (byte)'J': return J;
					case (byte)'j': return j;
					case (byte)'M': return M;
					case (byte)'G': return G;
					case (byte)'g': return g;
					case (byte)'K': return K;
					case (byte)'k': return k;
					case (byte)'W': return W;
					case (byte)'"': return DoubleQuote;
					case (byte)'\'': return Quote;
				}
				break;

			case 2:
				switch (buffer[offset]) {
					case (byte)'T':
						switch (buffer[offset + 1]) {
							case (byte)'j': return Tj;
							case (byte)'J': return TJ;
							case (byte)'*': return T_Star;
							case (byte)'c': return Tc;
							case (byte)'w': return Tw;
							case (byte)'z': return Tz;
							case (byte)'L': return TL;
							case (byte)'f': return Tf;
							case (byte)'r': return Tr;
							case (byte)'s': return Ts;
							case (byte)'d': return Td;
							case (byte)'D': return TD;
							case (byte)'m': return Tm;
						}
						break;
					case (byte)'B':
						switch (buffer[offset + 1]) {
							case (byte)'I': return BI;
							case (byte)'X': return BX;
							case (byte)'T': return BT;
							case (byte)'*': return B_Star;
						}
						break;
					case (byte)'E':
						switch (buffer[offset + 1]) {
							case (byte)'T': return ET;
							case (byte)'I': return EI;
							case (byte)'X': return EX;
						}
						break;
					case (byte)'C':
						switch (buffer[offset + 1]) {
							case (byte)'S': return CS;
						}
						break;
					case (byte)'c':
						switch (buffer[offset + 1]) {
							case (byte)'m': return cm;
							case (byte)'s': return cs;
						}
						break;
					case (byte)'d':
						switch (buffer[offset + 1]) {
							case (byte)'0': return d0;
							case (byte)'1': return d1;
						}
						break;
					case (byte)'r':
						switch (buffer[offset + 1]) {
							case (byte)'e': return re;
							case (byte)'g': return rg;
						}
						break;
					case (byte)'R':
						if (buffer[offset + 1] == (byte)'G') return RG;
						break;
					case (byte)'S':
						if (buffer[offset + 1] == (byte)'C') return SC;
						break;
					case (byte)'f':
						if (buffer[offset + 1] == (byte)'*') return f_Star;
						break;
					case (byte)'b':
						if (buffer[offset + 1] == (byte)'*') return b_Star;
						break;
					case (byte)'D':
						switch (buffer[offset + 1]) {
							case (byte)'o': return Do;
							case (byte)'P': return DP;
						}
						break;
					case (byte)'g':
						if (buffer[offset + 1] == (byte)'s') return gs;
						break;
					case (byte)'s':
						if (buffer[offset + 1] == (byte)'h') return sh;
						if (buffer[offset + 1] == (byte)'c') return sc;
						break;
					case (byte)'M':
						if (buffer[offset + 1] == (byte)'P') return MP;
						break;
					case (byte)'I':
						if (buffer[offset + 1] == (byte)'D') return ID;
						break;
					case (byte)'W':
						if (buffer[offset + 1] == (byte)'*') return W_Star;
						break;
				}
				break;

			case 3:
				switch (buffer[offset]) {
					case (byte)'B':
						if (buffer[offset + 2] == (byte)'C') {
							switch (buffer[offset + 1]) {
								case (byte)'M':
									return BMC;
								case (byte)'D':
									return BDC;
							}
						}
						break;
					case (byte)'E':
						if (buffer[offset + 1] == (byte)'M' && buffer[offset + 2] == (byte)'C') return EMC;
						break;
					case (byte)'S':
						if (buffer[offset + 1] == (byte)'C' && buffer[offset + 2] == (byte)'N') return SCN;
						break;
					case (byte)'s':
						if (buffer[offset + 1] == (byte)'c' && buffer[offset + 2] == (byte)'n') return scn;
						break;
				}
				break;
		}
		return new OperatorInfo(Kind.Unknown, Encoding.ASCII.GetString(buffer, offset, length), Cat.Unknown, Sem.StateSetup, Variable);
	}
}
