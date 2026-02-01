using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBarController : MonoSingleton<HealthBarController>
    {
        [SerializeField]
        private List<Image> _healthIcons = new();
        private void Start()
        {
            Player.OnPlayerHealthChange += OnHealthChange;
        }

        private void OnDestroy()
        {
            Player.OnPlayerHealthChange -= OnHealthChange;
        }

        private void OnHealthChange(float intHP)
        {
            var maxHp = Player.PlayerEntityData.health;
            var iconCount = _healthIcons.Count;
            int iconsToShow = Mathf.RoundToInt(intHP / maxHp * iconCount);

            int i = 0;
            
            for (; i < iconsToShow; i++)
            {
                _healthIcons[i].gameObject.SetActive(true);
            }
            
            for (; i < iconCount; i++)
            {
                _healthIcons[i].gameObject.SetActive(false);
            }
        }
    }
}