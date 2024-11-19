using System.Collections;
using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Inventory;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Interactables
{
	/// <summary>
	/// This compoonent collects pickables from inventory (this can be AI, player, multiple thing at once) to ICollectibleFromInventory implementation. 
	/// </summary>
	public class InventoryCollectingZone : MonoBehaviour
	{
		[SerializeField] GameObject _iCollectableByInventory;

		IInventoryCollector _inventoryCollector;
		Dictionary<InventoryManager, Coroutine> _coroutineDictionary = new Dictionary<InventoryManager, Coroutine>();
		PickableDefinition[] _results = new PickableDefinition[10];
		float _collectingTimer;
		float _collectingIntervalInSeconds;

		void Awake()
		{
			_inventoryCollector = _iCollectableByInventory.GetComponent<IInventoryCollector>();
			if (_inventoryCollector == null)
			{
				Debug.LogError("Inventory Collector not found.", this);
				return;
			}

			_collectingIntervalInSeconds = _inventoryCollector.CollectingFromInventoryInterval;
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent(out InventoryManager inventory))
			{
				_coroutineDictionary.Add(inventory, StartCoroutine(Co_Collect(inventory)));
			}
		}
		
		void OnTriggerExit(Collider other)
		{
			if (other.TryGetComponent(out InventoryManager inventory))
			{
				StopCoroutine(_coroutineDictionary[inventory]);
				_coroutineDictionary.Remove(inventory);
				_collectingTimer = 0f;
			}
		}

		IEnumerator Co_Collect(InventoryManager inventoryManager)
		{
			while (true)
			{
				if (_collectingTimer >= _collectingIntervalInSeconds)
				{
					int results = _inventoryCollector.GetRequiredPickableDefinition(_results); 
					if (results == 0)
					{
						yield return null;
						continue;
					}

					
					for (int i = 0; i < results; i++)
					{
						if (inventoryManager.ContainsPickable(_results[i]))
						{
							inventoryManager.TakePickable(_results[i], out Pickable pickable);
							_inventoryCollector.CollectFromInventory(pickable);
							_collectingTimer = 0f;
							break;
						}
					}
				}
				else
				{
					_collectingTimer += Time.deltaTime;
				}

				yield return null;
			}
		}
	}
}
