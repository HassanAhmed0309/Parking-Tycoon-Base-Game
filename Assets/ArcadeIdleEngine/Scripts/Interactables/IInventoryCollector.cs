using ArcadeBridge.ArcadeIdleEngine.Pickables;

namespace ArcadeBridge.ArcadeIdleEngine.Interactables
{
	public interface IInventoryCollector
	{
		int GetRequiredPickableDefinition(PickableDefinition[] results);
		void CollectFromInventory(Pickable pickable);
		float CollectingFromInventoryInterval { get; }
	}
}