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
using System.IO;

namespace Outer_Space
{
    class Options
    {
        public static float StarChance { get { return SceneManager.optionsScene.StarChanceSlider.Value; } }
        public static float SoundVolume { get { return SceneManager.optionsScene.SoundVolumeSlider.Value; } }
        public static float MusicVolume { get { return SceneManager.optionsScene.MusicVolumeSlider.Value; } }

        public static void SaveOptions()
        {
            // Create if not existing
            if (!File.Exists("Options.txt"))
            {
                File.Create("Options.txt").Close();
            }

            // Write options to file
            using (StreamWriter sw = new StreamWriter("Options.txt"))
            {
                sw.WriteLine(StarChance);
                sw.WriteLine(SoundVolume);
                sw.WriteLine(MusicVolume);
            }
        }

        public static List<int> GetOptions()
        {
            List<int> options = new List<int>();

            // Check if file exists
            if (File.Exists("Options.txt"))
            {
                // Read file
                using (StreamReader sr = new StreamReader("Options.txt"))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        options.Add(int.Parse(sr.ReadLine()));
                    }
                }
            }
            else // add default values if file does not exist
            {
                for (int i = 0; i < 3; i++)
                {
                    options.Add(10);
                }
            }
            return options;
        }
    }
}
