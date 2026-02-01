using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float cameraFollowSpeed = 0.1f;

    // Update is called once per frame
    void Update()
    {
        if (Player.Instance== null) return;
        
        var zcache = transform.position.z;
        var newSpot = Vector2.Lerp(transform.position, Player.Instance.transform.position, Time.deltaTime * cameraFollowSpeed);
        var cacheOldPos = transform.position;
        transform.position = new Vector3(newSpot.x, newSpot.y, zcache);

        if (Vector3.Distance(cacheOldPos, newSpot) > float.Epsilon)
        {
            InputManager.CalculateMouseWorldPosition();
        }
    }
}
