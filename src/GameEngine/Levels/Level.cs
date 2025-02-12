using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

using GameEngine.Interfaces;
using GameEngine.Entities;
using GameEngine.Graphics;
using GameEngine.Utilities;
using GameEngine.Entities.Mobs;
using GameEngine.Networking.Packets;

namespace GameEngine.Levels
{
    class Level : IUpdateable
    {
        Stopwatch watch = new Stopwatch();
        List<Entity> entities = new List<Entity>();
        List<Entity> active_entities = new List<Entity>();
        List<Player> players = new List<Player>();
        List<Entity> tmp = new List<Entity>();
        List<EntityCorrection> corrections = new List<EntityCorrection>();
        //protected Action mapLoading;

        protected Map map;

        /// <summary>
        /// Level inicializalas
        /// </summary>
        public void Init()
        {
            watch.Start();
        }

        public void AddCorrection(EntityCorrection correction)
        {
            corrections.Add(correction);
        }

        public void InitAStar()
        {
            int[,] graph_map = new int[map.GetWidth(), map.GetHeight()];
            for (int y = 0; y < map.GetHeight(); y++)
            {
                for (int x = 0; x < map.GetWidth(); x++)
                {
                    Tile tile = map.GetTile(x, y);
                    if (tile == null)
                    {
                        graph_map[x, y] = -1;
                        continue;
                    }
                    if (tile.IsSolid())
                        graph_map[x, y] = -1;
                    else
                        graph_map[x, y] = 1;
                }
            }
            AStar.Initialize(graph_map, map.GetWidth(), map.GetHeight());
        }

        public bool TilePenetration(Vector2 xy)
        {
            bool penetrateable = false;
            if (map == null) return penetrateable;
            int xFuture = (int)xy.X;
            int yFuture = (int)xy.Y;
            if (0 > xFuture && xFuture > -Map.tileSize)
            {
                xFuture = -1;
            }
            else
            {
                xFuture /= Map.tileSize;
            }
            if (0 > yFuture && yFuture > -Map.tileSize)
            {
                yFuture = -1;
            }
            else
            {
                yFuture /= Map.tileSize;
            }
            Tile tile = map.GetTile(xFuture, yFuture);
            if (tile == null) penetrateable = false;
            else if (tile.IsPenetrateable()) penetrateable = true;
            
            return penetrateable;
        }

        public bool TileCollisionForParticles(double x, double y)
        {
            bool penetrateable = false;
            if (map == null) return penetrateable;
            int xFuture = (int)x;
            int yFuture = (int)y;
            if (0 > xFuture && xFuture > -Map.tileSize)
            {
                xFuture = -1;
            }
            else
            {
                xFuture /= Map.tileSize;
            }
            if (0 > yFuture && yFuture > -Map.tileSize)
            {
                yFuture = -1;
            }
            else
            {
                yFuture /= Map.tileSize;
            }
            Tile tile = map.GetTile(xFuture, yFuture);
            if (tile == null) penetrateable = false;
            else if (tile.IsPenetrateable()) penetrateable = true;
            return penetrateable;
        }

        public List<Mob> GetMobs()
        {
            List<Mob> mobs = new List<Mob>();
            entities.ForEach(entity =>
            {
                if (entity is Mob m) mobs.Add(m);
            });
            /*Debug.WriteLine("entity count: " + entities.Count);
            mobs = entities.FindAll(e => e is Mob).ConvertAll(e => (Mob)e);
            Debug.WriteLine("mob count: " + mobs.Count);*/
            return mobs;
        }

        public List<Player> GetPlayers()
        {
            return players;
        }

        public void AddEntity(Entity entity)
        {
            entity.Initalize(this);
            if (entity is Player p) players.Add(p);
            tmp.Add(entity);
            if (map != null)
            {
                map.AddEntity(entity);
            }
        }

        public void Render(Vector2 playerCoords, Screen screen)
        {
            if (map == null) return;
            //Render map
            map.Render(playerCoords, screen);
        }

        public void Update()
        {
            while (corrections.Count != 0)
            {
                EntityCorrection correction = corrections[0];

                foreach (Entity entity in entities)
                {
                    if (entity.ID == correction.ID)
                    {
                        entity.Correct(correction.X, correction.Y);
                        break;
                    }
                }
                corrections.RemoveAt(0);
            }

            entities.AddRange(tmp);
            tmp.Clear();

            //Remove entities from map first
            if (map != null)
            {
                entities.ForEach(entity =>
                {
                    if (entity.IsRemoved()) map.RemoveEntity(entity);
                });
            }
            //Remove entities
            entities.RemoveAll(entity => entity.IsRemoved());

            players.RemoveAll(player => player.IsRemoved());

            //Maintain active entities
            active_entities.Clear();
            entities.ForEach(e =>
            {
                bool inside = false;
                players.ForEach(p =>
                {
                    if (p.IsWithin(e.GetXY())) inside = true;
                });
                if (inside) active_entities.Add(e);
            });

            //Update active entities
            active_entities.ForEach(entity => entity.Update());

            //Update map after entities
            if (map != null)
            {
                map.Update();
                map.UpdateSectors(active_entities);
            }
        }

        public bool InLineOfSight(int xstart, int ystart, int xend, int yend)
        {
            bool result = true;
            xstart /= Map.tileSize;
            ystart /= Map.tileSize;
            xend /= Map.tileSize;
            yend /= Map.tileSize;
            while (xstart != xend && ystart != yend)
            {
                if (xstart != xend)
                {
                    if (xstart < xend) xstart++;
                    else xstart--;
                }

                if (ystart != yend)
                {
                    if (ystart < yend) ystart++;
                    else ystart--;
                }

                Tile tile = map.GetTile(xstart, ystart);
                if (tile == null) result = false;
                else if (tile.IsSolid()) result = false;

                if (!result) break;
            }
            return result;
        }

        public bool TileCollision(int xChange, int yChange, int width, int height)
        {
            bool solid = false;
            if (map == null) return solid;
            for (int i = 0; i < 4; i++)
            {
                // i % 2 and i >> 1 creates all 4 corners of a tile (0,0) (0,1) (1,0) (1,1)
                int xFuture = (xChange + (i % 2) * width);
                int yFuture = (yChange + (i >> 1) * height);
                if (0 > xFuture && xFuture > -Map.tileSize)
                {
                    xFuture = -1;
                }
                else
                {
                    xFuture /= Map.tileSize;
                }
                if (0 > yFuture && yFuture > -Map.tileSize)
                {
                    yFuture = -1;
                }
                else
                {
                    yFuture /= Map.tileSize;
                }
                Tile tile = map.GetTile(xFuture, yFuture);
                if (tile == null) solid = true;
                else if (tile.IsSolid()) solid = true;
            }
            return solid;
        }
    }
}
