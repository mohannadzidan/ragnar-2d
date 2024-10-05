using UnityEngine;


[RequireComponent(typeof(IsoCharacterAnimator))]
[RequireComponent(typeof(Navigator))]
[DisallowMultipleComponent()]
public class AnimationNavigatorSync : MonoBehaviour
{
    private Navigator navigator;

    private IsoCharacterAnimator isoAnim;


    void Start()
    {
        navigator = GetComponent<Navigator>();
        isoAnim = GetComponent<IsoCharacterAnimator>();
    }


    void Update()
    {
        var norm = navigator.direction.normalized;
        Vector2 isometricDirection = new Vector2(norm.x, norm.y * 2f).normalized;
        navigator.active = isoAnim.IsMovementAnimation();
        if (navigator.reached)
        {
            isoAnim.UpdateAnimator(isometricDirection * .25f);
        }
        else
        {
            isoAnim.UpdateAnimator((navigator.speed / navigator.moveSpeed) * isometricDirection);
        }
    }

}
