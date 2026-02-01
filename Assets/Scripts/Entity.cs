using Colors;
using NaughtyAttributes;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected EntityData data;

    protected float _health;

    public float Health
    {
        get => _health;
        protected set
        {
            _health = value;
            HealthChanged(Health);
        }
    }

    protected bool _isDead;

    protected bool isDead
    {
        get => _isDead;
        set
        {
            if (value == _isDead)
            {
                return;
            }

            _isDead = value;
            if (_isDead)
            {
                OnDeath();
            }
        }
    }

    public float movementSpeed {get; set;}
    [SerializeField] protected SpriteRenderer spriteRenderer;
    public bool automaticSpriteColor;

    [ShowNonSerializedField]
    private int _colorAngle;
    public int ColorAngle
    {
        get => _colorAngle;
        set
        {
            _colorAngle = value % 360;
            OnColorChange(_colorAngle);
        }
    }

    protected virtual void OnColorChange(int colorAngle)
    {
        if (automaticSpriteColor && spriteRenderer)
        {
            spriteRenderer.color = new ColorHSV(colorAngle).ToColor();
        }
    }

    protected Collider2D hurtBox;
    
    // Override in subclasses (e.g. Projectile) as needed.
    protected virtual bool ColliderIsTrigger => false;

    protected virtual void Awake()
    {
        hurtBox = GetComponent<Collider2D>();
        if (hurtBox == null)
        {
            // Ensure there's at least some Collider2D present.
            hurtBox = gameObject.AddComponent<BoxCollider2D>();
        }

        hurtBox.isTrigger = ColliderIsTrigger;
    }

    protected virtual void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>();
        }

        col.isTrigger = ColliderIsTrigger;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    { }

    protected virtual void OnEnable() {
        movementSpeed = data.movementSpeed;
        Health = data.health;
        _colorAngle = data.colorAngle;
    }

    // Update is called once per frame
    protected virtual void Update()
    { }
    
    protected virtual void HealthChanged(float newHealth)
    {
        if (newHealth <= 0)
        {            
            isDead = true;
        }
    }

    public virtual void TakeDamage(int amount, int attackColorAngle)
    {
        if (isDead || amount <= 0)
        {
            return;
        }

        // Scale damage by color similarity.
        var delta = Mathf.Abs(Mathf.DeltaAngle(ColorAngle, attackColorAngle)); // degrees in [0, 180]
        // cosine scaling, no leniancy:
        //var colorScaled = (-Mathf.Cos(delta * Mathf.Deg2Rad) + 1) / 2f;

        // linear scaling with 15 degree leniancy:
        float colorScaled = Mathf.Clamp01((delta - 30f) / 12f);

        var scaledDamage = amount * colorScaled;
        Health = Mathf.Max(0, Health - scaledDamage);
    }

    protected virtual void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
    }

    // Override to add VFX, drops, despawn logic, etc.
    protected virtual void OnDeath()
    { 
        gameObject.SetActive(false);
    }
    
}
