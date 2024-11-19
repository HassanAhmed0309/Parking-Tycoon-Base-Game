using System.Collections;
using ArcadeBridge.ArcadeIdleEngine.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ArcadeBridge.ArcadeIdleEngine.Booting
{
	public class GameBooter : MonoBehaviour
	{
		[SerializeField] IntVariable _currentLevel;
		[SerializeField] Image _progressBarImage;

		bool _saveFileRestored;

		void OnEnable()
		{
			_currentLevel.ValueChanged += CurrentLevel_ValueChanged;
		}

		void OnDisable()
		{
			_currentLevel.ValueChanged -= CurrentLevel_ValueChanged;
		}

		void CurrentLevel_ValueChanged(int obj)
		{
			_saveFileRestored = true;
		}
		
		IEnumerator Start()
		{
			while (!_saveFileRestored)
			{
				yield return null;
			}

			if (_currentLevel.RuntimeValue == 0)
			{
				_currentLevel.RuntimeValue = 1;
			}
			AsyncOperation operation = SceneManager.LoadSceneAsync(_currentLevel.RuntimeValue);
			operation.allowSceneActivation = false;

			while (!operation.isDone)
			{
				if (operation.progress >= 0.9f && _saveFileRestored)
				{
					operation.allowSceneActivation = true;
				}
				
				_progressBarImage.fillAmount = operation.progress;
				yield return null;
			}
			yield return null;
		}
	}
}
