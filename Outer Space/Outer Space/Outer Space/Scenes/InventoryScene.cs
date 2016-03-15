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
    public class InventoryScene : Scene
    {
        public Button Back { get; set; }

        public InfoButton InventoryInfo { get; set; }
        public InfoButton CraftingInfo { get; set; }

        public InventoryScene()
        {
            this.Back = new Button(new Vector2(Globals.ScreenSize.X - 200, 32), "Back", TextureManager.SpriteFont20);

            string[] info = new string[3];
            info[0] = "This is your inventory.";
            info[1] = "This is where all your items are stored.";
            info[2] = "Equip your ship with modules from your inventory to use in combat.";
            InventoryInfo = new InfoButton(new Vector2(20, 20), info);
            info = new string[3];
            info[0] = "This is the crafting interface.";
            info[1] = "Put three modules of your choice in the top slots.";
            info[2] = "Press craft, and the three modules will convert to a new random one.";
            CraftingInfo = new InfoButton(new Vector2(935, 117), info);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            Back.Draw(spriteBatch);

            InventoryInfo.Draw(spriteBatch);
            CraftingInfo.Draw(spriteBatch);

            SceneManager.mapScene.ThePlayer.DrawInventory(spriteBatch);
        }

        public override void Update()
        {
            base.Update();

            SceneManager.mapScene.ThePlayer.UpdateInventory();

            Back.Update();
            if (Back.Press())
            {
                SceneManager.ChangeScene(SceneManager.mapScene);
            }

            InventoryInfo.Update();
            CraftingInfo.Update();
        }
    }
}
