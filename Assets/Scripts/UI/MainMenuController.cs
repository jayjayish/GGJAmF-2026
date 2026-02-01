using Data;
using Scenes;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void OnPlayPressed()
    {
        SceneManager.UnloadSceneAsync(GlobalTypes.SceneName.MainMenu);
        SceneManager.LoadSceneAsync(GlobalTypes.SceneName.Gameplay);
    }
}
