using ArcadeBridge.ArcadeIdleEngine.Helpers;
using ArcadeBridge.ArcadeIdleEngine.Interactables;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Processors.Sellers
{
	[SelectionBase]
	public class PickableSellerFloatingImage : MonoBehaviour, IInventoryCollector
	{
		[SerializeField] SellerFloatingImageDefinition _definition;
		
		Camera _camera;

		public float CollectingFromInventoryInterval => _definition.CollectingFromInventoryInterval;
		
		void Awake()
		{
			_camera = Camera.main;
		}

		void OnJump(Pickable pickable)
		{
			pickable.ReleaseToPool();
			Vector3 point = _camera.WorldToScreenPoint(pickable.transform.position);
			_definition.FloatingImageResourceAnimator.Play(point, pickable.SellValue);
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
	}
}
