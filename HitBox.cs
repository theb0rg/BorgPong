using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BorgPong
{
   public class HitBox
    {
        public int X;
        public int Y;
        public int width;
        public int height;

        public HitBox(int X, int Y, int width, int height)
        {
            this.X = X;
            this.Y = Y;
            this.width = width;
            this.height = height;
        }

        public int Bottom
        {
            get { return Y + height; }
        }

        public int Top
        {
            get { return Y; }
        }


    }
}
