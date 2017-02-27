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

    class CurveMask : IMask<AForge.Point>
    {        
        private Extensions.isInCurve curveFunction;
        public string tag;

        public CurveMask (Extensions.isInCurve curveFunction, string tag)
        {
            this.curveFunction = curveFunction;
            this.tag = tag;
        }

        public bool isIn(AForge.Point point)
        {
            return curveFunction(point);
        }
    }
}
