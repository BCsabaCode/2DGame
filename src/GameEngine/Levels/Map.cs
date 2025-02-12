using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Graphics.Canvas;

using GameEngine.Graphics;
using GameEngine.Utilities;
using GameEngine.Inputs;
using GameEngine.Entities;
using GameEngine.Interfaces;
using Windows.Foundation;

namespace GameEngine.Levels
{
    public enum MapState
    {
        WALL,
        WALL_HIDEN,
        FLOOR,
        FLOOR_HIDEN,
        VOID,
        PLAYER
    }
    class Map : IUpdateable
    {
        public static readonly int tileSize = 32;
        readonly int _width;
        readonly int _height;
        readonly int[,] _floor;
        MapState[,] minimap = null;
        bool render_minimap = false;
        bool prev_state = KeyBoard.tab;
        bool player_moved = false;
        CanvasBitmap minimapImage = null;
        int playerX;
        int playerY;

        List<Sector> sectors = new List<Sector>();

        public Map(int width, int height, int[,] floor, string sector_data)
        {
            _width = width;
            _height = height;
            _floor = floor;

            LoadSectors(sector_data);
            Sector.SetMap(this);
        }

        public Map(int width, int height, int[,] floor)
        {
            _width = width;
            _height = height;
            _floor = floor;

            Sector.SetMap(this);
        }

        public void AddSector(Sector sector)
        {
            sectors.Add(sector);
        }

        public void SetMinimapData(MapState[,] data)
        {
            if (minimap == null) minimap = data;
            else throw new ArgumentException("Minimap already set");
        }

        void LoadSectors(string sector_data)
        {
            //TODO
        }

        public int GetWidth()
        {
            return _width;
        }

        public int GetHeight()
        {
            return _height;
        }

