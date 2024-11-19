using ArcadeBridge.ArcadeIdleEngine.Data;
using ArcadeBridge.ArcadeIdleEngine.Pools;
using ArcadeIdleEngine.ExternalAssets.NaughtyAttributes_2._1._4.Core.MetaAttributes;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Pickables
{
    [CreateAssetMenu(menuName = nameof(ArcadeIdleEngine) + "/" + nameof(Pickables) + "/" + nameof(PickableDefinition))]
    public class PickableDefinition : ScriptableObject
    {
        [field: SerializeField, Tooltip("Object pool that is associated with this pickable.")] 
        public PickablePool Pool { get; private set; }
        
        [field: SerializeField, Tooltip("If you want to save how many pickable of this type you have, reference a int variable.")] 
        public IntVariable Variable { get; private set; }
        
        [field: SerializeField, Tooltip("Enable if it should be seen when player collects.")]
        public  bool Visible { get; private set; }
        
        [field: SerializeField, Tooltip("Enable it if you want to be able to sell this pickable.")]
        public  bool Sellable { get; private set; }
        
        [field: SerializeField, ShowIf(nameof(Sellable))] 
        public int SellValue { get; private set; }
        
        [field: SerializeField, Tooltip("Enable it and assing a sprite if you want to show the pickable on the UI. You can access the sprite by pickableDefinition.Sprite property.")]
        public bool HasSprite { get; private set; }
        
        [field: SerializeField, ShowIf(nameof(HasSprite))] 
        public Sprite Sprite { get; private set; }

        public ItemType CurrentItemType;
    }
}

public enum ItemType
{
    none,
    carTicket,
    money
}