using System;
using System.ComponentModel;
using PDFPatcher.Common;
using PDFPatcher.Model;

namespace PDFPatcher.Functions.Editor
{
	sealed partial class InsertBookmarkForm : DraggableForm
	{
		public event EventHandler OkClicked;

		/// <summary>
		/// 获取或设置书签标题。
		/// </summary>
		public string Title { get => _TitleBox.Text; set => _TitleBox.Text = value; }
		public string Comment { get => _CommentBox.Text; set => _CommentBox.Text = value; }
		/// <summary>
		/// 获取或设置书签的位置。
		/// </summary>
		public float TargetPosition { get => (float)_PositionBox.Value; set => _PositionBox.SetValue(value); }
		int _TargetPageNumber;
		public int TargetPageNumber {
			get => _TargetPageNumber;
			set { _TargetPageNumber = value; _PageLabel.Text = $"第{value.ToText()}页"; }
		}
		/// <summary>
		/// 获取新书签的插入位置（当前书签后：1；子书签：2；父书签后：3；当前书签前：4）
		/// </summary>
		public InsertBookmarkPositionType InsertMode => _AfterCurrentBox.Checked ? InsertBookmarkPositionType.AfterCurrent
					: _AsChildBox.Checked ? InsertBookmarkPositionType.AsChild
					: _AfterParentBox.Checked ? InsertBookmarkPositionType.AfterParent
					: _BeforeCurrentBox.Checked ? InsertBookmarkPositionType.BeforeCurrent
					: InsertBookmarkPositionType.Undefined;

		[Browsable(false)]
		internal Controller Controller { get; set; }

		public InsertBookmarkForm() {
			InitializeComponent();
			this.OnFirstLoad(OnLoad);
		}
		public void SetInsertMode(InsertBookmarkPositionType positionType) {
			switch (positionType) {
				case InsertBookmarkPositionType.AfterCurrent: _AfterCurrentBox.Checked = true; return;
				case InsertBookmarkPositionType.AsChild: _AsChildBox.Checked = true; return;
				case InsertBookmarkPositionType.AfterParent: _AfterParentBox.Checked = true; return;
				case InsertBookmarkPositionType.BeforeCurrent: _BeforeCurrentBox.Checked = true; return;
			}
		}
		void OnLoad() {
			VisibleChanged += (s, args) => {
				if (!Visible) {
					return;
				}
				_TitleBox.Focus();
				_TitleBox.SelectAll();
			};
			_AfterCurrentBox.DoubleClick += InsertModeBox_DoubleClick;
			_AfterParentBox.DoubleClick += InsertModeBox_DoubleClick;
			_AsChildBox.DoubleClick += InsertModeBox_DoubleClick;
			_BeforeCurrentBox.DoubleClick += InsertModeBox_DoubleClick;
			_ReplaceBookmarkBox.DoubleClick += InsertModeBox_DoubleClick;
			_OkButton.Click += (s, args) => {
				OkClicked?.Invoke(this, args);
				if (_AsChildBox.Checked || _AfterParentBox.Checked || _ReplaceBookmarkBox.Checked) {
					_AfterCurrentBox.Checked = true;
				}
				Hide();
			};
			_CancelButton.Click += (s, args) => Hide();
		}

		void InsertModeBox_DoubleClick(object sender, EventArgs e) {
			_OkButton.PerformClick();
		}

		protected override void OnActivated(EventArgs e) {
			base.OnActivated(e);
			_TitleBox.Focus();
		}

		protected override void OnDeactivate(EventArgs e) {
			Hide();
			base.OnDeactivate(e);
		}
	}
}
