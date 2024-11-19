using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArcadeBridge.ArcadeIdleEngine.Booting
{
	public static class AutoBootSceneLoader
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
		public static void LoadLoadingScene()
		{
			int buildIndex = SceneManager.GetActiveScene().buildIndex;
			if (buildIndex != 0)
			{
				SceneManager.LoadScene(0);
			}
		}
	}
}
