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
    interface IMask<T>
    {
        bool isIn(T point);
    }

    abstract class CurveMask : IMask<AForge.Point>
    {        
        private Extensions.isInCurve curveFunction;
        public string tag;
        protected Rectangle rect;
        
        protected static Font drawFont = new Font("Arial", 16);
        protected static SolidBrush drawBrush = new SolidBrush(Color.GreenYellow);
        protected static Pen maskPen = new Pen(drawBrush, 2);

        public override string ToString()
        {
            return tag;
        }

        public virtual void drawMask(Graphics gr)
        {
            gr.DrawString(tag, drawFont, drawBrush, rect.X, rect.Y);
        }

        protected CurveMask (Extensions.isInCurve curveFunction, Rectangle draw, string tag)
        {
            this.curveFunction = curveFunction;
            this.tag = tag;

            rect = draw;
        }

        public bool isIn(AForge.Point point)
        {
            return curveFunction(point);
        }
    }

    class RectMask : CurveMask
    {
        public RectMask(Rectangle rect, Rectangle draw, string tag) : base(pt => MathFunctions.isPointInRectangle(pt, rect), draw, tag) {}

        public override void drawMask(Graphics gr)
        {
            base.drawMask(gr);
            gr.DrawRectangle(maskPen, rect);
        }
    }

    class EllipMask : CurveMask
    {
        public EllipMask(Rectangle ellip, Rectangle draw, string tag) : base(pt => MathFunctions.isPointInEllipse(pt, ellip), draw, tag) {}

        public override void drawMask(Graphics gr)
        {
            base.drawMask(gr);
            gr.DrawEllipse(maskPen, rect);
        }
    }
}
