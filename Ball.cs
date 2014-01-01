using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BorgPong
{
    public class GameObject
    {
        Texture2D texture;
        public Vector2 Position;
        public Vector2 Velocity;
        private BoundingBoxSize boundingBoxSize;
        

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    boundingBoxSize.Width,
                    boundingBoxSize.Height);
            }
        }

        public GameObject(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.Position = position;
            boundingBoxSize = new BoundingBoxSize(texture);
        }

        public GameObject(Texture2D texture, Vector2 position, Vector2 velocity)
            : this(texture,position)
        {
            this.Velocity = velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, Color.White);
        }
        public void Draw(SpriteBatch spriteBatch, float size)
        {
            spriteBatch.Draw(texture, Position, null,Color.White,0F,Vector2.Zero,size,SpriteEffects.None,0F);
        }
        public void SetBoundingBoxSize(float size)
        {
            boundingBoxSize.Resize(size);
        }
    }
}
