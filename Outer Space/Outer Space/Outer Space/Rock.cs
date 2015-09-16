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
            level.ToAdd.Add(new Text(new Vector2(Globals.CombatScreenSize.X / 2, Globals.CombatScreenSize.Y / 2), "Rock heading towards you! Move!", Color.Red, 120, false, 2f));
        }

        // Method(s)
        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            // Move
            Position += new Vector2((float)Math.Cos(Direction) * 3, (float)Math.Sin(Direction) * 3);

            // Hit Player
            if (Box.Intersects(level.Player.Box))
            {
                Dead = true;
                level.CreatePieces(Position, Texture);
                level.Player.TakeDamage(20, 0);
                Globals.ScreenShakeTimer = 30;
            }

            // Outside screen
            if (Position.Y - Texture.Height / 2 > Globals.ScreenSize.Y)
            {
                Dead = true;
            }
        }
    }
}
