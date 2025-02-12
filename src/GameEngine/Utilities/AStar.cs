using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace GameEngine.Utilities
{
    class AStar
    {
        static bool _initialized = false;

        static List<Node> _open;
        static List<Node> _closed;
        static Node _current;
        static int[,] _map;
        static int _mapWidth, _mapHeight;
        static float _xStart, _yStart;
        static float _xEnd, _yEnd;


        public static void Initialize(int[,] map, int mapWidth, int mapHeight)
        {
            if (map == null) return;
            _open = new List<Node>();
            _closed = new List<Node>();
            _map = map;
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;

            _initialized = true;
            Debug.WriteLine("A* Initialized");
        }

        public static bool IsInitialized()
        {
            return _initialized;
        }

        public static List<Vector2> FindPath(float xStart, float yStart, float xEnd, float yEnd)
        {
            if (!_initialized) return null;
            _open.Clear();
            _closed.Clear();
            _xStart = xStart;
            _yStart = yStart;
            _xEnd = xEnd;
            _yEnd = yEnd;
            //Debug.WriteLine("Finding path,from X: " + xStart + " Y: " + yStart + " to X: " + xEnd + " Y: " + yEnd);
            if (xEnd < 0 || xEnd > _mapWidth || yEnd < 0 || yEnd > _mapHeight)
            {
                Debug.WriteLine("Returning null because destination out of the map");
                return null;
            }
            else
            {
                if (_map[(int)xEnd, (int)yEnd] == -1)
                {
                    Debug.WriteLine("Returning null because destination out of the map");
                    return null;
                }
            }
            _current = new Node(null, xStart, yStart, 0, 0);

            _closed.Add(_current);
            AddNeigborsToOpenList();
            while (_current.X != _xEnd || _current.Y != _yEnd)
            {
                if (_open.Count() == 0)
                {
                    //Debug.WriteLine("Returning null because _open is empty");
                    return null;
                }
                _current = _open[0];
                _open.Remove(_current);
                _closed.Add(_current);
                AddNeigborsToOpenList();
            }
            List<Vector2> path = new List<Vector2>();
            path.Insert(0, new Vector2(_current.X, _current.Y));
            while (_current.X != _xStart || _current.Y != _yStart)
            {
                _current = _current.Parent;
                path.Insert(0, new Vector2(_current.X, _current.Y));
            }
            return path;
        }

        static bool FindNeigborInList(List<Node> nodes, Node node)
        {
            return nodes.Any(n => (n.X == node.X && n.Y == node.Y));
        }

        static void AddNeigborsToOpenList()
        {
            if (Distance((int)_xStart, (int)_yStart) > 50) return;
            Node node;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x != 0 && y != 0)
                    {
                        if (_current.X + x >= 0 && _current.X + x < _mapWidth && _current.Y + y >= 0 && _current.Y + y < _mapHeight)
                        {
                            int cX = (int)_current.X;
                            int cY = (int)_current.Y;
                            if (x == y)
                            {
                                if (_map[cX + x, cY + x - y] == -1) continue;
                                if (_map[cX + x - y, cY + y] == -1) continue;
                            }
                            else
                            {
                                if (_map[cX + x, cY + x + y] == -1) continue;
                                if (_map[cX + x + y, cY + y] == -1) continue;
                            }
                        }
                        else
                            continue;
                    }
                    node = new Node(_current, _current.X + x, _current.Y + y, _current.G, Distance(x, y));
                    if ((x != 0 || y != 0) //not current node
                        && _current.X + x >= 0 && _current.X + x < _mapWidth //check boundarioes
                        && _current.Y + y >= 0 && _current.Y + y < _mapHeight
                        && _map[(int)_current.X + x,(int)_current.Y + y] != -1 //check for node if it's walkable
                        && !FindNeigborInList(_open, node) && !FindNeigborInList(_closed, node)) //if not already a node
                    {
                        node.G = node.Parent.G;
                        node.G += _map[(int)_current.X + x, (int)_current.Y + y]; //add movement cost
                        if (x != 0 && y != 0)
                        {
                            node.G += 1.0; //if it's diagonal movement add extra cost
                        }
                        _open.Add(node);
                    }
                }
            }
            _open.Sort();
        }

        static double Distance(int x, int y)
        {
            return Math.Sqrt(Math.Pow(_current.X + x - _xEnd, 2) + Math.Pow(_current.Y + y - _yEnd, 2));
        }
    }
}
