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
        }

        public static void Update()
        {
            SoundEffect.MasterVolume = Options.SoundVolume / 100f;
            MediaPlayer.Volume = Options.MusicVolume / 100f;
        }
    }
}
