using ArcadeBridge.ArcadeIdleEngine.Processors;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Helpers
{
	public static class ArcadeIdleHelper
	{
		public static Vector3 GetPoint(int currentIndex, RowColumnHeight rowColumnHeight)
		{
			float maxRowWidth = (rowColumnHeight.RowCount - 1) * rowColumnHeight.RowColumnOffset;
			float maxColumnWidth = (rowColumnHeight.ColumnCount - 1) * rowColumnHeight.RowColumnOffset;
			int columnIndex = currentIndex % rowColumnHeight.ColumnCount;
			int rowIndex = currentIndex / rowColumnHeight.ColumnCount % rowColumnHeight.RowCount;
			int heightIndex = currentIndex / (rowColumnHeight.RowCount * rowColumnHeight.ColumnCount);
			Vector3 up = Vector3.up * (rowColumnHeight.HeightOffset * heightIndex);
			Vector3 right = Vector3.right * (columnIndex * rowColumnHeight.RowColumnOffset - maxColumnWidth / 2f);
			Vector3 forward = Vector3.forward * (rowIndex * rowColumnHeight.RowColumnOffset - maxRowWidth / 2f);
			Vector3 targetPos = up + right + forward;
			return targetPos;
		}
	}
}
