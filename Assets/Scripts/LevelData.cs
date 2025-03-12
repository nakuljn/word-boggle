using System;
using System.Collections.Generic;

namespace WordBoggle
{

    [Serializable]
    public class LevelModeData
    {
        public List<LevelData> data;
    }

    [Serializable]
    public class LevelData
    {
        public int bugCount;
        public int wordCount;
        public int timeSec;
        public int totalScore;
        public GridSize gridSize;
        public List<GridTile> gridData;
    }

    [Serializable]
    public class GridSize
    {
        public int x;
        public int y;
    }

    [Serializable]
    public class GridTile
    {
        public int tileType;
        public string letter;
    }
}

