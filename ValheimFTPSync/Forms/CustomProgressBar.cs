using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace CustomControls
{
    public class CustomProgressBar : ProgressBar
    {
        private static readonly Color s_defaultForeColor = Color.Black;

        [Description("Text on ProgressBar")]
        [Category("Additional Options"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [AllowNull]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
                Invalidate();//redraw component after change value from VS Properties section
            }
        }
        private SolidBrush _progressBrush = (SolidBrush)Brushes.LightGreen;
        private SolidBrush _fontBrush = new SolidBrush(DefaultForeColor);

        public override Color ForeColor
        {
            get
            {
                return _fontBrush.Color;
            }
            set
            {
                _fontBrush.Dispose();
                _fontBrush = new SolidBrush(value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void ResetForeColor()
        {
            ForeColor = s_defaultForeColor;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldSerializeForeColor()
        {
            return ForeColor != s_defaultForeColor;
        }

        [Category("Additional Options"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ProgressColor
        {
            get
            {
                return _progressBrush.Color;
            }
            set
            {
                _progressBrush.Dispose();
                _progressBrush = new SolidBrush(value);
            }
        }
        [Category("Additional Options"), Description("Font of the text on ProgressBar")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [AllowNull]
        public override Font Font
        {
            get => base.Font;
            set => base.Font = value;
        }

        public CustomProgressBar() : base()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            ForeColor = s_defaultForeColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle rect = ClientRectangle;

            //ProgressBarRenderer.DrawHorizontalBar(e.Graphics, rect);

            rect.Inflate(-1, -1);

            if (Value > 0)
            {
                Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(((float)Value / Maximum) * rect.Width), rect.Height);
                e.Graphics.FillRectangle(_progressBrush, clip);
            }
            SizeF len = e.Graphics.MeasureString(Text, Font);
            e.Graphics.DrawString(Text, Font, _fontBrush, new PointF(10, this.Height / 2 - 8));

        }
        public new void Dispose()
        {
            _fontBrush.Dispose();
            _progressBrush.Dispose();
            base.Dispose();
        }
    }
}
