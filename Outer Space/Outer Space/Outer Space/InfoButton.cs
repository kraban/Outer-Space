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
    public class InfoButton : Button
    {
        public string[] Info { get; set; }

        private bool flip;

        public InfoButton(Vector2 position, string[] info)
            : base(position, "?", TextureManager.SpriteFont20)
        {
            this.Info = info;
            this.TextColor = new Color(0, 255, 255);
        }

        public override bool Press()
        {
            return false;
        }

        public override void Update()
        {
            if (Convert.ToBoolean(Options.TutorialTips))
            {
                if (flip)
                {
                    Size = MathHelper.Lerp(Size, 1.5f, 0.02f);
                }
                else
                {
                    Size = MathHelper.Lerp(Size, 0.5f, 0.02f);
                }
                if (Size > 1.4f)
                {
                    flip = !flip;
                }
                else if (Size < 0.6f)
                {
                    flip = !flip;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Convert.ToBoolean(Options.TutorialTips))
            {
                base.Draw(spriteBatch);

                if (Hover())
                {
                    spriteBatch.Draw(TextureManager.fade, new Vector2(0, 0), null, Color.White * 0.75f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.001f);
                    for (int i = 0; i < Info.Length; i++)
                    {
                        spriteBatch.DrawString(TextureManager.SpriteFont15, Info[i], new Vector2(Globals.ScreenSize.X / 2, Globals.ScreenSize.Y / 2 - Info.Length * 20 + i * 20), Color.White, 0f, new Vector2(TextureManager.SpriteFont15.MeasureString(Info[i]).X / 2, 0), 1f, SpriteEffects.None, 0f);
                    }
                }
            }
        }
    }
}
