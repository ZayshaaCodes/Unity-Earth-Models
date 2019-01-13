using System.Collections.Generic;

namespace Cookie042.EarthHeightmap
{
    public class BlockData
    {
        public short[,] RawHeightData;
        public Dictionary<int, Chunk> chunks = new Dictionary<int, Chunk>();

        public int minValue;
        public int maxValue;

        public BlockData(short[,] rawHeightData)
        {
            RawHeightData = rawHeightData;
        }
    }
}
