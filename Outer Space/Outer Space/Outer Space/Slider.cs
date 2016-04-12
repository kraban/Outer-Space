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
    public class Slider
    {
        public Texture2D Texture { get; set; }
        public Texture2D SlideTexture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 SlidePosition { get; set; }
        public Rectangle SlideBox { get { return new Rectangle((int)SlidePosition.X - SlideTexture.Width / 2, (int)SlidePosition.Y - SlideTexture.Height / 2, SlideTexture.Width, SlideTexture.Height); } }
        public float MaxValue { get; set; }
        public float Value { get; set; }
        public string Name { get; set; }

        private SpriteFont spriteFont;
        private bool active;
        private bool playSound;

        public Slider(Texture2D texture, Texture2D slideTexture, Vector2 position, float maxValue, float value, SpriteFont spriteFont, string name, bool playSound)
        {
            this.Texture = texture;
            this.SlideTexture = slideTexture;
            this.Position = position;
            this.MaxValue = maxValue;
            this.Value = value;
            this.spriteFont = spriteFont;
            this.Name = name;
            this.playSound = playSound;
            this.SlidePosition = new Vector2((Position.X - Texture.Width / 2) + (int)((Value / MaxValue) * Texture.Width), Position.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(Texture.Width / 2, Texture.Height / 2), 1f, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(SlideTexture, SlidePosition, null, Color.White, 0f, new Vector2(SlideTexture.Width / 2, SlideTexture.Height / 2), 1f, SpriteEffects.None, 0f);

            spriteBatch.DrawString(spriteFont, Name + ": " + Value.ToString(), new Vector2(Position.X - Texture.Width / 2 - spriteFont.MeasureString(Name + ": ").X - 50, Position.Y), Color.White, 0f, new Vector2(0, spriteFont.MeasureString(Name + ": " + Value.ToString()).Y / 2), 1f, SpriteEffects.None, 0f);
        }

        public void Update()
        {
            Value = (SlidePosition.X + Texture.Width / 2 - Position.X) / MaxValue * Texture.Width;

            if (Globals.MRectangle.Intersects(SlideBox) && Globals.PrevMState.LeftButton == ButtonState.Released && Camera.ChangeSceneDelay < -10)
            {
                active = true;
                if (playSound && Globals.MState.LeftButton == ButtonState.Pressed)
                {
                    SoundManager.click.Play();
                }
            }

            if (Globals.MState.LeftButton == ButtonState.Pressed && active)
            {
                SlidePosition = new Vector2(MathHelper.Clamp(Globals.MState.X, Position.X - Texture.Width / 2, Position.X + Texture.Width / 2), Position.Y);
            }
            else
            {
                active = false;
            }
        }
    }
}
