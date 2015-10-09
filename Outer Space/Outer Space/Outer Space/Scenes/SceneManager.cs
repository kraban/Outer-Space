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
        public static Scene CurrentScene { get; private set; }
        private static Scene changeTo;

        private static int changeSceneTimer;

        public static MenuScene menuScene { get; set; }
        public static OptionsScene optionsScene { get; set; }
        public static ShipSelectScene shipSelectScene { get; set; }

        public static void Initialize()
        {
            menuScene = new MenuScene();
            optionsScene = new OptionsScene();
            shipSelectScene = new ShipSelectScene();

            CurrentScene = menuScene;
            changeTo = menuScene;

            changeSceneTimer = 40;
        }

        public static void Update()
        {
            changeSceneTimer--;
            if (changeSceneTimer <= 0)
            {
                CurrentScene = changeTo;
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
