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
    public class TutorialScene : Scene
    {
        private Scene scene;
        private Button next;
        private int tutorialStage;

        public TutorialScene()
            : base()
        {
            this.scene = new ShipSelectScene();
            this.next = new Button(new Vector2(200, 200), "Next", TextureManager.SpriteFont20);
        }

        public override void Update()
        {
            base.Update();
            scene.Update();

            next.Update();

            if (tutorialStage == 0)
            {
                
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            scene.Draw(spriteBatch);

            next.Draw(spriteBatch);

            if (tutorialStage == 0)
            {
                spriteBatch.DrawString(TextureManager.SpriteFont15, "Welcome to the tutorial!\nChoose a ship to continue", new Vector2(200, 200), Color.White);
            }
        }
    }
}
