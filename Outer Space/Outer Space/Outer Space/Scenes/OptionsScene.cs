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

        public Slider StarChanceSlider { get; set; }

        public OptionsScene()
        {
            this.Back = new Button(new Vector2(200, 200), "Back", TextureManager.SpriteFont20);

            this.StarChanceSlider = new Slider(TextureManager.slider, TextureManager.slideButton, new Vector2(300, 300), 100, 10, TextureManager.SpriteFont20, "Star Chance");
        }

        public override void Update()
        {
            base.Update();

            Back.Update();
            if (Back.Press())
            {
                SceneManager.ChangeScene(SceneManager.menuScene);
            }

            StarChanceSlider.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Back.Draw(spriteBatch);

            StarChanceSlider.Draw(spriteBatch);
        }
    }
}
