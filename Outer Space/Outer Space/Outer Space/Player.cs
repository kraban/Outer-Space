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
    public enum Location { left, middle, right }

    class Player : GameObject
    {
        // Public properties
        public Location ShipLocation { get; set; }
        public List<Weapon> Weapons { get; set; }
        public Shield PlayerShield { get; set; }

        public Bar Health { get; set; }
        public Bar Energy { get; set; }

        // Constructor(s)
        public Player()
            : base()
        {
            this.Direction = (float)Math.PI * 1.5f;
            this.Texture = TextureManager.player;
            this.Position = new Vector2(200, Globals.ScreenSize.Y - Texture.Height);
            this.ShipLocation = Location.middle;

            this.Health = new Bar(new Vector2(200, Globals.ScreenSize.Y - 30), 100, 20, 100, Color.Red);
            this.Energy = new Bar(new Vector2(350, Globals.ScreenSize.Y - 30), 100, 20, 50, Color.OrangeRed);

            this.Weapons = new List<Weapon>();
            Weapons.Add(new Weapon());

            PlayerShield = new Shield(3);
        }

        // Method(s)
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Health.Draw(spriteBatch);
            Energy.Draw(spriteBatch);

            PlayerShield.Draw(spriteBatch);
        }

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            // Move
            Vector2 move = new Vector2((int)ShipLocation * 100 + 100, Position.Y) - Position;
            Position += move * 0.05f;
        }

        public void Action(int tilesMatched, TileType tileType, Level level)
        {
            if (tileType == TileType.cog)
            {
                Energy.Change(tilesMatched * 5 + (tilesMatched - 3) * 10);
            }
            else if (Energy.Value > 10)
            {
                Energy.Change(-10);
                if (tileType == TileType.shoot)
                {
                    Weapons.First().Action(new Vector2(Position.X, Position.Y - Texture.Height / 2), Direction, tilesMatched, level);
                }

                if (tileType == TileType.left && ShipLocation != Location.left)
                {
                    ShipLocation--;
                }

                if (tileType == TileType.right && ShipLocation != Location.right)
                {
                    ShipLocation++;
                }

                if (tileType == TileType.shield && PlayerShield.Charges != PlayerShield.MaxCharges)
                {
                    PlayerShield.Charges++;
                } 
            }
        }
    }
}
