using System.Collections.Generic;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Data
{
	[CreateAssetMenu(menuName = nameof(ArcadeIdleEngine) + "/" + nameof(Data) + "/" + nameof(UniqueIntListVariable))]
	public class UniqueIntListVariable : Saveable<List<int>>
	{
		public override void RestoreState(object obj)
		{
			if (obj == null)
			{
				obj = GetDefaultValue;
			}
			var list = (List<int>)obj;
			RuntimeValue = new List<int>(list);
		}

		public void AddElement(int element)
		{
			if (RuntimeValue.Contains(element))
			{
				Debug.LogError($"you are trying to add element {element} twice into the {name} list");
				return;
			}
			
			RuntimeValue.Add(element);
			OnValueChanged(RuntimeValue);
		}

		public void Sort()
		{
			RuntimeValue.Sort();
		}
		
		public void AddElementAt(int index, int element)
		{
			if (RuntimeValue.Contains(element))
			{
				Debug.LogError($"you are trying to add {element} element twice into the list");
				return;
			}
			
			RuntimeValue[index] = element;
			OnValueChanged(RuntimeValue);
		}

		public bool Contains(int element)
		{
			return RuntimeValue.Contains(element);
		}
	}
}
