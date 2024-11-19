using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Processors.Transformers
{
    [CreateAssetMenu(menuName = nameof(ArcadeIdleEngine) + "/" + nameof(Processors) + "/" + nameof(Transformers) + "/" + nameof(MultipleConditionRuleset))]
    public class MultipleConditionRuleset : ScriptableObject
    {
        public PickableDefinitionCountPair[] Inputs;
        public PickableDefinitionCountPair Output;
    }
}