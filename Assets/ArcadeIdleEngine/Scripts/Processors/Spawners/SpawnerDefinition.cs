using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Processors.Spawners
{
	[CreateAssetMenu(menuName = nameof(ArcadeIdleEngine) + "/" + nameof(Processors) + "/" + nameof(Spawners)+ "/" + nameof(SpawnerDefinition))]
	public class SpawnerDefinition : ScriptableObject
	{
		[field: SerializeField] public PickableDefinition SpawningPickable { get; set; }
		[field: SerializeField, Min(0.02f)] public float SpawnInterval { get; set; }
		[field: SerializeField] public float InventoryFeedingInterval { get; set; }
		[field: SerializeField] public RowColumnHeight SpawningLayout { get; set; }
		[field: SerializeField] public float JumpDuration { get; set; }
		[field: SerializeField] public float JumpHeight { get; set; }
	}
}
