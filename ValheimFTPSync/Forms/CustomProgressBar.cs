using System.ComponentModel;
using System.Windows.Forms;

namespace CustomControls
{
    public class CustomProgressBar : ProgressBar
    {
        private string _text = string.Empty;
        [Category("Additional Options"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string CustomText
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                Invalidate();//redraw component after change value from VS Properties section
            }
        }
        private SolidBrush _progressColourBrush = (SolidBrush)Brushes.LightGreen;

        [Category("Additional Options"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ProgressColor
        {
            get
            {
                return _progressColourBrush.Color;
            }
            set
            {
                _progressColourBrush.Dispose();
                _progressColourBrush = new SolidBrush(value);
            }
        }
        [Description("Font of the text on ProgressBar"), Category("Additional Options")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TextFont { get; set; } = DefaultFont;

        private SolidBrush _textColourBrush = (SolidBrush)Brushes.Black;
        [Category("Additional Options")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TextColor
        {
            get
            {
                return _textColourBrush.Color;
            }
            set
            {
                _textColourBrush.Dispose();
                _textColourBrush = new SolidBrush(value);
            }
        }

        public CustomProgressBar() : base()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void  OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle rect = ClientRectangle;

            //ProgressBarRenderer.DrawHorizontalBar(e.Graphics, rect);

            rect.Inflate(-1, -1);

            if (Value > 0)
            {
                Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(((float)Value / Maximum) * rect.Width), rect.Height);

                e.Graphics.FillRectangle(_progressColourBrush, clip);
            }
            SizeF len = e.Graphics.MeasureString(CustomText, TextFont);

            e.Graphics.DrawString(CustomText , TextFont, _textColourBrush, new PointF(10, this.Height / 2  - 8 ));
           
        }
        public new void Dispose()
        { 
            _textColourBrush.Dispose();
            _progressColourBrush.Dispose();
            base.Dispose();
        }
    }
}
