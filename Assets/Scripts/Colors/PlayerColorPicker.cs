using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class PlayerColorPicker : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer spriteRendererBox;
    private const float FadeInTime = 0.5f;
    private const float FadeOutTime = 0.5f;

    private TweenerCore<Color, Color, ColorOptions> _tweenerCore;
    private TweenerCore<Color, Color, ColorOptions> _tweenerCoreBox;

    private void Start()
    {
        var color = spriteRenderer.color;
        color.a = 0f;
        spriteRenderer.color = color;

        color = spriteRendererBox.color;
        color.a = 0f;
        spriteRendererBox.color = color;
    }

    public void SetRotation(float hue)
    {
        transform.localRotation = Quaternion.Euler(0, 0, hue);
    }

    public void Fadeout()
    {
        CheckTweenerCores();

        _tweenerCore = spriteRenderer.DOFade(0f, FadeOutTime).OnComplete(ClearTween);
        _tweenerCoreBox = spriteRendererBox.DOFade(0f, FadeOutTime).OnComplete(ClearTween);
    }

    public void Fadein()
    {
        CheckTweenerCores();
        
        _tweenerCore = spriteRenderer.DOFade(1f, FadeInTime).OnComplete(ClearTween);
        _tweenerCoreBox = spriteRendererBox.DOFade(1f, FadeInTime).OnComplete(ClearTween);
    }

    private void ClearTween()
    {
        _tweenerCore = null;
        _tweenerCoreBox = null;
    }

    private void CheckTweenerCores()
    {
        if (_tweenerCore != null)
        {
            _tweenerCore.Kill();
        }

        if (_tweenerCoreBox != null)
        {
            _tweenerCoreBox.Kill();
        }
    }
}
