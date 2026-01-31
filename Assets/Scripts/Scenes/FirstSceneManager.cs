using System;
using Data;
using UnityEngine;

namespace Scenes
{
    public class FirstSceneManager : MonoBehaviour
    {
        [SerializeField] private GlobalTypes.SceneName sceneName;
        private int _itemsToLoad;
        
        private void Start()
        {
            _itemsToLoad = 2;
            InputManager.Initialize(CheckAllInitialized);
            SceneManager.CacheSceneSettingsVarAsync(CheckAllInitialized);
        }

        private void CheckAllInitialized()
        {
            _itemsToLoad--;
            if (_itemsToLoad == 0)
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            SceneManager.LoadSceneAsync(GlobalTypes.SceneName.UI);
            SceneManager.LoadSceneAsync(sceneName);
        }
    }
}