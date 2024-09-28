using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text.pdf;
using PDFPatcher.Common;

namespace PDFPatcher.Model
{
	enum PdfPageCommandType
	{
		Normal,
		Text,
		Enclosure,
		Matrix,
		Font,
		InlineImage
	}
	interface IPdfPageCommandContainer
	{
		bool HasCommand { get; }
		IList<PdfPageCommand> Commands { get; }
	}
	class PdfPageCommand
	{
		#region 操作符中文名称
		static readonly Dictionary<string, string> __OperatorNames = Init();
		static Dictionary<string, string> Init() {
			return new Dictionary<string, string> {
				{ "'", "换行字符串" },
				{ "\"", "换行字符串" },
				{ "b", "闭合非零画线填充" },
				{ "B", "非零画线填充" },
				{ "b*", "闭合奇偶画线填充" },
				{ "B*", "奇偶画线填充" },
				{ "BDC", "标记内容区及属性" },
				{ "BI", "内嵌图像" },
				{ "BMC", "标记内容区" },
				{ "BT", "文本区" },
				{ "BX", "兼容区" },
				{ "c", "曲线" },
				{ "cm", "矩阵" },
				{ "CS", "画线色域" },
				{ "cs", "非线色域" },
				{ "d", "虚线图案" },
				{ "d0", "Type3字宽" },
				{ "d1", "Type3字宽及容器" },
				{ "Do", "绘制对象" },
				{ "DP", "标记内容点及属性" },
				{ "EI", "内嵌图像结束" },
				{ "EMC", "标记内容结束" },
				{ "ET", "文本区尾" },
				{ "EX", "兼容区尾" },
				{ "f", "非零填充" },
				{ "F", "非零填充" },
				{ "f*", "奇偶填充" },
				{ "G", "画线灰色" },
				{ "g", "非线灰色" },
				{ "gs", "绘图参数" },
				{ "h", "终点" },
				{ "i", "平滑度容限" },
				{ "ID", "内嵌图像数据" },
				{ "J", "线端样式" },
				{ "j", "连接线样式" },
				{ "K", "画线四色" },
				{ "k", "非线四色" },
				{ "l", "直线" },
				{ "m", "始点" },
				{ "M", "斜接面上限" },
				{ "MP", "标记内容点" },
				{ "n", "闭合路径不填充" },
				{ "q", "绘图状态" },
				{ "Q", "绘图状态出栈" },
				{ "re", "矩形" },
				{ "RG", "画线三色" },
				{ "rg", "非线三色" },
				{ "ri", "颜色渲染意向" },
				{ "s", "画封闭线" },
				{ "S", "画线" },
				{ "SC", "画线颜色" },
				{ "sc", "非线颜色" },
				{ "SCN", "画线颜色" },
				{ "scn", "非线颜色" },
				{ "sh", "阴影" },
				{ "T*", "换行" },
				{ "Tc", "字距" },
				{ "Td", "换行" },
				{ "TD", "换行" },
				{ "Tf", "字体" },
				{ "Tj", "字符串" },
				{ "TJ", "字符串" },
				{ "Tk", "单独字符渲染" },
				{ "TL", "行距" },
				{ "Tm", "文本矩阵" },
				{ "Tr", "文本渲染" },
				{ "Ts", "文本垂直偏移" },
				{ "Tw", "词距" },
				{ "Tz", "文本水平拉伸" },
				{ "v", "控尾曲线" },
				{ "w", "线宽" },
				{ "W", "非零裁剪" },
				{ "W*", "奇偶裁剪" },
				{ "y", "控首曲线" }
			};
		}
		#endregion

		public virtual PdfLiteral Name { get; }
		public PdfObject[] Operands { get; }
		public virtual PdfPageCommandType Type => PdfPageCommandType.Normal;
		internal bool HasOperand => Operands?.Length > 0;

		public PdfPageCommand(PdfLiteral oper, List<PdfObject> operands) {
			Name = oper;
			if (operands?.Count > 0) {
				Operands = new PdfObject[operands[operands.Count - 1] is PdfLiteral ? operands.Count - 1 : operands.Count];
				operands.CopyTo(0, Operands, 0, Operands.Length);
			}
		}

		internal static PdfPageCommand Create(string name, params PdfObject[] operands) {
			return new PdfPageCommand(new PdfLiteral(name), new List<PdfObject>(operands));
		}

		internal virtual void WriteToPdf(Stream target) {
			if (Operands != null) {
				foreach (var oi in Operands) {
					WriteOperand(oi, target);
				}
			}
			WriteOperator(Name, target);
		}

		protected static void WriteOperand(PdfObject operand, Stream target) {
			operand.ToPdf(null, target);
			target.WriteByte((byte)' ');
		}

		protected static void WriteOperator(PdfLiteral opName, Stream target) {
			opName.ToPdf(null, target);
			target.WriteByte((byte)'\n');
		}

		internal static bool GetFriendlyCommandName(string oper, out string friendlyName) {
			return __OperatorNames.TryGetValue(oper, out friendlyName);
		}

		internal string GetOperandsText() {
			return Operands != null ? Processor.PdfHelper.GetArrayString(Operands) : null;
		}
	}

	sealed class EnclosingCommand : PdfPageCommand, IPdfPageCommandContainer
	{
		const string BQ = "q";
		const string BT = "BT";
		const string BDC = "BDC";
		const string BMC = "BMC";
		const string BX = "BX";
		const string EQ = "Q";
		const string ET = "ET";
		const string EMC = "EMC";
		const string EX = "EX";

