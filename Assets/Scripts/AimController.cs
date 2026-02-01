using UnityEngine;

public class AimController : MonoBehaviour
{
    public Vector2 pos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputManager.AddAimAction(UpdateMousePos);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void UpdateMousePos(Vector2 vec)
    {
        Debug.Log($"Mouse x: {vec.x}, Mouse y: {vec.y}");
        pos = Camera.main.ScreenToWorldPoint(new Vector3(vec.x, vec.y, -Camera.main.transform.position.z));
        transform.position = pos;
    }
}
