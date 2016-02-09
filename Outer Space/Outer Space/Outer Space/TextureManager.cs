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
            ship1,
            none,
            selected,
            shot,
            pixel,
            rock,
            jammed,
            ship2,
            ship3,
            enemyShip,
            slider,
            slideButton,
            inventory,
            inventorySlot,
            level,
            sun,
            boss,
            bossForeground,
            enemyShipEngineAnimation,
            ship1EngineAnimation,
            ship2EngineAnimation,
            ship3EngineAnimation,
            bossEngineAnimation,
            wrench,
            flee,
            explosion,
            blackHole,
            satellite;

        public static List<Texture2D> tiles;
        public static List<Texture2D> hulls;
        public static List<Texture2D> shields;
        public static List<Texture2D> weapons;


        public static SpriteFont SpriteFont20 { get; set; }
        public static SpriteFont SpriteFont15 { get; set; }

        // Initialize
        public static void Initialize(ContentManager content, GraphicsDevice graphicsDevice)
        {
            ship1 = content.Load<Texture2D>("ShipTextures/Ship");
            none = content.Load<Texture2D>("None");
            selected = content.Load<Texture2D>("Selected");
            shot = content.Load<Texture2D>("Shot");
            rock = content.Load<Texture2D>("Rock");
            jammed = content.Load<Texture2D>("Jammed");
            ship2 = content.Load<Texture2D>("ShipTextures/Ship2Game");
            ship3 = content.Load<Texture2D>("ShipTextures/Ship3");
            enemyShip = content.Load<Texture2D>("ShipTextures/EnemyShip1");
            slider = content.Load<Texture2D>("Slider");
            slideButton = content.Load<Texture2D>("SlideButton");
            inventory = content.Load<Texture2D>("Inventory");
            inventorySlot = content.Load<Texture2D>("InventorySlot");
            level = content.Load<Texture2D>("Level");
            sun = content.Load<Texture2D>("SunBig");
            boss = content.Load<Texture2D>("ShipTextures/OuterSpaceBossCut");
            bossForeground = content.Load<Texture2D>("ShipTextures/OuterSpaceBossForeground");
            enemyShipEngineAnimation = content.Load<Texture2D>("ShipTextures/EnemyShipEngineAnimation");
            ship1EngineAnimation = content.Load<Texture2D>("ShipTextures/ship1EngineAnimation");
            ship2EngineAnimation = content.Load<Texture2D>("ShipTextures/ship2EngineAnimation");
            ship3EngineAnimation = content.Load<Texture2D>("ShipTextures/ship3EngineAnimation");
            bossEngineAnimation = content.Load<Texture2D>("ShipTextures/bossEngineAnimation");
            wrench = content.Load<Texture2D>("Wrench");
            flee = content.Load<Texture2D>("Flee");
            explosion = content.Load<Texture2D>("Explosion");
            blackHole = content.Load<Texture2D>("BlackHole");
            satellite = content.Load<Texture2D>("Satellite");

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
            shields.Add(content.Load<Texture2D>("Shields/ShieldRecolor"));

            // Weapons
            weapons = new List<Texture2D>();
            weapons.Add(content.Load<Texture2D>("Weapons/WeaponBackground"));
            weapons.Add(content.Load<Texture2D>("Weapons/WeaponForeground"));
        }
    }
}
