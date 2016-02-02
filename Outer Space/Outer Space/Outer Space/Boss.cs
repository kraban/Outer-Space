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
    class Boss : Enemy
    {
        private ChargeState charge;
        private float accelerate;
        private Rectangle chargeRectangle { get { return new Rectangle((int)Position.X - Texture.Width / 3, (int)Position.Y - Texture.Height / 3, (int)(Texture.Width * (2f/3f)), (int)(Texture.Height * (2f/3f))); } }

        public Boss()
            : base()
        {
            this.Texture = TextureManager.boss;
            this.charge = ChargeState.NotCharging;
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
                // Start charging
                // Will only charge is the player has a chance to avoid it
                if (charge == ChargeState.NotCharging && Globals.Randomizer.Next(0, 1001) < 3 && level.CheckPossibleMatches().Any(item => item.Type == TileType.left || item.Type == TileType.right || item.Type == TileType.shoot))
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

                // Miss
                if (Position.Y > Globals.ScreenSize.Y)
                {
                    charge = ChargeState.Finished;
                }
            }
        }
    }
}
