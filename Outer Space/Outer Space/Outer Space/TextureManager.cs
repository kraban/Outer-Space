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
            shot,
            pixel,
            rock,
            jammed,
            ship2,
            slider,
            slideButton;

        public static List<Texture2D> tiles;
        public static List<Texture2D> hulls;
        public static List<Texture2D> shields;
        public static List<Texture2D> weapons;


        public static SpriteFont SpriteFont20 { get; set; }
        public static SpriteFont SpriteFont15 { get; set; }

        // Initialize
        public static void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
        {
            player = content.Load<Texture2D>("Ship");
            none = content.Load<Texture2D>("None");
            selected = content.Load<Texture2D>("Selected");
            shot = content.Load<Texture2D>("ShotRed");
            rock = content.Load<Texture2D>("Rock");
            jammed = content.Load<Texture2D>("Jammed");
            ship2 = content.Load<Texture2D>("Ship2Game");
            slider = content.Load<Texture2D>("Slider");
            slideButton = content.Load<Texture2D>("SlideButton");

            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            // SpriteFonts
            SpriteFont20 = content.Load<SpriteFont>("SpriteFont20");
            SpriteFont15 = content.Load<SpriteFont>("SpriteFont15");

            // Tiles
            tiles = new List<Texture2D>();
            tiles.Add(content.Load<Texture2D>("Tiles/Shield"));
            tiles.Add(content.Load<Texture2D>("Tiles/Right"));
            tiles.Add(content.Load<Texture2D>("Tiles/Left"));
            tiles.Add(content.Load<Texture2D>("Tiles/Cog"));
            tiles.Add(content.Load<Texture2D>("Tiles/Shot"));

            // Hulls
            hulls = new List<Texture2D>();
            hulls.Add(content.Load<Texture2D>("Hulls/Hull1"));

            // Shields
            shields = new List<Texture2D>();
            shields.Add(content.Load<Texture2D>("Shields/Shield1"));

            // Weapons
            weapons = new List<Texture2D>();
            weapons.Add(content.Load<Texture2D>("Weapons/Weapon1"));
        }
    }
}
