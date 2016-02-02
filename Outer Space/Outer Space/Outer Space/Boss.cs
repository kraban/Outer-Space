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
    public enum DodgeState { Begin, Dodge, NotDodging }
    class Boss : Enemy
    {
        // Charge
        private ChargeState charge;
        private float accelerate;
        private Rectangle chargeRectangle { get { return new Rectangle((int)Position.X - Texture.Width / 3, (int)Position.Y - Texture.Height / 3, (int)(Texture.Width * (2f/3f)), (int)(Texture.Height * (2f/3f))); } }

        // Dodge
        private DodgeState dodge;

        public Boss()
            : base()
        {
            this.Texture = TextureManager.boss;
            this.charge = ChargeState.NotCharging;
            this.dodge = DodgeState.NotDodging;
            this.Depth = 0.5f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (dodge == DodgeState.Dodge)
            {
                for (int i = 0; i < 3; i++)
                {
                    spriteBatch.Draw(Texture, Position, null, Color.LightGray * (0.9f - ((float)i / 5f)), Direction, new Vector2(Texture.Width / 2, Texture.Height / 2), Size + 0.2f * (float)i, SpriteEffects.None, ((float)i / 10f));
                    spriteBatch.Draw(Texture, new Vector2((int)ShipLocation * 100 + 200, Position.Y), null, Color.LightGray * (0.9f - ((float)i / 5f)), Direction, new Vector2(Texture.Width / 2, Texture.Height / 2), 1 - Size + 0.1f * (float)i, SpriteEffects.None, ((float)i / 10f));
                }
            }
        }

        public override void UpdateLevel(Level level)
        {
            // Move to normal position after knockback
            if (charge == ChargeState.NotCharging)
            {
                Position = new Vector2(Position.X, (float)MathHelper.Lerp(Position.Y, Texture.Height, 0.05f));
            }
            if (level.Started)
            {
                // Place mine in tiles
                if (Globals.Randomizer.Next(0, 1001) < 10)
                {
                    int mineCount = 0;
                    for (int i = 0; i < level.Tiles.Count(); i++)
                    {
                        for (int j = 0; j < level.Tiles[i].Count(); j++)
                        {
                            if (level.Tiles[i][j].Mine)
                            {
                                mineCount++;
                            }
                        }
                    }
                    if (mineCount < 5)
                    {
                        int random = Globals.Randomizer.Next(0, level.Tiles.Count());
                        level.Tiles[random][Globals.Randomizer.Next(0, level.Tiles[random].Count() - 2)].Mine = true;
                    }
                }

                // Shield
                if (charge == ChargeState.NotCharging && dodge == DodgeState.NotDodging)
                {
                    bool shotAt = false;
                    foreach (Shot shot in level.GameObjects.Where(item => item is Shot))
                    {
                        if (shot.Targets.Any(item => item == "Boss") && shot.Position.Y < 300 && shot.Position.X < Position.X + 10 && shot.Position.X > Position.Y - 10)
                        {
                            shotAt = true;
                            break;
                        }
                    }
                    if (shotAt)
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
                if (dodge == DodgeState.Dodge)
                {
                    Size = MathHelper.Lerp(Size, 0, 0.1f);
                    if (Size < 0.1f)
                    {
                        dodge = DodgeState.NotDodging;
                        Position = new Vector2((int)ShipLocation * 100 + 200, Position.Y);
                        Size = 1;
                    }
                }

                // Start charging
                // Will only charge is the player has a chance to avoid it
                if (charge == ChargeState.NotCharging && dodge == DodgeState.NotDodging && Globals.Randomizer.Next(0, 1001) < 0 && level.CheckPossibleMatches().Any(item => item.Type == TileType.left || item.Type == TileType.right || item.Type == TileType.shoot))
                {
                    charge = ChargeState.Initialize;
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
                    accelerate += 0.01f;
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
                    Position += new Vector2((float)Math.Cos(Direction) * accelerate, (float)Math.Sin(Direction) * accelerate);
                    Vector2 afterChargeTarget = new Vector2((int)ShipLocation * 100 + 200, -200);
                    Direction = MathHelper.Lerp(Direction, (float)Math.Atan2(afterChargeTarget.Y - Position.Y, afterChargeTarget.X - Position.X), 0.03f);
                    if (Position.Y < -180)
                    {
                        charge = ChargeState.NotCharging;
                        Direction = StandardDirection;
                        Position = new Vector2((int)ShipLocation * 100 + 200, Position.Y);
                        accelerate = 0f;
                    }
                }
                // Hit player with charge
                if (level.Player.Box.Intersects(chargeRectangle))
                {
                    charge = ChargeState.Finished;
                    level.Player.KnockBack = 10;
                    Camera.ScreenShakeTimer = 30;
                }

                // Shot hit
                if (charge != ChargeState.NotCharging)
                {
                    foreach (Shot shot in level.GameObjects.Where(item => item is Shot))
                    {
                        if (shot.Targets.Any(item => item == "Boss") && shot.Box.Intersects(Box))
                        {
                            charge = ChargeState.Finished;
                        }
                    }
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
