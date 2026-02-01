using UnityEngine;

public class BossHealthBarController : MonoSingleton<BossHealthBarController>
{
    [SerializeField] private GameObject _scaleBar;
    [SerializeField] private GameObject _container;

    private float _healthBarScale;
    public float HealthPercent
    {
        set
        {
            _healthBarScale = value;
            _scaleBar.transform.localScale = new Vector3(1f - _healthBarScale, 1, 1);
        }
    }


// Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _scaleBar.transform.localScale = new Vector3(0f, 1, 1);
        _container.SetActive(false);
    }
    
    public void SetBarAction(bool isActive)
    {
        _container.SetActive(isActive);
    }
}
