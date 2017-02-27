using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace FlyCatcher
{
    class MaskContainer
    {
        public byte[,] maskArray { get; set; }

        int width { get { return maskArray.GetLength(1); } }
        int height { get { return maskArray.GetLength(0); } }

        #region Constructors        
        public MaskContainer(Bitmap mask)
        {
            maskArray = mask.convertToArray();
        }

        public MaskContainer(int width, int height, byte value)
        {
            maskArray = new byte[height, width];

            for (int x = 0; x < height; x++)
                for (int y = 0; y < width; y++)
                    this[x, y] = value;
        }

        public MaskContainer(byte[,] mask)
        {
            maskArray = mask;
        }

        public MaskContainer(MaskContainer mask)
        {
            maskArray = mask.maskArray;
        }

        #endregion

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                    str.Append($"{maskArray[x, y],4}");

                str.Append(Environment.NewLine);
            }

            return str.ToString();
        }

        private byte this[int x, int y]
        {
            get { return maskArray[x, y]; }
            set { maskArray[x, y] = value; }
        }

        public static MaskContainer operator +(MaskContainer mask, Rectangle rect)
        {
            return mask.iterate(rect, 255);
        }
        public static MaskContainer operator -(MaskContainer mask, Rectangle rect)
        {
            return mask.iterate(rect, 0);
        }

        public static MaskContainer operator +(MaskContainer mask, byte[,] array)
        {
            MaskContainer msk = new MaskContainer(mask);

            for (int x = 0; x < mask.width; x++)
                for (int y = 0; y < mask.height; y++)
                    msk[x, y] = (byte)(mask[x, y] + array[x, y]);

            return msk;
        }
        public static MaskContainer operator -(MaskContainer mask, byte[,] array)
        {
            MaskContainer msk = new MaskContainer(mask);

            for (int x = 0; x < mask.width; x++)
                for (int y = 0; y < mask.height; y++)
                    msk[x, y] = (byte)(mask[x, y] - array[x, y]);

            return msk;
        }

        #region Obsolette Operators
        public static MaskContainer operator +(MaskContainer mask, Bitmap picture)
        {
            return mask + picture.convertToArray();
        }
        public static MaskContainer operator -(MaskContainer mask, Bitmap picture)
        {
            return mask - picture.convertToArray();
        }

        //public static Mask operator +(Mask mask1, Mask mask2)
        //{
        //    return mask1 + mask2.maskArray;
        //}
        //public static Mask operator -(Mask mask1, Mask mask2)
        //{
        //    return mask1 + mask2.maskArray;
        //}
        #endregion

        private MaskContainer iterate(Rectangle rect, byte value)
        {
            MaskContainer mask = new MaskContainer(maskArray);

            for (int x = 0; x < height; x++)
                for (int y = 0; y < width; y++)
                    if (MathFunctions.isPointInRectangle(x, y, rect))
                        mask[x, y] = value;

            return mask;
        }
    }
}
