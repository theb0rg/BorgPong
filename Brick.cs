using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BorgPong
{
    public class Brick
    {
        int y;
        public Brick()
        {
            X = 0;
            y = 0;
            PreviousY = 0;
            Height = 100;
            Width = 20;
            LastMovements = new List<int>();
        }

        public Brick(Vector2 Position) : this()
        {
            this.X = (int)Position.X;
            this.Y = (int)Position.Y;
        }

        public Brick(int X, int Y)
            : this()
        {
            this.X = X;
            this.Y = Y;
        }

        public int X
        {
            get;
            set;
        }

        public int PreviousY
        {
            get;
            set;
        }

        public List<int> LastMovements
        {
            get;
            set;
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                PreviousY = y;
                y = value;
            }
        }

        public int Height
        {
            get;
            set;
        }

        public int Width
        {
            get;
            set;
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle(X, Y, Width, Height); }
        }
        public Vector2 Vector
        {
            get { return new Vector2(X, Y); }
        }


    }
}
