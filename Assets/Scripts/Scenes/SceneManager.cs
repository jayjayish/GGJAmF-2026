using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
#if UNITY_EDITOR
using System.Diagnostics;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using LogType = CoalCar.Utility.LogType;

namespace Scenes
{
	/// <summary>
	/// Static class in charge of managing all scenes loaded dynamically
	/// and additively.
	/// </summary>p
	public static class SceneManager
	{
		/// <summary>
		/// Will contain all scene instances by scene type
		/// </summary>
		private static readonly Dictionary<CrossworldTypes.SceneName, SceneInstance> LoadedScenes = new ();

		/// <summary>
		/// Cached addressable script. obj. containing all scene settings
		/// As this ref is loaded async as a script. obj. if the cached val is null,
		/// it must be obtained by calling "_GetSceneSettingsAsync()"
		/// </summary>
		private static SceneSettingsContainer _sceneSettingsContainer;

		//			--- LOADING SCENE VARS ---

		// Variables referring to the scene showing the "Loading ..." text
		// It'll be shown in between scenes (while loading and unloading scenes
		// and while the _loadingSceneCounter value is not 0)

		/// <summary>
		/// "_loadingSceneCounter":
		/// Counter used to control the show/hide of the "Loading..." scene
		/// See: <see cref="AddLoadingSceneCounter"/>
		/// and <see cref="RemoveLoadingSceneCounter"/>
		/// </summary>
		private static uint _loadingSceneCounter;

		private static bool _isLoadingSceneShowing;
		/// <summary>
		/// Indicates if the "Loading ..." scene is showing or not
		/// </summary>
		public static bool IsLoadingSceneShowing => _isLoadingSceneShowing;

		public static bool LoadingEnabled;

		public static Action OnLoadingCounterZero;
		public static Action OnLoadingCounterNonZero;

		/// <summary>
		/// Collections used to keep track of the loading/unloading in progress
		/// scenes. This is to prevent loading/unloading a new scene if a same scene
		/// type is in the middle of an async process of being loaded/unloaded.
		///
		/// </summary>
		private static readonly HashSet<CrossworldTypes.SceneName> LoadingScenes = new ();
		private static readonly HashSet<CrossworldTypes.SceneName> UnloadingScenes = new ();

		//			--- UNLOADING SCENE VARS ---

		/// <summary>
		/// When loading a new scene, Wait for a few miliseconds to
		/// wait for the scene's objects to load so that any of them can call
		/// "AddLoadingSceneCounter()" if needed, before the "Loading..." screen
		/// disappears
		/// </summary>
		private const float LoadSceneWaitTime = 0.1F;

		private static Scene _firstScene;

		private static CrossworldTypes.SceneName _lastSceneLoaded;
		/// <summary>
		/// Returns the last scene loaded
		/// </summary>
		public static CrossworldTypes.SceneName LastSceneLoaded => _lastSceneLoaded;

		// --------------------------------------------------------PUBLIC EVENTS

		/// <summary>
		/// Fired in "LoadSceneAsync()" after successfully loading a scene
		/// and after that method's local "onLoad" event arg.
		/// </summary>
		public static Action<SceneSettings> OnSceneLoad;

		/// <summary>
		/// Fired in "UnloadSceneAsync()" after successfully unloading a scene
		/// and after that method's local "onUnload" event arg.
		/// </summary>
		public static Action<CrossworldTypes.SceneName> OnSceneUnload;

		// -------------------------------------------------------PUBLIC METHODS

		/// <summary>
		/// Loads a scene asynchronously by sceneType.
		/// If successful, "onLoad" action will be called on success after
		/// the async operation.
		/// <br /><br />
		/// If "willSceneBeActive" = true, the loaded scene will be the active
		/// scene among all existing scenes
		/// (newly instantiated objects will be created in the active scene
		/// Also, lighting will act upon the active scene as well)
		/// <br /><br />
		/// No scene will be loaded if a scene with the given sceneType
		/// has already been loaded and hasn't been unloaded.
		/// <br /><br />
		/// The scene must exist in the "so-sceneSettings" scriptable
		/// object as an entry and its "SceneType" must be unique among all entries
		/// </summary>
		public static void LoadSceneAsync(CrossworldTypes.SceneName sceneName, bool willSceneBeActive = true, Action<CrossworldTypes.SceneName> onLoad = null)
		{
			if (LoadingScenes.Contains(sceneName))
			{
				_LogError(
					$"Cannot load new scene of type {sceneName}. " +
					$"A scene of the same type is already being loaded");
				return;
			}

			if (LoadedScenes.ContainsKey(sceneName))
			{
				_LogError($"Cannot load scene of type {sceneName}. It's already loaded");
				return;
			}

			// Checks finished. About to load scene

			// Delegate the loading scene workload after
			// checking for potential errors above
			_LoadSceneAsync(sceneName, willSceneBeActive, onLoad);
		}

