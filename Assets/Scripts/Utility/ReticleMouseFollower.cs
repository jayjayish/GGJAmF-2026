using UnityEngine;

public class ReticleMouseFollower : MonoSingleton<ReticleMouseFollower>
{
    // Update is called once per frame
    void Update()
    {
        transform.position = InputManager.GetMouseWorldPosition();
    }
}
