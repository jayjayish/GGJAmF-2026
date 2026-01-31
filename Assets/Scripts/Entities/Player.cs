using System;
using Entities;
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
    public Transform mouseTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        InputManager.AddMoveAction(OnMove);
        InputManager.AddAttackAction(Shoot);
    }

    // Update is called once per frame
    protected override void Update()
    {
        var moveVec = playerDirection * movementSpeed;
        transform.position += new Vector3(moveVec.x, moveVec.y, 0f);
    }

    private void OnMove(Vector2 vector)
    {
        playerDirection = vector.normalized;
    }

    private void Shoot()
    {
        var attack = ProjectileManager.SpawnProjectile(Data.GlobalTypes.ProjectileTypes.TestCircle, transform.position);
        attack.mouseDirectionNormalized = (mouseTransform.position- transform.position).normalized;
    }
    
    private Vector2 _toRelativePos(Vector2 vector)
    {
        return (Vector2)transform.position - vector;
    }
}
