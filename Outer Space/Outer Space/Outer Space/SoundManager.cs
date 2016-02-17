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
    class SoundManager
    {
        public static SoundEffect
            fire;

        public static void Initialize(ContentManager content)
        {
            fire = content.Load<SoundEffect>("SoundEffects/SoundDing");

        }

        public static void Update()
        {
            SoundEffect.MasterVolume = Options.SoundVolume / 100f;
            MediaPlayer.Volume = Options.MusicVolume / 100f;
        }
    }
}
