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

        // Constructor(s)
        public Player()
            : base()
        {
            this.Direction = (float)Math.PI * 1.5f;
            this.Texture = TextureManager.player;
            this.Position = new Vector2(200, Globals.ScreenSize.Y - Texture.Height);
            this.ShipLocation = Location.middle;

            this.Weapons = new List<Weapon>();
            Weapons.Add(new Weapon());
        }

        // Method(s)
        public override void Update()
        {
            base.Update();

            // Move
            Vector2 move = new Vector2((int)ShipLocation * 100 + 100, Position.Y) - Position;
            Position += move * 0.05f;
        }

        public void Action(int tilesMatched, TileType tileType, Level level)
        {
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
        }
    }
}
