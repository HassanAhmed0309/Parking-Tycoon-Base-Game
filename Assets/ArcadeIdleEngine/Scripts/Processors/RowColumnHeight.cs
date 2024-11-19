using System;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Processors
{
    [Serializable]
    public struct RowColumnHeight
    {
        [Range(1, 30)] public int RowCount;
        [Range(1, 30)] public int ColumnCount;
        [Range(1, 30)] public int HeightCount;

        public float HeightOffset;
        public float RowColumnOffset;

        public int additionFactor;
        
        public int GetCapacity()
        {
            return (RowCount * ColumnCount * HeightCount) + additionFactor;
        }
    }
}