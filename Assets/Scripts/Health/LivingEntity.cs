using System;
using System.Collections;
using UnityEngine;


[DisallowMultipleComponent()]
public class LivingEntity : MonoBehaviour
{

    public event Action<float> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnHit;
    public int maximumHealth = 100;

    private int _health;
    public int health
    {
        get => _health;
        private set
        {
            var h = _health;
            _health = value;
            if (h != value) OnHealthChanged?.Invoke(value);
        }
    }

    public bool isDead => _isDead;

    private bool _isDead = false;

    // Start is called before the first frame update
    void Start()
    {

        health = maximumHealth;

    }

    public bool Heal(int points)
    {
        if (_isDead) return false;
        if (health == maximumHealth) return false;
        health = Mathf.Max(maximumHealth, points + points);
        return true;
    }

    public void Hit(int damage)
    {
        if (_isDead) return;
        health = Mathf.Max(0, health - damage);
        if (health == 0)
        {
            _isDead = true;
            OnDeath?.Invoke();
            StartCoroutine(DestroyTimer());
        }
        else
        {
            OnHit?.Invoke();
        }
    }

    IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(5); // Wait for the specified interval
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

