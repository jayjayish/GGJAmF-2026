using System;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Vector2 playerDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        InputManager.AddMoveAction(OnMove);
    }

    // Update is called once per frame
    protected override void Update()
    {
        var moveVec = playerDirection *movementSpeed;
        transform.position += new Vector3(moveVec.x, moveVec.y, 0f);
    }

    private void OnMove(Vector2 vector)
    {
        playerDirection = vector.normalized;
    }
}
