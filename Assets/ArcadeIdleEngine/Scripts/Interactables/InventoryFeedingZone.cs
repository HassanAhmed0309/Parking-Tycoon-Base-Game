using System.Collections;
using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Inventory;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Interactables
{
    /// <summary>
    /// Gives Pickable to the InventoryManager OnTrigger, one InventoryManager at a time. Multiple InventoryManager can't collect simultaneously.
    /// </summary>
    public class InventoryFeedingZone : MonoBehaviour
    {
        [SerializeField] GameObject _iCollectableContainedGameObject;
        [SerializeField] private GameObject StackingPoint = null;
        
        Dictionary<InventoryManager, Coroutine> _coroutineDictionary = new Dictionary<InventoryManager, Coroutine>();
        IPickableProvider _pickableProvider;
        InventoryManager _inventoryManager;
        float _feedingTimer;
        float _feedingIntervalInSeconds;
        
        

        void Awake()
        {
            _pickableProvider = _iCollectableContainedGameObject.GetComponent<IPickableProvider>();
            if (_pickableProvider == null)
            {
                Debug.LogError(_iCollectableContainedGameObject.name + " doesn't have any ICollectable implementations. PLease attach a component that has ICollectable.", this);
                return;
            }
            
            if(StackingPoint == null)
                _pickableProvider.SetStockpilePoint(transform);
            else
                _pickableProvider.SetStockpilePoint(StackingPoint.transform);
            
            _feedingIntervalInSeconds = _pickableProvider.InventoryFeedingInterval;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out InventoryManager inventoryManager))
            {
                _coroutineDictionary.Add(inventoryManager, StartCoroutine(Co_Collect(inventoryManager)));
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out InventoryManager inventoryManager))
            {
                StopCoroutine(_coroutineDictionary[inventoryManager]);
                _coroutineDictionary.Remove(inventoryManager);
                
            }
        }

        IEnumerator Co_Collect(InventoryManager inventory)
        {
            while (true)
            {
                if (!inventory.CanTakePickable(_pickableProvider.ProductType))
                {
                    yield return null;
                    continue;
                }

                if (_feedingTimer >= _feedingIntervalInSeconds)
                {
                    Pickable pickable = _pickableProvider.GetProduct();
                    if (pickable)
                    {
                        inventory.AddPickable(pickable);
                        _feedingTimer = 0f;
                    }    
                }
                else
                {
                    _feedingTimer += Time.deltaTime;
                }
                
                yield return null;
            }
        }
    }
}