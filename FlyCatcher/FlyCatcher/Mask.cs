using System.Drawing;

namespace FlyCatcher
{
    interface IMask<in T>
    {
        string Tag { get; }
        string PrintOut();
        bool IsIn(T point);
        void DrawMask(Graphics gr);
    }

    abstract class CurveMask : IMask<AForge.Point>
    {
        private Extensions.isInCurve curveFunction;
        public string Tag { get; }
        protected RectangleF percentRectangle;

        protected static Font drawFont = new Font("Arial", 16);
        protected static SolidBrush drawBrush = new SolidBrush(Color.GreenYellow);
        protected static Pen maskPen = new Pen(drawBrush, 2);

        public override string ToString() => Tag;

        public virtual string PrintOut() => $"{Tag} {percentRectangle.Left} {percentRectangle.Top} {percentRectangle.Right} {percentRectangle.Bottom}";

        public virtual void DrawMask(Graphics gr) => gr.DrawString(Tag, drawFont, drawBrush, MathFunctions.PercentToValue(0, gr.ClipBounds.Width, percentRectangle.X), MathFunctions.PercentToValue(0, gr.ClipBounds.Height, percentRectangle.Y));

        protected CurveMask(Extensions.isInCurve curveFunction, RectangleF percentRect, string tag)
        {
            this.curveFunction = curveFunction;
            this.Tag = tag;

            percentRectangle = percentRect;
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
            gr.DrawRectangle(maskPen, MathFunctions.recalculateRectangle(percentRectangle, gr.ClipBounds).Round());
        }

        public override string PrintOut() => "rectangle =" + base.PrintOut();
    }

    class EllipMask : CurveMask
    {
        public EllipMask(RectangleF ellip, RectangleF percentRectangle, string tag) : base(pt => MathFunctions.isPointInEllipse(pt, ellip), percentRectangle, tag) { }

        public override void DrawMask(Graphics gr)
        {
            base.DrawMask(gr);
            gr.DrawEllipse(maskPen, MathFunctions.recalculateRectangle(percentRectangle, gr.ClipBounds));
        }

        public override string PrintOut() => "ellipse = " + base.PrintOut();
    }
}
