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
    public class Level
    {
        // Public properties
        public List<List<Tile>> Tiles { get; set; }
        public Tile Selected { get; set; }
        public Point BoardSize { get { return new Point(8, 8); } }
        public List<GameObject> GameObjects { get; set; }
        public List<GameObject> ToAdd { get; set; }

        // Constructor(s)
        public Level()
        {
            this.Tiles = new List<List<Tile>>();
            this.GameObjects = new List<GameObject>();
            GameObjects.Add(new Player());
            GameObjects.Add(new Enemy());
            ToAdd = new List<GameObject>();
        }

        // Method(s)
        public void InitializeTiles()
        {
            for (int i = 0; i < BoardSize.X; i++)
            {
                Tiles.Add(new List<Tile>()); 
                for (int j = 0; j < BoardSize.Y; j++)
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

            // Selected
            if (Selected != null)
            {
                spriteBatch.Draw(TextureManager.selected, Selected.Position, null, Color.White, 0f, new Vector2(TextureManager.selected.Width / 2, TextureManager.selected.Height / 2), 1f, SpriteEffects.None, 0f); 
            }

            // Game objects
            foreach (GameObject go in GameObjects)
            {
                go.Draw(spriteBatch);
            }
        }

        public void CheckMatch()
        {
            for (int i = 0; i < BoardSize.X; i++)
            {
                for (int j = 0; j < BoardSize.Y; j++)
                {
                    // Check if down is same
                    if (j < BoardSize.Y - 1)
                    {
                        int number = 1;
                        while (Tiles[i][j].Type == Tiles[i][j + number].Type && !Tiles[i][j + number].Hidden && !Tiles[i][j].Hidden && !Tiles[i][j + number].Moving && !Tiles[i][j].Moving)
                        {
                            number++;
                            if (j + number >= BoardSize.Y)
                            {
                                break;
                            }
                        }

                        if (number >= 3)
                        {
                            // Player action
                            Player player = (Player)GameObjects.First(item => item.GetType().Name == "Player");
                            player.Action(number, Tiles[i][j].Type, this);

                            for (int k = 0; k < number; k++)
                            {
                                Tiles[i][j + k].Hide();

                                // Create pieces and text
                                if (k == 0)
                                {
                                    GameObjects.Add(new Text(new Vector2(Tiles[i][j].Position.X, Tiles[i][j].Position.Y + (number * 64) / 2 - 32), Tiles[i][j + k].Type.ToString(), Color.Red, 120, false, 2));
                                }
                                Explosion(i, j + k);
                            }
                        }
                    }

                    // Check if right is same
                    if (i < BoardSize.X - 1 && !Tiles[i][j].Hidden)
                    {
                        int number = 1;
                        while (Tiles[i][j].Type == Tiles[i + number][j].Type && !Tiles[i + number][j].Hidden && !Tiles[i][j].Hidden && !Tiles[i + number][j].Moving && !Tiles[i][j].Moving)
                        {
                            number++;
                            if (i + number >= BoardSize.X)
                            {
                                break;
                            }
                        }

                        if (number >= 3)
                        {
                            // Player action
                            Player player = (Player)GameObjects.First(item => item.GetType().Name == "Player");
                            player.Action(number, Tiles[i][j].Type, this);

                            for (int k = 0; k < number; k++)
                            {
                                Tiles[i + k][j].Hide();

                                // Create pieces and text
                                if (k == 0)
                                {
                                    GameObjects.Add(new Text(new Vector2(Tiles[i][j].Position.X + (number * 64) / 2 - 32, Tiles[i][j].Position.Y), Tiles[i + k][j].Type.ToString(), Color.Red, 120, false, 2));
                                }
                                Explosion(i + k, j);
                            }
                        }
                    }
                }
            }
        }

        public bool CheckSingleMatch(int x, int y)
        {
            int numberDown = 0, numberUp = 0, numberRight = 0, numberLeft = 0;
            // Check if down is same
            if (y < BoardSize.Y - 1)
            {
                while (Tiles[x][y].Type == Tiles[x][y + numberDown + 1].Type && !Tiles[x][y + numberDown + 1].Hidden)
                {
                    numberDown++;
                    if (y + numberDown + 1 >= BoardSize.Y)
                    {
                        break;
                    }
                }

                if (numberDown >= 2)
                {
                    return true;
                }
            }
            // Check if Up is same
            if (y > 0)
            {
                while (Tiles[x][y].Type == Tiles[x][y - numberUp - 1].Type && !Tiles[x][y - numberUp - 1].Hidden)
                {
                    numberUp++;
                    if (y - numberUp <= 0)
                    {
                        break;
                    }
                }

                if (numberUp + numberDown >= 2)
                {
                    return true;
                }
            }

            // Check if Right is same
            if (x < BoardSize.X - 1)
            {
                while (Tiles[x][y].Type == Tiles[x + numberRight + 1][y].Type && !Tiles[x + numberRight + 1][y].Hidden)
                {
                    numberRight++;
                    if (x + numberRight + 1 >= BoardSize.X)
                    {
                        break;
                    }
                }

                if (numberRight >= 2)
                {
                    return true;
                }
            }
            // Check if Left is same
            if (x > 0)
            {
                while (Tiles[x][y].Type == Tiles[x - numberLeft - 1][y].Type && !Tiles[x - numberLeft - 1][y].Hidden)
                {
                    numberLeft++;
                    if (x - numberLeft <= 0)
                    {
                        break;
                    }
                }

                if (numberLeft + numberRight >= 2)
                {
                    return true;
                }
            }
            return false;
        }

        public void Explosion(int x, int y)
        {
            for (int i = 0; i < Globals.Randomizer.Next(10, 15); i++)
            {
                GameObjects.Add(new Piece(new Vector2(Tiles[x][y].Position.X + Globals.Randomizer.Next(-20, 20), Tiles[x][y].Position.Y + Globals.Randomizer.Next(-20, 20)), Tiles[x][y].Texture));
            }
        }

        public bool CheckAdjacent(int x, int y)
        {
            // Tiles to check
            List<Point> adjacentTiles;
            adjacentTiles = new List<Point>();

            // add adjacent tiles to check if not on boarder
            if (Selected.TilePosition.X != 0)
            {
                adjacentTiles.Add(new Point(-1, 0));
            }
            if (Selected.TilePosition.X != BoardSize.X - 1)
            {
                adjacentTiles.Add(new Point(1, 0));
            }
            if (Selected.TilePosition.Y != 0)
            {
                adjacentTiles.Add(new Point(0, -1));
            }
            if (Selected.TilePosition.Y != BoardSize.Y - 1)
            {
                adjacentTiles.Add(new Point(0, 1));
            }

            // Check
            for (int i = 0; i < adjacentTiles.Count; i++)
            {
                if (Tiles[x][y] == Tiles[Selected.TilePosition.X + adjacentTiles[i].X][Selected.TilePosition.Y + adjacentTiles[i].Y])
                {
                    return true; 
                }
            }
            return false;
        }

        public void Update()
        {
            // Update tiles
            bool canSelect = true;
            for (int i = 0; i < Tiles.Count; i++)
            {
                for (int j = 0; j < Tiles[i].Count; j++)
                {
                    Tiles[i][j].TilePosition = new Point(i, j);
                    Tiles[i][j].UpdateLevel(this);

                    if (!Tiles[i][j].Hidden)
                    {

                        // Above hidden
                        if (j < BoardSize.Y - 1 && Tiles[i][j + 1].Hidden && Tiles[i][j + 1].Size < 0.1)
                        {
                            Tile temp = Tiles[i][j];
                            Tiles[i][j] = Tiles[i][j + 1];
                            Tiles[i][j + 1] = temp;
                        }

                        // Select
                        if (canSelect && Globals.MState.LeftButton == ButtonState.Pressed && Globals.PrevMState.LeftButton == ButtonState.Released && Globals.MRectangle.Intersects(Tiles[i][j].Box) && !Tiles.Any(Column => Column.Any(item => item.Moving == true)))
                        {
                            canSelect = false;
                            if (Selected == null)
                            {
                                Selected = Tiles[i][j];
                            }
                            else if (Selected == Tiles[i][j])
                            {
                                Selected = null;
                            }
                            else
                            {
                                // Check if pressed tile next to selected
                                if (CheckAdjacent(i, j))
                                {
                                    Tile temp = Tiles[i][j];
                                    Tiles[i][j] = Selected;
                                    Tiles[i][j].Moving = true;
                                    Tiles[Selected.TilePosition.X][Selected.TilePosition.Y] = temp;
                                    Tiles[Selected.TilePosition.X][Selected.TilePosition.Y].Moving = true;

                                    // Check if swap is matching
                                    if (!CheckSingleMatch(i, j) && !CheckSingleMatch(Selected.TilePosition.X, Selected.TilePosition.Y))
                                    {
                                        Tiles[i][j] = Tiles[Selected.TilePosition.X][Selected.TilePosition.Y];
                                        Tiles[i][j].Moving = false;
                                        Tiles[Selected.TilePosition.X][Selected.TilePosition.Y] = Selected;
                                        Tiles[Selected.TilePosition.X][Selected.TilePosition.Y].Moving = false;
                                        GameObjects.Add(new Text(Tiles[i][j].Position - (Tiles[i][j].Position - Tiles[Selected.TilePosition.X][Selected.TilePosition.Y].Position), "Invalid Swap!", Color.Red, 60, false, 2));
                                    }
                                    Selected = null;
                                }
                                else
                                {
                                    Selected = Tiles[i][j];
                                }
                            }
                        } 
                    }
                    else if (j== 0)
                    {
                        // "New" tile if hidden on top layer
                        Tiles[i][j].UnHide();
                    }
                }
            }

            CheckMatch();

            // Game objects
            foreach (GameObject go in GameObjects)
            {
                go.UpdateLevel(this);
            }

            // Add
            foreach (GameObject toAdd in ToAdd)
            {
                GameObjects.Add(toAdd);
            }
            ToAdd.Clear();

            // Remove
            for (int i = GameObjects.Count - 1; i >= 0; i--)
            {
                if (GameObjects[i].Dead)
                {
                    GameObjects.RemoveAt(i);
                }
            }
        }
    }
}
