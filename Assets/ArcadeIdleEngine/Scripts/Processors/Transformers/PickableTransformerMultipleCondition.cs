using System;
using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Helpers;
using ArcadeBridge.ArcadeIdleEngine.Interactables;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Processors.Transformers
{
    public class PickableTransformerMultipleCondition : MonoBehaviour, IPickableProvider, IInventoryCollector
    {
        public event Action<PickableDefinition, int> Collected;
        public event Action Produced;

        [SerializeField] MultipleConditionDefinition _multipleConditionDefinition;

        Transform _modifiedStockpilePoint;
        Dictionary<PickableDefinition, int> _neededResources = new Dictionary<PickableDefinition, int>();
        Stack<Pickable> _spawnedPickables = new Stack<Pickable>();
        float _currentModifiedItemTimer;
        int _workSpeedMultiplier;

        public PickableDefinition ProductType => _multipleConditionDefinition.Ruleset.Output.PickableDefinition;
        public MultipleConditionRuleset Ruleset => _multipleConditionDefinition.Ruleset;
        public float InventoryFeedingInterval => _multipleConditionDefinition.InventoryFeedingInterval;
        public float CollectingFromInventoryInterval => _multipleConditionDefinition.CollectingFromInventoryInterval;

        void Awake()
        {
            foreach (PickableDefinitionCountPair rulesetTypeCountPair in _multipleConditionDefinition.Ruleset.Inputs)
            {
                _neededResources.Add(rulesetTypeCountPair.PickableDefinition, rulesetTypeCountPair.Count);
            }
        }

        public void SetStockpilePoint(Transform stockpilePoint)
        {
            _modifiedStockpilePoint = stockpilePoint;
        }

        public Pickable GetProduct()
        {
            if (_spawnedPickables.Count > 0)
            {
                return _spawnedPickables.Pop();
            }

            return null;
        }

        public int GetRequiredPickableDefinition(PickableDefinition[] results)
        {
            int i = 0;
            foreach (KeyValuePair<PickableDefinition, int> neededResource in _neededResources)
            {
                if (neededResource.Value > 0)
                {
                    results[i] = neededResource.Key;
                    i++;
                }
            }
            return i;
        }

        public void CollectFromInventory(Pickable pickable)
        {
            TweenHelper.JumpToDisappearIntoPool(pickable, transform.position, _multipleConditionDefinition.JumpHeight, 1, _multipleConditionDefinition.JumpDuration);
            _neededResources[pickable.Definition] -= 1;
            Collected?.Invoke(pickable.Definition, _neededResources[pickable.Definition]);
            if (_neededResources[pickable.Definition] <= 0)
            {
                ProduceOutput();
            }
        }

        void JumpOrganized(Pickable pickableItem, Transform pivotPoint, int index)
        {
            Vector3 point = ArcadeIdleHelper.GetPoint(index, _multipleConditionDefinition.RowColumnHeight);
            Vector3 adjustedPos = pivotPoint.TransformPoint(point);
            TweenHelper.Jump(pickableItem.transform, adjustedPos, _multipleConditionDefinition.JumpDuration);
        }

        void ProduceOutput()
        {
            foreach (var neededResource in _neededResources)
            {
                if (neededResource.Value > 0)
                {
                    return;
                }
            }

            for (int i = 0; i < _multipleConditionDefinition.Ruleset.Output.Count; i++)
            {
                Pickable p = _multipleConditionDefinition.Ruleset.Output.PickableDefinition.Pool.TakeFromPool();
                p.transform.position = transform.position;
                JumpOrganized(p, _modifiedStockpilePoint, _spawnedPickables.Count);
                _spawnedPickables.Push(p);
            }
            
            foreach (PickableDefinitionCountPair rulesetTypeCountPair in _multipleConditionDefinition.Ruleset.Inputs)
            {
                _neededResources[rulesetTypeCountPair.PickableDefinition] = rulesetTypeCountPair.Count;
            }
            Produced?.Invoke();
        }
    }
}