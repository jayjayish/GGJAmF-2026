using System;
using System.Collections.Generic;
using CoalCar.Utility.Data;
using Crossworld.Utility;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Crossworld.Scenes
{
	/// <summary>
	/// Represents an entry in the SceneSettings scriptable object
	/// </summary>
	[CreateAssetMenu(fileName = "so-sceneSettings", menuName = "ScriptableObjects/Scenes/so-sceneSettings", order = 1)]
	[Serializable]
	public class SceneSettings : ScriptableObject
	{
		/// <summary>
		/// Gravity default value
		/// </summary>
		public static readonly Vector3 DefaultGravity = new (0F, -9.81F, 0F);

		[SerializeField]
		protected CrossworldTypes.SceneName sceneName;
		public CrossworldTypes.SceneName SceneName => sceneName;

		[Tooltip("Core: Represents non-golfing scenes like the Lobby\n\n" +
				 "Normal: Represents both, single and networked golfing scenes\n\n")]
		[SerializeField]
		protected GlobalTypes.SceneType sceneType;
		public GlobalTypes.SceneType SceneType => sceneType;

		[Header("Default obstacles for this course")]
		[SerializeField]
		private List<CrossworldTypes.TowerType> towerTypes = new (4);
		public List<CrossworldTypes.TowerType> TowerTypes => towerTypes;

		[Header("Scene Addressable Address Reference")]
		[SerializeField]
		private AssetReference sceneRef;
		public AssetReference SceneRef => sceneRef;

		// [SerializeField]
		// private Sprite previewShot;
		// public Sprite PreviewShot => previewShot;

		[Header("Physics per level")]
		[Header("Gravity on this level (default = (0F, -9.81F, 0F)")]
		[SerializeField] private Vector3 gravity = DefaultGravity;
		public Vector3 Gravity => gravity;

		[Header("Drag multiplier per level (for balls and obstacles)")]
		[SerializeField, Range(0F, 9999F)] private float drag;
		public float Drag => drag;
	}
}
