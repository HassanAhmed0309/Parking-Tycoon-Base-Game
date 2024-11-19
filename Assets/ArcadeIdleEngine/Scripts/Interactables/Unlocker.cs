using System.Collections;
using ArcadeBridge.ArcadeIdleEngine.Economy;
using ArcadeBridge.ArcadeIdleEngine.Helpers;
using ArcadeBridge.ArcadeIdleEngine.Inventory;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using ArcadeIdleEngine.ExternalAssets.NaughtyAttributes_2._1._4.Core.MetaAttributes;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ArcadeBridge.ArcadeIdleEngine.Interactables
{
	public class Unlocker : MonoBehaviour
	{
		[SerializeField, Tooltip("This will be called when enough resource spent.")] 
		UnityEvent _onUnlocked;
		
		[SerializeField, Tooltip("Text which shows how much resource do we have need to unlock."), BoxGroup("UI")] 
		TextMeshProUGUI _resourceCountText;
		
		[SerializeField, Tooltip("Image which shows how much resource do we have need to unlock."), BoxGroup("UI")] 
		Image _progressBar;
		
		[SerializeField, Tooltip("Resource to spend when unlocking."), BoxGroup("Spending")] 
		PickableDefinition _neededResource;
		
		[SerializeField, BoxGroup("Spending")] Ease _spendingSpeedCurve;
		[SerializeField, Range(0f, 100f), BoxGroup("Spending")] float _spendingSpeed;
		
		[SerializeField, Tooltip("Amount of resource needed for unlocking."), Min(0), BoxGroup("Spending")] 
		int _requiredResource;
		
		[SerializeField, Tooltip("If true, then it will be locked again when it's unlocked so it can be unlocked multiple times."), BoxGroup("Spending")] 
		bool _workMultipleTimes;

		InventoryManager _inventoryManager;
		ResourceSpender _resourceSpender;
		Tween _spendingTween;
		Coroutine _cor;
		WaitForSeconds _waitForSeconds;
		int _previousResourceSpentAmount;
		int _collectedResource;
		
		void Awake() 
		{
			_waitForSeconds = new WaitForSeconds(0.1f);
		}
		
		void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent(out InventoryManager inventory) && other.TryGetComponent(out ResourceSpender resourceSpender))
			{
				_inventoryManager = inventory;
				_resourceSpender = resourceSpender;
				_cor = StartCoroutine(CheckInventory(inventory));
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.TryGetComponent(out InventoryManager _) && other.TryGetComponent(out ResourceSpender resourceSpender))
			{
				StopSpending();
				_inventoryManager = null;
				_resourceSpender = null;
			}
		}

		void OnValidate()
		{
			_resourceCountText.text = _requiredResource.ToString();
		}
		
		public void SetRequiredResource(int requiredResource)
		{
			_collectedResource = 0;
			_previousResourceSpentAmount = 0;
			_progressBar.fillAmount = 0;
			_requiredResource = requiredResource;
			_resourceCountText.text = _requiredResource.ToString();
		}

		IEnumerator CheckInventory(InventoryManager inventoryManager)
		{
			while (true)
			{
				if (!inventoryManager.IsInteractable)
				{
					yield return _waitForSeconds;
					continue;
				}

				int resourceAmount = _neededResource.Variable.RuntimeValue;
				TweenHelper.SpendResource(_requiredResource, _collectedResource, resourceAmount, out _spendingTween, _spendingSpeed, _spendingSpeedCurve, SpendMoney);
				yield break;
			}
		}

		void SpendMoney(int x)
		{
			int decreasingAmountDelta = x - _previousResourceSpentAmount;
			_resourceSpender.Spend(_neededResource, decreasingAmountDelta, transform);
			_collectedResource += decreasingAmountDelta;
			_resourceCountText.text = (_requiredResource - _collectedResource).ToString();
			_previousResourceSpentAmount = x;
			_progressBar.fillAmount = (float)_collectedResource / _requiredResource;
			
			
			if (_collectedResource >= _requiredResource)
			{
				_onUnlocked?.Invoke();
				StopSpending();

				if (_workMultipleTimes)
				{
					_requiredResource += 1000;
					_cor = StartCoroutine(CheckInventory(_inventoryManager));
				}
				else
				{
					gameObject.SetActive(false);
				}
			}
		}
		
		void StopSpending()
		{
			_spendingTween?.Kill();
			if (_cor != null)
			{
				StopCoroutine(_cor);				
			}
		}
	}
}