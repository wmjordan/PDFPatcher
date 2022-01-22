/************************************************************************************************
 * GlassButton - How to create an animating glass button using only GDI+ (and not using WPF).   *
 *                                                                                              *
 * Original developed by Łukasz Świątkowski - lukasz.swiatkowski@gmail.com                      *
 * Form-/Perfomance-/Behavior-Improvements by Fink Christoph - fink.christoph@gmail.com         *
 *                                                                                              *
 * Feel free to use this control in your application or to improve it in any way!               *
 ***********************************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace EnhancedGlassButton;

/// <summary>
///     Represents a glass button control.
/// </summary>
[ToolboxBitmap(typeof(Button))]
[ToolboxItem(true)]
[ToolboxItemFilter("System.Windows.Forms")]
[Description("Raises an event when the user clicks it.")]
public class GlassButton : Button
{
	#region " Global Vareables "

	private IContainer components;
	private Timer timer;

	#region " Vareables for Drawing "

	private GraphicsPath outerBorderPath;
	private GraphicsPath ContentPath;
	private GraphicsPath GlowClip;
	private GraphicsPath GlowBottomRadial;
	private GraphicsPath ShinePath;
	private GraphicsPath BorderPath;

	private PathGradientBrush GlowRadialPath;

	private LinearGradientBrush ShineBrush;

	private Pen outerBorderPen;
	private Pen BorderPen;

	private Color specialSymbolColor;

	private Brush specialSymbolBrush;
	private Brush ContentBrush;

	private Rectangle rect;
	private Rectangle rect2;

	#endregion

	/// <summary>
	///     The ToolTip of the Control.
	/// </summary>
	private readonly ToolTip toolTip = new();

	/// <summary>
	///     If false, the shine isn't drawn (-> symbolizes an disabled control).
	/// </summary>
	private bool drawShine = true;

	/// <summary>
	///     Set the trynsperency of the special Symbols.
	/// </summary>
	private const int transparencyFactor = 128;

	#endregion

	#region " Constructors "

	/// <summary>
	///     Initializes a new instance of the <see cref="Glass.GlassButton" /> class.
	/// </summary>
	public GlassButton() {
		DoubleBuffered = true;

		InitializeComponent();

		roundCorner = 6;
		timer.Interval = animationLength / framesCount;
		base.BackColor = Color.Transparent;
		BackColor = Color.Black;
		ForeColor = Color.White;
		OuterBorderColor = Color.White;
		InnerBorderColor = Color.Black;
		ShineColor = Color.White;
		GlowColor = Color.FromArgb(unchecked((int)0xFF8DBDFF));
		alternativeForm = false;
		showFocusBorder = false;
		animateGlow = false;
		showSpecialSymbol = false;
		specialSymbol = SpecialSymbols.Play;
		specialSymbolColor = Color.White;
		toolTipText = "";
		specialSymbolBrush = new SolidBrush(Color.FromArgb(transparencyFactor, specialSymbolColor));
		alternativeFocusBorderColor = Color.Black;
		alternativeFormDirection = Direction.Left;

		RecalcRect(currentFrame / (framesCount - 1f));

		SetStyle(
			ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
			ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
		SetStyle(ControlStyles.Opaque, false);

		SizeChanged += GlassButton_SizeChanged;
		MouseEnter += GlassButton_MouseEnter;
		MouseLeave += GlassButton_MouseLeave;
		GotFocus += GlassButton_GotFocus;
		LostFocus += GlassButton_LostFocus;
	}

	private void InitializeComponent() {
		components = new Container();
		timer = new Timer(components);
		SuspendLayout();
		// 
		// timer
		// 
		timer.Tick += timer_Tick;
		ResumeLayout(false);
	}

	/// <summary>
	///     Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing) {
		if (disposing) {
			if (imageButton != null) {
				imageButton.Parent.Dispose();
				imageButton.Parent = null;
				imageButton.Dispose();
				imageButton = null;
			}

			DisposeAll(outerBorderPath, ContentPath, GlowClip, GlowBottomRadial, ShinePath, BorderPath, GlowRadialPath,
				ShineBrush, outerBorderPen, BorderPen, specialSymbolBrush, ContentBrush, toolTip, components);
		}

		base.Dispose(disposing);
	}

	private static void DisposeAll(params IDisposable[] objects) {
		foreach (IDisposable item in objects) {
			item?.Dispose();
		}
	}

	#endregion

	#region " Fields and Properties "

	private Color backColor;

	/// <summary>
	///     Gets or sets the background color of the control.
	/// </summary>
	/// <returns>A <see cref="T:System.Drawing.Color" /> value representing the background color.</returns>
	[DefaultValue(typeof(Color), "Black")]
	public new Color BackColor {
		get => backColor;
		set {
			if (backColor.Equals(value)) {
				return;
			}

			backColor = value;
			UseVisualStyleBackColor = false;

			RecalcRect(currentFrame / (framesCount - 1f));

			OnBackColorChanged(EventArgs.Empty);
		}
	}

	/// <summary>
	///     Gets or sets the foreground color of the control.
	/// </summary>
	/// <returns>The foreground <see cref="T:System.Drawing.Color" /> of the control.</returns>
	[DefaultValue(typeof(Color), "White")]
	public new Color ForeColor {
		get => base.ForeColor;
		set {
			base.ForeColor = value;

			RecalcRect(currentFrame / (framesCount - 1f));
		}
	}

	private Color innerBorderColor;

	/// <summary>
	///     Gets or sets the inner border color of the control.
	/// </summary>
	/// <returns>A <see cref="T:System.Drawing.Color" /> value representing the color of the inner border.</returns>
	[DefaultValue(typeof(Color), "Black")]
	[Category("Appearance")]
	[Description("The inner border color of the control.")]
	public Color InnerBorderColor {
		get => innerBorderColor;
		set {
			if (innerBorderColor == value) {
				return;
			}

			innerBorderColor = value;

			RecalcRect(currentFrame / (framesCount - 1f));

			if (IsHandleCreated) {
				Invalidate();
			}
		}
	}

	/// <summary>
	///     Gets or sets the color of the special symbol.
	/// </summary>
	/// <value>The color of the special symbol.</value>
	[DefaultValue(typeof(Color), "White")]
	[Category("Appearance")]
	[Description("The inner border color of the control.")]
	public Color SpecialSymbolColor {
		get => specialSymbolColor;
		set {
			if (specialSymbolColor == value) {
				return;
			}

			specialSymbolColor = value;
			specialSymbolBrush = new SolidBrush(Color.FromArgb(transparencyFactor, specialSymbolColor));

			if (IsHandleCreated) {
				Invalidate();
			}
		}
	}

	private int roundCorner;

	/// <summary>
	///     Gets or sets the corner radius.
	/// </summary>
	/// <value>The corner radius.</value>
	[DefaultValue(6)]
	[Category("Appearance")]
	[Description("The radius of the corners.")]
	public int CornerRadius {
		get => roundCorner;
		set {
			if (roundCorner == value) {
				return;
			}

			roundCorner = value;

			RecalcRect(currentFrame / (framesCount - 1f));

			if (IsHandleCreated) {
				Invalidate();
			}
		}
	}

	private string toolTipText;

	/// <summary>
	///     Gets or sets the tool tip text.
	/// </summary>
	/// <value>The tool tip text.</value>
	[DefaultValue("")]
	[Category("Appearance")]
	[Description("The ToolTip-Text of the button. Leave blank to not show a ToolTip.")]
	public string ToolTipText {
		get => toolTipText;
		set {
			if (toolTipText == value) {
				return;
			}

			toolTipText = value;

			if (toolTipText.Length > 0) {
				toolTip.SetToolTip(this, toolTipText);
			}

			if (IsHandleCreated) {
				Invalidate();
			}
		}
	}

	private bool alternativeForm;

	/// <summary>
	///     Gets or sets the alternative form.
	/// </summary>
	/// <value>The alternative form.</value>
	[DefaultValue(false)]
	[Category("Appearance")]
	[Description("Draws the Button in an alternative Form.")]
	public bool AlternativeForm {
		get => alternativeForm;
		set {
			if (alternativeForm == value) {
				return;
			}

			alternativeForm = value;

			RecalcRect(currentFrame / (framesCount - 1f));

			if (IsHandleCreated) {
				Invalidate();
			}
		}
	}

	private bool animateGlow;

	/// <summary>
	///     Gets or sets a value indicating whether the glow is animated.
	/// </summary>
	/// <value><c>true</c> if glow is animated; otherwise, <c>false</c>.</value>
	[DefaultValue(false)]
	[Category("Appearance")]
	[Description("If true the glow is animated.")]
	public bool AnimateGlow {
		get => animateGlow;
		set {
			if (animateGlow == value) {
				return;
			}

			animateGlow = value;
			if (IsHandleCreated) {
				Invalidate();
			}
		}
	}

	private bool showSpecialSymbol;

	/// <summary>
	///     Gets or sets a value indicating whether a special symbol is drawn.
	/// </summary>
	/// <value><c>true</c> if special symbol is drawn; otherwise, <c>false</c>.</value>
	[DefaultValue(false)]
	[Category("Appearance")]
	[Description("If true, the selectet special symbol will be drawn on the button.")]
	public bool ShowSpecialSymbol {
		get => showSpecialSymbol;
		set {
			if (showSpecialSymbol == value) {
				return;
			}

			showSpecialSymbol = value;

			if (IsHandleCreated) {
				Invalidate();
			}
		}
	}

	/// <summary>
	///     List of all aviable special symbols.
	/// </summary>
	public enum SpecialSymbols
	{
		ArrowLeft,
		ArrowRight,
		ArrowUp,
		ArrowDown,
		Play,
		Pause,
		Stop,
		FastForward,
		Forward,
		Backward,
		FastBackward,
		Speaker,
		NoSpeaker,
		Repeat,
		RepeatAll,
		Shuffle
	}

	private SpecialSymbols specialSymbol;

	/// <summary>
	///     Gets or sets the special symbol.
	/// </summary>
	/// <value>The special symbol.</value>
	[DefaultValue(typeof(SpecialSymbols), "Play")]
	[Category("Appearance")]
	[Description("Sets the type of the special symbol on the button.")]
	public SpecialSymbols SpecialSymbol {
		get => specialSymbol;
		set {
			if (specialSymbol == value) {
				return;
			}

			specialSymbol = value;

			if (IsHandleCreated) {
				Invalidate();
			}
		}
	}

	public enum Direction
	{
		Left,
		Right
	}

	private Direction alternativeFormDirection;

	/// <summary>
	///     Gets or sets the alternative form direction.
	/// </summary>
	/// <value>The alternative form direction.</value>
	[DefaultValue(typeof(Direction), "Left")]
	[Category("Appearance")]
	[Description("Sets the Direction of the alternative Form.")]
	public Direction AlternativeFormDirection {
		get => alternativeFormDirection;
		set {
			if (alternativeFormDirection == value) {
				return;
			}

			alternativeFormDirection = value;

			RecalcRect(currentFrame / (framesCount - 1f));

			if (IsHandleCreated) {
				Invalidate();
			}
		}
	}

	private bool showFocusBorder;

	/// <summary>
	///     Gets or sets a value indicating whether the focus border is shown.
	/// </summary>
	/// <value><c>true</c> if focus border shown; otherwise, <c>false</c>.</value>
	[DefaultValue(false)]
	[Category("Appearance")]
	[Description("Draw the normal Focus-Border. Alternativ Focus-Border will be drawed if false.")]
	public bool ShowFocusBorder {
		get => showFocusBorder;
		set {
			if (showFocusBorder == value) {
				return;
			}

			showFocusBorder = value;

			RecalcRect(currentFrame / (framesCount - 1f));

			if (IsHandleCreated) {
				Invalidate();
			}
		}
	}

	private Color alternativeFocusBorderColor;

	/// <summary>
	///     Gets or sets the color of the alternative focus border.
	/// </summary>
	/// <value>The color of the alternative focus border.</value>
	[DefaultValue(typeof(Color), "Black")]
	[Category("Appearance")]
	[Description("The color of the alternative Focus-Border.")]
	public Color AlternativeFocusBorderColor {
		get => alternativeFocusBorderColor;
		set {
			if (alternativeFocusBorderColor == value) {
				return;
			}

			alternativeFocusBorderColor = value;

			RecalcRect(currentFrame / (framesCount - 1f));

			if (IsHandleCreated) {
				Invalidate();
			}
		}
	}

	private Color outerBorderColor;

	/// <summary>
	///     Gets or sets the outer border color of the control.
	/// </summary>
	/// <returns>A <see cref="T:System.Drawing.Color" /> value representing the color of the outer border.</returns>
	[DefaultValue(typeof(Color), "White")]
	[Category("Appearance")]
	[Description("The outer border color of the control.")]
	public Color OuterBorderColor {
		get => outerBorderColor;
		set {
			if (outerBorderColor == value) {
				return;
			}

			outerBorderColor = value;

			RecalcRect(currentFrame / (framesCount - 1f));

			if (IsHandleCreated) {
				Invalidate();
			}
		}
	}

	private Color shineColor;

	/// <summary>
	///     Gets or sets the shine color of the control.
	/// </summary>
	/// <returns>A <see cref="T:System.Drawing.Color" /> value representing the shine color.</returns>
	[DefaultValue(typeof(Color), "White")]
	[Category("Appearance")]
	[Description("The shine color of the control.")]
	public Color ShineColor {
		get => shineColor;
		set {
			if (shineColor == value) {
				return;
			}

			shineColor = value;

			RecalcRect(currentFrame / (framesCount - 1f));

			if (IsHandleCreated) {
				Invalidate();
			}
		}
	}

	private Color glowColor;

	/// <summary>
	///     Gets or sets the glow color of the control.
	/// </summary>
	/// <returns>A <see cref="T:System.Drawing.Color" /> value representing the glow color.</returns>
	[DefaultValue(typeof(Color), "255,141,189,255")]
	[Category("Appearance")]
	[Description("The glow color of the control.")]
	public Color GlowColor {
		get => glowColor;
		set {
			if (glowColor == value) {
				return;
			}

			glowColor = value;

			RecalcRect(currentFrame / (framesCount - 1f));

			if (IsHandleCreated) {
				Invalidate();
			}
		}
	}

	private bool isHovered;
	private bool isFocused;
	private bool isFocusedByKey;
	private bool isKeyDown;
	private bool isMouseDown;
	private bool isPressed => isKeyDown || (isMouseDown && isHovered);

	/// <summary>
	///     Gets the state of the button control.
	/// </summary>
	/// <value>The state of the button control.</value>
	[Browsable(false)]
	public PushButtonState State {
		get {
			if (!Enabled) {
				return PushButtonState.Disabled;
			}

			if (isPressed) {
				return PushButtonState.Pressed;
			}

			if (isHovered) {
				return PushButtonState.Hot;
			}

			if (isFocused || IsDefault) {
				return PushButtonState.Default;
			}

			return PushButtonState.Normal;
		}
	}

	#endregion

	#region " Overrided Methods "

	/// <summary>
	///     Raises the <see cref="E:System.Windows.Forms.Control.Click" /> event.
	/// </summary>
	/// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
	protected override void OnClick(EventArgs e) {
		isKeyDown = isMouseDown = false;
		base.OnClick(e);
	}

	/// <summary>
	///     Raises the <see cref="E:System.Windows.Forms.Control.Enter" /> event.
	/// </summary>
	/// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
	protected override void OnEnter(EventArgs e) {
		isFocused = isFocusedByKey = true;
		base.OnEnter(e);
	}

	/// <summary>
	///     Raises the <see cref="E:System.Windows.Forms.Control.Leave" /> event.
	/// </summary>
	/// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
	protected override void OnLeave(EventArgs e) {
		base.OnLeave(e);
		isFocused = isFocusedByKey = isKeyDown = isMouseDown = false;
		Invalidate();
	}

	/// <summary>
	///     Raises the <see cref="M:System.Windows.Forms.ButtonBase.OnKeyUp(System.Windows.Forms.KeyEventArgs)" /> event.
	/// </summary>
	/// <param name="kevent">A <see cref="T:System.Windows.Forms.KeyEventArgs" /> that contains the event data.</param>
	protected override void OnKeyDown(KeyEventArgs kevent) {
		if (kevent.KeyCode == Keys.Space) {
			isKeyDown = true;
			Invalidate();
		}

		base.OnKeyDown(kevent);
	}

	/// <summary>
	///     Raises the <see cref="M:System.Windows.Forms.ButtonBase.OnKeyUp(System.Windows.Forms.KeyEventArgs)" /> event.
	/// </summary>
	/// <param name="kevent">A <see cref="T:System.Windows.Forms.KeyEventArgs" /> that contains the event data.</param>
	protected override void OnKeyUp(KeyEventArgs kevent) {
		if (isKeyDown && kevent.KeyCode == Keys.Space) {
			isKeyDown = false;
			Invalidate();
		}

		base.OnKeyUp(kevent);
	}

	/// <summary>
	///     Raises the <see cref="E:System.Windows.Forms.Control.MouseDown" /> event.
	/// </summary>
	/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
	protected override void OnMouseDown(MouseEventArgs e) {
		if (!isMouseDown && e.Button == MouseButtons.Left) {
			isMouseDown = true;
			isFocusedByKey = false;
			Invalidate();
		}

		base.OnMouseDown(e);
	}

	/// <summary>
	///     Raises the <see cref="E:System.Windows.Forms.Control.MouseUp" /> event.
	/// </summary>
	/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
	protected override void OnMouseUp(MouseEventArgs e) {
		if (isMouseDown) {
			isMouseDown = false;
			Invalidate();
		}

		base.OnMouseUp(e);
	}

	/// <summary>
	///     Raises the <see cref="M:System.Windows.Forms.Control.OnMouseMove(System.Windows.Forms.MouseEventArgs)" /> event.
	/// </summary>
	/// <param name="mevent">A <see cref="T:System.Windows.Forms.MouseEventArgs" /> that contains the event data.</param>
	protected override void OnMouseMove(MouseEventArgs mevent) {
		base.OnMouseMove(mevent);
		if (mevent.Button == MouseButtons.None) {
			return;
		}

		if (!ClientRectangle.Contains(mevent.X, mevent.Y)) {
			if (!isHovered) {
				return;
			}

			isHovered = false;
			Invalidate();
		}
		else if (!isHovered) {
			isHovered = true;
			Invalidate();
		}
	}

	/// <summary>
	///     Raises the <see cref="E:System.Windows.Forms.Control.MouseEnter" /> event.
	/// </summary>
	/// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
	protected override void OnMouseEnter(EventArgs e) {
		isHovered = true;
		FadeIn();
		Invalidate();
		base.OnMouseEnter(e);
	}

	/// <summary>
	///     Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave" /> event.
	/// </summary>
	/// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
	protected override void OnMouseLeave(EventArgs e) {
		isHovered = false;
		FadeOut();
		Invalidate();
		base.OnMouseLeave(e);
	}

	#endregion

	#region " Painting "

	/// <summary>
	///     Raises the <see cref="M:System.Windows.Forms.ButtonBase.OnPaint(System.Windows.Forms.PaintEventArgs)" /> event.
	/// </summary>
	/// <param name="pevent">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
	protected override void OnPaint(PaintEventArgs pevent) {
		SmoothingMode sm = pevent.Graphics.SmoothingMode;
		pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

		DrawButtonBackground(pevent.Graphics);
		DrawForegroundFromButton(pevent);
		DrawButtonForeground(pevent.Graphics);

		pevent.Graphics.SmoothingMode = sm;
	}

	/// <summary>
	///     Draws the button background.
	/// </summary>
	/// <param name="g">The graphics to draw on.</param>
	private void DrawButtonBackground(Graphics g) {
		//white border
		g.DrawPath(outerBorderPen, outerBorderPath);

		//content
		g.FillPath(ContentBrush, ContentPath);

		//glow
		if ((isHovered || isAnimating) && !isPressed) {
			g.SetClip(GlowClip, CombineMode.Intersect);
			g.FillPath(GlowRadialPath, GlowBottomRadial);

			g.ResetClip();
		}

		//shine
		if (drawShine && Enabled) {
			g.FillPath(ShineBrush, ShinePath);
		}

		//black border
		g.DrawPath(BorderPen, BorderPath);

		//Draws the special Symbol
		if (showSpecialSymbol) {
			DrawSpecialSymbol(g);
		}
	}

	/// <summary>
	///     Draws the special symbol.
	/// </summary>
	/// <param name="g">The graphics to draw on.</param>
	private void DrawSpecialSymbol(Graphics g) {
		const int offset = 15;
		int LineWidth = Width / 15;
		Pen pen = new(specialSymbolBrush, Width / 8) { EndCap = LineCap.ArrowAnchor };
		Pen aPen = new(specialSymbolBrush, Width / 4) { EndCap = LineCap.ArrowAnchor };
		Font font = new("Arial", LineWidth * 4, FontStyle.Bold);

		switch (specialSymbol) {
			#region " Arrow Left "

			case SpecialSymbols.ArrowLeft:
				g.DrawLine(aPen, Width - (Width / 5), Height / 2, Width / 8, Height / 2);
				break;

			#endregion

			#region " Arrow Right "

			case SpecialSymbols.ArrowRight:
				g.DrawLine(aPen, Width / 6, Height / 2, Width - (Width / 8), Height / 2);
				break;

			#endregion

			#region " Arrow Up "

			case SpecialSymbols.ArrowUp:
				g.DrawLine(aPen, Width / 2, Height - (Height / 5), Width / 2, Height / 8);
				break;

			#endregion

			#region " Arrow Down "

			case SpecialSymbols.ArrowDown:
				g.DrawLine(aPen, Width / 2, Height / 5, Width / 2, Height - (Height / 8));
				break;

			#endregion

			#region " Play "

			case SpecialSymbols.Play:
				g.FillPolygon(specialSymbolBrush,
					new Point[] {
						new((Width / 4) + (Width / 20), Height / 4),
						new(Width - (Width / 4) + (Width / 20), Height / 2),
						new((Width / 4) + (Width / 20), Height - (Height / 4))
					});
				break;

			#endregion

			#region " Pause "

			case SpecialSymbols.Pause:
				g.FillRectangle(specialSymbolBrush, new Rectangle(Width / 4, Height / 4,
					((Width / 2) - (Width / 10)) / 2, Height / 2));
				g.FillRectangle(specialSymbolBrush, new Rectangle((Width / 2) + (Width / 20), Height / 4,
					((Width / 2) - (Width / 10)) / 2, Height / 2));
				break;

			#endregion

			#region " Stop "

			case SpecialSymbols.Stop:
				g.FillRectangle(specialSymbolBrush, new Rectangle((Width / 4) + (Width / 20),
					(Height / 4) + (Height / 20),
					(Width / 2) - (Width / 10), (Height / 2) - (Width / 10)));
				break;

			#endregion

			#region " FastForward "

			case SpecialSymbols.FastForward:
				g.FillPolygon(specialSymbolBrush,
					new Point[] {
						new(Width / 4, Height / 4), new(Width / 2, Height / 2), new(Width / 4, Height - (Height / 4))
					});
				g.FillPolygon(specialSymbolBrush,
					new Point[] {
						new(Width / 2, Height / 4), new(3 * Width / 4, Height / 2),
						new(Width / 2, Height - (Height / 4))
					});
				g.FillRectangle(specialSymbolBrush, new Rectangle(3 * Width / 4, Height / 4,
					Width / 12, Height / 2));
				break;

			#endregion

			#region " Forward "

			case SpecialSymbols.Forward:
				g.FillPolygon(specialSymbolBrush,
					new Point[] {
						new((Width / 4) + (Width / 12), Height / 4), new((Width / 2) + (Width / 12), Height / 2),
						new((Width / 4) + (Width / 12), Height - (Height / 4))
					});
				g.FillPolygon(specialSymbolBrush,
					new Point[] {
						new((Width / 2) + (Width / 12), Height / 4), new((3 * Width / 4) + (Width / 12), Height / 2),
						new((Width / 2) + (Width / 12), Height - (Height / 4))
					});
				break;

			#endregion

			#region " Backward "

			case SpecialSymbols.Backward:
				g.FillPolygon(specialSymbolBrush,
					new Point[] {
						new((Width / 4) - (Width / 12), Height / 2), new((Width / 2) - (Width / 12), Height / 4),
						new((Width / 2) - (Width / 12), Height - (Height / 4))
					});
				g.FillPolygon(specialSymbolBrush,
					new Point[] {
						new((Width / 2) - (Width / 12), Height / 2), new((3 * Width / 4) - (Width / 12), Height / 4),
						new((3 * Width / 4) - (Width / 12), Height - (Height / 4))
					});
				break;

			#endregion

			#region " FastBackward "

			case SpecialSymbols.FastBackward:
				g.FillPolygon(specialSymbolBrush,
					new Point[] {
						new(Width / 4, Height / 2), new(Width / 2, Height / 4), new(Width / 2, Height - (Height / 4))
					});
				g.FillPolygon(specialSymbolBrush,
					new Point[] {
						new(Width / 2, Height / 2), new(3 * Width / 4, Height / 4),
						new(3 * Width / 4, Height - (Height / 4))
					});
				g.FillRectangle(specialSymbolBrush, new Rectangle((Width / 4) - (Width / 12), Height / 4,
					Width / 12, Height / 2));
				break;

			#endregion

			#region " Speaker "

			case SpecialSymbols.Speaker:
				g.DrawPolygon(new Pen(specialSymbolBrush, Width / 20),
					new Point[] {
						new((Width / 2) - (Width / 6) - (Width / offset), (Height / 4) + (Height / 10)),
						new((Width / 2) - (Width / offset), (Height / 4) + (Height / 10)),
						new((Width / 2) + (Width / 5) - (Width / offset), Height / 4),
						new((Width / 2) + (Width / 5) - (Width / offset), 3 * Height / 4),
						new((Width / 2) - (Width / offset), (3 * Height / 4) - (Height / 10)),
						new((Width / 2) - (Width / 6) - (Width / offset), (3 * Height / 4) - (Height / 10))
					});
				g.DrawLine(new Pen(specialSymbolBrush, Width / 20), (Width / 2) - (Width / offset),
					(Height / 4) + (Height / 10) + (Width / 40), (Width / 2) - (Width / offset),
					Height - ((Height / 4) + (Height / 10) + (Width / 40)));
				break;

			#endregion

			#region " NoSpeaker "

			case SpecialSymbols.NoSpeaker:
				g.DrawPolygon(new Pen(specialSymbolBrush, Width / 20),
					new Point[] {
						new((Width / 2) - (Width / 6) - (Width / offset), (Height / 4) + (Height / 10)),
						new((Width / 2) - (Width / offset), (Height / 4) + (Height / 10)),
						new((Width / 2) + (Width / 5) - (Width / offset), Height / 4),
						new((Width / 2) + (Width / 5) - (Width / offset), 3 * Height / 4),
						new((Width / 2) - (Width / offset), (3 * Height / 4) - (Height / 10)),
						new((Width / 2) - (Width / 6) - (Width / offset), (3 * Height / 4) - (Height / 10))
					});
				g.DrawLine(new Pen(specialSymbolBrush, Width / 20), (Width / 2) - (Width / offset),
					(Height / 4) + (Height / 10) + (Width / 40), (Width / 2) - (Width / offset),
					Height - ((Height / 4) + (Height / 10) + (Width / 40)));
				g.DrawLine(new Pen(specialSymbolBrush, Width / 20),
					(int)((Width / 2) - (Width / 3.5) - (Width / offset)), (3 * Height / 4) - (Height / 10),
					(Width / 2) + (Width / 3) - (Width / offset), (Height / 4) + (Height / 12) + (Width / 40));
				break;

			#endregion

			#region " Repeat "

			case SpecialSymbols.Repeat:
				g.DrawLine(new Pen(specialSymbolBrush, LineWidth),
					new Point(Width / 4, Height / 3),
					new Point((int)(Width - (Width / 2.4)), Height / 3));
				g.DrawArc(new Pen(specialSymbolBrush, LineWidth), (int)(Width - (Width * 0.6)), Height / 3,
					Width / 3, Height / 3, 270, 180);
				g.DrawLine(new Pen(specialSymbolBrush, LineWidth),
					new Point((int)(Width - (Width / 2.4)), Height - (Height / 3)),
					new Point((int)(Width / 3.2), Height - (Height / 3)));
				g.DrawLine(pen,
					new Point((int)(Width / 3.2), Height - (Height / 3)),
					new Point(Width / 4, Height - (Height / 3)));
				break;

			#endregion

			#region " RepeatAll "

			case SpecialSymbols.RepeatAll:
				g.DrawLine(new Pen(specialSymbolBrush, LineWidth),
					new Point((int)(Width / 2.4), Height / 3),
					new Point((int)(Width - (Width / 2.4)), Height / 3));
				g.DrawArc(new Pen(specialSymbolBrush, LineWidth), (int)(Width - (Width * 0.6)), Height / 3,
					Width / 3, Height / 3, 270, 180);
				g.DrawLine(new Pen(specialSymbolBrush, LineWidth),
					new Point((int)(Width - (Width / 2.4)), Height - (Height / 3)),
					new Point((int)(Width / 2.4), Height - (Height / 3)));
				g.DrawLine(pen,
					new Point((int)(Width / 2.4), Height - (Height / 3)),
					new Point(Width / 3, Height - (Height / 3)));
				g.DrawArc(new Pen(specialSymbolBrush, LineWidth), Width / 4, Height / 3,
					Width / 3, Height / 3, 90, 180);
				break;

			#endregion

			#region " Shuffle "

			case SpecialSymbols.Shuffle:
				g.DrawString("1", font, specialSymbolBrush, Width / 2 / 4, (Height / 2) - (LineWidth * 2));
				int sWidth = (int)g.MeasureString("2", font).Width;
				int sHeigth = (int)g.MeasureString("2", font).Height;
				g.DrawString("2", font, specialSymbolBrush, (Width / 2) - (sWidth / 2) - (Width / (2 * offset)),
					Height - LineWidth - sHeigth);
				sWidth = (int)g.MeasureString("3", font).Width;
				g.DrawString("3", font, specialSymbolBrush, Width - (Width / 2 / 4) - sWidth - (Width / (2 * offset)),
					(Height / 2) - (LineWidth * 2));
				g.DrawArc(pen, Width / 2 / 2, Height / 6, Width - (Width / 2), (int)(Height / 2.2), 170, 210);
				break;

			#endregion

			default:
				break;
		}
	}

	/// <summary>
	///     Draws the button foreground.
	/// </summary>
	/// <param name="g">The graphics to draw on.</param>
	private void DrawButtonForeground(Graphics g) {
		if (!ShowFocusBorder || !Focused || !ShowFocusCues || alternativeForm) {
			return;
		}

		Rectangle rect = ClientRectangle;
		rect.Inflate(-4, -4);
		ControlPaint.DrawFocusRectangle(g, rect);
	}

	private Button imageButton;

	/// <summary>
	///     Draws the foreground from button.
	/// </summary>
	/// <param name="pevent">The <see cref="System.Windows.Forms.PaintEventArgs" /> instance containing the event data.</param>
	private void DrawForegroundFromButton(PaintEventArgs pevent) {
		if (imageButton == null) {
			imageButton = new Button { Parent = new TransparentControl(), BackColor = Color.Transparent };
			imageButton.FlatAppearance.BorderSize = 0;
			imageButton.FlatStyle = FlatStyle.Flat;
		}

		if (direction != 0) {
			imageButton.SuspendLayout();
		}

		imageButton.ForeColor = ForeColor;
		imageButton.Font = Font;
		imageButton.RightToLeft = RightToLeft;
		imageButton.Image = Image;
		imageButton.ImageAlign = ImageAlign;
		imageButton.ImageIndex = ImageIndex;
		imageButton.ImageKey = ImageKey;
		imageButton.ImageList = ImageList;
		imageButton.Padding = Padding;
		imageButton.Size = Size;
		imageButton.Text = Text;
		imageButton.TextAlign = TextAlign;
		imageButton.TextImageRelation = TextImageRelation;
		imageButton.UseCompatibleTextRendering = UseCompatibleTextRendering;
		imageButton.UseMnemonic = UseMnemonic;
		if (direction != 0) {
			imageButton.ResumeLayout();
		}

		InvokePaint(imageButton, pevent);
	}

	private sealed class TransparentControl : Control
	{
		protected override void OnPaintBackground(PaintEventArgs pevent) { }
		protected override void OnPaint(PaintEventArgs e) { }
	}

	/// <summary>
	///     Creates the round rectangle.
	/// </summary>
	/// <param name="rectangle">The rectangle.</param>
	/// <param name="radius">The radius.</param>
	/// <returns></returns>
	private GraphicsPath CreateRoundRectangle(Rectangle rectangle, int radius) {
		GraphicsPath path = new();
		int l = rectangle.Left;
		int t = rectangle.Top;
		int w = rectangle.Width;
		int h = rectangle.Height;
		int d = radius << 1;

		if (alternativeForm) {
			if (alternativeFormDirection == Direction.Left) {
				path.AddArc(l, t, h, h, 90, 180);
				path.AddLine(l + h, t, l + w, t);
				path.AddCurve(new Point[] {
					new(l + w, t), new(l + w - (h / 6), t + (h / 4)), new((int)(l + w - (h / 4.7)), t + (h / 2)),
					new(l + w - (h / 6), t + (3 * h / 4)), new(l + w, t + h)
				});
				path.AddLine(l + h, t + h, l + w, t + h);
			}
			else {
				path.AddCurve(new Point[] {
					new(l, t), new(l + (h / 6), t + (h / 4)), new((int)(l + (h / 4.85)), t + (h / 2)),
					new(l + (h / 6), t + (3 * h / 4)), new(l, t + h)
				});
				path.AddLine(l, t + h, l + w - h, t + h);
				path.AddArc(l + w - h, t, h, h, 90, -180);
				path.AddLine(l + w - h, t, l, t);
			}
		}
		else {
			path.AddArc(l, t, d, d, 180, 90); // topleft
			path.AddLine(l + radius, t, l + w - radius, t); // top
			path.AddArc(l + w - d, t, d, d, 270, 90); // topright
			path.AddLine(l + w, t + radius, l + w, t + h - radius); // right
			path.AddArc(l + w - d, t + h - d, d, d, 0, 90); // bottomright
			path.AddLine(l + w - radius, t + h, l + radius, t + h); // bottom
			path.AddArc(l, t + h - d, d, d, 90, 90); // bottomleft
			path.AddLine(l, t + h - radius, l, t + radius); // left
		}

		path.CloseFigure();

		return path;
	}

	/// <summary>
	///     Creates the top round rectangle.
	/// </summary>
	/// <param name="rectangle">The rectangle.</param>
	/// <param name="radius">The radius.</param>
	/// <returns></returns>
	private GraphicsPath CreateTopRoundRectangle(Rectangle rectangle, int radius) {
		GraphicsPath path = new();
		int l = rectangle.Left;
		int t = rectangle.Top;
		int w = rectangle.Width;
		int h = rectangle.Height;
		int d = radius << 1;

		if (alternativeForm) {
			if (alternativeFormDirection == Direction.Left) {
				path.AddArc(l, t, h * 2, h * 2, 180, 90);
				path.AddLine(l + h, t, l + w, t);
				path.AddCurve(new Point[] {
					new(l + w, t), new(l + w - (h / 3), t + (h / 2)), new((int)(l + w - (h / 2.35)), t + h)
				});
			}
			else {
				path.AddCurve(
					new Point[] { new(l, t), new(l + (h / 3), t + (h / 2)), new((int)(l + (h / 2.35)), t + h) });
				path.AddLine((int)(l + (h / 2.35)), t + h, l + w - h, t + h);
				path.AddArc(l + w - (h * 2), t, h * 2, h * 2, 0, -90);
			}
		}
		else {
			path.AddArc(l, t, d, d, 180, 90); // topleft
			path.AddLine(l + radius, t, l + w - radius, t); // top
			path.AddArc(l + w - d, t, d, d, 270, 90); // topright
			path.AddLine(l + w, t + radius, l + w, t + h); // right
			path.AddLine(l + w, t + h, l, t + h); // bottom
			path.AddLine(l, t + h, l, t + radius); // left
		}

		path.CloseFigure();

		return path;
	}

	/// <summary>
	///     Creates the bottom radial path.
	/// </summary>
	/// <param name="rectangle">The rectangle.</param>
	/// <returns></returns>
	private static GraphicsPath CreateBottomRadialPath(Rectangle rectangle) {
		GraphicsPath path = new();
		RectangleF rect = rectangle;
		rect.X -= rectangle.Width * .35f;
		rect.Y -= rectangle.Height * .15f;
		rect.Width *= 1.7f;
		rect.Height *= 2.3f;
		path.AddEllipse(rect);
		path.CloseFigure();
		return path;
	}

	/// <summary>
	///     Handles the SizeChanged event of the GlassButton control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
	private void GlassButton_SizeChanged(object sender, EventArgs e) {
		RecalcRect(currentFrame / (framesCount - 1f));
	}

	/// <summary>
	///     Handles the MouseLeave event of the GlassButton control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
	private void GlassButton_MouseLeave(object sender, EventArgs e) {
		RecalcGlow(currentFrame / (framesCount - 1f));
	}

	/// <summary>
	///     Handles the MouseEnter event of the GlassButton control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
	private void GlassButton_MouseEnter(object sender, EventArgs e) {
		RecalcGlow(currentFrame / (framesCount - 1f));
	}

	/// <summary>
	///     Handles the LostFocus event of the GlassButton control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
	private void GlassButton_LostFocus(object sender, EventArgs e) {
		RecalcOuterBorder();
	}

	/// <summary>
	///     Handles the GotFocus event of the GlassButton control.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
	private void GlassButton_GotFocus(object sender, EventArgs e) {
		RecalcOuterBorder();
	}

	/// <summary>
	///     Recalcs the rectangles for drawing.
	/// </summary>
	/// <param name="glowOpacity">The glow opacity.</param>
	private void RecalcRect(float glowOpacity) {
		try {
			int rCorner = roundCorner;

			if (roundCorner > Height / 2) {
				rCorner = Height / 2;
			}

			if (roundCorner > Width / 2) {
				rCorner = Width / 2;
			}

			rect = RecalcOuterBorder();

			rect = RecalcContent(rect, out rect2);

			RecalcGlow(glowOpacity);

			rect2 = RecalcShine(rect2);

			BorderPath = CreateRoundRectangle(rect, rCorner);

			BorderPen = new Pen(innerBorderColor);
		}
		catch { }
	}

	/// <summary>
	///     Recalcs the shine.
	/// </summary>
	/// <param name="rect2">The rect2.</param>
	/// <returns></returns>
	private Rectangle RecalcShine(Rectangle rect2) {
		int rCorner = roundCorner;

		if (roundCorner > Height / 2) {
			rCorner = Height / 2;
		}

		if (roundCorner > Width / 2) {
			rCorner = Width / 2;
		}

		if (rect2.Width > 0 && rect2.Height > 0) {
			rect2.Height++;
			ShinePath = CreateTopRoundRectangle(rect2, rCorner);

			rect2.Height++;
			int opacity = 0x99;
			if (isPressed) {
				opacity = (int)((.4f * opacity) + .5f);
			}

			ShineBrush = new LinearGradientBrush(rect2, Color.FromArgb(opacity, shineColor),
				Color.FromArgb(opacity / 3, shineColor), LinearGradientMode.Vertical);

			rect2.Height -= 2;

			drawShine = true;
		}
		else {
			drawShine = false;
		}

		return rect2;
	}

	/// <summary>
	///     Recalcs the glow.
	/// </summary>
	/// <param name="glowOpacity">The glow opacity.</param>
	private void RecalcGlow(float glowOpacity) {
		int rCorner = roundCorner;

		if (roundCorner > Height / 2) {
			rCorner = Height / 2;
		}

		if (roundCorner > Width / 2) {
			rCorner = Width / 2;
		}

		GlowClip = CreateRoundRectangle(rect, rCorner);
		GlowBottomRadial = CreateBottomRadialPath(rect);

		GlowRadialPath = new PathGradientBrush(GlowBottomRadial);

		int opacity = (int)((0xB2 * glowOpacity) + .5f);

		if (!animateGlow) {
			opacity = isHovered ? 255 : 0;
		}

		GlowRadialPath.CenterColor = Color.FromArgb(opacity, glowColor);
		GlowRadialPath.SurroundColors = new[] { Color.FromArgb(0, glowColor) };
	}

	/// <summary>
	///     Recalcs the content.
	/// </summary>
	/// <param name="rect">The rect.</param>
	/// <param name="rect2">The rect2.</param>
	/// <returns></returns>
	private Rectangle RecalcContent(Rectangle rect, out Rectangle rect2) {
		int rCorner = roundCorner;

		if (roundCorner > Height / 2) {
			rCorner = Height / 2;
		}

		if (roundCorner > Width / 2) {
			rCorner = Width / 2;
		}

		rect.X++;
		rect.Y++;
		rect.Width -= 2;
		rect.Height -= 2;

		rect2 = rect;
		rect2.Height >>= 1;

		ContentPath = CreateRoundRectangle(rect, rCorner);
		int opacity = isPressed ? 0xcc : 0x7f;
		ContentBrush = new SolidBrush(Color.FromArgb(opacity, backColor));
		return rect;
	}

	/// <summary>
	///     Recalcs the outer border.
	/// </summary>
	/// <returns></returns>
	private Rectangle RecalcOuterBorder() {
		int rCorner = roundCorner;

		if (roundCorner > Height / 2) {
			rCorner = Height / 2;
		}

		if (roundCorner > Width / 2) {
			rCorner = Width / 2;
		}

		Rectangle rect;
		rect = ClientRectangle;
		rect.Width--;
		rect.Height--;
		outerBorderPath = CreateRoundRectangle(rect, rCorner);
		rect.Inflate(1, 1);
		GraphicsPath region = CreateRoundRectangle(rect, rCorner);
		Region = new Region(region);
		rect.Inflate(-1, -1);

		Color col = outerBorderColor;
		if (Focused && !ShowFocusBorder) {
			col = alternativeFocusBorderColor;
		}

		outerBorderPen = new Pen(col);
		return rect;
	}

	#endregion

	#region " Unused Properties & Events "

	/// <summary>This property is not relevant for this class.</summary>
	/// <returns>This property is not relevant for this class.</returns>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new FlatButtonAppearance FlatAppearance => base.FlatAppearance;

	/// <summary>This property is not relevant for this class.</summary>
	/// <returns>This property is not relevant for this class.</returns>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new FlatStyle FlatStyle {
		get => base.FlatStyle;
		set => base.FlatStyle = value;
	}

	/// <summary>This property is not relevant for this class.</summary>
	/// <returns>This property is not relevant for this class.</returns>
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new bool UseVisualStyleBackColor {
		get => base.UseVisualStyleBackColor;
		set => base.UseVisualStyleBackColor = value;
	}

	#endregion

	#region " Animation Support "

	private const int animationLength = 300;
	private const int framesCount = 10;
	private int currentFrame;
	private int direction;

	private bool isAnimating => direction != 0;

	private void FadeIn() {
		direction = 1;
		timer.Enabled = true;
	}

	private void FadeOut() {
		direction = -1;
		timer.Enabled = true;
	}

	private void timer_Tick(object sender, EventArgs e) {
		if (!timer.Enabled || !animateGlow) {
			return;
		}

		RecalcRect(currentFrame / (framesCount - 1f));
		Refresh();
		currentFrame += direction;
		if (currentFrame == -1) {
			currentFrame = 0;
			timer.Enabled = false;
			direction = 0;
			return;
		}

		if (currentFrame != framesCount) {
			return;
		}

		currentFrame = framesCount - 1;
		timer.Enabled = false;
		direction = 0;
	}

	#endregion
}