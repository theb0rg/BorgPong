using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BorgPong
{
    public class Brick : GameObject
    {
        public int Score = 0;
        public Brick(Texture2D texture) : base(texture,new Vector2(0,0))
        {
            Height = 100;
            Width = 20;
            LastMovements = new List<int>();
        }

        public Brick(Texture2D texture,Vector2 Position) : this(texture)
        {
            this.X = (int)Position.X;
            this.Y = (int)Position.Y;
        }

        public Brick(Texture2D texture, int X, int Y)
            : this(texture)
        {
            this.X = X;
            this.Y = Y;
        }

        public float X
        {
            get { return base.Position.X; }
            set { base.Position.X = value; }
        }

        public List<int> LastMovements
        {
            get;
            set;
        }

        public float Y
        {
            get
            {
                return base.Position.Y;
            }
            set
            {
                base.Position.Y = value;
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
            get { return new Rectangle((int)X, (int)Y, Width, Height); }
        }
        public Vector2 Vector
        {
            get { return new Vector2(X, Y); }
        }
        public HitBox HitBox
        {
            get { return new HitBox((int)X,(int)Y,Width, Height); }
        }


    }
}
