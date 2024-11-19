using System;
using ArcadeBridge.ArcadeIdleEngine.Pickables;

namespace ArcadeBridge.ArcadeIdleEngine.Processors.Transformers
{
    [Serializable]
    public struct PickableDefinitionCountPair
    {
        public PickableDefinition PickableDefinition;
        public int Count;
    }
}