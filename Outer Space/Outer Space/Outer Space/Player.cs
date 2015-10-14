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
    public class Player : Ship
    {
        public int MoveLeft { get; set; }
        public int MoveRight { get; set; }

        public Bar Energy { get; set; }

        // Tiles chances
        public List<TileType> TileChance { get; set; }

        // Inventory
        private Item selectedItem;
        private int selectedItemArrayPosition;

        // Constructor(s)
        public Player()
            : base((float)Math.PI * 1.5f)
        {
            this.Texture = TextureManager.ship2;
            this.Position = new Vector2(300, Globals.ScreenSize.Y - Texture.Height);

            this.Energy = new Bar(new Vector2(450, Globals.ScreenSize.Y - 30), 100, 20, 100, Color.OrangeRed);

            // Tiles chances
            TileChance = new List<TileType>();
            for (int i = 0; i < Enum.GetNames(typeof(TileType)).Length; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    TileChance.Add((TileType)i);
                }
            }

            // Weapontargets

            // BUG EQUIPED WEAPONS DO NOT HAVE TARGETS
            foreach (Weapon w in Weapons)
            {
                w.Targets.Add("Enemy");
            }

            // !TEMPORARY! Increase shoot tile chance
            //for (int i = 0; i < 40; i++)
            //{
            //    TileChance.Add(TileType.shoot);
            //}

            // Temp items
            for (int i = 0; i < 25; i++)
            {
                Inventory[i] = (new Item());
            }
            Inventory[5] = new Weapon();
            Inventory[6] = new Shield(new Vector2(200, 200), 5, 5, 5);
            Inventory[7] = new Hull(this);
        }

        // Method(s)
        public void UpdateInventory()
        {
            for (int i = 0; i < Inventory.Length; i++)
			{
                if (Inventory[i].Pressed() && Inventory[i].Type != ItemType.trash)
                {
                    selectedItem = Inventory[i];
                    selectedItemArrayPosition = i;
                }
            }

            if (Globals.MState.LeftButton == ButtonState.Released)
            {
                if (selectedItem != null)
                {
                    // Move item in inventory
                    for (int i = 0; i < Inventory.Length; i++)
                    {
                        if (Inventory[i].HoverOver())
                        {
                            Item temp = Inventory[i];
                            Inventory[i] = selectedItem;
                            Inventory[selectedItemArrayPosition] = temp;
                            break;
                        }
                    }
                }

                if (selectedItem != null)
                {
                    // replace weapon
                    for (int i = 0; i < Weapons.Count; i++)
                    {
                        if (Weapons[i].HoverOver() && selectedItem.Type == Weapons[i].Type)
                        {
                            Item temp = Weapons[i];
                            Weapons[i] = (Weapon)selectedItem;
                            Inventory[selectedItemArrayPosition] = temp;
                            break;
                        }
                    }
                }

                if (selectedItem != null)
                {
                    if (ShipShield.HoverOver() && selectedItem.Type == ShipShield.Type)
                    {
                        Item temp = ShipShield;
                        ShipShield = (Shield)selectedItem;
                        Inventory[selectedItemArrayPosition] = temp;
                    }
                }

                selectedItem = null;
                selectedItemArrayPosition = 0;
            }
        }

        public void DrawInventory(SpriteBatch spriteBatch)
        {
            // Selected item
            if (selectedItem != null)
            {
                spriteBatch.Draw(selectedItem.Texture, new Vector2(Globals.MState.X - 32, Globals.MState.Y - 32), Color.White * 0.5f); 
            }

            // Weapons
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Weapons", new Vector2(100, 100), Color.White);
            for (int i = 0; i < Weapons.Count; i++)
            {
                Weapons[i].DrawInventory(spriteBatch, new Vector2(i * 64 + 32, 164));
            }

            // Shield
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Shield", new Vector2(100, 250), Color.White);
            ShipShield.DrawInventory(spriteBatch, new Vector2(100, 314));

            // Hull
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Hull", new Vector2(100, 400), Color.White);
            ShipHull.DrawInventory(spriteBatch, new Vector2(100, 464));

            // Inventory
            spriteBatch.Draw(TextureManager.inventory, new Vector2(300 - 32, 200 - 32), Color.White);

            spriteBatch.DrawString(TextureManager.SpriteFont20, "Inventory", new Vector2(400, 100), Color.White);
            int row = 0;
            for (int i = 0; i < Inventory.Length; i++)
            {
                Inventory[i].DrawInventory(spriteBatch, new Vector2(300 + (i - row * 5) * 64, 200 + row * 64));
                if (i - row * 5 >= 4)
                {
                    row++;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Energy.Draw(spriteBatch);

            // Hull
            ShipHull.Position = new Vector2(ShipHull.Texture.Width / 2, Globals.ScreenSize.Y - 150 - ShipHull.Texture.Height);

            // Shield
            ShipShield.Position = new Vector2(ShipShield.Texture.Width / 2, Globals.ScreenSize.Y - 150);

            // Weapons
            for (int i = 0; i < Weapons.Count; i++)
            {
                Weapons[i].Position = new Vector2(Weapons[i].Texture.Width / 2 + 64, Globals.ScreenSize.Y - i * Weapons[i].Texture.Height - 150);
                if (i == SelectedWeapon)
                {
                    spriteBatch.Draw(TextureManager.selected, new Vector2(Weapons[i].Position.X - Weapons[i].Texture.Width / 2, Weapons[i].Position.Y - Weapons[i].Texture.Height / 2), Color.White);
                }
                Weapons[i].Draw(spriteBatch);
            }

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
                spriteBatch.Draw(Texture, new Vector2(i * 100 + 200, Globals.ScreenSize.Y - Texture.Height), null, Color.White * 0.3f, (float)Math.PI * 1.5f, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0.9f);
            }
        }

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            // Select weapon
            if (Globals.KState.IsKeyDown(Keys.D1))
            {
                SelectedWeapon = 0;
            }
            else if (Globals.KState.IsKeyDown(Keys.D2))
            {
                SelectedWeapon = 1;
            }

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

        public void Action(int tilesMatched, TileType tileType, Level level, bool manuallyMatched)
        {
            if (tileType == TileType.cog)
            {
                Energy.Change(tilesMatched * 5 + (tilesMatched - 3) * 10);
            }
            else if (Energy.Value > 10 || !manuallyMatched)
            {
                if (manuallyMatched)
                {
                    Energy.Change(-10);  
                }

                if (tileType == TileType.shoot && Weapons[SelectedWeapon].Disabled < 0)
                {
                    Weapons[SelectedWeapon].ShootMethods[Weapons[SelectedWeapon].Action](new Vector2(Position.X, Position.Y - Texture.Height / 2), Direction, tilesMatched, level, false); 
                }

                if (tileType == TileType.left && ShipLocation != Location.left)
                {
                    MoveLeft += 20 * tilesMatched;
                    DirectionSpeed = -0.0005f;
                }

                if (tileType == TileType.right && ShipLocation != Location.right)
                {
                    MoveRight += 20 * tilesMatched;
                    DirectionSpeed = 0.0005f;
                }

                if (tileType == TileType.shield && ShipShield.Value != ShipShield.Width)
                {
                    ShipShield.Change(10 * tilesMatched);
                } 
            }
            else
            {
                level.CombatText("NOT ENOUGH ENERGY!");
            }
        }
    }
}
