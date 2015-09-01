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
    class Shield : GameObject
    {
        // Public properties
        public int MaxCharges { get; private set; }
        public int Charges { get; set; }

        // Constructor(s)
        public Shield(int maxCharges)
            : base()
        {
            this.MaxCharges = maxCharges;
        }

        // Method(s)
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Shield: " + Charges + "/" + MaxCharges, new Vector2(0, Globals.ScreenSize.Y - 30), Color.White);
        }
    }
}
