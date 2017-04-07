using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;

namespace FlyCatcher
{
    interface IMask<in T>
    {
        string Tag { get; }
        bool IsIn(T point);
        void DrawMask(Graphics gr);
    }

    abstract class CurveMask : IMask<AForge.Point>
    {
        private Extensions.isInCurve curveFunction;
        public string Tag { get; }
        protected RectangleF rect;

        protected static Font drawFont = new Font("Arial", 16);
        protected static SolidBrush drawBrush = new SolidBrush(Color.GreenYellow);
        protected static Pen maskPen = new Pen(drawBrush, 2);

        public override string ToString() => Tag;

        public virtual void DrawMask(Graphics gr) => gr.DrawString(Tag, drawFont, drawBrush, MathFunctions.PercentToValue(0, gr.ClipBounds.Width, rect.X), MathFunctions.PercentToValue(0, gr.ClipBounds.Height, rect.Y));

        protected CurveMask(Extensions.isInCurve curveFunction, RectangleF draw, string tag)
        {
            this.curveFunction = curveFunction;
            this.Tag = tag;

            rect = draw;
        }

        public bool IsIn(AForge.Point point) => curveFunction(point);
    }

    class RectMask : CurveMask
    {
        public RectMask(RectangleF rect, RectangleF percentRectangle, string tag) : base(pt => MathFunctions.isPointInRectangle(pt, rect), percentRectangle, tag) { }

        public override void DrawMask(Graphics gr)
        {
            base.DrawMask(gr);

            //gr.DrawRectangle(maskPen, rect);
            gr.DrawRectangle(maskPen, MathFunctions.recalculateRectangle(rect, gr.ClipBounds).Round());
        }
    }

    class EllipMask : CurveMask
    {
        public EllipMask(RectangleF ellip, RectangleF percentRectangle, string tag) : base(pt => MathFunctions.isPointInEllipse(pt, ellip), percentRectangle, tag) { }

        public override void DrawMask(Graphics gr)
        {
            base.DrawMask(gr);            
            gr.DrawEllipse(maskPen, MathFunctions.recalculateRectangle(rect, gr.ClipBounds));
        }
    }
}
