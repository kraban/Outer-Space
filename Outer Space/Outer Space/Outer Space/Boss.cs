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
    public enum ChargeState { Initialize, Beginning, Charge, Finished, NotCharging }
    public enum DodgeState { Dodge, NotDodging }
    class Boss : Enemy
    {
        private List<int> possibleAttacks;
        private int attackCooldown;
        private int deathTimer;

        // Shoot
        public Vector2 LeftShootPosition { get { return new Vector2(Position.X - 50, Position.Y + 50); } }
        public Vector2 RightShootPosition { get { return new Vector2(Position.X + 50, Position.Y + 50); } }
        private int shootTimer;

        // Charge
        private ChargeState charge;
        private float accelerate;
        private Rectangle chargeRectangle { get { return new Rectangle((int)Position.X - Texture.Width / 3, (int)Position.Y - Texture.Height / 3, (int)(Texture.Width * (2f/3f)), (int)(Texture.Height * (2f/3f))); } }

        // Dodge
        private DodgeState dodge;

        public Boss()
            : base(Difficulty.Hard)
        {
            this.TextureBackground = TextureManager.boss;
            this.Texture = TextureManager.bossForeground;
            this.charge = ChargeState.NotCharging;
            this.dodge = DodgeState.NotDodging;
            this.Depth = 0.5f;
            this.Opacity = 0f;
            this.possibleAttacks = new List<int>();
            this.EngineAnimation = TextureManager.bossEngineAnimation;
            this.MaxFrame = 3;
            this.FrameWidth = 163;
            this.FrameHeight = 139;
            this.Colour = Color.Red;

            // Modules
            this.Health = new Bar(new Vector2(70, 10), 200, 25, 150, Color.Red);
            ShipShield = new Shield(new Vector2(270, 10), (int)ShipShield.Width, 20, 0, 0, 3);
            ShipShield.Description = "|W|Shield: |0,0,255|" + ShipShield.MaxValue + "|W|\nWill teleport to another location just before being hit.";
            ShipHull = new Hull(this, 0, 3);
            ShipHull.Description = "|W|Armor: |255,255,0|" + ShipHull.Armor + "|255,255,100|\nPlaces mines on tiles in the tileboard.\nThe Player will take 20 damage when matching tiles with mines.\nMines will be disarmed when matched or then falling.";
            Weapons[0].Description = "|W|Charges forward and deals massive damage if it hits the Player.\nThe Boss is vurnerable when charging.";
            Weapons[1].Description = "|W|Shoot several shots, eighter in a X pattern or a V patterns.";
            Weapons[1].Method = Weapon.ListOfMethods()[0];
        }

        public override void TakeDamage(float damage, float goThroughShield, DamageType damageType, bool FromShield)
        {
            if (charge != ChargeState.NotCharging)
            {
                charge = ChargeState.Finished;
                base.TakeDamage(damage, goThroughShield, damageType, FromShield);
            }
            else if (damageType == DamageType.laser)
            {
                dodge = DodgeState.Dodge;
                SoundManager.bossTeleport.Play();
                List<int> possibleLocations = new List<int>();
                for (int i = 0; i < 3; i++)
                {
                    if (i != (int)ShipLocation)
                    {
                        possibleLocations.Add(i);
                    }
                }
                ShipLocation = (Location)possibleLocations[Globals.Randomizer.Next(0, possibleLocations.Count())];
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (dodge == DodgeState.Dodge)
            {
                FrameWidth = 0;
                for (int i = 0; i < 3; i++)
                {
                    spriteBatch.Draw(TextureBackground, Position, null, Color.LightGray * (0.9f - ((float)i / 5f)), Direction, new Vector2(Texture.Width / 2, Texture.Height / 2), Size + 0.2f * (float)i, SpriteEffects.None, ((float)i / 10f));
                    spriteBatch.Draw(TextureBackground, new Vector2((int)ShipLocation * 100 + 200, Position.Y), null, Color.LightGray * (0.9f - ((float)i / 5f)), Direction, new Vector2(Texture.Width / 2, Texture.Height / 2), 1 - Size + 0.1f * (float)i, SpriteEffects.None, ((float)i / 10f));
                }
            }
            else
            {
                FrameWidth = 163;
            }
        }

        public override void UpdateLevel(Level level)
        {
            // Animation
            AnimationTimer++;
            if (AnimationTimer > 7)
            {
                AnimationTimer = 0;
                if (Frame < MaxFrame)
                {
                    Frame++;
                }
                else
                {
                    Frame = 0;
                }
            }

            // Renew attacks
            if (possibleAttacks.Count() == 0)
            {
                possibleAttacks.Add(0);
                possibleAttacks.Add(1);
                possibleAttacks.Add(1);
                possibleAttacks.Add(2);
                possibleAttacks.Add(2);
                possibleAttacks.Add(2);
            }

            DamageOverTime();
            // Weapons
            foreach (Weapon w in Weapons)
            {
                w.UpdateLevel(level);
            }
            // Move to normal position after knockback
            if (charge == ChargeState.NotCharging)
            {
                Position = new Vector2(Position.X, (float)MathHelper.Lerp(Position.Y, Texture.Height, 0.05f));
            }
            if (level.Started)
            {
                Health.Change(-1);
                // Die
                if (Health.Value <= 0 && deathTimer == 0 && dodge == DodgeState.NotDodging && charge == ChargeState.NotCharging)
                {
                    deathTimer = 180;
                    SoundManager.die.Play();
                }
                if (deathTimer > 0)
                {
                    deathTimer--;
                    if (deathTimer % 5 == 0)
                    {
                        dodge = DodgeState.Dodge;
                        List<int> possibleLocations = new List<int>();
                        for (int i = 0; i < 3; i++)
                        {
                            if (i != (int)ShipLocation)
                            {
                                possibleLocations.Add(i);
                            }
                        }
                        ShipLocation = (Location)possibleLocations[Globals.Randomizer.Next(0, possibleLocations.Count())];
                    }
                }
                if (deathTimer == 1)
                {
                    Dead = true;
                    SoundManager.explosion.Play();
                    for (int i = 0; i < 100; i++)
                    {
                        level.ToAdd.Add(new Piece(Position, Texture, 60, 3f));
                    }
                    SceneManager.ChangeScene(SceneManager.winScene);
                }

                attackCooldown--;
                // Choose attack
                if (dodge == DodgeState.NotDodging && charge == ChargeState.NotCharging && shootTimer < -20 && Globals.Randomizer.Next(0, 101) < 3 && attackCooldown < 0 && deathTimer == 0)
                {
                    attackCooldown = 60;
                    int attack = possibleAttacks[Globals.Randomizer.Next(0, possibleAttacks.Count())];
                    possibleAttacks.Remove(attack);
                    if (attack == 0)
                    {
                        if (ShipLocation == level.Player.ShipLocation)
                        {
                            charge = ChargeState.Initialize;
                            SoundManager.bossChargeAttack.Play();
                        }
                        else
                        {
                            SoundManager.bossTeleport.Play();
                            dodge = DodgeState.Dodge;
                            ShipLocation = level.Player.ShipLocation;
                            possibleAttacks.Add(0);
                        }
                    }
                    else if (attack == 1)
                    {
                        shootTimer = 200;
                        Opacity = 0;
                        SoundManager.bossChargeShot.Play();
                    }
                    else if (attack == 2)
                    {
                        // Place mine in tiles
                        int random = Globals.Randomizer.Next(0, level.Tiles.Count());
                        level.Tiles[random][Globals.Randomizer.Next(0, level.Tiles[random].Count() - 2)].Mine = true;
                        SoundManager.bossMine.Play();
                    }
                }

                // Shoot
                shootTimer--;
                if (shootTimer > 0)
                {
                    if (shootTimer > 100)
                    {
                        Opacity += 0.015f;
                    }
                    else if (shootTimer < 20)
                    {
                        Opacity -= 0.07f;
                    }
                    if (shootTimer == 100)
                    {
                        if (ShipLocation == level.Player.ShipLocation)
                        {
                            Weapon.FireBossV(this, Weapons[1], 0, level, false);
                        }
                        else
                        {
                            Weapon.FireBossX(this, Weapons[1], 0, level, false);
                        }
                    }
                }

                // Shield
                if (dodge == DodgeState.Dodge)
                {
                    Weapons[1].ShotsToShoot.Clear();
                    Size = MathHelper.Lerp(Size, 0, 0.1f);
                    if (Size < 0.1f)
                    {
                        dodge = DodgeState.NotDodging;
                        Position = new Vector2((int)ShipLocation * 100 + 200, Position.Y);
                        Size = 1;
                    }
                }

                // Back away to gain more power for charge
                if (charge == ChargeState.Initialize)
                {
                    accelerate = -2;
                    charge = ChargeState.Beginning;
                }
                if (charge == ChargeState.Beginning)
                {
                    Position += new Vector2((float)Math.Cos(Direction) * accelerate, (float)Math.Sin(Direction) * accelerate);
                    accelerate += 0.023f;
                    if (accelerate > 0)
                    {
                        charge = ChargeState.Charge;
                    }
                }

                // Charge
                if (charge == ChargeState.Charge)
                {
                    Position += new Vector2((float)Math.Cos(Direction) * accelerate, (float)Math.Sin(Direction) * accelerate);
                    accelerate += 0.2f;
                }
                else if (charge == ChargeState.Finished) // Return to normal position
                {
                    if (accelerate < 4)
                    {
                        accelerate = 4;
                    }
                    Position += new Vector2((float)Math.Cos(Direction) * accelerate, (float)Math.Sin(Direction) * accelerate);
                    Vector2 afterChargeTarget = new Vector2((int)ShipLocation * 100 + 200, -400);
                    Direction = MathHelper.Lerp(Direction, (float)Math.Atan2(afterChargeTarget.Y - Position.Y, afterChargeTarget.X - Position.X), 0.03f);
                    if (Position.Y < -300)
                    {
                        charge = ChargeState.NotCharging;
                        Direction = StandardDirection;
                        Position = new Vector2((int)ShipLocation * 100 + 200, Position.Y);
                        accelerate = 0f;
                    }
                }
                // Hit player with charge
                if (level.Player.Box.Intersects(chargeRectangle) && charge == ChargeState.Charge)
                {
                    charge = ChargeState.Finished;
                    level.Player.KnockBack = 10;
                    level.Player.TakeDamage(50, 0.5f, DamageType.rock, false);
                    Camera.ScreenShakeTimer = 30;
                }

                // Miss
                if (Position.Y > Globals.ScreenSize.Y)
                {
                    charge = ChargeState.Finished;
                }
            }
        }
    }
}
