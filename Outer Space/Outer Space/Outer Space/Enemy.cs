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
    class Enemy : GameObject
    {
        // Constructor(s)
        public Enemy()
            : base()
        {
            this.Position = new Vector2(100, 100);
            this.Texture = TextureManager.player;
        }
    }
}
