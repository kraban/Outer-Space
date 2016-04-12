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
    public class TextureButton : GameObject
    {
        private List<Piece> pieces;
        private int exploded;

        public TextureButton(Vector2 position, Texture2D texture)
            : base()
        {
            this.Position = position;
            this.Texture = texture;

            this.pieces = new List<Piece>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (exploded <= 0)
            {
                base.Draw(spriteBatch);
            }

            foreach (Piece p in pieces)
            {
                p.Draw(spriteBatch);
            }
        }

        public bool HoverOver()
        {
            if (Globals.MRectangle.Intersects(Box))
            {
                return true;
            }
            return false;
        }

        public bool Pressed()
        {
            if (Globals.MRectangle.Intersects(Box) && Globals.MState.LeftButton == ButtonState.Pressed && Globals.PrevMState.LeftButton == ButtonState.Released && exploded <= 0 && Camera.ChangeSceneDelay < -10)
            {
                Explode();
                SoundManager.click.Play();
                return true;
            }
            return false;
        }

        public override void Update()
        {
            base.Update();

            // Hover over
            if (exploded <= 0)
            {
                if (Globals.MRectangle.Intersects(Box))
                {
                    Size = MathHelper.Lerp(Size, 2, 0.1f);
                }
                else
                {
                    Size = MathHelper.Lerp(Size, 1, 0.1f);
                }
            }

            exploded--;
            foreach (Piece p in pieces)
            {
                p.Update();
            }

            for (int i = pieces.Count - 1; i >= 0; i--)
            {
                if (pieces[i].Dead)
                {
                    pieces.RemoveAt(i);
                }
            }
        }

        public void Explode()
        {
            exploded = 60;
            for (int i = 0; i < Globals.Randomizer.Next(5, 10); i++)
            {
                pieces.Add(new Piece(new Vector2(Globals.Randomizer.Next((int)Position.X - 20, (int)Position.X + 20), Globals.Randomizer.Next((int)Position.Y - 20, (int)Position.Y + 20)), Texture, 60, Size));
            }
        }
    }
}
