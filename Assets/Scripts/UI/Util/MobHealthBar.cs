using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ragnar.UI.Compnents
{
    public class MobHealthBar : MonoBehaviour
    {

        public GameObject barFill;

        public LivingEntity entity;
        // Start is called before the first frame update
        void Start()
        {
            entity.OnHealthChanged += UpdateHealth;
            UpdateHealth(entity.health);
        }

        private void UpdateHealth(float health)
        {
                var scale = barFill.transform.localScale;
                scale.x = health / entity.maximumHealth;
                barFill.transform.localScale = scale;
        }
    }
}
