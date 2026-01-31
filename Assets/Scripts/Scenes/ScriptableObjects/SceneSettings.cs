using System;
using Data;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Scenes
{
	[CreateAssetMenu(fileName = "so-sceneSettings", menuName = "ScriptableObjects/Scenes/so-sceneSettings", order = 1)]
	[Serializable]
	public class SceneSettings : ScriptableObject
	{
		[SerializeField]
		protected GlobalTypes.SceneName sceneName;
		public GlobalTypes.SceneName SceneName => sceneName;
		[Header("Scene Addressable Address Reference")]
		[SerializeField]
		private AssetReference sceneRef;
		public AssetReference SceneRef => sceneRef;
	}
}
