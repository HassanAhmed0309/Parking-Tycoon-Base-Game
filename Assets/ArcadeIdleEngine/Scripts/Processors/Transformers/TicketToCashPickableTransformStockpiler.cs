using System;
using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Helpers;
using ArcadeBridge.ArcadeIdleEngine.Interactables;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Processors.Transformers
{
    public class TicketToCashPickableTransformStockpiler : MonoBehaviour, IUpgradable, IInventoryCollector, IPickableProvider
    {
        [SerializeField] StockpilerDefinition _stockpilerDefinition;
        [SerializeField] Transform _unmodifiedStockpilePoint;

        Stack<Pickable> _unmodifiedPickables = new Stack<Pickable>();
        Stack<Pickable> _modifiedPickables = new Stack<Pickable>();
        Transform _modifiedStockpilePoint;
        //float _currentModifiedItemTimer;
        float _currentStockpilingTimer;
        int _workSpeedMultiplier;

        [SerializeField] private TicketingSystem currentTicketingSystem;
        [SerializeField] private int inputItems;
        [SerializeField] private int outputItems;
        
        
        public PickableDefinition ProductType => _stockpilerDefinition.Ruleset.Output;
        public float InventoryFeedingInterval => _stockpilerDefinition.InventoryFeedingInterval;
        public float CollectingFromInventoryInterval => _stockpilerDefinition.CollectingFromInventoryInterval;
        bool IsUnmodifiedItemCapacityFull => _unmodifiedPickables.Count >= _stockpilerDefinition.RowColumnHeight.GetCapacity();

        private void Start()
        {
            currentTicketingSystem.giveRef?.Invoke(this);
        }

        public void SetStockpilePoint(Transform stockpilePoint)
        {
            _modifiedStockpilePoint = stockpilePoint;
        }

        public void Upgrade()
        {
            _workSpeedMultiplier++;
        }

        public int ReturnAmountOfCash()
        {
            return _modifiedPickables.Count;
        }
        
        
        public void SpawnCash(int amount)
        {
            for (int i = 0 ; i < amount; i++)
            {
                Pickable p = _stockpilerDefinition.Ruleset.Output.Pool.TakeFromPool();
                p.transform.position = _modifiedStockpilePoint.transform.position;
                JumpOrganized(p, _modifiedStockpilePoint, _modifiedPickables.Count);
                _modifiedPickables.Push(p);
            }
        }
        
        public Pickable GetProduct()
        {
            if (_modifiedPickables.Count > 0)
            {
                return _modifiedPickables.Pop();
            }
            return null;
        }

        public int GetRequiredPickableDefinition(PickableDefinition[] results)
        {
            if (IsUnmodifiedItemCapacityFull)
            {
                return 0;
            }
            
            results[0] = _stockpilerDefinition.Ruleset.Input;
            return 1;
        }

        public void CollectFromInventory(Pickable pickable)
        {
            JumpOrganized(pickable, _unmodifiedStockpilePoint, _unmodifiedPickables.Count);
            _unmodifiedPickables.Push(pickable);
            
            //Update the ticket System here
            //Add a new ticket to the system
            currentTicketingSystem.AddToTickets(1);
        }

        public void ModifyItem(CarMovement car)
        {
            //Replace pickableItemItem with the position of the car from where the cash will come.
            for (int i = 0 ; i < outputItems; i++)
            {
                Pickable p = _stockpilerDefinition.Ruleset.Output.Pool.TakeFromPool();
                p.transform.position = car.transform.position;
                JumpOrganized(p, _modifiedStockpilePoint, _modifiedPickables.Count);
                _modifiedPickables.Push(p);
                //_currentModifiedItemTimer = 0f;
            }
            //Set destination for despawn point
            car.SetNewDestination(car.despawnPoint);
        }

        public void RemoveTheItem()
        {
            Pickable pickableItem = _unmodifiedPickables.Pop();
            pickableItem.DisappearSlowlyToPool();
        }

        void JumpOrganized(Pickable pickableItem, Transform pivotPoint, int index)
        {
            Vector3 point = ArcadeIdleHelper.GetPoint(index, _stockpilerDefinition.RowColumnHeight);
            Vector3 adjustedPos = pivotPoint.TransformPoint(point);
            TweenHelper.Jump(pickableItem.transform, adjustedPos, _stockpilerDefinition.JumpHeight, 1, _stockpilerDefinition.JumpDuration);
        }
    }
}