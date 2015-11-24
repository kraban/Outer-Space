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
        public bool Flee { get; set; }
        public float Speed { get; set; }

        public Bar Energy { get; set; }

        // Inventory
        private Item selectedItem;
        private Point selectedItemArrayPosition;

        // Constructor(s)
        public Player()
            : base((float)Math.PI * 1.5f)
        {
            this.Texture = TextureManager.ship2;
            this.Position = new Vector2(300, Globals.ScreenSize.Y - Texture.Height);

            this.Energy = new Bar(new Vector2(450, Globals.ScreenSize.Y - 30), 100, 20, 100, Color.OrangeRed);

            // Weapontargets
            foreach (Weapon w in Weapons)
            {
                w.Targets.Add("Enemy");
            }

            Inventory[0, 0] = new Weapon(this);
            Inventory[1, 0] = new Shield(new Vector2(200, Globals.ScreenSize.Y - 30), 100, 10, 20);
            Inventory[2, 0] = new Hull(this);
            for (int i = 0; i < Inventory.GetLength(0); i++)
            {
                for (int j = 0; j < Inventory.GetLength(1) - 1; j++)
                {
                    Inventory[i, j] = new Weapon(this);
                }
            }
        }

        // Method(s)
        public void EquipItem(Item item)
        {
            if (item.Type == ItemType.weapon)
            {
                Weapon weapon = (Weapon)item;
                weapon.Targets.Add("Enemy");
            }
        }

        public void SwapItem(Point swapWith)
        {
            Item temp = Inventory[swapWith.X, swapWith.Y];
            Inventory[swapWith.X, swapWith.Y] = selectedItem;
            Inventory[selectedItemArrayPosition.X, selectedItemArrayPosition.Y] = temp;

        }

        public void UpdateInventory()
        {
            for (int i = 0; i < Inventory.GetLength(0); i++)
			{
                for (int j = 0; j < Inventory.GetLength(1); j++)
                {
                    if (Inventory[i, j].Pressed() && Inventory[i, j].Type != ItemType.trash)
                    {
                        selectedItem = Inventory[i, j];
                        selectedItemArrayPosition = new Point(i, j);
                    } 
                }
            }

            if (Globals.MState.LeftButton == ButtonState.Released)
            {
                if (selectedItem != null)
                {
                    // Move item in inventory
                    for (int i = 0; i < Inventory.GetLength(0); i++)
                    {
                        for (int j = 0; j < Inventory.GetLength(1); j++)
                        {
                            if (Inventory[i, j].HoverOver())
                            {
                                if ((i == 0 && j == 5) || (selectedItemArrayPosition.X == 0 && selectedItemArrayPosition.Y == 5)) // shield
                                {
                                    if (selectedItem.Type == ItemType.shield && Inventory[i, j].Type == ItemType.shield)
                                    {
                                        SwapItem(new Point(i, j));
                                        break;
                                    }
                                }
                                else if ((i == 1 && j == 5) || (selectedItemArrayPosition.X == 1 && selectedItemArrayPosition.Y == 5)) // hull
                                {
                                    if (selectedItem.Type == ItemType.hull && Inventory[i, j].Type == ItemType.hull)
                                    {
                                        SwapItem(new Point(i, j));
                                        break;
                                    }
                                }
                                else if ((i > 1 && j == 5) || (selectedItemArrayPosition.X > 1 && selectedItemArrayPosition.Y == 5)) // weapons
                                {
                                    if (((selectedItem.Type == ItemType.weapon || Inventory[i, j].Type == ItemType.weapon) && (Weapons.Count > 1 || !Weapons.Any(item => item == selectedItem)) || (selectedItem.Type == ItemType.weapon && Inventory[i, j].Type == ItemType.weapon)))
                                    {
                                        SwapItem(new Point(i, j));
                                        EquipItem(selectedItem);
                                        break;
                                    }
                                }
                                else // inventory
                                {
                                    SwapItem(new Point(i, j));
                                    break; 
                                }
                            } 
                        }
                    }
                }

                selectedItem = null;
                selectedItemArrayPosition = new Point(0, 0);
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
            for (int i = 0; i < 3; i++)
            {
                Inventory[2 + i, 5].DrawInventory(spriteBatch, new Vector2(i * 64 + 32, 164));
                spriteBatch.Draw(TextureManager.inventorySlot, new Vector2(i * 64 - 32 + 32, 164 - 32), Color.White);
            }

            // Shield
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Shield", new Vector2(100, 250), Color.White);
            ShipShield.DrawInventory(spriteBatch, new Vector2(100, 314));
            spriteBatch.Draw(TextureManager.inventorySlot, new Vector2(100 - 32, 314 - 32), Color.White);

            // Hull
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Hull", new Vector2(100, 400), Color.White);
            ShipHull.DrawInventory(spriteBatch, new Vector2(100, 464));
            spriteBatch.Draw(TextureManager.inventorySlot, new Vector2(100 - 32, 464 - 32), Color.White);

            // Inventory
            spriteBatch.Draw(TextureManager.inventory, new Vector2(300 - 32, 200 - 32), Color.White);

            spriteBatch.DrawString(TextureManager.SpriteFont20, "Inventory", new Vector2(400, 100), Color.White);
            for (int i = 0; i < Inventory.GetLength(0); i++)
            {
                for (int j = 0; j < Inventory.GetLength(1) - 1; j++)
			    {
			        Inventory[i, j].DrawInventory(spriteBatch, new Vector2(i * 64 + 300, j * 64 + 200));
			    }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Energy.Draw(spriteBatch);

            // Hull
            ShipHull.Position = new Vector2(ShipHull.Texture.Width / 2, Globals.ScreenSize.Y - 100 - ShipHull.Texture.Height);

            // Shield
            ShipShield.Position = new Vector2(ShipShield.Texture.Width / 2, Globals.ScreenSize.Y - 100);

            // Weapons
            for (int i = 0; i < Weapons.Count; i++)
            {
                Weapons[i].Position = new Vector2(Weapons[i].Texture.Width / 2 + 64, Globals.ScreenSize.Y - i * Weapons[i].Texture.Height - 100);
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
            if (!Flee)
            {
                Position = new Vector2(Position.X, (float)MathHelper.Lerp(Position.Y, Globals.ScreenSize.Y - Texture.Height, 0.1f));
                base.UpdateLevel(level);
                // Select weapon
                if (Globals.KState.IsKeyDown(Keys.D1))
                {
                    SelectedWeapon = 0;
                }
                else if (Globals.KState.IsKeyDown(Keys.D2) && Weapons.Count >= 2)
                {
                    SelectedWeapon = 1;
                }
                else if (Globals.KState.IsKeyDown(Keys.D3) && Weapons.Count >= 3)
                {
                    SelectedWeapon = 2;
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
            else
            {
                Direction = MathHelper.Lerp(Direction, (float)Math.PI, 0.03f);
                Speed += 0.1f;
                Position += new Vector2((float)Math.Cos(Direction) * Speed, (float)Math.Sin(Direction) * Speed);
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
                    Weapons[SelectedWeapon].ShootMethods[Weapons[SelectedWeapon].Action](this   , tilesMatched, level, false); 
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
                    ShipShield.Change(ShipShield.ShieldHeal * tilesMatched);
                } 
            }
            else
            {
                level.CombatText("NOT ENOUGH ENERGY!");
            }
        }
    }
}
