using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace milano88.UI.Controls
{
    public class MSGradientPanel : Panel
    {
        private BufferedGraphics _bufGraphics;
        private readonly BufferedGraphicsContext _bufContext = BufferedGraphicsManager.Current;

        public MSGradientPanel()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            this.Size = new Size(200, 200);
            this.BackColor = Color.Transparent;
            UpdateGraphicsBuffer();
        }

        private Color _color1 = Color.SkyBlue;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "SkyBlue")]
        public Color Color1
        {
            get => _color1;
            set
            {
                _color1 = value;
                this.Invalidate();
            }
        }

        private Color _color2 = Color.SlateBlue;
        [Category("Custom Properties")]
        [DefaultValue(typeof(Color), "SlateBlue")]
        public Color Color2
        {
            get => _color2;
            set
            {
                _color2 = value;
                this.Invalidate();
            }
        }

        private GradientStyle _style = MSGradientPanel.GradientStyle.Horizontal;
        public enum GradientStyle
        {
            Horizontal,
            Vertical
        }
        [Category("Custom Properties")]
        [DefaultValue(GradientStyle.Horizontal)]
        public MSGradientPanel.GradientStyle Style
        {
            get => _style;
            set
            {
                _style = value;
                this.Invalidate();
            }
        }

        private void IncreaseGraphicsQuality(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        private void UpdateGraphicsBuffer()
        {
            if (this.Width > 0 && this.Height > 0)
            {
                _bufContext.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);
                _bufGraphics = _bufContext.Allocate(this.CreateGraphics(), this.ClientRectangle);
                IncreaseGraphicsQuality(_bufGraphics.Graphics);
            }
        }

        protected override void OnSizeChanged(EventArgs e) => UpdateGraphicsBuffer();

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (Parent != null && BackColor == Color.Transparent)
            {
                Rectangle rect = new Rectangle(Left, Top, Width, Height);
                _bufGraphics.Graphics.TranslateTransform(-rect.X, -rect.Y);
                try
                {
                    using (PaintEventArgs pea = new PaintEventArgs(_bufGraphics.Graphics, rect))
                    {
                        pea.Graphics.SetClip(rect);
                        InvokePaintBackground(Parent, pea);
                        InvokePaint(Parent, pea);
                    }
                }
                finally
                {
                    _bufGraphics.Graphics.TranslateTransform(rect.X, rect.Y);
                }
            }
            else
            {
                using (SolidBrush backColor = new SolidBrush(this.BackColor))
                    _bufGraphics.Graphics.FillRectangle(backColor, ClientRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Brush brush;
            if (this._style == MSGradientPanel.GradientStyle.Vertical)
                brush = new LinearGradientBrush(base.ClientRectangle, this._color1, this._color2, 720f);
            else
                brush = new LinearGradientBrush(base.ClientRectangle, this._color1, this._color2, 90f);
            _bufGraphics.Graphics.FillRectangle(brush, base.ClientRectangle);
            brush.Dispose();
            _bufGraphics.Render(e.Graphics);
        }
    }
}
