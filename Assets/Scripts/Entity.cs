using Colors;
using NaughtyAttributes;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] protected EntityData data;
    public int health { get; protected set; }

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
        // Initialize runtime health from the data template.
        if (health <= 0 && data != null)
        {
            health = data.health;
        }
        movementSpeed = data.movementSpeed * 0.01f; // scale it down
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

    public virtual void TakeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        health = Mathf.Max(0, health - amount);

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
    protected virtual void OnDeath() { }
    
}
