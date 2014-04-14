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

namespace HW06_FlipGame
{
    class Table
    {
        public bool[,] table = new bool[4, 4];
        public Table previousTable;

        public int y;
        public int x;


        public Table(bool[,] newTable, Table prevTable = null)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    table[j, i] = newTable[j, i];
                }
            }
            previousTable = prevTable;
        }

    }
}
