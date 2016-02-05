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
    public class Player : Ship
    {
        public int MoveLeft { get; set; }
        public int MoveRight { get; set; }
        public bool Move { get; set; }
        public float Speed { get; set; }

        public Bar Energy { get; set; }

        // Rank
        public int Rank { get; set; }
        public Bar Experience { get; set; }
        public List<string> RankPerks { get; set; }

        // Inventory
        private Item selectedItem;
        private Point selectedItemArrayPosition;

        // Constructor(s)
        public Player()
            : base((float)Math.PI * 1.5f)
        {
            this.Texture = TextureManager.boss;
            this.Position = new Vector2(300, Globals.ScreenSize.Y - Texture.Height);

            this.Energy = new Bar(new Vector2(400, Globals.ScreenSize.Y - 35), 100, 20, 100, Color.Orange);
            this.Rank = 1;
            this.Experience = new Bar(new Vector2(Globals.ScreenSize.X / 2 - 330, 50), 300, 25, 100, Color.Green);
            this.Experience.Change(-Experience.MaxValue + 10);
            this.RankPerks = new List<string>();

            // Weapontargets
            foreach (Weapon w in Weapons)
            {
                w.Targets.Add("Enemy");
                w.Targets.Add("Boss");
            }

            Inventory[0, 0] = new Weapon(this, Globals.Randomizer.Next(0, Weapon.ListOfMethods().Count()));
            Inventory[1, 0] = new Shield(new Vector2(200, Globals.ScreenSize.Y - 35), 100, 20, 20, Globals.Randomizer.Next(0, Shield.ListOfShieldMethods().Count()));
            Inventory[2, 0] = new Hull(this, Globals.Randomizer.Next(0, Hull.ListOfHullMethods().Count()));
            for (int i = 0; i < 5; i++)
            {
                AddItem(new Item(Item.HealPlayer, ItemType.misc, TextureManager.wrench, "|W|Right click to regain 10 % health.", "Wrench"));
                AddItem(new Item(Item.Flee, ItemType.misc, TextureManager.flee, "|W|Used to flee from combat.", "Flee"));
            }
            //for (int i = 0; i < 20; i++)
            //{
            //    AddItem(new Weapon(this, Globals.Randomizer.Next(0, Weapon.ListOfMethods().Count())));
            //}
        }

        // Method(s)
        public bool ItemInInventory(string name)
        {
            foreach (Item item in Inventory)
            {
                if (item.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public Item GetItemInInventory(string name)
        {
            foreach (Item item in Inventory)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }

        public void AddItem(Item item)
        {
            bool done = false;
            for (int i = 0; i < Inventory.GetLength(1) - 1; i++)
            {
                for (int j = 0; j < Inventory.GetLength(0); j++)
                {
                    if (item.Type != ItemType.misc)
                    {
                        if (Inventory[j, i].Type == ItemType.nothing)
                        {
                            Inventory[j, i] = item;
                            Inventory[j, i].RecentlyAcquired = true;
                            done = true;
                            break;
                        }
                    }
                    else
                    {
                        if (Inventory[j, i].Type == ItemType.nothing )
                        {
                            Inventory[j, i] = item;
                            Inventory[j, i].RecentlyAcquired = true;
                            done = true;
                            break;
                        }
                        else if (Inventory[j, i].Type == item.Type && Inventory[j, i].Name == item.Name)
                        {
                            Inventory[j, i].RecentlyAcquired = true;
                            Inventory[j, i].NumberOfItems++;
                            done = true;
                            break;
                        }
                    }
                }
                if (done)
                {
                    break;
                }
            }
        }

        public void RankUp()
        {
            int random = Globals.Randomizer.Next(0, 5);
            SceneManager.mapScene.NewRank.Flash = 10;
            if (random == 0)
            {
                AddItem(new Shield(new Vector2(200, Globals.ScreenSize.Y - 35), 100, 20, 20, Globals.Randomizer.Next(0, Shield.ListOfShieldMethods().Count())));
                RankPerks.Add("Shield Module");
            }
            else if (random == 1)
            {
                AddItem(new Hull(this, Globals.Randomizer.Next(0, Hull.ListOfHullMethods().Count())));
                RankPerks.Add("Hull Module");
            }
            else if (random == 2)
            {
                AddItem(new Weapon(this, Globals.Randomizer.Next(0, Weapon.ListOfMethods().Count())));
                RankPerks.Add("Weapon Module");
            }
            else if (random == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    AddItem(new Item(Item.HealPlayer, ItemType.misc, TextureManager.wrench, "|W|Right click to regain 10 % health.", "Wrench"));
                    AddItem(new Item(Item.Flee, ItemType.misc, TextureManager.flee, "|W|Used to flee from combat.", "Flee"));
                }
                RankPerks.Add("Misc items");
            }
            else if (random == 4)
            {
                Health.MaxValue += 10;
                Health.Change(10);
                RankPerks.Add("Increase health by 10");
            }

        }

        public void GainExperience(float value)
        {
            float overflow = Experience.Change(value);
            while (overflow > 0)
            {
                Rank++;
                RankUp();
                Experience.Change(-Experience.MaxValue);
                Experience.MaxValue = Rank * 50 + 100;
                overflow = Experience.Change(overflow);
            }
        }

        public void DrawRank(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Rank: " + Rank, new Vector2(Globals.ScreenSize.X / 2 - TextureManager.SpriteFont20.MeasureString("Rank: " + Rank).X / 2, 100), new Color(0, 255, 255));
            Experience.Draw(spriteBatch);

            for (int i = 0; i < RankPerks.Count(); i++)
            {
                spriteBatch.DrawString(TextureManager.SpriteFont15, RankPerks[i], new Vector2(Globals.ScreenSize.X / 2 - TextureManager.SpriteFont15.MeasureString(RankPerks[i]).X / 2, i * 25 + 150), Color.White);
            }
        }

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
                    if (Inventory[i, j].Pressed() && Inventory[i, j].Type != ItemType.nothing)
                    {
                        selectedItem = Inventory[i, j];
                        selectedItemArrayPosition = new Point(i, j);
                    }

                    // Right click
                    if (Inventory[i, j].PressedRight() && Inventory[i, j].Type != ItemType.nothing)
                    {
                        Inventory[i, j].UseItem(this, Inventory[i, j]);
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

                // remove
                for (int i = Inventory.GetLength(0) - 1; i >= 0; i--)
                {
                    for (int j = Inventory.GetLength(1) - 1; j >= 0; j--)
                    {
                        if (Inventory[i, j].Dead)
                        {
                            Inventory[i, j] = new Item(Item.Nothing, ItemType.nothing, TextureManager.none, "", "");
                        }
                    }
                }
            }
        }

        public void DrawInventory(SpriteBatch spriteBatch)
        {
            // Health
            Health.Draw(spriteBatch);
            spriteBatch.DrawString(TextureManager.SpriteFont15, "Current Health:", new Vector2(Health.Position.X + 15, Health.Position.Y - 30), Color.White);

            if (selectedItem != null)
            {
                spriteBatch.Draw(selectedItem.Texture, new Vector2(Globals.MState.X - 32, Globals.MState.Y - 32), null, selectedItem.Colour * 0.5f, 0f, Vector2.Zero, 1f, SpriteEffects.None, selectedItem.Depth);
                spriteBatch.Draw(selectedItem.TextureBackground, new Vector2(Globals.MState.X - 32, Globals.MState.Y - 32), null, Color.White * 0.5f, 0f, Vector2.Zero, 1f, SpriteEffects.None, selectedItem.Depth + 0.1f); 
            }

            // Weapons
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Weapons", new Vector2(40, 130), Color.White);
            for (int i = 0; i < 3; i++)
            {
                Inventory[2 + i, 5].DrawInventory(spriteBatch, new Vector2(i * 64 + 32, 200));
                spriteBatch.Draw(TextureManager.inventorySlot, new Vector2(i * 64 - 32 + 32, 200 - 32), Color.White);
            }

            // Shield
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Shield", new Vector2(60, 258), Color.White);
            ShipShield.DrawInventory(spriteBatch, new Vector2(100, 328));
            spriteBatch.Draw(TextureManager.inventorySlot, new Vector2(100 - 32, 328 - 32), Color.White);

            // Hull
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Hull", new Vector2(70, 386), Color.White);
            ShipHull.DrawInventory(spriteBatch, new Vector2(100, 456));
            spriteBatch.Draw(TextureManager.inventorySlot, new Vector2(100 - 32, 456 - 32), Color.White);

            // Inventory
            spriteBatch.Draw(TextureManager.inventory, new Vector2(300 - 32, 200 - 32), Color.White);

            spriteBatch.DrawString(TextureManager.SpriteFont20, "Inventory", new Vector2(360, 100), Color.White);
            for (int i = 0; i < Inventory.GetLength(0); i++)
            {
                for (int j = 0; j < Inventory.GetLength(1) - 1; j++)
			    {
			        Inventory[i, j].DrawInventory(spriteBatch, new Vector2(i * 64 + 300, j * 64 + 200));
			    }
            }
        }

        public void DrawMap(SpriteBatch spriteBatch, Vector2 position, float size, float direction)
        {
            spriteBatch.Draw(Texture, position, null, Color.White, direction, new Vector2(Texture.Width / 2, Texture.Height / 2), size, SpriteEffects.None, Depth);
            spriteBatch.Draw(engineAnimation, position, new Rectangle(frame * 64, 0, 64, 64), Color.White, direction, new Vector2(Texture.Width / 2, Texture.Height / 2), size, SpriteEffects.None, Depth - 0.1f);
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
            if (!Move)
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
                Animation();
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
                    KnockBack = 3;
                    Weapons[SelectedWeapon].Method(this, Weapons[SelectedWeapon], tilesMatched, level, false);
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
