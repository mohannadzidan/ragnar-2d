using UnityEngine;

[RequireComponent(typeof(LivingEntity))]
[RequireComponent(typeof(IsoCharacterAnimator))]
[DisallowMultipleComponent()]
public class LivingEntityHandler : MonoBehaviour
{
    private LivingEntity entity;
    private Navigator navigator;
    private IsoCharacterAnimator isoAnim;
    private new Collider2D collider;
    void Start()
    {
        navigator = GetComponent<Navigator>();
        entity = GetComponent<LivingEntity>();
        isoAnim = GetComponent<IsoCharacterAnimator>();
        collider = GetComponent<Collider2D>();
        entity.OnDeath += OnDeath;
        entity.OnHit += OnHit;
    }
    void OnHit()
    {
        isoAnim.Hit();

    }
    void OnDeath()
    {
        isoAnim.Death();
        if (navigator)
        {
            Destroy(navigator);
        }
        if(collider){
            Destroy(collider);
        }
        Destroy(this);
    }
}
