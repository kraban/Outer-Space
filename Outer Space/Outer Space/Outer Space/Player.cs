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
        private Button craft;
        private int currentlyCrafting;

        // Constructor(s)
        public Player()
            : base((float)Math.PI * 1.5f)
        {
            this.Texture = TextureManager.boss;
            this.Position = new Vector2(300, Globals.ScreenSize.Y - Texture.Height);

            this.Energy = new Bar(new Vector2(400, Globals.ScreenSize.Y - 35), 100, 20, 100, Color.Orange);
            this.Rank = 1;
            this.Experience = new Bar(new Vector2(Globals.ScreenSize.X / 2 - 330, 50), 300, 25, 100, Color.Green);
            this.Experience.Change(-Experience.MaxValue);
            this.RankPerks = new List<string>();
            Targets.Add("Enemy");
            Targets.Add("Boss");

            this.craft = new Button(new Vector2(700, 400), "Craft", TextureManager.SpriteFont20);
            for (int i = 0; i < 2; i++)
            {
                AddItem(new Item(Globals.Flee));
            }

            // Startmodules
            Inventory[2, 5] = new Weapon(this, 2, -1);
            Inventory[3, 5] = new Weapon(this, 3, -1);
            this.ShipHull = new Hull(this, 6, -1);
            this.ShipShield = new Shield(new Vector2(200, Globals.ScreenSize.Y - 35), 100, 20, 60, 0, -1);
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
            for (int i = 0; i < Inventory.GetLength(1) - 2; i++)
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
                        if (Inventory[j, i].Type == ItemType.nothing)
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
            int random = Globals.Randomizer.Next(0, 7);
            SceneManager.mapScene.NewRank.Flash = 10;
            if (random == 0)
            {
                AddItem(new Shield(new Vector2(200, Globals.ScreenSize.Y - 35), 100, 20, 60 + Globals.Randomizer.Next(-10, 20), Globals.Randomizer.Next(0, Shield.ListOfShieldMethods().Count()), 2));
                RankPerks.Add("Shield Module");
            }
            else if (random == 1)
            {
                AddItem(new Hull(this, Globals.Randomizer.Next(0, Hull.ListOfHullMethods().Count()), 2));
                RankPerks.Add("Hull Module");
            }
            else if (random == 2)
            {
                AddItem(new Weapon(this, Globals.Randomizer.Next(0, Weapon.ListOfMethods().Count()), 2));
                RankPerks.Add("Weapon Module");
            }
            else if (random == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    AddItem(new Item(Globals.Heal));
                    AddItem(new Item(Globals.Flee));
                }
                RankPerks.Add("Misc items");
            }
            else if (random == 4)
            {
                Health.MaxValue += 10;
                Health.Change(10);
                RankPerks.Add("Increase health by 10");
            }
            else if (random == 5)
            {
                shieldRegeneration += 0.01f;
                RankPerks.Add("Increase shield regeneration");
            }
            else if (random == 6)
            {
                BonusDamage += 2;
                RankPerks.Add("Increase damage");
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
                spriteBatch.DrawString(TextureManager.SpriteFont15, RankPerks[i], new Vector2(Globals.ScreenSize.X / 2 - TextureManager.SpriteFont15.MeasureString(RankPerks[i]).X / 2, i * 25 + 150), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);
            }
        }

        public void SwapItem(Point swapWith)
        {
            Item temp = Inventory[swapWith.X, swapWith.Y];
            Inventory[swapWith.X, swapWith.Y] = selectedItem;
            Inventory[selectedItemArrayPosition.X, selectedItemArrayPosition.Y] = temp;

        }

        public bool CanCraft()
        {
            List<ItemType> craftableTypes = new List<ItemType>();
            craftableTypes.Add(ItemType.hull);
            craftableTypes.Add(ItemType.shield);
            craftableTypes.Add(ItemType.weapon);
            if (craftableTypes.Any(item => item == Inventory[0, 6].Type) && craftableTypes.Any(item => item == Inventory[1, 6].Type) && craftableTypes.Any(item => item == Inventory[2, 6].Type))
            {
                return true;
            }
            return false;
        }

        public void UpdateInventory()
        {
            // Crafting
            if (currentlyCrafting > 0)
            {
                for (int i = 0; i < Globals.Randomizer.Next(5, 10); i++)
                {
                    SceneManager.GameObjects.Add(new Piece(new Vector2(864, 350), TextureManager.explosion, 90, 1.5f));
                }
            }

            craft.Update();
            currentlyCrafting--;
            if (CanCraft() && currentlyCrafting < 0)
            {
                if (craft.Press())
                {
                    currentlyCrafting = 60;
                }
            }
            if (currentlyCrafting == 0)
            {
                int itemLevel = (int)((Inventory[0, 6].ItemLevel + Inventory[1, 6].ItemLevel + Inventory[2, 6].ItemLevel) / 3 + MathHelper.Lerp(-0.2f, 0.2f, (float)Globals.Randomizer.NextDouble()));
                Inventory[0, 6] = new Item(Globals.Nothing);
                Inventory[1, 6] = new Item(Globals.Nothing);
                Inventory[2, 6] = new Item(Globals.Nothing);
                if (Inventory[3, 6].Type != ItemType.nothing)
                {
                    AddItem(Inventory[3, 6]);
                }
                int random = Globals.Randomizer.Next(0, 3);
                if (random == 0)
                {
                    Inventory[3, 6] = new Weapon(this, Globals.Randomizer.Next(0, Weapon.ListOfMethods().Count()), itemLevel);
                }
                else if (random == 1)
                {
                    Inventory[3, 6] = new Hull(this, Globals.Randomizer.Next(0, Hull.ListOfHullMethods().Count()), itemLevel);
                }
                else if (random == 2)
                {
                    Inventory[3, 6] = new Shield(new Vector2(200, Globals.ScreenSize.Y - 35), 100, 20, 60 + itemLevel * 20 + Globals.Randomizer.Next(-5, 15), Globals.Randomizer.Next(0, Shield.ListOfShieldMethods().Count()), itemLevel);
                }
            }

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
                                        break;
                                    }
                                }
                                else if (!(i > 2 && j == 6)) // inventory
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
            spriteBatch.DrawString(TextureManager.SpriteFont15, "Current Health:", new Vector2(Health.Position.X + 15, Health.Position.Y - 30), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);

            if (selectedItem != null)
            {
                spriteBatch.Draw(selectedItem.Texture, new Vector2(Globals.MState.X - 32, Globals.MState.Y - 32), null, selectedItem.Colour * 0.5f, 0f, Vector2.Zero, 1f, SpriteEffects.None, selectedItem.Depth);
                spriteBatch.Draw(selectedItem.TextureBackground, new Vector2(Globals.MState.X - 32, Globals.MState.Y - 32), null, Color.White * 0.5f, 0f, Vector2.Zero, 1f, SpriteEffects.None, selectedItem.Depth + 0.1f); 
            }

            // Weapons
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Weapons", new Vector2(40, 130), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);
            for (int i = 0; i < 3; i++)
            {
                Inventory[2 + i, 5].DrawInventory(spriteBatch, new Vector2(i * 64 + 32, 200));
                spriteBatch.Draw(TextureManager.inventorySlot, new Vector2(i * 64 - 32 + 32, 200 - 32), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);
            }

            // Shield
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Shield", new Vector2(60, 258), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);
            ShipShield.DrawInventory(spriteBatch, new Vector2(100, 328));
            spriteBatch.Draw(TextureManager.inventorySlot, new Vector2(100 - 32, 328 - 32), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);

            // Hull
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Hull", new Vector2(70, 386), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);
            ShipHull.DrawInventory(spriteBatch, new Vector2(100, 456));
            spriteBatch.Draw(TextureManager.inventorySlot, new Vector2(100 - 32, 456 - 32), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);

            // Inventory
            spriteBatch.Draw(TextureManager.inventory, new Vector2(300 - 32, 200 - 32), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);

            spriteBatch.DrawString(TextureManager.SpriteFont20, "Inventory", new Vector2(360, 100), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);
            for (int i = 0; i < Inventory.GetLength(0); i++)
            {
                for (int j = 0; j < Inventory.GetLength(1) - 2; j++)
			    {
			        Inventory[i, j].DrawInventory(spriteBatch, new Vector2(i * 64 + 300, j * 64 + 200));
			    }
            }

            // Crafting
            spriteBatch.DrawString(TextureManager.SpriteFont20, "Crafting", new Vector2(805, 100), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);
            craft.Draw(spriteBatch);
            for (int i = 0; i < 3; i++)
            {
                if (currentlyCrafting > 0)
                {
                    Vector2 target = new Vector2(864, 350);
                    Inventory[i, 6].DrawInventory(spriteBatch, new Vector2(MathHelper.Lerp(i * 64 + 800, target.X, (float)(60 - currentlyCrafting) / 60), MathHelper.Lerp(200, target.Y, (float)(60 - currentlyCrafting) / 60)));
                }
                else
                {
                    Inventory[i, 6].DrawInventory(spriteBatch, new Vector2(i * 64 + 800, 200));
                }
                spriteBatch.Draw(TextureManager.inventorySlot, new Vector2(i * 64 + 800 - 32, 200 - 32), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);
            }
            spriteBatch.Draw(TextureManager.craftingArrow, new Vector2(832, 350 - 32 - 80), null, CanCraft() ? new Color(0, 255, 255) : Color.Gray, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f); 
            Inventory[3, 6].DrawInventory(spriteBatch, new Vector2(864, 350));
            spriteBatch.Draw(TextureManager.inventorySlot, new Vector2(864 - 32, 350 - 32), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.01f);
        }

        public void DrawMap(SpriteBatch spriteBatch, Vector2 position, float size, float direction)
        {
            spriteBatch.Draw(Texture, position, null, Color.White, direction, new Vector2(Texture.Width / 2, Texture.Height / 2), size, SpriteEffects.None, Depth);
            spriteBatch.Draw(EngineAnimation, position, new Rectangle(Frame * 64, 0, 64, 64), Color.White, direction, new Vector2(Texture.Width / 2, Texture.Height / 2), size, SpriteEffects.None, Depth - 0.1f);
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
                // Die
                if (Health.Value <= 0)
                {
                    Move = true;
                    Dead = true;
                }

                Position = new Vector2(Position.X, (float)MathHelper.Lerp(Position.Y, Globals.ScreenSize.Y - Texture.Height, 0.1f));
                base.UpdateLevel(level);
                // Select weapon
                for (int i = 0; i < Weapons.Count(); i++)
                {
                    if (Weapons[i].Pressed())
                    {
                        SelectedWeapon = i;
                    }
                }
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
                        if (level.GameObjects.Any(item => item is Enemy))
                        {
                            Enemy enemy = (Enemy)level.GameObjects.First(item => item is Enemy);
                            enemy.ShootTimer = 90;
                        }
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
                        if (level.GameObjects.Any(item => item is Enemy))
                        {
                            Enemy enemy = (Enemy)level.GameObjects.First(item => item is Enemy);
                            enemy.ShootTimer = 90;
                        }
                    }
                }
            }
            else
            {
                Animation();
                Direction = MathHelper.Lerp(Direction, (float)Math.PI, 0.03f);
                Speed += 0.1f;
                Position += new Vector2((float)Math.Cos(Direction) * Speed, (float)Math.Sin(Direction) * Speed);

                if (Dead == true)
                {
                    Speed -= 0.06f;
                    Direction += MathHelper.Lerp(-0.15f, 0.15f, (float)Globals.Randomizer.NextDouble());
                    for (int i = 0; i < Globals.Randomizer.Next(1, 2); i++)
                    {
                        level.ToAdd.Add(new Piece(new Vector2(Position.X + Globals.Randomizer.Next(-20, 20), Position.Y + Globals.Randomizer.Next(-20, 20)), Texture, 60, 0.5f));
                        level.ToAdd.Add(new Piece(new Vector2(Position.X + Globals.Randomizer.Next(-20, 20), Position.Y + Globals.Randomizer.Next(-20, 20)), TextureManager.explosion, 60, 1.5f));
                    }
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
                    ShipShield.Change((ShipShield.ShieldHeal / 3) * tilesMatched);
                } 
            }
            else
            {
                level.CombatText("NOT ENOUGH ENERGY!");
            }
        }
    }
}
