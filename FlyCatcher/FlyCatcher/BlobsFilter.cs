using AForge.Imaging;

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace FlyCatcher
{
    class BlobFilter : IBlobsFilter
    {
        public int min { private get; set; }
        public int max { private get; set; }

        public bool Check(Blob blob) => blob.Area >= min && blob.Area <= max;
    }
}