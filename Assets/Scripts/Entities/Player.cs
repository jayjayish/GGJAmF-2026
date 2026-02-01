using System;
using Data;
using DG.Tweening;
using Entities;
using UnityEngine;

public class Player : Character
{
    private static Player _instance;
    public static Player Instance => _instance;
    public static PlayerData PlayerEntityData => (PlayerData)_instance.data;

    [SerializeField] private PlayerColorPicker colorPicker;
    [SerializeField] private Vector2 playerDirection;
    [SerializeField] private int colorRotationRate = 1;
    [SerializeField] private float shootCooldown = 0.5f;
    [SerializeField] private Animator animator;
    private const string AttackingBool = "IsAttacking";
    private const string ForwardBool = "IsForward";

    [SerializeField] private int iFrames = 20;


    private bool _isLeft, _isRight, _isShoot, _canShoot, _showingPicker, _pickerTimer;
    private float _timeLastLeftRight, _timeLastShoot;
    private const float PickerFadeTimeout = 1f;
    private const float SpriteScale = 0.75f;
    private bool isInvincible;


    private int _iFrameCount = 0;
    private static readonly int IsForward = Animator.StringToHash(ForwardBool);
    private static readonly int IsAttacking = Animator.StringToHash(AttackingBool);

    public static Action<float> OnPlayerHealthChange;

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
        InputManager.AddAttackDownAction(Shoot);
        InputManager.AddLeftDownAction(OnLeftDown);
        InputManager.AddRightDownAction(OnRightDown);
        InputManager.AddLeftUpAction(OnLeftUp);
        InputManager.AddRightUpAction(OnRightUp);
        InputManager.AddAttackDownAction(OnAttackDown);;
        InputManager.AddAttackUpAction(OnAttackUp);
        
        ReticleMouseFollower.Instance.SetAlpha(1f);

        _canShoot = true;
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
        Shoot();
        ManageColorPicker();
        FaceDirection();
    }
    
    #region Inputs

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

    private void OnAttackUp()
    {
        _isShoot = false;
    }

    private void OnAttackDown()
    {
        _isShoot = true;
    }

    private void OnMove(Vector2 vector)
    {
        playerDirection = vector.normalized;
    }
    
    private void MoveCharacter()
    {
        var moveVec = movementSpeed * Time.deltaTime * playerDirection;
        animator.SetBool(IsForward, playerDirection.y < 0.1f);
        transform.position += new Vector3(moveVec.x, moveVec.y, 0f);
    }
    
    private void Shoot()
    {
        if (!_isShoot)
        {
            return;
        }

        if (_timeLastShoot + shootCooldown < Time.time && !_canShoot)
        {
            _canShoot = true;
        }

        if (!_canShoot)
        {
            return;
        }
        var position = transform.position;
        var attack = ProjectileManager.SpawnProjectile(GlobalTypes.ProjectileTypes.PlayerMain, position, ColorAngle);
        var projDirection = (InputManager.GetMouseWorldPosition() - position).normalized;
        attack.moveDirection = projDirection;
        var angle = -Vector2.SignedAngle(projDirection, Vector2.right);
        attack.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        animator.SetTrigger(IsAttacking);
        _timeLastShoot = Time.time;
        _canShoot = false;
    }
    
    private void FaceDirection()
    {
        if (!_isShoot)
        {
            FaceDirectionOnVector(playerDirection);
        }
        else
        {
            var position = transform.position;
            var projDirection = (InputManager.GetMouseWorldPosition() - position).normalized;
            FaceDirectionOnVector(projDirection);
        }
    }

    private void FaceDirectionOnVector(Vector3 direction)
    {
        if (direction.x > 0f)
        {
            spriteRenderer.transform.localScale = SpriteScale * Vector3.one;
        }
        else if (direction.x < 0)
        {
            var newScale = SpriteScale * Vector3.one;
            newScale.x *= -1;
            spriteRenderer.transform.localScale = newScale;
        }
    }
    
    #endregion

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Take contact damage from other Entities (e.g. mobs).
        var otherMob = collision.collider != null ? collision.collider.GetComponentInParent<EnemyBase>() : null;
        if (otherMob != null && otherMob.GetAttackDamage() > 0)
        {
            TakeDamage(otherMob.GetAttackDamage(), otherMob.ColorAngle);
            return;
        }
    }    

    private void OnCollisionStay2D(Collision2D collision) {
        if (!isInvincible) {            
            var otherMob = collision.collider != null ? collision.collider.GetComponentInParent<EnemyBase>() : null;
            if (otherMob != null && otherMob.GetAttackDamage() > 0)
            {
                TakeDamage(otherMob.GetAttackDamage(), otherMob.ColorAngle);
                return;
            }
        }
    }    

    private void OnTriggerEnter2D(Collider2D collider) {
        var hitProjectile = collider != null ? collider.GetComponentInParent<Projectile>() : null;
        if (hitProjectile != null && !hitProjectile.isPlayer) {
            TakeDamage(hitProjectile.attackDamage, hitProjectile.ColorAngle);
        }
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
        Health = Mathf.Max(0, Health - amount);
        Debug.Log(" Player health: " + Health);

        if (Health <= 0)
        {            
            isDead = true;
        }

        isInvincible = true;
    }

    protected override void HealthChanged(float newHealth)
    {
        base.HealthChanged(newHealth);

        OnPlayerHealthChange?.Invoke(newHealth);
        
        Debug.Log("PlayerHP: " + Health);
    }


    protected override void OnColorChange(int colorAngle)
    {
        base.OnColorChange(colorAngle);
        
        colorPicker.SetRotation(colorAngle);
    }
}
