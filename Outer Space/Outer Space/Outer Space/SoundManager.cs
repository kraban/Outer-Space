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
using System.Diagnostics;

namespace Outer_Space
{
    class SoundManager
    {
        private static bool fadeMusic;
        private static bool flipFade;
        private static Song changeTo;

        public static SoundEffect
            click,
            explosion,
            shoot,
            match,
            die,
            notEnoughEnergy,
            craft,
            swapItem,
            victory,
            bossChargeShot,
            bossMine,
            bossChargeAttack,
            bossTeleport;

        public static Song
            menu,
            combat,
            boss,
            map;

        public static void Initialize(ContentManager content)
        {
            click = content.Load<SoundEffect>("SoundEffects/Blip");
            explosion = content.Load<SoundEffect>("SoundEffects/Explosion");
            shoot = content.Load<SoundEffect>("SoundEffects/Shoot");
            match = content.Load<SoundEffect>("SoundEffects/Match");
            die = content.Load<SoundEffect>("SoundEffects/Die");
            notEnoughEnergy = content.Load<SoundEffect>("SoundEffects/NotEnoughEnergy");
            craft = content.Load<SoundEffect>("SoundEffects/Craft");
            swapItem = content.Load<SoundEffect>("SoundEffects/SwapItem");
            victory = content.Load<SoundEffect>("SoundEffects/Victory");
            bossChargeShot = content.Load<SoundEffect>("SoundEffects/BossChargeShot");
            bossMine = content.Load<SoundEffect>("SoundEffects/BossMine");
            bossChargeAttack = content.Load<SoundEffect>("SoundEffects/BossChargeAttack");
            bossTeleport = content.Load<SoundEffect>("SoundEffects/BossTeleport");

            menu = content.Load<Song>("Music/Corrupted");
            combat = content.Load<Song>("Music/Combat");
            boss = content.Load<Song>("Music/Fly");
            map = content.Load<Song>("Music/Map");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(menu);
        }

        public static void Update()
        {
            SoundEffect.MasterVolume = Options.SoundVolume / 100f;
            if (fadeMusic == false)
            {
                MediaPlayer.Volume = Options.MusicVolume / 100f;
            }

            if (fadeMusic)
            {
                if (flipFade == false)
                {
                    MediaPlayer.Volume = MathHelper.Lerp(MediaPlayer.Volume, 0f, 0.03f);
                    if (MediaPlayer.Volume < 0.001)
                    {
                        flipFade = true;
                        MediaPlayer.Play(changeTo);
                    }
                }
                else
                {
                    MediaPlayer.Volume = MathHelper.Lerp(MediaPlayer.Volume, Options.MusicVolume / 100f, 0.02f);
                    if (Options.SoundVolume / 100f - MediaPlayer.Volume < 0.001)
                    {
                        fadeMusic = false;
                    }
                }
            }
        }

        public static void ChangeMusic(Song song)
        {
            fadeMusic = true;
            changeTo = song;
            flipFade = false;
        }
    }
}
