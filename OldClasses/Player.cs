using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sagescroll
{
    class Player
    {
        #region Variables
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        String name;

        public int Points
        {
            get { return points; }
            set { points = value; }
        }
        int points;

        public int PlayerLevel
        {
            get { return playerLevel; }
            set { playerLevel = value; }
        }
        int playerLevel;

        #endregion

        public Player(String name)
        {
            this.Name = name;
            this.Points = 0;
            this.playerLevel = 0; 
        }

        public Player(string name, int points, int playerLevel)
        {
            this.Name = name;
            this.Points = points;
            this.playerLevel = playerLevel; 
        }
    }
}
