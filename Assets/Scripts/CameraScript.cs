using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] float cameraFollowSpeed = 0.1f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.Instance != null)
        {   
            var zcache = transform.position.z;
            var newSpot = Vector2.Lerp(transform.position, Player.Instance.transform.position, Time.deltaTime * cameraFollowSpeed);
            transform.position = new Vector3(newSpot.x, newSpot.y, zcache);
        }
    }
}
