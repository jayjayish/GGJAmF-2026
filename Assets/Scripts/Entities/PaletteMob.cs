using UnityEngine;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Colors;
using Data;
using DG.Tweening;

public class PaletteMob : BasicMob
{
    private HashSet<GlobalTypes.Color> _colorsEncountered = new ();
    [SerializeField]
    private SerializedDictionary<GlobalTypes.Color, SpriteRenderer> _colorSprites;
    [SerializeField]
    private float wobbleCoefficient;
    [SerializeField]
    private float wobbleAmp;
    [SerializeField]
    private float rotateCoefficient;
    [SerializeField]
    private float rotateRadius;
    [SerializeField]
    private float lerpCoefficent;
    [SerializeField]
    private ParticleSystem deathParticles;


    private Vector3 _targetPosition;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        foreach (var sp in _colorSprites.Values)
        {
            sp.DOFade(0.2f, 1f);
        }
    }

    protected override void Update()
    {
        base.Update();
        var wobble = wobbleAmp * Mathf.Sin(Time.time * wobbleCoefficient);
        spriteRenderer.transform.localPosition = new Vector3(0f, wobble, 0f);
        
        var x = Mathf.Sin(Time.time * rotateCoefficient) * rotateRadius;
        var y = Mathf.Cos(Time.time * rotateCoefficient) * rotateRadius;
        Vector3 offset = new Vector3(x, y, 0f);

        _targetPosition = Player.Instance.transform.position + offset;
        transform.position = Vector3.Lerp(transform.position, _targetPosition, lerpCoefficent);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit Pal");
        var hitProjectile = collision?.gameObject.GetComponentInParent<Projectile>();
        if (hitProjectile != null && hitProjectile.isPlayer)
        {
            PaletteHit(hitProjectile.ColorAngle);
        }
    }

    private void PaletteHit(int hitColorAngle)
    {
        var hitColor = ColorHSV.GetClosest(hitColorAngle);

        if (_colorsEncountered.Contains(hitColor))
        {
            return;
        }

        _colorSprites[hitColor].DOFade(1f, 0.5f);
        _colorsEncountered.Add(hitColor);

        CheckAllHit();
    }

    private void CheckAllHit()
    {
        if (_colorsEncountered.Count >= 6)
        {       
            deathParticles.transform.position = transform.position;
            deathParticles.Play();
            GameFlowManager.Instance.BossStart();
        }
    }
}
