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
    public class SpaceObject : GameObject
    {
        private Modifier modifier;
        public SpaceObject(Texture2D texture, Vector2 position, float size, Modifier modifier)
        {
            this.Texture = texture;
            this.Position = position;
            this.Size = size;
            this.Direction = MathHelper.Lerp(0, (float)Math.PI * 2, (float)Globals.Randomizer.NextDouble());
            this.modifier = modifier;
        }

        public override void Update()
        {
            base.Update();
            if (modifier == Modifier.BlackHole)
            {
                Direction += 0.01f;
            }
            else if (modifier == Modifier.Satellite)
            {
                Direction += 0.005f;
                Position += new Vector2(0.3f * (float)Math.Cos(Direction), 0.3f * (float)Math.Sin(Direction));

                foreach (Level level in SceneManager.mapScene.Levels)
                {
                    if (level.Distance(Position) < 100 && !level.HasModifier(Modifier.Satellite))
                    {
                        level.LevelModifiers.Add(Modifier.Satellite);
                    }
                    else if (level.HasModifier(Modifier.Satellite) && level.Distance(Position) < 105 && level.Distance(Position) > 100)
                    {
                        level.LevelModifiers.Remove(Modifier.Satellite);
                    }
                }
            }
        }
    }
}
