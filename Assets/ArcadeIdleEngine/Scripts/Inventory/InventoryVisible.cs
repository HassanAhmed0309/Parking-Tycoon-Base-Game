using System;
using ArcadeBridge.ArcadeIdleEngine.Helpers;
using ArcadeBridge.ArcadeIdleEngine.Pickables;
using ArcadeBridge.ArcadeIdleEngine.Processors;
using DG.Tweening;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Inventory
{
    [Serializable]
	public class InventoryVisible : InventoryBase
	{
		[SerializeField] public RowColumnHeight _rowColumnHeight;

		protected override void MovePickable(Pickable pickable)
		{
			Vector3 targetPos = ArcadeIdleHelper.GetPoint(Count, _rowColumnHeight);
			Transform trans = pickable.transform;
			trans.DOKill();
			trans.SetParent(StackingPoint);
			trans.DOLocalRotate(Vector3.zero, PickUpDuration).SetRecyclable();
			trans.DOLocalMove(targetPos, PickUpDuration).SetEase(PickUpEase).SetRecyclable();
		}
		protected override bool IsCapacityFull()
		{
			return Count >= _rowColumnHeight.GetCapacity();
		}

		protected override void OnPickableRemoving(Pickable pickable)
		{
			int indexOf = Pickables.IndexOf(pickable);
			if (Pickables.Count > indexOf + 1)
			{
				for (int i = indexOf + 1; i < Pickables.Count; i++)
				{
					// TODO: Fix this
					Pickables[i].transform.DOLocalMove(Pickables[i - 1].transform.localPosition, 0.1f).SetRecyclable();
				}
			}
		}
	}
}
