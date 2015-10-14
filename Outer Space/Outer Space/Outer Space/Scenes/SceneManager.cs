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
    public static class SceneManager
    {
        public static List<Star> Stars { get; set; }

        public static Scene CurrentScene { get; private set; }
        private static Scene changeTo;

        private static int changeSceneTimer;

        public static MenuScene menuScene { get; set; }
        public static OptionsScene optionsScene { get; set; }
        public static ShipSelectScene shipSelectScene { get; set; }
        public static InventoryScene inventoryScene { get; set; }

        public static void Initialize()
        {
            menuScene = new MenuScene();
            optionsScene = new OptionsScene();
            shipSelectScene = new ShipSelectScene();
            inventoryScene = new InventoryScene();

            CurrentScene = menuScene;
            changeTo = menuScene;

            changeSceneTimer = 40;

            Stars = new List<Star>();
            for (int i = 0; i < Globals.Randomizer.Next(40, 50); i++)
            {
                Stars.Add(new Star(new Vector2(Globals.Randomizer.Next(5, Globals.ScreenSize.X - 5), Globals.Randomizer.Next(5, Globals.ScreenSize.Y * 2))));
            }
        }

        public static void Update()
        {
            changeSceneTimer--;
            if (changeSceneTimer <= 0)
            {
                CurrentScene = changeTo;
            }

            // Stars
            if (Globals.Randomizer.Next(0, 101) < Options.StarChance)
            {
                Stars.Add(new Star(new Vector2(-5, Globals.Randomizer.Next(5, Globals.ScreenSize.Y * 2))));
            }

            foreach (Star s in Stars)
            {
                s.Update();
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (Star s in Stars)
            {
                s.Draw(spriteBatch);
            }
        }

        public static void ChangeScene(Scene scene)
        {
            Camera.ChangeSceneDelay = 30;
            if (changeTo != scene)
            {
                changeTo = scene;
                changeSceneTimer--;
                changeSceneTimer = 70;
            }
        }
    }
}
