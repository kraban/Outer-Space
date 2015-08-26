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
    class Level
    {
        // Public properties
        public List<List<Tile>> Tiles { get; set; }
        public Tile Selected { get; set; }

        // Constructor(s)
        public Level()
        {
            this.Tiles = new List<List<Tile>>();
        }

        // Method(s)
        public void InitializeTiles()
        {
            for (int i = 0; i < 7; i++)
            {
                Tiles.Add(new List<Tile>()); 
                for (int j = 0; j < 7; j++)
                {
                    // List with avalible tileTypes
                    List<int> avalibleTileType = new List<int>();
                    for (int k = 0; k < Enum.GetNames(typeof(TileType)).Length; k++)
                    {
                        avalibleTileType.Add(k);
                    }

                    // Check if 2 left is same
                    if (j > 1)
                    {
                        if (Tiles[i][j - 1].Type == Tiles[i][j - 2].Type)
                        {
                            avalibleTileType.Remove((int)Tiles[i][j - 1].Type);
                        }
                    }

                    // Check if 2 up is same
                    if (i > 1)
                    {
                        if (Tiles[i - 1][j].Type == Tiles[i - 2][j].Type)
                        {
                            if (avalibleTileType.Contains((int)Tiles[i - 1][j].Type))
                            {
                                avalibleTileType.Remove((int)Tiles[i - 1][j].Type);
                            }
                        }
                    }

                    // Add tile
                    Tiles[i].Add(new Tile(new Point(i, j), (TileType)avalibleTileType[Globals.Randomizer.Next(0, avalibleTileType.Count)]));
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw Tiles
            for (int i = 0; i < Tiles.Count; i++)
            {
                for (int j = 0; j < Tiles[i].Count; j++)
                {
                    Tiles[i][j].Draw(spriteBatch);
                }
            }
        }

        public void Update()
        {
            // Update tiles
            for (int i = 0; i < Tiles.Count; i++)
            {
                for (int j = 0; j < Tiles[i].Count; j++)
                {
                    Tiles[i][j].Update();
                    Tiles[i][j].TilePosition = new Point(i, j);

                    // Select
                    if (Globals.MState.LeftButton == ButtonState.Pressed && Globals.PrevMState.LeftButton == ButtonState.Released && Globals.MRectangle.Intersects(Tiles[i][j].Box))
                    {
                        if (Selected == null)
                        {
                            Selected = Tiles[i][j];
                            Tiles[i][j].color = Color.Red;
                        }
                        else if (Selected == Tiles[i][j])
                        {
                            Selected = null;
                        }
                        else
                        {
                            // Check if pressed tile next to selected
                            if (Tiles[i][j] == Tiles[Selected.TilePosition.X - 1][Selected.TilePosition.Y] || Tiles[i][j] == Tiles[Selected.TilePosition.X + 1][Selected.TilePosition.Y] ||
                                Tiles[i][j] == Tiles[Selected.TilePosition.X][Selected.TilePosition.Y + 1] || Tiles[i][j] == Tiles[Selected.TilePosition.X][Selected.TilePosition.Y - 1])
                            {
                                Tiles[i][j].color = Color.White;
                                Tiles[Selected.TilePosition.X][Selected.TilePosition.Y].color = Color.White;

                                Tile temp = Tiles[i][j];
                                Tiles[i][j] = Selected;
                                Tiles[Selected.TilePosition.X][Selected.TilePosition.Y] = temp;
                                Selected = null;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
