using ArcadeBridge.ArcadeIdleEngine.Data;
using TMPro;
using UnityEngine;

namespace ArcadeBridge.ArcadeIdleEngine.Monitors
{
	public class IntVariableMonitor : MonoBehaviour
	{
		[SerializeField] IntVariable _monitorVariable;
		[SerializeField] TextMeshProUGUI _monitorText;

		void OnEnable()
		{
			_monitorVariable.ValueChanged += SetText;
		}
		
		void OnDisable()
		{
			_monitorVariable.ValueChanged -= SetText;
		}

		void Start()
		{
			SetText(_monitorVariable.RuntimeValue);
		}

		void SetText(int obj)
		{
			_monitorText.text = obj.ToString();
		}
	}
}
