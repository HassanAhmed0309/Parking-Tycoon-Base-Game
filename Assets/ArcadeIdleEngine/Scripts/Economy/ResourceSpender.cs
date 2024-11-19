using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Helpers;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using ArcadeBridge.ArcadeIdleEngine.Pools;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Economy
{
	public class ResourceSpender : MonoBehaviour
	{
		const int VISUAL_FEEDBACK_SPAWN_RATE_MAX = 40;
		
		[SerializeField] PickablePool[] _spendablePickables;
		[SerializeField, Tooltip("controls how frequent resource object will be shown"), Range(1, VISUAL_FEEDBACK_SPAWN_RATE_MAX)] 
		int _visualFeedbackSpawnRate;
		[SerializeField, Range(0f, 10f)] float _jumpHeight;
		[SerializeField, Range(0f, 3f)] float _jumpDuration;

		Dictionary<PickableDefinition, PickablePool> _spendableResources;
		int _spawnCount;

		void Awake()
		{
			_spendableResources = new Dictionary<PickableDefinition, PickablePool>();
			foreach (PickablePool resourceSpenderData in _spendablePickables)
			{
				_spendableResources.Add(resourceSpenderData.PickableDefinition, resourceSpenderData);
			}
		}

		public void Spend(PickableDefinition pickableDefinition, int amount, Transform moneyTargetPoint)
		{
			pickableDefinition.Variable.RuntimeValue -= amount;
			if (amount == 0)
			{
				return;
			}
			_spawnCount++;
			if (_spawnCount >= VISUAL_FEEDBACK_SPAWN_RATE_MAX + 1 - _visualFeedbackSpawnRate)
			{
				PickablePool pool = _spendableResources[pickableDefinition];
				Pickable pickable = pool.TakeFromPool();
				Transform trans = pickable.transform;
				trans.position = transform.position;
				TweenHelper.Jump(trans, moneyTargetPoint.position, _jumpHeight, 1, _jumpDuration, pickable.ReleaseToPool);
				_spawnCount = 0;
			}
		}
	}
}