using GameEngine.Entities.Mobs;
using GameEngine.Entities.PickupAbles;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameEngine.Levels
{
    class LevelGenerator : Level
    {
        Random random;
        public LevelGenerator(int seed, int size, bool gen_npc)
        {
            random = new Random(seed);
            Generate(size, gen_npc);
        }

        int NextGaussian(int mean, int sigma)
        {
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double r = mean + sigma * Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return (int)Math.Round(r);
        }

        void Generate(int size, bool gen_npc)
        {
            List<Room> rooms = new List<Room>();
            List<Hallway> hallways = new List<Hallway>();

            Room start = new Room(0, 0, 16, 16);
            //Open up starting branches
            start.OpenBranch(1);
            start.OpenBranch(2);
            rooms.Add(start);

            Room room = start;

            int rn = 0;
            int hn = 0;
            int depth = 100; //First branch extra long
            int again = 10;
            while (room != null)
            {
                //Branch depth achieved go back to previous room for new branching
                if (depth == 0)
                {
                    while (!room.HasOpenBranch())
                    {
                        room = room.Parent;
                        if (room == null) break;
                    }
                    if (room != null)
                    {
                        depth = random.Next(5, 10);
                        Debug.WriteLine("Starting new branch");
                    }
                    else
                    {
                        break;
                    }
                }
                //If we closed of all branches without adding a new room we reached the potential end point of this branch
                if (!room.HasOpenBranch())
                {
                    depth = 0;
                    Debug.WriteLine("Closed off all branches");
                    continue;
                }
                int width;
                int height;
                int x;
                int y;
                int w = NextGaussian(20, 10);
                int h = NextGaussian(20, 10);
                if (w < 10 || h < 10) continue;
                if (again == 3)
                {
                    w = 20;
                    h = 10;
                }
                if (again == 2)
                {
                    w = 10;
                    h = 20;
                }
                if (again == 1)
                {
                    w = 10;
                    h = 10;
                }
                BranchDir dir = (BranchDir)random.Next(4);
                while (!room.IsOpen(dir))
                {
                    dir = (BranchDir)random.Next(4);
                }
                switch (dir)
                {
                    case BranchDir.North:
                        {
                            width = random.Next(2, 4);
                            height = random.Next(4, 6);
                            if ((room.Y - height - h) > 0)
                            {
                                x = random.Next(room.CenterX - w / 2, room.CenterX - width / 2);
                                if (x < 0) x = room.X;
                                y = room.Y - height - h;
                                if ((x + w) < size && (y + h) < size)
                                {
                                    Hallway hallway = new Hallway(room.CenterX - width / 2, room.Y - height, width, height, HallWayOrientation.Vertical);
                                    Room new_room = new Room(x, y, w, h, room);
                                    bool good = true;
                                    rooms.ForEach(r =>
                                    {
                                        if (r.IsInside(new_room)) good = false;
                                    });
                                    if (good)
                                    {
                                        rooms.Add(new_room);
                                        hallways.Add(hallway);
                                        new_room.Allocate(BranchDir.South, hallway);
                                        room.Allocate(BranchDir.North, hallway);
                                        int b;
                                        if (depth >= 3) b = random.Next(2, 3); else b = random.Next(1, 3);
                                        if (depth == 1) b = 0;
                                        if (depth >= 5) b = 3;
                                        while (b != 0)
                                        {
                                            int open_b = random.Next(4);
                                            if (new_room.OpenBranch(open_b)) b--;
                                        }
                                        room = new_room;
                                        depth--;
                                        rn++;
                                        hn++;
                                    }
                                    else
                                    {
                                        if (again == 0)
                                        {
                                            room.Close(BranchDir.North);
                                            Debug.WriteLine("10 retry was not enough on north");
                                            again = 10;
                                        }
                                        again--;
                                    }
                                }
                                else
                                {
                                    room.Close(BranchDir.North);
                                }
                            }
                            else
                            {
                                room.Close(BranchDir.North);
                            }
                            break;
                        }
                    case BranchDir.East:
                        {
                            width = random.Next(4, 6);
                            height = random.Next(2, 4);
                            if ((room.X + room.Width + width + w) < size)
                            {
                                x = room.X + room.Width + width;
                                y = random.Next(room.CenterY - h / 2, room.CenterY - width / 2);
                                if (y < 0) y = room.Y;
                                if ((x + w) < size && (y + h) < size)
                                {
                                    Hallway hallway = new Hallway(room.X + room.Width, room.CenterY - height / 2, width, height, HallWayOrientation.Horizontal);
                                    Room new_room = new Room(x, y, w, h, room);
                                    bool good = true;
                                    rooms.ForEach(r =>
                                    {
                                        if (r.IsInside(new_room)) good = false;
                                    });
                                    if (good)
                                    {
                                        rooms.Add(new_room);
                                        hallways.Add(hallway);
                                        new_room.Allocate(BranchDir.West, hallway);
                                        room.Allocate(BranchDir.East, hallway);
                                        int b;
                                        if (depth >= 3) b = random.Next(2, 3); else b = random.Next(1, 3);
                                        if (depth == 1) b = 0;
                                        if (depth >= 5) b = 3;
                                        while (b != 0)
                                        {
                                            int open_b = random.Next(4);
                                            if (new_room.OpenBranch(open_b)) b--;
                                        }
                                        room = new_room;
                                        depth--;
                                        rn++;
                                        hn++;
                                    }
                                    else
                                    {
                                        if (again == 0)
                                        {
                                            room.Close(BranchDir.East);
                                            Debug.WriteLine("10 retry was not enough east");
                                            again = 10;
                                        }
                                        again--;
                                    }
                                }
                                else
                                {
                                    room.Close(BranchDir.East);
                                }
                            }
                            else
                            {
                                room.Close(BranchDir.East);
                            }
                            break;
                        }
                    case BranchDir.South:
                        {
                            width = random.Next(2, 4);
                            height = random.Next(4, 6);
                            if ((room.Y + room.Height + height + h) < size)
                            {
                                x = random.Next(room.CenterX - w / 2, room.CenterX - width / 2);
                                if (x < 0) x = room.X;
                                y = room.Y + room.Height + height;
                                if ((x + w) < size && (y + h) < size)
                                {
                                    Hallway hallway = new Hallway(room.CenterX - width / 2, room.Y + room.Height, width, height, HallWayOrientation.Vertical);
                                    Room new_room = new Room(x, y, w, h, room);
                                    bool good = true;
                                    rooms.ForEach(r =>
                                    {
                                        if (r.IsInside(new_room)) good = false;
                                    });
                                    if (good)
                                    {
                                        rooms.Add(new_room);
                                        hallways.Add(hallway);
                                        new_room.Allocate(BranchDir.North, hallway);
                                        room.Allocate(BranchDir.South, hallway);
                                        int b;
                                        if (depth >= 3) b = random.Next(2, 3); else b = random.Next(1, 3);
                                        if (depth == 1) b = 0;
                                        if (depth >= 5) b = 3;
                                        while (b != 0)
                                        {
                                            int open_b = random.Next(4);
                                            if (new_room.OpenBranch(open_b)) b--;
                                        }
                                        room = new_room;
                                        depth--;
                                        rn++;
                                        hn++;
                                    }
                                    else
                                    {
                                        if (again == 0)
                                        {
                                            room.Close(BranchDir.South);
                                            Debug.WriteLine("10 retry was not enough on south");
                                            again = 10;
                                        }
                                        again--;
                                    }
                                }
                                else
                                {
                                    room.Close(BranchDir.South);
                                }
                            }
                            else
                            {
                                room.Close(BranchDir.South);
                            }
                            break;
                        }
                    case BranchDir.West:
                        {
                            width = random.Next(4, 6);
                            height = random.Next(2, 4);
                            if ((room.X - width - w) > 0)
                            {
                                x = room.X - width - w;
                                y = random.Next(room.CenterY - h / 2, room.CenterY - width / 2);
                                if (y < 0) y = room.Y;
                                if ((x + w) < size && (y + h) < size)
                                {
                                    Hallway hallway = new Hallway(room.X - width, room.CenterY - height / 2, width, height, HallWayOrientation.Horizontal);
                                    Room new_room = new Room(x, y, w, h, room);
                                    bool good = true;
                                    rooms.ForEach(r =>
                                    {
                                        if (r.IsInside(new_room)) good = false;
                                    });
                                    if (good)
                                    {
                                        rooms.Add(new_room);
                                        hallways.Add(hallway);
                                        new_room.Allocate(BranchDir.East, hallway);
                                        room.Allocate(BranchDir.West, hallway);
                                        int b;
                                        if (depth >= 3) b = random.Next(2, 3); else b = random.Next(1, 3);
                                        if (depth == 1) b = 0;
                                        if (depth >= 5) b = 3;
                                        while (b != 0)
                                        {
                                            int open_b = random.Next(4);
                                            if (new_room.OpenBranch(open_b)) b--;
                                        }
                                        room = new_room;
                                        depth--;
                                        rn++;
                                        hn++;
                                    }
                                    else
                                    {
                                        if (again == 0)
                                        {
                                            room.Close(BranchDir.West);
                                            Debug.WriteLine("10 retry was not enough on west");
                                            again = 10;
                                        }
                                        again--;
                                    }
                                }
                                else
                                {
                                    room.Close(BranchDir.West);
                                }
                            }
                            else
                            {
                                room.Close(BranchDir.West);
                            }
                            break;
                        }
                }
            }
            Debug.WriteLine("Generated: " + rn + " rooms and " + hn + " hallways");

            //Create map
            CreateMap(rooms, hallways, size);
            Debug.WriteLine("Map created");

            //Generate sectors
            CreateSectors(rooms);

            CreateHallwaySectors(hallways);
            Debug.WriteLine("Sectors created");
            map.Finalise();

            //Make minimap
            CreateMinimapData(rooms, hallways, size + 2);
            Debug.Write("Minimap data created");

            //Generate NPCs
            if (gen_npc)
            {
                int s = GenerateEntities(rooms);
                Debug.WriteLine("Generated: " + s + " NPCs & Objects");
            }
        }

        int GenerateEntities(List<Room> rooms)
        {
            int sum = 0;
            int biggestRoom = 0;
            //Search for biggest room
            for (int i = 1; i < rooms.Count;i++)
            {
                if (rooms[biggestRoom].GetSize() < rooms[i].GetSize())
                {
                    biggestRoom = i;
                }
            }
            int biggestRoomID = rooms[biggestRoom].Id;
            //Spawn mobs in normal rooms
            rooms.ForEach(room =>
            {
                if (room.Id != 0 && room.Id != biggestRoomID)
                {
                    int c = room.GetSize() / 40;
                    while (c > 0)
                    {
                        int x = random.Next(room.X * Map.tileSize, (room.X + room.Width - 1) * Map.tileSize);
                        int y = random.Next(room.Y * Map.tileSize, (room.Y + room.Height - 1) * Map.tileSize);
                        int r = random.Next(100);
                        if (r > 50)
                        {
                            x -= x % Map.tileSize;
                            y -= y % Map.tileSize;
                            if (random.Next(100) > 50)
                            {
                                AddEntity(new Arrows(x, y, random.Next(10, 20)));
                            }
                            else
                            {
                                AddEntity(new HP_Potion(x, y, 20));
                            }
                        }
                        else if (r > 0)
                        {
                            AddEntity(new Dummy(x, y));
                        }
                        c--;
                        sum++;
                    }
                }
            });

            return sum;
        }

        void FilterRooms(List<Room> rooms)
        {
            //Start from 1 because 0(start room) prevented adding ones that would overlap
            int i = 1;
            int removed = 0;
            while (true)
            {
                if (i < rooms.Count)
                {
                    List<Room> temp = new List<Room>();
                    Room r = rooms[i];
                    foreach (Room room in rooms)
                    {
                        if (r.Id != room.Id)
                        {
                            if (r.IsInside(room))
                            {
                                if ((r.Width * r.Height) > (room.Height * room.Width))
                                {
                                    temp.Add(room);
                                    removed++;
                                }
                                else
                                {
                                    temp.Add(r);
                                    i--;
                                    removed++;
                                    break;
                                }
                            }
                        }
                    }
                    temp.ForEach(room => rooms.Remove(room));
                    i++;
                }
                else
                {
                    Debug.WriteLine("Filtering stopped after: " + i + " iteration, removed: " + removed + " rooms");
                    break;
                }
            }
            if (removed != 0)
            {
                FilterRooms(rooms);
            }
        }

        void CreateMap(List<Room> rooms, List<Hallway> hallways, int size)
        {
            int[,] floor = new int[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    floor[x, y] = 0;
                }
            }

            //Fill floor with room tiles
            rooms.ForEach(room =>
            {
                int type = random.Next(100);
                for (int y = room.Y;y < (room.Y+room.Height); y++)
                {
                    for (int x = room.X; x< (room.X+room.Width);x++)
                    {
                        if (type > 50)
                        {
                            int r = random.Next(100);
                            if (r < 80) floor[x, y] = 2;
                            else if (r < 20) floor[x, y] = 6;
                            else floor[x, y] = 19;
                        }
                        else
                        {
                            floor[x, y] = 20;
                        }
                    }
                }
            });

            //Fill floor with hallway tiles
            hallways.ForEach(hallway =>
            {
                for (int y = hallway.Y; y < (hallway.Y + hallway.Height); y++)
                {
                    for (int x = hallway.X; x < (hallway.X + hallway.Width); x++)
                    {
                        int r = random.Next(100);
                        if (r < 80) floor[x, y] = 2;
                        else floor[x, y] = 6;
                    }
                }
            });

            map = new Map(size, size, floor);
        }

        void CreateHallwaySectors(List<Hallway> hallways)
        {
            hallways.ForEach(hallway =>
            {
                if (hallway.Orientation == HallWayOrientation.Horizontal)
                {
                    int[] hor = new int[hallway.Width];
                    int[] hor_2 = new int[hallway.Width];
                    int[] hor_NT = new int[hallway.Width];
                    int[] hor_ST = new int[hallway.Width];

                    hor_NT[0] = 18;
                    hor_ST[0] = 16;
                    for (int i = 1; i < hallway.Width; i++)
                    {
                        hor_NT[i] = 11;
                        hor_ST[i] = 15;
                    }
                    hor_NT[hallway.Width - 1] = 12;
                    hor_ST[hallway.Width - 1] = 14;
                    hor_2[0] = 7;
                    hor[0] = 3;
                    for (int i = 1; i < hallway.Width; i++)
                    {
                        hor[i] = 3;
                        hor_2[i] = 3;
                    }
                    hor_2[hallway.Width - 1] = 9;

                    Sector sector = new Sector(hallway.X, hallway.Y, hallway.Width, hallway.Height);

                    sector.AddWall(new Wall(0.0f, 0.0f, 0, hallway.Width, hor, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(0.0f, 0.0f, 1, hallway.Width, hor, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(0.0f, -1.0f, 2, hallway.Width, hor_NT, WallOrientation.HorizontalTop));

                    sector.AddWall(new Wall(0.0f, hallway.Height + 0.5f, 0, hallway.Width, hor_2, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(0.0f, hallway.Height + 0.5f, 1, hallway.Width, hor_2, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(0.0f, hallway.Height, 2, hallway.Width, hor_ST, WallOrientation.HorizontalTop));

                    sector.Finalise();

                    map.AddSector(sector);
                }
                else
                {
                    int[] ver = new int[hallway.Height];
                    int[] ver_WT = new int[hallway.Height];
                    int[] ver_ET = new int[hallway.Height];
                    int[] ver_2 = new int[hallway.Height];

                    for (int i = 0; i < hallway.Height; i++)
                    {
                        ver[i] = 4;
                        ver_WT[i] = 17;
                        ver_ET[i] = 13;
                        ver_2[i] = 4;
                    }
                    ver_2[0] = 10;
                    ver_2[hallway.Height - 1] = 8;

                    Sector sector = new Sector(hallway.X, hallway.Y, hallway.Width, hallway.Height);

                    sector.AddWall(new Wall(0.0f, 0.0f, 0, hallway.Height, ver, WallOrientation.Vertical));
                    sector.AddWall(new Wall(0.0f, 0.0f, 1, hallway.Height, ver, WallOrientation.Vertical));
                    sector.AddWall(new Wall(-1.0f, 0.0f, 2, hallway.Height, ver_WT, WallOrientation.VerticalTop));

                    sector.AddWall(new Wall(hallway.Width + 0.5f, 0.0f, 0, hallway.Height, ver_2, WallOrientation.Vertical));
                    sector.AddWall(new Wall(hallway.Width + 0.5f, 0.0f, 1, hallway.Height, ver_2, WallOrientation.Vertical));
                    sector.AddWall(new Wall(hallway.Width, 0.0f, 2, hallway.Height, ver_ET, WallOrientation.VerticalTop));

                    sector.Finalise();

                    map.AddSector(sector);
                }
            });
        }

        void CreateSectors(List<Room> rooms)
        {
            rooms.ForEach(room =>
            {
                Sector sector = new Sector(room.X, room.Y, room.Width, room.Height);
                
                if (room.IsAllocated(BranchDir.North))
                {
                    Hallway hallway = room.GetHallway(BranchDir.North);

                    int h_l_length = hallway.X - room.X;
                    int[] hor_l = new int[h_l_length];
                    int[] hor_NT_l = new int[h_l_length + 1];

                    int h_r_length = room.Width - (hallway.X + hallway.Width - room.X);
                    int[] hor_r = new int[h_r_length];
                    int[] hor_NT_r = new int[h_r_length + 1];

                    hor_NT_l[0] = 18;
                    hor_l[0] = 3;
                    hor_NT_r[0] = 18;
                    hor_r[0] = 3;
                    for (int i = 1; i < h_l_length; i++)
                    {
                        hor_NT_l[i] = 11;
                        hor_l[i] = 3;
                    }
                    hor_NT_l[h_l_length] = 12;
                    hor_l[h_l_length - 1] = 3;
                    for (int i = 1; i < h_r_length; i++)
                    {
                        hor_NT_r[i] = 11;
                        hor_r[i] = 3;
                    }
                    hor_NT_r[h_r_length] = 12;
                    hor_r[h_r_length - 1] = 3;

                    sector.AddWall(new Wall(0.0f, 0.0f, 0, h_l_length, hor_l, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(0.0f, 0.0f, 1, h_l_length, hor_l, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(-1.0f, -1.0f, 2, h_l_length + 1, hor_NT_l, WallOrientation.HorizontalTop));

                    sector.AddWall(new Wall(hallway.X + hallway.Width - room.X, 0.0f, 0, h_r_length, hor_r, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(hallway.X + hallway.Width - room.X, 0.0f, 1, h_r_length, hor_r, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(hallway.X + hallway.Width - room.X, -1.0f, 2, h_r_length + 1, hor_NT_r, WallOrientation.HorizontalTop));
                }
                else
                {
                    int[] hor = new int[room.Width];
                    int[] hor_NT = new int[(room.Width + 2)];

                    hor_NT[0] = 18;
                    for (int i = 1; i < (room.Width + 1); i++)
                    {
                        hor_NT[i] = 11;
                    }
                    hor_NT[room.Width + 1] = 12;
                    for (int i = 0; i < room.Width; i++)
                    {
                        hor[i] = 3;

                    }

                    sector.AddWall(new Wall(0.0f, 0.0f, 0, room.Width, hor, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(0.0f, 0.0f, 1, room.Width, hor, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(-1.0f, -1.0f, 2, room.Width + 2, hor_NT, WallOrientation.HorizontalTop));
                }

                if (room.IsAllocated(BranchDir.South))
                {
                    Hallway hallway = room.GetHallway(BranchDir.South);

                    int h2_l_length = hallway.X - room.X + 1;
                    int[] hor_2_l = new int[h2_l_length];
                    int[] hor_ST_l = new int[h2_l_length];

                    int h2_r_length = room.Width - (hallway.X + hallway.Width - room.X) + 1;
                    int[] hor_2_r = new int[h2_r_length];
                    int[] hor_ST_r = new int[h2_r_length];

                    hor_ST_l[0] = 16;
                    hor_2_l[0] = 7;
                    hor_ST_r[0] = 16;
                    hor_2_r[0] = 7;
                    for (int i = 1; i < h2_l_length;i++)
                    {
                        hor_ST_l[i] = 15;
                        hor_2_l[i] = 3;
                    }
                    hor_ST_l[h2_l_length - 1] = 14;
                    hor_2_l[h2_l_length - 1] = 9;
                    for (int i = 1; i < h2_r_length;i++)
                    {
                        hor_ST_r[i] = 15;
                        hor_2_r[i] = 3;
                    }
                    hor_ST_r[h2_r_length - 1] = 14;
                    hor_2_r[h2_r_length - 1] = 9;

                    sector.AddWall(new Wall(-1.0f, room.Height + 0.5f, 0, h2_l_length, hor_2_l, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(-1.0f, room.Height + 0.5f, 1, h2_l_length, hor_2_l, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(-1.0f, room.Height, 2, h2_l_length, hor_ST_l, WallOrientation.HorizontalTop));

                    sector.AddWall(new Wall(hallway.X + hallway.Width - room.X, room.Height + 0.5f, 0, h2_r_length, hor_2_r, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(hallway.X + hallway.Width - room.X, room.Height + 0.5f, 1, h2_r_length, hor_2_r, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(hallway.X + hallway.Width - room.X, room.Height, 2, h2_r_length, hor_ST_r, WallOrientation.HorizontalTop));
                }
                else
                {
                    int[] hor_2 = new int[(room.Width + 2)];
                    int[] hor_ST = new int[(room.Width + 2)];

                    hor_ST[0] = 16;
                    hor_2[0] = 7;
                    for (int i = 1; i < (room.Width + 1); i++)
                    {

                        hor_ST[i] = 15;
                        hor_2[i] = 3;
                    }
                    hor_ST[room.Width + 1] = 14;
                    hor_2[room.Width + 1] = 9;

                    sector.AddWall(new Wall(-1.0f, room.Height + 0.5f, 0, room.Width + 2, hor_2, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(-1.0f, room.Height + 0.5f, 1, room.Width + 2, hor_2, WallOrientation.Horizontal));
                    sector.AddWall(new Wall(-1.0f, room.Height, 2, room.Width + 2, hor_ST, WallOrientation.HorizontalTop));
                }

                if (room.IsAllocated(BranchDir.West))
                {
                    Hallway hallway = room.GetHallway(BranchDir.West);

                    int v_r_length = hallway.Y - room.Y;
                    int[] ver_r = new int[v_r_length];
                    int[] ver_WT_r = new int[v_r_length];

                    int v_l_length = room.Height - (hallway.Y + hallway.Height - room.Y);
                    int[] ver_l = new int[v_l_length];
                    int[] ver_WT_l = new int[v_l_length];

                    for (int i = 0; i < v_r_length; i++)
                    {
                        ver_r[i] = 4;
                        ver_WT_r[i] = 17;
                    }
                    for (int i = 0; i < v_l_length; i++)
                    {
                        ver_l[i] = 4;
                        ver_WT_l[i] = 17;
                    }

                    sector.AddWall(new Wall(0.0f, 0.0f, 0, v_r_length, ver_r, WallOrientation.Vertical));
                    sector.AddWall(new Wall(0.0f, 0.0f, 1, v_r_length, ver_r, WallOrientation.Vertical));
                    sector.AddWall(new Wall(-1.0f, 0.0f, 2, v_r_length, ver_WT_r, WallOrientation.VerticalTop));
 
                    sector.AddWall(new Wall(0.0f, hallway.Y + hallway.Height - room.Y, 0, v_l_length, ver_l, WallOrientation.Vertical));
                    sector.AddWall(new Wall(0.0f, hallway.Y + hallway.Height - room.Y, 1, v_l_length, ver_l, WallOrientation.Vertical));
                    sector.AddWall(new Wall(-1.0f, hallway.Y + hallway.Height - room.Y, 2, v_l_length, ver_WT_l, WallOrientation.VerticalTop));
                }
                else
                {
                    int[] ver = new int[room.Height];
                    int[] ver_WT = new int[room.Height];

                    for (int i = 0; i < room.Height; i++)
                    {
                        ver[i] = 4;
                        ver_WT[i] = 17;
                    }

                    sector.AddWall(new Wall(0.0f, 0.0f, 0, room.Height, ver, WallOrientation.Vertical));
                    sector.AddWall(new Wall(0.0f, 0.0f, 1, room.Height, ver, WallOrientation.Vertical));
                    sector.AddWall(new Wall(-1.0f, 0.0f, 2, room.Height, ver_WT, WallOrientation.VerticalTop));
                }

                if (room.IsAllocated(BranchDir.East))
                {
                    Hallway hallway = room.GetHallway(BranchDir.East);

                    int v2_r_length = hallway.Y - room.Y;
                    int[] ver_2_r = new int[v2_r_length + 1];
                    int[] ver_ET_r = new int[v2_r_length];

                    int v2_l_length = room.Height - (hallway.Y + hallway.Height - room.Y) + 1;
                    int[] ver_2_l = new int[v2_l_length];
                    int[] ver_ET_l = new int[v2_l_length];

                    ver_2_r[0] = 10;
                    ver_ET_r[0] = 13;
                    for (int i = 1; i < v2_r_length; i++)
                    {
                        ver_ET_r[i] = 13;
                        ver_2_r[i] = 4;
                    }
                    ver_2_r[v2_r_length] = 8;
                    ver_2_l[0] = 10;
                    for (int i = 0; i < (v2_l_length - 1);i++)
                    {
                        ver_ET_l[i] = 13;
                    }
                    for (int i = 1; i < v2_l_length; i++)
                    {
                        ver_2_l[i] = 4;
                    }
                    ver_2_l[v2_l_length - 1] = 8;

                    sector.AddWall(new Wall(room.Width + 0.5f, -1.0f, 0, v2_r_length + 1, ver_2_r, WallOrientation.Vertical));
                    sector.AddWall(new Wall(room.Width + 0.5f, -1.0f, 1, v2_r_length + 1, ver_2_r, WallOrientation.Vertical));
                    sector.AddWall(new Wall(room.Width, 0.0f, 2, v2_r_length, ver_ET_r, WallOrientation.VerticalTop));

                    sector.AddWall(new Wall(room.Width + 0.5f, hallway.Y + hallway.Height - room.Y, 0, v2_l_length, ver_2_l, WallOrientation.Vertical));
                    sector.AddWall(new Wall(room.Width + 0.5f, hallway.Y + hallway.Height - room.Y, 1, v2_l_length, ver_2_l, WallOrientation.Vertical));
                    sector.AddWall(new Wall(room.Width, hallway.Y + hallway.Height - room.Y, 2, v2_l_length - 1, ver_ET_l, WallOrientation.VerticalTop));
                }
                else
                {
                    int[] ver_2 = new int[(room.Height + 2)];
                    int[] ver_ET = new int[room.Height];

                    for (int i = 0; i < room.Height; i++)
                    {
                        ver_ET[i] = 13;
                    }
                    ver_2[0] = 10;
                    for (int i = 1; i < (room.Height + 1); i++)
                    {
                        ver_2[i] = 4;
                    }
                    ver_2[room.Height + 1] = 8;

                    sector.AddWall(new Wall(room.Width + 0.5f, -1.0f, 0, room.Height + 2, ver_2, WallOrientation.Vertical));
                    sector.AddWall(new Wall(room.Width + 0.5f, -1.0f, 1, room.Height + 2, ver_2, WallOrientation.Vertical));
                    sector.AddWall(new Wall(room.Width, 0.0f, 2, room.Height, ver_ET, WallOrientation.VerticalTop));
                }
                
                sector.Finalise();

                map.AddSector(sector);
            });
        }

        void CreateMinimapData(List<Room> rooms, List<Hallway> hallways, int size)
        {
            MapState[,] data = new MapState[size, size];

            for (int y = 0;y < size;y++)
            {
                for (int x = 0; x < size; x++)
                {
                    data[x, y] = MapState.VOID;
                }
            }

            rooms.ForEach(room =>
            {
                //Fill room floor
                for (int y = room.Y + 1; y < (room.Y + 1 + room.Height); y++)
                {
                    for (int x = room.X + 1; x < (room.X + 1 + room.Width); x++)
                    {
                        data[x, y] = MapState.FLOOR_HIDEN;
                    }
                }

                //Fill room walls
                for (int i = room.X; i < (room.X + 2 + room.Width);i++)
                {
                    data[i, room.Y] = MapState.WALL_HIDEN;
                    data[i, room.Y + 1 + room.Height] = MapState.WALL_HIDEN;
                }
                for (int i = room.Y + 1; i < (room.Y + room.Height); i++)
                {
                    data[room.X, i] = MapState.WALL_HIDEN;
                    data[room.X + 1 + room.Width, i] = MapState.WALL_HIDEN;
                }
            });

            hallways.ForEach(hallway =>
            {
                //Fill hallway floor
                for (int y = hallway.Y + 1; y < (hallway.Y + 1 + hallway.Height); y++)
                {
                    for (int x = hallway.X + 1; x < (hallway.X + 1 + hallway.Width); x++)
                    {
                        data[x, y] = MapState.FLOOR_HIDEN;
                    }
                }

                //Fill hallway walls
                if (hallway.Orientation == HallWayOrientation.Horizontal)
                {
                    for (int i = hallway.X + 1; i < (hallway.X + hallway.Width); i++)
                    {
                        data[i, hallway.Y] = MapState.WALL_HIDEN;
                        data[i, hallway.Y + 1 + hallway.Height] = MapState.WALL_HIDEN;
                    }
                }
                else
                {
                    for (int i = hallway.Y + 1; i < (hallway.Y + hallway.Height); i++)
                    {
                        data[hallway.X, i] = MapState.WALL_HIDEN;
                        data[hallway.X + 1 + hallway.Width, i] = MapState.WALL_HIDEN;
                    }
                }
            });

            map.SetMinimapData(data);
        }
    }
}
