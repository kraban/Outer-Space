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
    class Rock : GameObject
    {
        // Constructor(s)
        public Rock(Player player, Level level)
            : base()
        {
            this.Texture = TextureManager.rock;
            this.Position = new Vector2(Globals.Randomizer.Next(400, Globals.ScreenSize.X - 300), -100);
            this.Direction = (float)Math.Atan2((player.Position - Position).Y, (player.Position - Position).X);
            level.CombatText("Rock heading towards you! Move!");
        }

        // Method(s)
        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            // Move
            Position += new Vector2((float)Math.Cos(Direction) * 2.5f, (float)Math.Sin(Direction) * 2.5f);

            // Hit Player
            if (Box.Intersects(level.Player.Box))
            {
                Dead = true;
                level.CreatePieces(Position, Texture);
                level.Player.TakeDamage(15, 0, DamageType.rock, false);
                Camera.ScreenShakeTimer = 30;
            }

            // Outside screen
            if (Position.Y - Texture.Height / 2 > Globals.ScreenSize.Y)
            {
                Dead = true;
            }
        }
    }
}
