using System;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using DG.Tweening;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Inventory
{
    [Serializable]
	public class InventoryInvisible : InventoryBase
	{
		[SerializeField] int _capacity;

		protected override void MovePickable(Pickable pickable)
		{
			Transform trans = pickable.transform;
			trans.DOKill();
			trans.SetParent(StackingPoint);
			trans.DOLocalRotate(Vector3.zero, PickUpDuration).SetRecyclable();
			trans.DOLocalMove(Vector3.zero, PickUpDuration).SetEase(PickUpEase).SetRecyclable().OnComplete(() =>
			{
				pickable.gameObject.SetActive(false);
			});
		}
		
		protected override bool IsCapacityFull()
		{
			return Count >= _capacity;
		}
	}
}
