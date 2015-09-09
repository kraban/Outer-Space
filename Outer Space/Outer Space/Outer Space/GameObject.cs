using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Outer_Space
{
    public abstract class GameObject
    {
        // Public properties
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle Box { get { return new Rectangle((int)Position.X - Texture.Width / 2, (int)Position.Y - Texture.Height / 2, Texture.Width, Texture.Height); } }
        public float Size { get; set; }
        public float Direction { get; set; }
        public float Depth { get; set; }
        public bool Dead { get; set; }

        // Constructor(s)
        public GameObject()
        {
            // Default texture
            this.Texture = TextureManager.none;
            this.Size = 1;
        }

        // Method(s)
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, Direction, new Vector2(Texture.Width / 2, Texture.Height / 2), Size, SpriteEffects.None, Depth);
        }

        public virtual void Update()
        {

        }

        public virtual void UpdateLevel(Level level)
        { }

        public bool OutsideScreen()
        {
            if (Position.X + Texture.Width / 2 < 0 || Position.X - Texture.Width / 2 > Globals.ScreenSize.X || Position.Y + Texture.Height / 2 < 0 || Position.Y - Texture.Height / 2 > Globals.ScreenSize.Y)
            {
                return true;
            }
            return false;
        }
    }
}
