using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Helpers;
using ArcadeBridge.ArcadeIdleEngine.Interactables;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Processors.Spawners
{
    public class PickableSpawner : MonoBehaviour, IUpgradable, IPickableProvider
    {
        [SerializeField] SpawnerDefinition _definition;

        Transform _stackingPoint;
        Stack<Pickable> _spawnedPickables = new Stack<Pickable>();
        float _currentTimer;
        int _workSpeedMultiplier;
        
        public PickableDefinition ProductType => _definition.SpawningPickable;
        public float InventoryFeedingInterval => _definition.InventoryFeedingInterval;

        void Update()
        {
            if (_spawnedPickables.Count < _definition.SpawningLayout.GetCapacity())
            {
                _currentTimer += Time.deltaTime;

                if (_currentTimer >= _definition.SpawnInterval / (1f + _workSpeedMultiplier))
                {
                    Pickable pickable = _definition.SpawningPickable.Pool.TakeFromPool();
                    pickable.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
                    pickable.transform.SetParent(_stackingPoint);
                    
                    Vector3 point = ArcadeIdleHelper.GetPoint(_spawnedPickables.Count, _definition.SpawningLayout);
                    // TODO: scale of the stacking point causes the pickables to drop into wrong places
                    Matrix4x4 localToWorldMatrix = Matrix4x4.TRS(_stackingPoint.position, _stackingPoint.rotation, Vector3.one);
                    Vector3 adjustedPos = localToWorldMatrix.MultiplyPoint3x4(point);
                    TweenHelper.Jump(pickable.transform, adjustedPos, _definition.JumpHeight,1, _definition.JumpDuration);

                    _spawnedPickables.Push(pickable);
                    
                    _currentTimer = 0f;
                }
            }
        }

        public void Upgrade()
        {
            _workSpeedMultiplier++;
        }

        public void SetStockpilePoint(Transform stockpilePoint)
        {
            _stackingPoint = stockpilePoint;
            //Debug.Log("Stacking Point Set to : " + _stackingPoint.gameObject.name);
        }
        
        public Pickable GetProduct()
        {
            if (_spawnedPickables.Count > 0)
            {
                return _spawnedPickables.Pop();
            }

            return null;
        }
    }
}