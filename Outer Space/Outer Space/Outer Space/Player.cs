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

    public class Player : GameObject
    {
        // Public properties
        public Location ShipLocation { get; set; }
        public List<Weapon> Weapons { get; set; }
        public Shield PlayerShield { get; set; }
        public int MoveLeft { get; set; }
        public int MoveRight { get; set; }

        public Bar Health { get; set; }
        public Bar Energy { get; set; }

        // Tiles chances
        public List<TileType> TileChance { get; set; }

        // Private variable(s)
        private float directionSpeed;

        // Constructor(s)
        public Player()
            : base()
        {
            this.Direction = (float)Math.PI * 1.5f;
            this.Texture = TextureManager.player;
            this.Position = new Vector2(200, Globals.ScreenSize.Y - Texture.Height);
            this.ShipLocation = Location.middle;

            this.Health = new Bar(new Vector2(200, Globals.ScreenSize.Y - 30), 100, 20, 100, Color.Red);
            this.Energy = new Bar(new Vector2(350, Globals.ScreenSize.Y - 30), 100, 20, 100, Color.OrangeRed);

            this.Weapons = new List<Weapon>();
            Weapons.Add(new Weapon());

            PlayerShield = new Shield(3);

            // Tiles chances
            TileChance = new List<TileType>();
            for (int i = 0; i < Enum.GetNames(typeof(TileType)).Length; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    TileChance.Add((TileType)i);
                }
            }

            // !TEMPORARY! Increase shoot tile chance
            for (int i = 0; i < 20; i++)
            {
                TileChance.Add(TileType.shoot);
            }
        }

        // Method(s)
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Health.Draw(spriteBatch);
            Energy.Draw(spriteBatch);

            PlayerShield.Draw(spriteBatch);

            // Right
            if (MoveRight > 0)
            {
                spriteBatch.DrawString(TextureManager.SpriteFont20, ((float)MoveRight / 60f).ToString("0.00"), new Vector2(Position.X + 50, Position.Y - 50), Color.White, 0f, new Vector2(TextureManager.SpriteFont20.MeasureString(((float)MoveRight / 60f).ToString("0.00")).X / 2, TextureManager.SpriteFont20.MeasureString(((float)MoveRight / 60f).ToString("0.00")).Y / 2), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureManager.tiles[1], new Vector2(Position.X + 50 - MoveRight % 20, Position.Y), null, Color.White, 0f, new Vector2(TextureManager.tiles[1].Width / 2, TextureManager.tiles[1].Height / 2), 0.5f, SpriteEffects.None, 0.9f);
            }

            // Left
            if (MoveLeft > 0)
            {
                spriteBatch.DrawString(TextureManager.SpriteFont20, ((float)MoveLeft / 60f).ToString("0.00"), new Vector2(Position.X - 50, Position.Y - 50), Color.White, 0f, new Vector2(TextureManager.SpriteFont20.MeasureString(((float)MoveLeft / 60f).ToString("0.00")).X / 2, TextureManager.SpriteFont20.MeasureString(((float)MoveLeft / 60f).ToString("0.00")).Y / 2), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(TextureManager.tiles[2], new Vector2(Position.X - 50 + MoveLeft % 20, Position.Y), null, Color.White, 0f, new Vector2(TextureManager.tiles[2].Width / 2, TextureManager.tiles[2].Height / 2), 0.5f, SpriteEffects.None, 0.9f);
            }

            // Locations
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(Texture, new Vector2(i * 100 + 100, Globals.ScreenSize.Y - Texture.Height), null, Color.White * 0.3f, (float)Math.PI * 1.5f, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0.9f);
            }
        }

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            // Move
            Vector2 move = new Vector2((int)ShipLocation * 100 + 100, Position.Y) - Position;
            Position += move * 0.05f;

            // Ship tilt
            if (move.Length() > 10)
            {
                Direction += move.Length() * directionSpeed;
            }
            Direction = MathHelper.Lerp(Direction, (float)Math.PI * 1.5f, 0.1f);

            // Right
            if (MoveRight > 0)
            {
                MoveRight--;
                if (Globals.KState.IsKeyDown(Keys.D) && Globals.PrevKState.IsKeyUp(Keys.D) && ShipLocation != Location.right)
                {
                    ShipLocation++;
                    MoveRight = 0; 
                }
            }

            // Left
            if (MoveLeft > 0)
            {
                MoveLeft--;
                if (Globals.KState.IsKeyDown(Keys.A) && Globals.PrevKState.IsKeyUp(Keys.A) && ShipLocation != Location.left)
                {
                    ShipLocation--;
                    MoveLeft = 0; 
                }
            }
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
                    MoveLeft += 20 * tilesMatched;
                    directionSpeed = -0.0005f;
                }

                if (tileType == TileType.right && ShipLocation != Location.right)
                {
                    MoveRight += 20 * tilesMatched;
                    directionSpeed = 0.0005f;
                }

                if (tileType == TileType.shield && PlayerShield.Charges != PlayerShield.MaxCharges)
                {
                    PlayerShield.Charges++;
                } 
            }
            else
            {
                level.ToAdd.Add(new Text(new Vector2(Globals.ScreenSize.X / 2, Globals.ScreenSize.Y / 2), "NOT ENOUGH ENERGY!", Color.Red, 120, false, 3f));
            }
        }
    }
}
