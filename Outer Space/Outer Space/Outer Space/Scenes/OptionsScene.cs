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

        public Slider StarChanceSlider { get; set; }
        
        // Sound
        public Slider SoundVolumeSlider { get; set; }
        public Slider MusicVolumeSlider { get; set; }

        public OptionsScene()
        {
            this.Back = new Button(new Vector2(200, 200), "Back", TextureManager.SpriteFont20);
            this.Controls = new Button(new Vector2(200, 250), "Controls", TextureManager.SpriteFont20);

            this.StarChanceSlider = new Slider(TextureManager.slider, TextureManager.slideButton, new Vector2(350, 300), 100, Options.GetOptions()[0], TextureManager.SpriteFont20, "Star Chance");
            this.SoundVolumeSlider = new Slider(TextureManager.slider, TextureManager.slideButton, new Vector2(350, 400), 100, Options.GetOptions()[1], TextureManager.SpriteFont20, "Sound Volume");
            this.MusicVolumeSlider = new Slider(TextureManager.slider, TextureManager.slideButton, new Vector2(350, 500), 100, Options.GetOptions()[2], TextureManager.SpriteFont20, "Music Volume");
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

            StarChanceSlider.Update();
            SoundVolumeSlider.Update();
            MusicVolumeSlider.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Back.Draw(spriteBatch);
            Controls.Draw(spriteBatch);

            StarChanceSlider.Draw(spriteBatch);
            SoundVolumeSlider.Draw(spriteBatch);
            MusicVolumeSlider.Draw(spriteBatch);
        }
    }
}
