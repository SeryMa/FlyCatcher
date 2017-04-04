using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AForge;
using AForge.Imaging;

namespace FlyCatcher
{
    class BlobFilter : IBlobsFilter
    {
        public int min { private get; set; }
        public int max { private get; set; }

        //public BlobFilter() { }

        public bool Check(Blob blob) => blob.Area >= min && blob.Area <= max;
    }
}
