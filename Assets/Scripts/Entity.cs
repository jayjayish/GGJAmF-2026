using Colors;
using NaughtyAttributes;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected EntityData data;
    public float health { get; protected set; }

    [SerializeField] protected float movementSpeed;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected bool automaticSpriteColor;

    [ShowNonSerializedField]
    private int _colorAngle;
    public int ColorAngle
    {
        get => _colorAngle;
        set
        {
            _colorAngle = value % 360;
            if (automaticSpriteColor && spriteRenderer)
            {
                spriteRenderer.color = new ColorHSV(_colorAngle).ToColor();
            }
        }
    }
    
    protected bool isDead;
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
    {
    }

    protected virtual void OnEnable() {
        movementSpeed = data.movementSpeed;
        health = data.health;

    }

    // Update is called once per frame
    protected virtual void Update() { }

    [Button]
    public void AddColor()
    {
        ColorAngle += 10;
    }
    
    [Button]
    public void SubtractColor()
    {
        ColorAngle -= 10;
    }

    public virtual void TakeDamage(int amount, int attackColorAngle)
    {
        if (isDead)
        {
            return;
        }

        // Scale damage by color similarity.
        // colorScaled = cos( abs(deltaColorAngle) / 2 )
        // Use shortest angle difference so 350° vs 10° is treated as 20°.
        var delta = Mathf.Abs(Mathf.DeltaAngle(ColorAngle, attackColorAngle)); // degrees in [0, 180]
        var colorScaled = Mathf.Cos((delta * Mathf.Deg2Rad) / 2f);
        colorScaled = Mathf.Clamp01(colorScaled);

        if (amount <= 0)
        {
            return;
        }

        var scaledDamage = amount * colorScaled;
        health = Mathf.Max(0, health - scaledDamage);
        Debug.Log("health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        OnDeath();
    }

    // Override to add VFX, drops, despawn logic, etc.
    protected virtual void OnDeath() { 
        gameObject.SetActive(false);
    }
    
}
