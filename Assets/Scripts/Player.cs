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
    
        transform.position = playerDirection * movementSpeed;
    }

    private void OnMove(Vector2 vector)
    {
        playerDirection = vector.normalized;
    }
}
