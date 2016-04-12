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
    public class OptionsScene : Scene
    {
        public Button Back { get; set; }
        public Button Controls { get; set; }

        public Button TutorialTips { get; set; }
        public bool TipsEnable { get; set; }

        public Slider StarChanceSlider { get; set; }
        
        // Sound
        public Slider SoundVolumeSlider { get; set; }
        public Slider MusicVolumeSlider { get; set; }

        public OptionsScene()
        {
            this.Back = new Button(new Vector2(200, 200), "Back", TextureManager.SpriteFont20);
            this.Controls = new Button(new Vector2(200, 250), "Controls", TextureManager.SpriteFont20);
            this.TipsEnable = Convert.ToBoolean(Options.GetOptions()[3]);
            this.TutorialTips = new Button(new Vector2(600, 500), "Tips: " + TipsEnable.ToString(), TextureManager.SpriteFont20);
            TutorialTips.NormalColor = TipsEnable ? Color.Green : Color.Red;

            this.StarChanceSlider = new Slider(TextureManager.slider, TextureManager.slideButton, new Vector2(900, 200), 100, Options.GetOptions()[0], TextureManager.SpriteFont20, "Star Chance", false);
            this.SoundVolumeSlider = new Slider(TextureManager.slider, TextureManager.slideButton, new Vector2(900, 300), 100, Options.GetOptions()[1], TextureManager.SpriteFont20, "Sound Volume", true);
            this.MusicVolumeSlider = new Slider(TextureManager.slider, TextureManager.slideButton, new Vector2(900, 400), 100, Options.GetOptions()[2], TextureManager.SpriteFont20, "Music Volume", false);
        }

        public override void Update()
        {
            base.Update();

            Back.Update();
            if (Back.Press())
            {
                Options.SaveOptions();
                SceneManager.ChangeScene(SceneManager.menuScene);
            }

            Controls.Update();
            if (Controls.Press())
            {
                SceneManager.ChangeScene(SceneManager.controlScene);
            }

            TutorialTips.Update();
            if (TutorialTips.Press())
            {
                TipsEnable = !TipsEnable;
                TutorialTips.Write = "Tips: " + TipsEnable.ToString();
                TutorialTips.NormalColor = TipsEnable ? Color.Green : Color.Red;
            }

            StarChanceSlider.Update();
            SoundVolumeSlider.Update();
            MusicVolumeSlider.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Back.Draw(spriteBatch);
            Controls.Draw(spriteBatch);
            TutorialTips.Draw(spriteBatch);

            StarChanceSlider.Draw(spriteBatch);
            SoundVolumeSlider.Draw(spriteBatch);
            MusicVolumeSlider.Draw(spriteBatch);
        }
    }
}
