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
    public class RankScene : Scene
    {
        public Button Back { get; set; }

        public InfoButton RankInfo { get; set; }

        public RankScene()
            : base()
        {
            this.Back = new Button(new Vector2(200, 200), "Back", TextureManager.SpriteFont20);

            string[] info = new string[3];
            info[0] = "This is your Rank.";
            info[1] = "You gain rank by deafeating enemies.";
            info[2] = "When you increase in rank, you gain a random bonus.";
            RankInfo = new InfoButton(new Vector2(20, 20), info);
        }

        public override void Update()
        {
            base.Update();

            Back.Update();
            if (Back.Press())
            {
                SceneManager.ChangeScene(SceneManager.mapScene);
            }

            RankInfo.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Back.Draw(spriteBatch);
            SceneManager.mapScene.ThePlayer.DrawRank(spriteBatch);

            RankInfo.Draw(spriteBatch);
        }
    }
}
