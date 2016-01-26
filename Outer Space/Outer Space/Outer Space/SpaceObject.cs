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
    class SpaceObject : GameObject
    {
        public SpaceObject(Texture2D texture, Vector2 position, float size)
        {
            this.Texture = texture;
            this.Position = position;
            this.Size = size;
        }
    }
}
