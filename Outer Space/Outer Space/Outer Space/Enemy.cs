﻿using System;
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
        // Public properties
        public Bar Health { get; set; }

        // Constructor(s)
        public Enemy()
            : base()
        {
            this.Position = new Vector2(100, 100);
            this.Texture = TextureManager.player;

            this.Health = new Bar(new Vector2(0, 0), 100, 20, 100, Color.Red);
        }

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            if (Health.Value <= 0)
            {
                Dead = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Health.Draw(spriteBatch);
        }
    }
}
