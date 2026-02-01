using System;
using Entities;
using UnityEngine;

public class Player : Character
{
    private static Player _instance;
    public static Player Instance => _instance;
    public PlayerData PlayerEntityDaya => (PlayerData)data;

    [SerializeField] private PlayerColorPicker colorPicker;
    [SerializeField] private Vector2 playerDirection;
    [SerializeField] private int colorRotationRate = 1;

    [SerializeField] private int iFrames = 20;


    private bool _isLeft, _isRight, _showingPicker, _pickerTimer;
    private float _timeLastLeftRight;
    private const float PickerFadeTimeout = 1f;
    private bool isInvincible;


    private int _iFrameCount = 0;

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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        InputManager.AddMoveAction(OnMove);
        InputManager.AddAttackAction(Shoot);
        InputManager.AddLeftDownAction(OnLeftDown);
        InputManager.AddRightDownAction(OnRightDown);
        InputManager.AddLeftUpAction(OnLeftUp);
        InputManager.AddRightUpAction(OnRightUp);
        
        ReticleMouseFollower.Instance.SetAlpha(1f);
    }

    private void OnRightDown()
    {
        _isRight = true;
    }

    private void OnLeftDown()
    {
        _isLeft = true;
    }
    
    private void OnRightUp()
    {
        _isRight = false;
    }

    private void OnLeftUp()
    {
        _isLeft = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (isInvincible) {
            _iFrameCount++;
            if (_iFrameCount > iFrames) {
                isInvincible = false;
                _iFrameCount = 0;
            }
        }
        MoveCharacter();
        ManageColorPicker();
    }

    private void ManageColorPicker()
    {
        if (_showingPicker && !_isLeft && !_isRight)
        {
            if (!_pickerTimer)
            {
                _pickerTimer = true;
                _timeLastLeftRight = Time.time;
            }
            else if (Time.time > _timeLastLeftRight + PickerFadeTimeout)
            {
                _pickerTimer = false;
                _showingPicker = false;
                colorPicker.Fadeout();
            }
        }
        else if (!_showingPicker && (_isLeft || _isRight))
        {
            _showingPicker = true;
            _pickerTimer = false;
            colorPicker.Fadein();
        }

        if (_isLeft && !_isRight)
        {
            ColorAngle += colorRotationRate;
        }
        else if (!_isLeft && _isRight)
        {
            ColorAngle -= colorRotationRate;
        }

    }

    private void MoveCharacter()
    {
        var moveVec = movementSpeed * Time.deltaTime * playerDirection;
        transform.position += new Vector3(moveVec.x, moveVec.y, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Take contact damage from other Entities (e.g. mobs).
        var otherMob = collision.collider != null ? collision.collider.GetComponentInParent<BasicMob>() : null;
        if (otherMob != null && otherMob.getAttackDamage() > 0)
        {
            TakeDamage(otherMob.getAttackDamage(), otherMob.ColorAngle);
            return;
        }
    }    

    private void OnCollisionStay2D(Collision2D collision) {
        if (!isInvincible) {            
            var otherMob = collision.collider != null ? collision.collider.GetComponentInParent<BasicMob>() : null;
            if (otherMob != null && otherMob.getAttackDamage() > 0)
            {
                TakeDamage(otherMob.getAttackDamage(), otherMob.ColorAngle);
                return;
            }
        }
    }    

    private void OnTriggerEnter2D(Collider2D collider) {
        var hitProjectile = collider != null ? collider.GetComponentInParent<Projectile>() : null;
        Debug.Log("player getting hit by projectile ");
        if (hitProjectile != null && !hitProjectile.isPlayer) {
            TakeDamage(hitProjectile.attackDamage, hitProjectile.ColorAngle);
        }
    }

    private void OnMove(Vector2 vector)
    {
        playerDirection = vector.normalized;
    }
    
    public override void TakeDamage(int amount, int attackColorAngle)
    {
        if (isDead || isInvincible)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }
        // Player does not take scaled damage
        health = Mathf.Max(0, health - amount);
        Debug.Log(" Player health: " + health);

        if (health <= 0)
        {
            Die();
        }

        isInvincible = true;
    }

    private void Shoot()
    {
        var attack = ProjectileManager.SpawnProjectile(Data.GlobalTypes.ProjectileTypes.PlayerMain, transform.position, ColorAngle);
        attack.moveDirection = (InputManager.GetMouseWorldPosition() - transform.position).normalized;
    }

    protected override void OnColorChange(int colorAngle)
    {
        base.OnColorChange(colorAngle);
        
        colorPicker.SetRotation(colorAngle);
    }
}
