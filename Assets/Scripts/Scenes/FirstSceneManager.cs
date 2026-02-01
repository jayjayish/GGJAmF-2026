using System;
using Colors;
using Data;
using Entities;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

namespace Scenes
{
    public class FirstSceneManager : MonoBehaviour
    {
        [SerializeField] private GlobalTypes.SceneName sceneName;
        private int _itemsToLoad;
        
        private void Start()
        {
            _itemsToLoad = 4;
            InputManager.Initialize(CheckAllInitialized);
            SceneManager.CacheSceneSettingsVarAsync(CheckAllInitialized);
            ProjectileManager.Initialize(CheckAllInitialized);
            EnemyManager.Initialize(CheckAllInitialized);
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

            // for (int i = 0; i < 6; i++)
            // {
            //     var hue = i * 60;
            //     var hsvColor = new ColorHSV(hue);
            //     var rgbColor = hsvColor.ToColor();
            //     Debug.Log($"<color=#{rgbColor.ToHexString()}>{rgbColor.ToHexString()}</color>");
            // }
        }

        [Button]
        public void TestSpawnSlime()
        {
            EnemyManager.SpawnEnemy(GlobalTypes.EnemyTypes.Slime, Vector2.zero, 0);
        }
    }
}