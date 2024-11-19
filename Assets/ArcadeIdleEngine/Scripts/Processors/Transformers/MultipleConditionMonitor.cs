using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Processors.Transformers
{
    public class MultipleConditionMonitor : MonoBehaviour
	{
		[SerializeField] PickableTransformerMultipleCondition _pickableTransformerMultipleCondition;
		[SerializeField] ConditionMonitorEntity[] _inputs;
		[SerializeField] ConditionMonitorEntity _output;
		
		Dictionary<PickableDefinition, ConditionMonitorEntity> _pickableMonitors;

		void Awake()
		{
			_pickableMonitors = new Dictionary<PickableDefinition, ConditionMonitorEntity>();

			if (_inputs.Length != _pickableTransformerMultipleCondition.Ruleset.Inputs.Length)
			{
				Debug.LogWarning("Not enough text fields to monitor! Add new text fields so monitor can show them", gameObject);
				return;
			}
			
			for (int i = 0; i < _pickableTransformerMultipleCondition.Ruleset.Inputs.Length; i++)
			{
				PickableDefinitionCountPair pickableDefinitionCountPair = _pickableTransformerMultipleCondition.Ruleset.Inputs[i];
				_pickableMonitors.Add(pickableDefinitionCountPair.PickableDefinition, _inputs[i]);
				_inputs[i].Initialize(pickableDefinitionCountPair.PickableDefinition.Sprite, pickableDefinitionCountPair.Count.ToString());
			}
			
			_output.Initialize(_pickableTransformerMultipleCondition.Ruleset.Output.PickableDefinition.Sprite, _pickableTransformerMultipleCondition.Ruleset.Output.Count.ToString());
		}

		void OnEnable()
		{
			_pickableTransformerMultipleCondition.Collected += PickableTransformerMultipleConditionCollected;
			_pickableTransformerMultipleCondition.Produced += PickableTransformerMultipleConditionProduced;
		}

		void OnDisable()
		{
			_pickableTransformerMultipleCondition.Collected -= PickableTransformerMultipleConditionCollected;
			_pickableTransformerMultipleCondition.Produced -= PickableTransformerMultipleConditionProduced;
		}

		void OnValidate()
		{
			_pickableTransformerMultipleCondition ??= GetComponentInParent<PickableTransformerMultipleCondition>();
		}

		void PickableTransformerMultipleConditionProduced()
		{
			for (int i = 0; i < _pickableTransformerMultipleCondition.Ruleset.Inputs.Length; i++)
			{
				PickableDefinitionCountPair pair = _pickableTransformerMultipleCondition.Ruleset.Inputs[i];
				_pickableMonitors[pair.PickableDefinition].SetText(pair.Count.ToString());
			}
		}

		void PickableTransformerMultipleConditionCollected(PickableDefinition arg1, int arg2)
		{
			_pickableMonitors[arg1].SetText(arg2.ToString());
		}
	}
}
