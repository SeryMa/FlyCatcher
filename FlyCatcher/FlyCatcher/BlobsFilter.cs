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

    class MyStream : StreamWriter
    {
        private StreamWriter tmpStream;

        private string path;
        private string tmp;

        private bool appending;

        public void resetTmp()
        {
            if (tmpStream != null) tmpStream.Close();

            tmp = Path.GetTempFileName();
            tmpStream = new StreamWriter(tmp);
        }

        public MyStream(string path) : base(path)
        {
            appending = false;

            this.path = path;

            resetTmp();
        }

        public void StartAppending() => appending = true;
        public void StopAppending() => appending = false;
        private void finish() => File.AppendAllLines(path, File.ReadLines(tmp));

        #region Overrides
        public override void Close()
        {
            finish();
            tmpStream.Close();
            base.Close();
        }
        public override void Flush()
        {            
            tmpStream.Flush();
            base.Flush();
        }

        public override void Write(string value)
        {
            if (appending)
                base.Write(value);
            else
                tmpStream.Write(value);
        }

        public override void WriteLine()
        {
            if (appending)
                base.WriteLine();
            else
                tmpStream.WriteLine();
        }

        public override void WriteLine(string value)
        {
            if (appending)
                base.WriteLine(value);
            else
                tmpStream.WriteLine(value);
        }

        public override void Write(string format, object arg0)
        {
            if (appending)
                base.Write(format, arg0);
            else
                tmpStream.Write(format, arg0);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            if (appending)
                base.Write(format, arg0, arg1);
            else
                tmpStream.Write(format, arg0, arg1);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            if (appending)
                base.Write(format, arg0, arg1, arg2);
            else
                tmpStream.Write(format, arg0, arg1, arg2);
        }

        public override void Write(string format, params object[] arg)
        {
            if (appending)
                base.Write(format, arg);
            else
                tmpStream.Write(format, arg);
        }

        public override void WriteLine(string format, object arg0)
        {
            if (appending)
                base.WriteLine(format, arg0);
            else
                tmpStream.WriteLine(format, arg0);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            if (appending)
                base.WriteLine(format, arg0, arg1);
            else
                tmpStream.WriteLine(format, arg0, arg1);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            if (appending)
                base.WriteLine(format, arg0, arg1, arg2);
            else
                tmpStream.WriteLine(format, arg0, arg1, arg2);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            if (appending)
                base.WriteLine(format, arg);
            else
                tmpStream.WriteLine(format, arg);
        }
        #endregion
    }
}