using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Processors.Transformers
{
	[CreateAssetMenu(menuName = nameof(ArcadeIdleEngine) + "/" + nameof(Processors) + "/" + nameof(Transformers) + "/" + nameof(MultipleConditionDefinition))]
	public class MultipleConditionDefinition : ScriptableObject
	{
		[field: SerializeField] public MultipleConditionRuleset Ruleset { get; private set; }
		[field: SerializeField] public RowColumnHeight RowColumnHeight { get; private set; }
		[field: SerializeField, Range(0f, 10f)] public float JumpHeight { get; private set; }
		[field: SerializeField, Range(0.01f, 2f)] public float JumpDuration { get; private set; }
		[field: SerializeField, Range(0.01f, 2f)] public float InventoryFeedingInterval { get; private set; }
		[field: SerializeField, Range(0.01f, 2f)] public float CollectingFromInventoryInterval { get; private set; }
	}
}