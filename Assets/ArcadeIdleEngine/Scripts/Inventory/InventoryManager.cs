using ArcadeBridge.ArcadeIdleEngine.Actors;
using ArcadeBridge.ArcadeIdleEngine.Interactables;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ArcadeBridge.ArcadeIdleEngine.Inventory
{
	public class InventoryManager : MonoBehaviour
	{
		[SerializeField] InventoryInvisible _inventoryInvisible;
		[SerializeField] InventoryVisible _inventoryVisible;
		
		IInteractor _interactor;

		public int upgradeCounter = 0;
		
		public bool IsInteractable => _interactor.IsInteractable;

		void Awake()
		{
			_interactor = GetComponent<IInteractor>();
			if (_interactor == null)
			{
				_interactor = new AlwaysEnableInteraction();
			}
		}

		void OnEnable()
		{
            _inventoryVisible.PickableAdded += OnPickableAdded;
            _inventoryVisible.PickableRemoved += OnPickableRemoved;
			_inventoryInvisible.PickableAdded += OnPickableAdded;
			_inventoryInvisible.PickableRemoved += OnPickableRemoved;
		}

        void OnDisable()
        {
            _inventoryVisible.PickableAdded -= OnPickableAdded;
            _inventoryVisible.PickableRemoved -= OnPickableRemoved;
            _inventoryInvisible.PickableAdded -= OnPickableAdded;
            _inventoryInvisible.PickableRemoved -= OnPickableRemoved;
        }

        public void SetCharacterCapacity(CharacterCapacity characterCapacity)
        {
	        _inventoryVisible._rowColumnHeight.RowCount = characterCapacity.currentCapacity;
	        _inventoryVisible._rowColumnHeight.additionFactor = characterCapacity.tempCapacityAdder;

	        upgradeCounter = characterCapacity.noOfUpgrades;
        }

        public CharacterCapacity GetCharacterCapacity()
        {
	        CharacterCapacity cap = new CharacterCapacity();

	        cap.currentCapacity = _inventoryVisible._rowColumnHeight.HeightCount;
	        cap.noOfUpgrades = upgradeCounter;
	        cap.tempCapacityAdder = _inventoryVisible._rowColumnHeight.additionFactor;
	        
	        return cap;

        }
        
        public void AddPickable(Pickable pickable)
		{
			if (pickable.Definition.Visible)
			{
				_inventoryVisible.AddPickable(pickable);
			}
			else
			{
				_inventoryInvisible.AddPickable(pickable);
			}
		}
        
        public bool CanTakePickable(PickableDefinition definition)
		{
			if (!_interactor.IsInteractable)
			{
				return false;
			}

			bool result = false;
			if (definition.CurrentItemType == ItemType.money)
			{
				result = true;
			}
			else
			{
				//Check For inventory here. Does the inventory have the same item as the item we are here to pick?
				
				result = definition.Visible ? _inventoryVisible.IsItemPickable(definition) : _inventoryInvisible.CanTakePickable();
			}
			
            return result;
		}

        public bool TakePickable(PickableDefinition definition, out Pickable pickable)
		{
			if (definition.Visible)
			{
				return _inventoryVisible.TryTakePickable(definition, out pickable);
			}
			
			return _inventoryInvisible.TryTakePickable(definition, out pickable);
		}

        public bool ContainsPickable(PickableDefinition definition)
		{
			return definition.Visible ? _inventoryVisible.ContainsPickable(definition) : _inventoryInvisible.ContainsPickable(definition);
		}

        public bool TakeRandomPickable(out Pickable pickable)
		{
			if (_inventoryInvisible.IsEmpty() && _inventoryVisible.IsEmpty())
			{
				pickable = null;
				return false;
			}
			
			if (!_inventoryVisible.IsEmpty() && !_inventoryInvisible.IsEmpty())
			{
				if (Random.value > 0.5f)
				{
					return TakeRandomInvisiblePickable(out pickable);
				}
			
				return TakeRandomVisiblePickable(out pickable);
			}
			else if (!_inventoryVisible.IsEmpty())
			{
				return TakeRandomVisiblePickable(out pickable);
			}
			else if (!_inventoryInvisible.IsEmpty())
			{
				return TakeRandomInvisiblePickable(out pickable);
			}

			pickable = null;
			return false;
		}
        
        void OnPickableRemoved(PickableDefinition p)
        {
	        if (p.Variable)
	        {
		        p.Variable.RuntimeValue -= 1;
	        }
        }

        void OnPickableAdded(PickableDefinition p)
        {
	        if (p.Variable)
	        {
		        p.Variable.RuntimeValue += 1;
	        }
        }

        bool TakeRandomInvisiblePickable(out Pickable pickable)
		{
			return _inventoryInvisible.TakeRandomSellablePickable(out pickable);
		}

        bool TakeRandomVisiblePickable(out Pickable pickable)
		{
			return _inventoryVisible.TakeRandomSellablePickable(out pickable);
		}
	}
}
