using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Types
{
    internal struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2Int operator +(Vector2Int v1, Vector2Int v2)
        {
            var res = new Vector2Int();
            res.x = v1.x + v2.x;
            res.y = v1.y + v2.y;
            return res;
        }
        public static Vector2Int operator -(Vector2Int v1, Vector2Int v2)
        {
            var res = new Vector2Int();
            res.x = v1.x - v2.x;
            res.y = v1.y - v2.y;
            return res;
        }
        public override string ToString()
        {
            return $"X:{x} Y:{y}";
        }

        public static Vector2Int Zero => new Vector2Int(0,0);
    }
}
