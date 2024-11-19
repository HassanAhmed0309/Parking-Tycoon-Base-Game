using System.Collections;
using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Actors;
using ArcadeBridge.ArcadeIdleEngine.Data.Database;
using ArcadeBridge.ArcadeIdleEngine.Inventory;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Gathering
{
	public class PickableGatherer : MonoBehaviour
	{
		[SerializeField] GatheringToolDefinitionLookup gatheringToolDefinitionLookup;
		[SerializeField] InventoryManager _inventoryManager;
		[SerializeField] Transform _playerHand;
		[SerializeField] HumanoidAnimationManager _humanoidAnimationManager;

		List<GatherableSource> _gatherableSources = new List<GatherableSource>();
		GatheringTool _activeGatheringTool;
		WaitForSeconds _delayedCollectWait = new WaitForSeconds(0.7f);

		void Update()
		{
			if (!_inventoryManager.IsInteractable)
			{
				_humanoidAnimationManager.StopInteraction();
				if (_activeGatheringTool)
				{
					_activeGatheringTool.enabled = false;
				}
				return;
			}

			int gatherableSourcesCount = _gatherableSources.Count;
			if (!_activeGatheringTool && gatherableSourcesCount > 0)
			{
				TryInstantiateTool(_gatherableSources[0]);
			}

			if (_activeGatheringTool)
			{
				_activeGatheringTool.enabled = true;
				_humanoidAnimationManager.PlayInteraction(_activeGatheringTool.GatheringToolDefinition.InteractionAnimationId, 1f / _activeGatheringTool.GatheringToolDefinition.UseInterval);
			}
			else
			{
				return;
			}

			bool hasNonDepletedSource = false; 
			foreach (GatherableSource source in _activeGatheringTool.GatherableSources)
			{
				if (!source.Depleted)
				{
					hasNonDepletedSource = true;
					break;
				}
			}
			if (!hasNonDepletedSource)
			{
				_humanoidAnimationManager.StopInteraction();
				if (_activeGatheringTool)
				{
					_activeGatheringTool.enabled = false;
				}
			}

			if (gatherableSourcesCount == 0)
			{
				return;
			}
			for (int i = gatherableSourcesCount - 1; i >= 0; i--)
			{
				GatherableSource gatherableSource = _gatherableSources[i];
				_activeGatheringTool.AddGatherable(gatherableSource);
				gatherableSource.GatheredPickableInstantiated += GatherableSource_GatheredPickableInstantiated;
				_gatherableSources.RemoveAt(i);
			}
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent(out GatherableSource gatherablePickableSource))
			{
				_gatherableSources.Add(gatherablePickableSource);
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.TryGetComponent(out GatherableSource gatherablePickableSource))
			{
				_gatherableSources.Remove(gatherablePickableSource);
				if (!_activeGatheringTool)
				{
					return;
				}
				
				_activeGatheringTool.RemoveGatherable(gatherablePickableSource);
				gatherablePickableSource.GatheredPickableInstantiated -= GatherableSource_GatheredPickableInstantiated;
				if (!_activeGatheringTool.HasInteractableGatherable)
				{
					Destroy(_activeGatheringTool.gameObject);
					_activeGatheringTool = null;
					_humanoidAnimationManager.StopInteraction();
				}
			}
		}

		bool TryInstantiateTool(GatherableSource gatherableSource)
		{
			int highestTierIndex = -99999;
			int prefabIndex = -1;
			List<GatheringToolDefinition> availableObjects = gatheringToolDefinitionLookup.AvailableObjects;
			for (int i = 0; i < availableObjects.Count; i++)
			{
				GatheringToolDefinition tool = availableObjects[i];
				if (tool.CanGather(gatherableSource.GatherableDefinition) && tool.Tier > highestTierIndex)
				{
					highestTierIndex = tool.Tier;
					prefabIndex = i;
				}
			}

			if (prefabIndex != -1)
			{
				_activeGatheringTool = Instantiate(availableObjects[prefabIndex].GatheringToolPrefab, _playerHand);
				return true;
			}
			else
			{
				return false;
			}
		}

		void GatherableSource_GatheredPickableInstantiated(List<Pickable> pickables)
		{
			StartCoroutine(DelayedAddPickable(pickables));
		}

		IEnumerator DelayedAddPickable(List<Pickable> pickables)
		{
			yield return _delayedCollectWait;
			foreach (Pickable pickable in pickables)
			{
				_inventoryManager.AddPickable(pickable);
			}
			pickables.Clear();
		}
	}
}
