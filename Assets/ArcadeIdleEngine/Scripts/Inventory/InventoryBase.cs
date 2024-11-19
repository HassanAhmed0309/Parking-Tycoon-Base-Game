using System;
using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ArcadeBridge.ArcadeIdleEngine.Inventory
{
    [Serializable]
    public abstract class InventoryBase
    {
        [SerializeField] protected Transform StackingPoint;
        [SerializeField] protected float PickUpDuration = 0.3f;
        [SerializeField] protected Ease PickUpEase = Ease.OutCubic;

        protected List<Pickable> Pickables = new List<Pickable>();
        protected ItemType currInventoryType = ItemType.none;
        
        
        public event Action<PickableDefinition> PickableAdded;
        public event Action<PickableDefinition> PickableRemoved;

        protected int Count => Pickables.Count;
        
        public bool IsEmpty()
        {
            return Pickables.Count == 0;
        }

        public bool TakeRandomSellablePickable(out Pickable pickable)
        {
            if (Pickables.Count > 0)
            {
                int rnd = Random.Range(0, Pickables.Count);
                pickable = Pickables[rnd];
                if (pickable.Definition.Sellable)
                {
                    TakePickable(pickable);
                    return true;    
                }
            }

            pickable = null;
            return false;
        }

        public bool IsItemPickable(PickableDefinition definition)
        {
            bool result = CanTakePickable();
            
            //Check the state of inventory at this point
            if (definition.CurrentItemType == currInventoryType)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            
            return result;
        }
        
        public bool CanTakePickable()
        {
            bool canTake = !IsCapacityFull();
            return canTake;
        }

        public bool TryTakePickable(PickableDefinition definition, out Pickable pickable)
        {
            for (int i = Pickables.Count - 1; i >= 0; i--)
            {
                Pickable item = Pickables[i];
                if (item.Definition == definition)
                {
                    pickable = item;
                    TakePickable(pickable);
                    return true;
                }
            }

            pickable = null;
            return false;
        }

        public bool ContainsPickable(PickableDefinition definition)
        {
            foreach (Pickable item in Pickables)
            {
                if (item.Definition == definition)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddPickable(Pickable p)
        {
            MovePickable(p);
            Pickables.Add(p);
            if (p.Definition.CurrentItemType != ItemType.money)
            {
                currInventoryType = p.Definition.CurrentItemType;
            }
            PickableAdded?.Invoke(p.Definition);
        }

        protected abstract void MovePickable(Pickable pickable);
        protected abstract bool IsCapacityFull();
        protected virtual void OnPickableRemoving(Pickable pickable)
        {
        }
       
        void TakePickable(Pickable pickable)
        {
            OnPickableRemoving(pickable);
            Pickables.Remove(pickable);
            pickable.transform.SetParent(null);
            pickable.gameObject.SetActive(true);
            PickableRemoved?.Invoke(pickable.Definition);
        }
    }
}