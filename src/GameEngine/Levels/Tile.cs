using System;
using System.Collections.Generic;

namespace GameEngine.Levels
{
    class Tile
    {
        static SortedDictionary<int, Tile> tiles = new SortedDictionary<int, Tile>();

        readonly bool _solid;
        readonly bool _penetrateable;
        readonly int _ID;

        public Tile(bool solid, bool penetrateable, int ID)
        {
            _solid = solid;
            _penetrateable = penetrateable;
            _ID = ID;

            Init();
        }

        void Init()
        {
            try
            {
                tiles.Add(_ID, this);
            } 
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        public bool IsSolid()
        {
            return _solid;
        }

        public bool IsPenetrateable()
        {
            return _penetrateable;
        }

        public int GetID()
        {
            return _ID;
        }

        public static Tile GetTile(int ID)
        {
            if (tiles.ContainsKey(ID))
            {
                return tiles[ID];
            } 
            else
            {
                return null;
            }
        }
    }
}
