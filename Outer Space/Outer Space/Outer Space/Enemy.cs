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
    public enum Difficulty { Easy, Medium, Hard, Boss }
    public class Enemy : Ship
    {
        // Public properties
        public int ShootTimer { get; set; }
        public Difficulty EnemyDifficulty { get; set; }

        private int dodgeCooldown;

        // Constructor(s)
        public Enemy(Difficulty difficulty)
            : base((float)Math.PI * 0.5f)
        {
            this.EnemyDifficulty = difficulty;
            this.Position = new Vector2((int)ShipLocation * 100 + 200, -100);
            this.Texture = TextureManager.enemyShip;
            this.EngineAnimation = TextureManager.enemyShipEngineAnimation;
            this.MaxFrame = 1;
            this.Targets.Add("Player");
            this.ShootTimer = 60;

            // Possible modules
            List<int> possibleWeapons = new List<int>();

            this.Health = new Bar(new Vector2(70, 10), 100, 20, 15 * (int)EnemyDifficulty + 50 + Globals.Randomizer.Next(5, 10), Color.Red);
            this.ShipShield = new Shield(new Vector2(270, 10), 100, 20, 20 + Globals.Randomizer.Next(5, 10) + (int)difficulty * 5, Globals.EnemyShields[Globals.Randomizer.Next(0, Globals.EnemyShields.Count())], (int)difficulty);
            this.shieldRegeneration = 0.007f * (float)EnemyDifficulty;

            // Difficulty
            Inventory[2, 5] = new Weapon(this, Globals.EnemyWeapons[Globals.Randomizer.Next(0, Globals.EnemyWeapons.Count())], (int)difficulty);
            if (EnemyDifficulty == Difficulty.Easy)
            {
                Inventory[3, 5] = new Item(Item.Nothing, ItemType.nothing, TextureManager.none, "", "");
            }
            else
            {
                Inventory[3, 5] = new Weapon(this, Globals.EnemyWeapons[Globals.Randomizer.Next(0, Globals.EnemyWeapons.Count())], (int)difficulty);
            }

            // Enemy hull
            ShipHull = new Hull(this, Globals.EnemyHulls[Globals.Randomizer.Next(0, Globals.EnemyHulls.Count())], (int)difficulty);
        }

        // Method(s)

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Weapons
            for (int i = 0; i < Weapons.Count; i++)
            {
                Weapons[i].Position = new Vector2(Weapons[i].Texture.Width / 2 + 64, 200 + i * Weapons[i].Texture.Height);
                if (i == SelectedWeapon)
                {
                    spriteBatch.Draw(TextureManager.selected, new Vector2(Weapons[i].Position.X - Weapons[i].Texture.Width / 2, Weapons[i].Position.Y - Weapons[i].Texture.Height / 2), Color.White);
                }
                Weapons[i].Draw(spriteBatch);
            }

            // Hull
            ShipHull.Position = new Vector2(ShipHull.Texture.Width / 2, 200);

            // Shield
            ShipShield.Position = new Vector2(ShipShield.Texture.Width / 2, 264);
        }

        public override void UpdateLevel(Level level)
        {
            base.UpdateLevel(level);

            // Move to normal position after knockback
            Position = new Vector2(Position.X, (float)MathHelper.Lerp(Position.Y, Texture.Height + 40, 0.1f));

            if (level.Started)
            {
                // Shoot
                ShootTimer--;
                if (ShootTimer < 0 && ShipLocation == level.Player.ShipLocation && Weapons[SelectedWeapon].Disabled < 0)
                {
                    ShootTimer = 180;
                    KnockBack = -3;
                    Weapons[SelectedWeapon].Method(this, Weapons[SelectedWeapon], 0, level, false);
                }

                // Change weapon if not easy difficulty
                if (EnemyDifficulty != Difficulty.Easy && CurrentWeapon.Disabled > 0 && Globals.Randomizer.Next(0, 101) < 2)
                {
                    for (int i = 0; i < Weapons.Count(); i++)
                    {
                        if (Weapons[i].Disabled <= 0)
                        {
                            SelectedWeapon = i;
                        }
                    }
                }

                // Dodge if hard difficulty
                dodgeCooldown--;
                if (EnemyDifficulty == Difficulty.Hard && dodgeCooldown < 0 && (Health.Value / Health.MaxValue) < 0.7)
                {
                    foreach (Shot shot in level.GameObjects.Where(item => item is Shot))
                    {
                        if (shot.Direction > Math.PI)
                        {
                            shot.Direction -= (float)Math.PI * 2f;
                        }
                        if (Globals.Distance(Position, shot.Position) < 100 && Math.Abs(Math.Atan2(Position.Y - shot.Position.Y, Position.X - shot.Position.X) - shot.Direction) < 0.3f)
                        {
                            dodgeCooldown = 180;
                            if (Globals.Randomizer.Next(0, 101) < 30)
                            {
                                if (ShipLocation == Location.left)
                                {
                                    ShipLocation = Location.middle;
                                }
                                else if (ShipLocation == Location.middle)
                                {
                                    ShipLocation = Location.left;
                                }
                                else
                                {
                                    ShipLocation = Location.middle;
                                }
                            }
                        }
                    }
                }

                // Move
                if (Globals.Randomizer.Next(0, 1001) < 3 + (Health.Value / Health.MaxValue) * 5)
                {
                    if (ShipLocation < level.Player.ShipLocation)
                    {
                        ShipLocation++;
                        DirectionSpeed = -0.0005f;
                        ShootTimer = 60;
                    }
                    else if (ShipLocation > level.Player.ShipLocation)
                    {
                        ShipLocation--;
                        DirectionSpeed = 0.0005f;
                        ShootTimer = 60;
                    }
                }

                // Die
                if (Health.Value <= 0)
                {
                    Dead = true;

                    // Pieces
                    level.CreatePieces(Position, Texture);
                } 
            }
        }
    }
}
