using System;
using System.Collections.Generic;
using ArcadeBridge.ArcadeIdleEngine.Helpers;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ArcadeBridge.ArcadeIdleEngine.Gathering
{
	[SelectionBase, RequireComponent(typeof(Collider))]
	public class GatherableSource : MonoBehaviour
	{
		[field: SerializeField, Tooltip("Defines a lot of essential properties.")] 
		public GatherableDefinition GatherableDefinition { get; private set; }

		[SerializeField, Tooltip("Feedbacks will be played on this transform component.")]
		Transform _visualTransform;
		
		[SerializeField, Tooltip("As a reward, pickables will be spawned around the source.")]
		float _pickableSpawnRadius;

		List<Pickable> _instantiatedPickables = new List<Pickable>();
		int _currentHitPoint;

		public bool Depleted { get; private set; }
		
		public event Action<List<Pickable>> GatheredPickableInstantiated;

		void OnDestroy()
		{
			foreach (Pickable instantiatedPickable in _instantiatedPickables)
			{
				Destroy(instantiatedPickable);
			}
			_instantiatedPickables.Clear();
		}

		public void TakeHit(int amount)
		{
			if (_currentHitPoint == GatherableDefinition.MaxHitPoint)
			{
				return;
			}
			
			int newHitPoint = Mathf.Clamp(_currentHitPoint + amount, 0, GatherableDefinition.MaxHitPoint);
			bool isPickableSpawned = GatherableDefinition.Gather(_currentHitPoint, newHitPoint, out GatherableReward gatherableReward);
			if (isPickableSpawned)
			{
				for (int i = 0; i < gatherableReward.Amount; i++)
				{
					Vector3 randomPoint = Random.insideUnitCircle;
					randomPoint.z = randomPoint.y;
					randomPoint.y = 0f;
					randomPoint = randomPoint.normalized * _pickableSpawnRadius;
					Vector3 position = transform.position;
					Pickable item = gatherableReward.PickablePool.TakeFromPool();
					item.transform.position = position;
					_instantiatedPickables.Add(item);
					TweenHelper.Jump(item.transform, position + randomPoint, 2f, 1, 0.3f);
				}
				GatheredPickableInstantiated?.Invoke(_instantiatedPickables);
				
				if (GatherableDefinition.UseFeedbacks)
				{
					GatherableDefinition.PickableSpawnedFeedback.Play(_visualTransform);
				}
			}
			else
			{
				if (GatherableDefinition.UseFeedbacks)
				{
					GatherableDefinition.TakeHitFeedback.Play(_visualTransform);
				}
			}
			_currentHitPoint = newHitPoint;
			if (_currentHitPoint == GatherableDefinition.MaxHitPoint)
			{
				Depleted = true;
				TweenHelper.DisappearSlowly(_visualTransform, GatherableDefinition.DisappearingDuration, OnDisappeared);
			}
		}

		void OnDisappeared()
		{
			_visualTransform.gameObject.SetActive(false);
			TweenHelper.DelayedCall(GatherableDefinition.IdleDuration, OnRespawning);
		}

		void OnRespawning()
		{
			TweenHelper.ShowSlowly(_visualTransform, Vector3.one, GatherableDefinition.ReappearDuration, OnRespawned);
		}

		void OnRespawned()
		{
			_currentHitPoint = 0;
			Depleted = false;
		}
	}
}