		static readonly string[] __StartEnclosingCommands = new string[] { BQ, BT, BDC, BMC, BX };
		static readonly string[] __EndEnclosingCommands = new string[] { EQ, ET, EMC, EX };
		static readonly PdfLiteral[] __EnclosingCommands = new PdfLiteral[] {
			new PdfLiteral(EQ),
			new PdfLiteral(ET),
			new PdfLiteral(EMC),
			new PdfLiteral(EMC),
			new PdfLiteral(EX)
		};

		public bool HasCommand => Commands.Count > 0;
		public IList<PdfPageCommand> Commands { get; }
		public override PdfPageCommandType Type => PdfPageCommandType.Enclosure;
		public EnclosingCommand(PdfLiteral oper, List<PdfObject> operands)
			: base(oper, operands) {
			Commands = new List<PdfPageCommand>();
		}

		internal static EnclosingCommand Create(string name, IEnumerable<PdfObject> operands, params PdfPageCommand[] subCommands) {
			var c = new EnclosingCommand(new PdfLiteral(name), operands != null ? new List<PdfObject>(operands) : null);
			((List<PdfPageCommand>)c.Commands).AddRange(subCommands);
			return c;
		}

		internal override void WriteToPdf(Stream target) {
			base.WriteToPdf(target);
			if (HasCommand) {
				foreach (var cmd in Commands) {
					cmd.WriteToPdf(target);
				}
			}
			WriteOperator(ValueHelper.MapValue(Name.ToString(), __StartEnclosingCommands, __EnclosingCommands), target);
		}

		internal static bool IsStartingCommand(string oper) {
			return __StartEnclosingCommands.Contains(oper);
		}
		internal static bool IsEndingCommand(string oper) {
			return __EndEnclosingCommands.Contains(oper);
		}
	}

	class TextCommand : PdfPageCommand
	{
		public TextInfo TextInfo { get; private set; }
		public override PdfPageCommandType Type => PdfPageCommandType.Text;
		public TextCommand(PdfLiteral oper, List<PdfObject> operands, TextInfo text)
			: base(oper, operands) {
			TextInfo = text;
		}
	}

	sealed class PaceAndTextCommand : TextCommand
	{
		public string[] DecodedTexts { get; private set; }
		public PaceAndTextCommand(PdfLiteral oper, List<PdfObject> operands, TextInfo text, FontInfo font)
			: base(oper, operands, text) {
			var a = (PdfArray)Operands[0];
			DecodedTexts = new string[a.Size];
			int i = 0;
			foreach (var item in a.ArrayList) {
				if (item.Type == PdfObject.STRING) {
					DecodedTexts[i] = font.DecodeText((PdfString)item);
				}
				++i;
			}
			text.Text = String.Concat(DecodedTexts);
		}
	}

	sealed class MatrixCommand : PdfPageCommand
	{
		public static PdfLiteral CM = new PdfLiteral("cm");
		public static PdfLiteral TM = new PdfLiteral("Tm");
		public override PdfPageCommandType Type => PdfPageCommandType.Matrix;
		public MatrixCommand(PdfLiteral oper, List<PdfObject> operands)
			: base(oper, operands) {
		}
		public MatrixCommand(PdfLiteral oper, float a, float b, float c, float d, float e, float f)
			: base(oper, new List<PdfObject>(6) {
					new PdfNumber (a), new PdfNumber(b),
					new PdfNumber (c), new PdfNumber (d),
					new PdfNumber (e), new PdfNumber(f)
			}) {
		}
		public void Multiply(double[] matrix) {
			var m1 = Array.ConvertAll(Operands, (i) => ((PdfNumber)i).DoubleValue);
			Operands[0] = new PdfNumber(m1[0] * matrix[0] + m1[1] * matrix[2]);
			Operands[1] = new PdfNumber(m1[0] * matrix[1] + m1[1] * matrix[3]);
			Operands[2] = new PdfNumber(m1[2] * matrix[0] + m1[3] * matrix[2]);
			Operands[3] = new PdfNumber(m1[2] * matrix[1] + m1[3] * matrix[3]);
			Operands[4] = new PdfNumber(m1[4] * matrix[0] + m1[5] * matrix[2] + matrix[4]);
			Operands[5] = new PdfNumber(m1[4] * matrix[1] + m1[5] * matrix[3] + matrix[5]);
		}
	}

	sealed class FontCommand : PdfPageCommand
	{
		public FontCommand(PdfLiteral oper, List<PdfObject> operands, string fontName)
			: base(oper, operands) {
			FontName = fontName;
		}

		public string FontName { get; }
		public PdfName ResourceName {
			get => Operands[0] as PdfName;
			set => Operands[0] = value;
		}
		public PdfNumber FontSize {
			get => Operands[1] as PdfNumber;
			set => Operands[1] = value;
		}
		public override PdfPageCommandType Type => PdfPageCommandType.Font;
	}

	sealed class InlineImageCommand : PdfPageCommand
	{
		//static readonly PdfName __DCT = new PdfName ("DCT");
		//static readonly PdfName __CCF = new PdfName ("CCF");
		static readonly PdfLiteral __ID = new PdfLiteral("ID");
		static readonly PdfLiteral __EI = new PdfLiteral("EI");

		public InlineImageCommand(PdfLiteral oper, List<PdfObject> operands) : base(oper, operands) {
		}

		public PdfImageData Image => Operands[0] as PdfImageData;
		public override PdfPageCommandType Type => PdfPageCommandType.InlineImage;

		internal override void WriteToPdf(Stream target) {
			var img = Image;
			WriteOperator(Name, target);
			foreach (var item in img) {
				item.Key.ToPdf(null, target);
				target.WriteByte((byte)' ');
				item.Value.ToPdf(null, target);
				target.WriteByte((byte)'\n');
			}
			WriteOperator(__ID, target);
			target.Write(img.RawBytes, 0, img.RawBytes.Length);
			target.WriteByte((byte)'\n');
			WriteOperator(__EI, target);
		}
	}
}
