using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BorgPong
{
    public class BoundingBoxSize
    {
        public BoundingBoxSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.OrginalWidth = width;
            this.OrginalHeight = height;
        }

        public BoundingBoxSize(Texture2D texture) : this(texture.Width,texture.Height)
        {
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public int OrginalWidth { get; set; }
        public int OrginalHeight { get; set; }
        public void ResetSize()
        {
            Width = OrginalWidth;
            Height = OrginalHeight;
        }
        public void Resize(float size)
        {
            Width = (int)(Width * size);
            Height = (int)(Height * size);
        }
    }
}