		/// <summary>
		/// Unloads a scene asynchronously by sceneType. The scene must have
		/// been loaded previously with "LoadSceneAsync()".
		/// <br /><br />
		/// This method will return if no scene of the given type was
		/// loaded and currently active.
		/// </summary>
		public static void UnloadSceneAsync(CrossworldTypes.SceneName sceneName, Action<CrossworldTypes.SceneName> onUnload = null)
		{
			if (UnloadingScenes.Contains(sceneName))
			{
				_LogError(
					$"Cannot unload new scene of type {sceneName}. " +
					$"A scene of the same type is already being unloaded");
				return;
			}

			if (! LoadedScenes.ContainsKey(sceneName))
			{
				_LogError($"Cannot unload scene of type {sceneName}. It's not loaded");
				return;
			}

			// Delegate the unloading scene workload after
			// checking for potential errors above
			_UnloadSceneAsync(sceneName, onUnload);
		}

		/// <summary>
		/// Retrieves the Scene Settings entry that corresponds to the given
		/// "sceneName" if it exists.
		/// <br /><br />
		/// If no entries are found, an empty list will be returned
		/// </summary>
		public static void GetSceneSettingsAsync(
			CrossworldTypes.SceneName sceneName, Action <IList<SceneSettings>> onGet)
		{
			// Set (Caches) the "_sceneSettingsContainer" variable if it's null.
			// Needed before using "_sceneSettingsContainer" below
			CacheSceneSettingsVarAsync(() =>
			{
				IList<SceneSettings> scenes = _sceneSettingsContainer.GetAllSettings(sceneName);
				onGet?.Invoke(scenes);
			});
		}

		// /// <summary>
		// /// Retrieves the Scene Settings entry that corresponds to the given
		// /// "sceneType" if it exists.
		// /// <br /><br />
		// /// If no entries are found, an empty list will be returned
		// /// </summary>
		// /// <param name="sceneType"></param>
		// /// <param name="onGet"></param>
		// public static void GetSceneSettingsAsync(SceneType sceneType, Action <IList<SceneSettings>> onGet)
		// {
		// 	CacheSceneSettingsVarAsync(() =>
		// 	{
		// 		IList<SceneSettings> scenes = _sceneSettingsContainer.GetAllSettings(sceneType);
		// 		onGet?.Invoke(scenes);
		// 	});
		// }

		/// <summary>
		/// Adds one to the LoadingSceneCounter.
		/// <br /><br />
		/// If "LoadingSceneCounter" = 0 and: a scene is starting to load/unload or
		/// calling "AddLoadingSceneCounter()", the counter will go up
		/// and the "Loading..." scene will show up (load)
		/// </summary>
		public static void AddLoadingSceneCounter()
		{
			if (_loadingSceneCounter == 0)
			{
				_ShowLoadingUi();
				OnLoadingCounterNonZero?.Invoke();
			}
			++_loadingSceneCounter;
			#if UNITY_EDITOR
			StackTrace stackTrace = new StackTrace();
			if (stackTrace.FrameCount > 1)
			{
				var caller = stackTrace.GetFrame(1).GetMethod();
				_Log($"LoadingSceneCounter increased to {_loadingSceneCounter} by: {caller.DeclaringType?.FullName}.{caller.Name}");

			}
			else
			{
				_Log($"LoadingSceneCounter increased to {_loadingSceneCounter}");
			}
			#else
				_Log($"LoadingSceneCounter increased to {_loadingSceneCounter}");
			#endif
		}

