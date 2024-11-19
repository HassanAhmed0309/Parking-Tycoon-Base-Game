using ArcadeBridge.ArcadeIdleEngine.Helpers;
using ArcadeBridge.ArcadeIdleEngine.Interactables;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Processors.Sellers
{
	[SelectionBase]
    public class PickableSellerFloatingText : MonoBehaviour, IInventoryCollector
	{
		[SerializeField] SellerFloatingTextDefinition _definition;

		Camera _camera;

		public float CollectingFromInventoryInterval => _definition.CollectingFromInventoryInterval;
		
		void Awake()
		{
			_camera = Camera.main;
		}

		public int GetRequiredPickableDefinition(PickableDefinition[] results)
		{
			int length = _definition.PickablesForSale.Length;
			for (int i = 0; i < length; i++)
			{
				results[i] = _definition.PickablesForSale[i];
			}
			return _definition.PickablesForSale.Length;
		}

		public void CollectFromInventory(Pickable pickable)
		{
			TweenHelper.Jump(pickable.transform, transform.position, _definition.JumpHeight, 1, _definition.JumpDuration, () => OnJump(pickable));
		}
		
		void OnJump(Pickable pickable)
		{
			pickable.ReleaseToPool();
			int pickableSellValue = pickable.SellValue;
			_definition.IncomeResource.RuntimeValue += pickableSellValue;
			_definition.FloatingTextResourceAnimator.Play(transform, _camera.transform, pickableSellValue);
		}
	}
}
