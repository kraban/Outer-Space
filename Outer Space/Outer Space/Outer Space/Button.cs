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
    public class Button
    {
        public Vector2 Position { get; set; }
        public float Offset { get; set; }
        public String Write { get; set; }
        public SpriteFont spriteFont { get; set; }
        public Rectangle Box { get { return new Rectangle((int)Position.X, (int)Position.Y - (int)spriteFont.MeasureString(Write).Y / 2, (int)spriteFont.MeasureString(Write).X, (int)spriteFont.MeasureString(Write).Y); } }
        public Color NormalColor { get; set; }
        public Color HoverColor { get; set; }
        public Color TextColor { get; set; }

        // Explode
        private List<Text> characters;
        private int exploded;
        private float size;

        public Button(Vector2 position, string text, SpriteFont spriteFont)
        {
            this.Position = position;
            this.Write = text;
            this.spriteFont = spriteFont;
            this.NormalColor = new Color(255, 255, 255);
            this.TextColor = NormalColor;
            this.HoverColor = new Color(0, 255, 255);
            this.size = 1f;
            this.characters = new List<Text>();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (exploded <= 0)
            {
                spriteBatch.DrawString(spriteFont, Write, new Vector2(Position.X + Offset, Position.Y), TextColor, 0f, new Vector2(0, spriteFont.MeasureString(Write).Y / 2), size, SpriteEffects.None, 0f);
            }

            // Explode
            foreach (Text text in characters)
            {
                text.Draw(spriteBatch);
            }
        }

        public void Update()
        {
            if (exploded <= 0)
            {
                if (Hover())
                {
                    Offset = MathHelper.Lerp(Offset, 20, 0.1f);
                    size = MathHelper.Lerp(size, 1.5f, 0.1f);
                    TextColor = new Color((int)MathHelper.Lerp(TextColor.R, HoverColor.R, 0.1f), (int)MathHelper.Lerp(TextColor.G, HoverColor.G, 0.1f), (int)MathHelper.Lerp(TextColor.B, HoverColor.B, 0.1f));
                }
                else
                {
                    Offset = MathHelper.Lerp(0, Offset, 0.9f);
                    size = MathHelper.Lerp(1, size, 0.9f);
                    TextColor = new Color((int)MathHelper.Lerp(TextColor.R, NormalColor.R, 0.1f), (int)MathHelper.Lerp(TextColor.G, NormalColor.G, 0.1f), (int)MathHelper.Lerp(TextColor.B, NormalColor.B, 0.1f));
                } 
            }

            // Explode
            exploded--;
            foreach (Text text in characters)
            {
                text.Update();
                text.Direction += MathHelper.Lerp(0, 0.3f, (float)Globals.Randomizer.NextDouble());
            }

            for (int i = characters.Count - 1; i >= 0; i--)
            {
                if (characters[i].Dead)
                {
                    characters.RemoveAt(i);
                }
            }
        }

        public void Explode()
        {
            exploded = 60;
            for (int i = 0; i < Write.Length; i++)
            {
                characters.Add(new Text(new Vector2(Position.X + Offset + (spriteFont.MeasureString(Write[i].ToString()).X * (i + 0.5f)), Position.Y + (spriteFont.MeasureString(Write).Y) / 2), Write[i].ToString(), HoverColor, exploded, 1.4f, false, TextureManager.SpriteFont20));
                characters.Last().Speed = MathHelper.Lerp(2, 4, (float)Globals.Randomizer.NextDouble());
                characters.Last().MoveDirection = MathHelper.Lerp(0, (float)Math.PI * 2, (float)Globals.Randomizer.NextDouble());
            }
        }

        public bool Hover()
        {
            if (Globals.MRectangle.Intersects(Box))
            {
                return true;
            }
            return false;
        }

        public bool Press()
        {
            if (Globals.MRectangle.Intersects(Box) && Globals.MState.LeftButton == ButtonState.Pressed && Globals.PrevMState.LeftButton == ButtonState.Released && exploded <= 0)
            {
                Explode();
                return true;
            }
            return false;
        }
    }
}