		/// <summary>
		/// Removes one from the LoadingSceneCounter.
		/// <br /><br />
		/// If "LoadingSceneCounter" > 0 and: a scene is finishing to load/unload or
		/// calling "RemoveLoadingSceneCounter()", the counter will go down.
		/// If it reaches 0, the "Loading..." scene will hide (unload)
		/// </summary>
		public static void RemoveLoadingSceneCounter()
		{
			// Do nothing if counter is already zero
			if (_loadingSceneCounter == 0) { return; }
			--_loadingSceneCounter;

			if (_loadingSceneCounter == 0)
			{
				_HideLoadingUi();
				OnLoadingCounterZero?.Invoke();
			}

#if UNITY_EDITOR
			StackTrace stackTrace = new StackTrace();
			if (stackTrace.FrameCount > 1)
			{
				var caller = stackTrace.GetFrame(1).GetMethod();
				_Log($"LoadingSceneCounter decreased to {_loadingSceneCounter} by: {caller.DeclaringType?.FullName}.{caller.Name}");

			}
			else
			{
				_Log($"LoadingSceneCounter decreased to {_loadingSceneCounter}");
			}
#else
			_Log($"LoadingSceneCounter decreased to {_loadingSceneCounter}");
#endif
		}

		/// <summary>
		/// Returns true if the reference of an already loaded scene is found.
		/// The scene itself is returned as "scene" out var
		/// <br />
		/// If no scene ref is found, false will be returned.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="scene"></param>
		/// <returns></returns>
		public static bool TryGetLoadedSceneInstance(CrossworldTypes.SceneName name, out Scene scene)
		{
			if (LoadedScenes.TryGetValue(name, out var instance))
			{
				scene = instance.Scene;
				return true;
			}
			scene = default;
			return false;
		}

		/// <summary>
		/// Returns the default obstacle types set to the given scene
		/// <br />
		/// If no scene settings are found that matches the given "sceneName"
		/// "onGet" callback will still be fired with an empty list of
		/// obstacle types
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="onGet"></param>
		public static void GetObstacleTypesAsync(
			CrossworldTypes.SceneName sceneName, Action <List<CrossworldTypes.TowerType>> onGet)
		{
			CacheSceneSettingsVarAsync(() =>
			{
				List<CrossworldTypes.TowerType> obstacles = new();
				if (_sceneSettingsContainer.TryGetSetting(sceneName, out SceneSettings sceneSettings))
				{
					obstacles = sceneSettings.TowerTypes;
				}

				onGet?.Invoke(obstacles);
			});
		}

		public static void PlaceObjectInPlayerScene(List<GameObject> objectToMove)
		{
			foreach (GameObject go in objectToMove)
			{
				if (go.transform.parent == null)
				{
					UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, _firstScene);
				}
			}
		}

		public static void PlaceObjectInPlayerScene(params GameObject[] objectToMove) =>
			PlaceObjectInPlayerScene(objectToMove.ToList());

		public static void RecordActiveSceneAsFirstScene()
		{
			_firstScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
		}

		/// <summary>
		/// Returns true if last scene loaded was either the lobby or network
		/// </summary>
		public static bool IsLastSceneLoadedLobbyOrNetwork()
		{
			return _lastSceneLoaded is CrossworldTypes.SceneName.Lobby or CrossworldTypes.SceneName.Network;
		}

		// ------------------------------------------------------PRIVATE METHODS

		/// <summary>
		/// Caches the scene settings script. obj. addressable.
		/// Only if _sceneSettings = null, the setting will be loaded async
		///
		/// This method must be called before using "_sceneSettings" to set it
		/// </summary>
		public static void CacheSceneSettingsVarAsync(Action onSuccess)
		{
			if (_sceneSettingsContainer == null)
			{
				// Cache the scene settings async
				Addressables.LoadAssetAsync<SceneSettingsContainer>(GlobalAddresses.SceneContainerAddr).Completed += asyncHandle =>
				{
					if (asyncHandle.Status == AsyncOperationStatus.Succeeded)
					{
						_sceneSettingsContainer = asyncHandle.Result;
						onSuccess?.Invoke();
					}
					else
					{
						_LogError("SceneSettings couldn't be loaded");
					}
				};
			}
			else
			{
				onSuccess?.Invoke();
			}
		}

