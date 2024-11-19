using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Processors.Transformers
{
    [CreateAssetMenu(menuName = nameof(ArcadeIdleEngine) + "/" + nameof(Processors) + "/" + nameof(Transformers) + "/" + nameof(StockpilerRuleset))]
    public class StockpilerRuleset : ScriptableObject
    {
        [field: SerializeField] public PickableDefinition Input { get; set; }
        [field: SerializeField] public PickableDefinition Output { get; set; }
    }
}