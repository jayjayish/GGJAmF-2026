using System;
using UnityEngine;

public class Player : Character
{
    private static Player _instance;
    public static Player Instance => _instance;

    protected override void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            Debug.LogError("[MonoSingleton<" + typeof(Player).Name + ">]" + " duplicate instance was deleted");
            return;
        }

        _instance = this;
        
        base.Awake();
        // don't want player to be pushed
        rb.bodyType = RigidbodyType2D.Kinematic;
    }


    protected virtual void OnDestroy()
    {
        _instance = null;
    }
    
    [SerializeField] private Vector2 playerDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        InputManager.AddMoveAction(OnMove);
    }

    // Update is called once per frame
    protected override void Update()
    {
        var moveVec = playerDirection * data.movementSpeed;
        transform.position += new Vector3(moveVec.x, moveVec.y, 0f);
    }

    private void OnMove(Vector2 vector)
    {
        playerDirection = vector.normalized;
    }
}