		/// <summary>
		/// Will do all the scene loading process
		/// </summary>
		private static void _LoadSceneAsync(
			CrossworldTypes.SceneName sceneName,
			bool willSceneBeActive = true,
			Action<CrossworldTypes.SceneName> onLoad = null)
		{
			if (_sceneSettingsContainer == null)
			{
				_LogError($"Cannot load scene of type {sceneName}. No SceneSettings container is loaded.");
				return;
			}

			// Set (Caches) the "_sceneSettingsContainer" variable if it's null
			// Needed before using "_sceneSettingsContainer" below
			if (!_sceneSettingsContainer.TryGetSetting(sceneName, out SceneSettings sceneEntry))
			{
				_LogError(
					$"Cannot load scene." +
					$"Scene of type {sceneName} could not be found " +
					$"in SceneSettings");
			}
			else
			{
				LoadingScenes.Add(sceneName);
				AddLoadingSceneCounter();

				Addressables.LoadSceneAsync(sceneEntry.SceneRef, LoadSceneMode.Additive).Completed += asyncHandle =>
				{
					if (asyncHandle.Status == AsyncOperationStatus.Succeeded)
					{
						SceneInstance tSceneInst = asyncHandle.Result;

						if (sceneName != CrossworldTypes.SceneName.Network)
						{
							_lastSceneLoaded = sceneName;
						}
						LoadedScenes.Add(sceneName, tSceneInst);
						_Log($"Scene of type {sceneName} loaded successfully");

						// Set this scene as active ?
						if (willSceneBeActive)
						{
							UnityEngine.SceneManagement.SceneManager.SetActiveScene(tSceneInst.Scene);
							_Log($"Scene of type {sceneName} is now the active scene");
						}

						// Set scene Gravity (other than network scene
						// as network scene doesn't have any phuysics gameplay)
						if (sceneEntry.SceneName != CrossworldTypes.SceneName.Network)
						{
							Physics.gravity = sceneEntry.Gravity;
						}

						onLoad?.Invoke(sceneName); // Local arg event
						OnSceneLoad?.Invoke(sceneEntry); // Static class event
					}
					else
					{
						_LogError(
							$"Cannot async load scene of type {sceneName}: " +
							$"Status non successful");
					}

					LoadingScenes.Remove(sceneName);

					// ONLY FOR NON "LOADING..." SCENES:
					// Wait for a few miliseconds to
					// wait for the scene's objects to load so that any of them can call
					// "AddLoadingSceneCounter()" if needed, before the "Loading..." screen
					// disappears
					DOTween.Sequence().AppendInterval(LoadSceneWaitTime).OnComplete(RemoveLoadingSceneCounter);
				};
			}
		}

		/// <summary>
		/// Will do all the scene unloading process
		/// </summary>
		private static void _UnloadSceneAsync(
			CrossworldTypes.SceneName sceneName,
			Action<CrossworldTypes.SceneName> onUnload = null)
		{
			SceneInstance tSceneInst = LoadedScenes[sceneName];

			UnloadingScenes.Add(sceneName);
			AddLoadingSceneCounter();

			Addressables.UnloadSceneAsync(tSceneInst).Completed += asyncHandle =>
			{
				if (asyncHandle.Status == AsyncOperationStatus.Succeeded)
				{
					LoadedScenes.Remove(sceneName);
					_Log($"Scene of type {sceneName} unloaded successfully");

					onUnload?.Invoke(sceneName); // Local arg event
					OnSceneUnload?.Invoke(sceneName); // Static class event
				}
				else
				{
					_LogError(
						$"Cannot async unload scene of type {sceneName}: " +
						$"Status non successful");
				}

				UnloadingScenes.Remove(sceneName);
				RemoveLoadingSceneCounter();
			};
		}

		//		--- Methods used for showing/hiding the "Loading ..." Ui

		private static void _ShowLoadingUi()
		{
			if (_isLoadingSceneShowing) { return; }

			// Must set the flag to true the first time the loading screen
			// is shown as it's an async load, to prevent subsequent scene loads
			// in this batch to prevent a stack overflow of calls
			_isLoadingSceneShowing = true;

			_ShowHideLoadingUi(true);
		}

		private static void _HideLoadingUi()
		{
			// Return the flag back to false (default value) so that the next
			// batch of scenes to be loaded have the loading screen shown
			_isLoadingSceneShowing = false;

			_ShowHideLoadingUi(false);
		}

		private static void _ShowHideLoadingUi(bool willShow)
		{
			if (FirstSceneManager.Instance == null)
			{
				_LogError(
					$"\"Loading ...\" scene message cannot be shown as " +
					$"\"FirstSceneManager\" is null. " +
					$"This is not a game breaking error. " +
					$"However, make sure the \"FirstScene\" Unity scene is the" +
					$"very first scene to load. Then, select the next scene to load" +
					$"from the \"FirstSceneManager\" in-scene object");
				return;
			}

			LoadingEnabled = willShow;
		}
	}
}
