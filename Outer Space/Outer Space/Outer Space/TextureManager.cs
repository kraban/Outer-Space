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
    class TextureManager
    {
        public static Texture2D
            player,
            none,
            selected,
            shot;

        public static List<Texture2D> tiles;


        public static SpriteFont SpriteFont20 { get; set; }

        // Initialize
        public static void Initialize(ContentManager content)
        {
            player = content.Load<Texture2D>("Ship");
            none = content.Load<Texture2D>("Tiles/None");
            selected = content.Load<Texture2D>("Selected");
            shot = content.Load<Texture2D>("ShotRed");

            // SpriteFonts
            SpriteFont20 = content.Load<SpriteFont>("SpriteFont20");

            // Tiles
            tiles = new List<Texture2D>();
            tiles.Add(content.Load<Texture2D>("Tiles/Shield"));
            tiles.Add(content.Load<Texture2D>("Tiles/Right"));
            tiles.Add(content.Load<Texture2D>("Tiles/Left"));
            tiles.Add(content.Load<Texture2D>("Tiles/Cog"));
            tiles.Add(content.Load<Texture2D>("Tiles/Shot"));
        }
    }
}
