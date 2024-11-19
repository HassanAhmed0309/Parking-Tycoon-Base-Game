using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Pools
{
    [CreateAssetMenu(menuName = nameof(ArcadeIdleEngine) + "/" + nameof(Pools) + "/" + nameof(PickablePool))]
    public class PickablePool : ObjectPool<Pickable>
    {
        public PickableDefinition PickableDefinition => Behaviour.Definition;
    }
}