        public Tile GetTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= _width || y >= _height) return null;
            return Tile.GetTile(_floor[x, y]);
        }

        public void Finalise()
        {
            sectors.Sort();
        }

        public void Update()
        {
            bool now = KeyBoard.tab;
            //Toggle logc for Tab key
            if (now & !prev_state)
            {
                render_minimap = !render_minimap;
            } 
            prev_state = now;

            //If the minimap doesn't exists we force an update
            if (minimapImage == null && minimap != null) player_moved = true;

            //Updating minimap data
            if (player_moved && minimap != null)
            {
                int lx = playerX - 5;
                int ly = playerY - 5;
                int ux = playerX + 5;
                int uy = playerY + 5;
                if (lx < 0) lx = 0;
                if (ly < 0) ly = 0;
                if (ux > (_width + 2)) ux = _width + 2;
                if (uy > (_height + 2)) uy = _height + 2;

                for (int y = ly; y < uy;y++)
                {
                    for (int x = lx; x < ux; x++)
                    {
                        if (((playerX - x) * (playerX - x) + (playerY - y) * (playerY - y)) <= 25)
                        {
                            if (minimap[x, y] == MapState.FLOOR_HIDEN) minimap[x, y] = MapState.FLOOR;
                            if (minimap[x, y] == MapState.WALL_HIDEN) minimap[x, y] = MapState.WALL;
                            if (minimap[x, y] == MapState.PLAYER) minimap[x, y] = MapState.FLOOR; //In normal circumstances the player never leaves the floor
                        }
                    }
                }
                if (playerX >= 0 && playerX <= _width && playerY >= 0 && playerY <= _height)
                {
                    minimap[playerX + 1, playerY + 1] = MapState.PLAYER;
                }

                //If we are rendering minimap we also make new bitmap for it
                if (render_minimap) CreateNewMinimap();
            }
        }

        void CreateNewMinimap()
        {
            int width = _width + 2;
            int height = _height + 2;
            byte[] bitmap_bytes = new byte[width * height * 4];
            byte red = 0x00;
            byte green = 0x00;
            byte blue = 0x00;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++) //Increase by 4 because each color is 4 byte
                {
                    if (minimap[x,y] == MapState.FLOOR_HIDEN || minimap[x,y] == MapState.WALL_HIDEN || minimap[x,y] == MapState.VOID)
                    {
                        red = 0x80;
                        green = 0x80;
                        blue = 0x80;
                    } 
                    else if (minimap[x,y] == MapState.FLOOR) {
                        red = 0xA0;
                        green = 0xA0;
                        blue = 0xA0;
                    }
                    else if (minimap[x, y] == MapState.WALL)
                    {
                        red = 0x60;
                        green = 0x60;
                        blue = 0x60;
                    }
                    else if (minimap[x, y] == MapState.PLAYER)
                    {
                        red = 0xFF;
                        green = 0x00;
                        blue = 0x00;
                    }
                    bitmap_bytes[x * 4 + 0 + y * width * 4] = red;
                    bitmap_bytes[x * 4 + 1 + y * width * 4] = green;
                    bitmap_bytes[x * 4 + 2 + y * width * 4] = blue;
                    bitmap_bytes[x * 4 + 3 + y * width * 4] = 0x80; //Alpha
                }
            }
            minimapImage = Sprite.CreateBitmapFromBytes(bitmap_bytes, width, height);
        }

        public void Render(Vector2 playerXY, Screen screen)
        {
            //Offset to center screen
            Vector2 screenOffset = Coordinate.IsoToNormal(new Vector2((screen.GetWidth() / 2), (screen.GetHeight() / 2)));

            //Calculate offset
            int xScroll = (int)Math.Round(playerXY.X - screenOffset.X);
            int yScroll = (int)Math.Round(playerXY.Y - screenOffset.Y);

            //Set offset
            screen.SetOffset(xScroll, yScroll);

            //Setting render mode to isometric
            screen.SetRenderMode(RenderMode.Isometric);

            //Render tiles
            Sprite sprite;
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    //Render
                    sprite = Sprite.GetSprite(_floor[x, y]);
                    if (sprite != null)
                    {
                        screen.RenderRectangle(x * tileSize, y * tileSize, tileSize, sprite.GetBitmap());
                    }
                }
            }

            //Render sectors
            sectors.ForEach(sector => sector.Render(playerXY, screen));

            
            //Render minimap
            if (render_minimap & minimapImage != null)
            {
                screen.SetRenderMode(RenderMode.Normal2X);

                int x = (int)playerXY.X / tileSize - 125;
                int y = (int)playerXY.Y / tileSize - 125;
                int ux = 250;
                int uy = 250;
                if (x < 0)
                {
                    x = 0;
                }
                if (y < 0)
                {
                    y = 0;
                }
                if (x + 125 > _width + 2)
                {
                    x = _width + 2 - 250;
                }
                if (y + 125 > _height + 2)
                {
                    y = _height + 2 - 250;
                }
                Rect window = new Rect(x, y, ux, uy);
                screen.RenderMinimap(screen.GetWidth() / 4 - 250 / 2, screen.GetHeight() / 4 - 250 / 2, window, minimapImage);
            }
            int px = (int)playerXY.X / tileSize;
            int py = (int)playerXY.Y / tileSize;
            if (playerX != px || playerY != py) player_moved = true;
            else player_moved = false;
            playerX = px;
            playerY = py;
        }

        public void AddEntity(Entity entity)
        {
            int entityX = (int)entity.GetX() / tileSize;
            int entityY = (int)entity.GetY() / tileSize;

            foreach (Sector sector in sectors)
            {
                if (sector.IsInside(entityX, entityY))
                {
                    sector.AddEntity(entity);
                    break;
                }
            }
        }

        public void RemoveEntity(Entity entity)
        {
            sectors.ForEach(sector => sector.RemoveEntity(entity));
        }

        public void UpdateSectors(List<Entity> entities)
        {
            entities.ForEach(entity =>
            {
                bool inside = false;
                foreach (Sector sector in sectors)
                {
                    if (sector.Contains(entity))
                    {
                        inside = true;
                        int entityX = (int)entity.GetX() / tileSize;
                        int entityY = (int)entity.GetY() / tileSize;
                        if (!sector.IsInside(entityX, entityY)) {
                            sector.RemoveEntity(entity);
                            AddEntity(entity);
                        } 
                    }
                }
                if (!inside)
                {
                    AddEntity(entity);
                }
            });
        }
    }
}
