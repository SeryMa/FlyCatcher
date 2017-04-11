using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AForge.Imaging.Filters;
using AForge.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

namespace FlyCatcher
{
    class BlobFilter : IBlobsFilter
    {
        public int min { private get; set; }
        public int max { private get; set; }

        public bool Check(Blob blob) => blob.Area >= min && blob.Area <= max;
    }
}