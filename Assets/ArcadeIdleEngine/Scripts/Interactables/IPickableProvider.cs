using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Interactables
{
	public interface IPickableProvider
	{
		void SetStockpilePoint(Transform stockpilePoint);
		PickableDefinition ProductType { get; }
		float InventoryFeedingInterval { get; }
		Pickable GetProduct();
	}
}