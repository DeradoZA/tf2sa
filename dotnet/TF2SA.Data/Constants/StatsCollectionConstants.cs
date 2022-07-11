using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TF2SA.Data.Constants
{
    public class StatsCollectionConstants
    {
        public int SCOUT_ClassID { get; set; }
        public int SOLDIER_ClassID { get; set; }
        public int PYRO_ClassID { get; set; }
        public int DEMOMAN_ClassID { get; set; }
        public int HEAVY_ClassID { get; set; }
        public int ENGINEER_ClassID { get; set; }
        public int MEDIC_ClassID { get; set; }
        public int SNIPER_ClassID { get; set; }
        public int SPY_ClassID { get; set; }
        public int PLAYTIME_THRESHOLD { get; set; }

        public StatsCollectionConstants()
        {
            SCOUT_ClassID = 1;
            SOLDIER_ClassID = 2;
            PYRO_ClassID = 3;
            DEMOMAN_ClassID = 4;
            HEAVY_ClassID = 5;
            ENGINEER_ClassID = 6;
            MEDIC_ClassID = 7;
            SNIPER_ClassID = 8;
            SPY_ClassID = 9;
            PLAYTIME_THRESHOLD = 60;
        }


    }
}