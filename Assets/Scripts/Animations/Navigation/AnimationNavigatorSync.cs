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
        navigator.active = isoAnim.IsMovementAnimation();
        if (navigator.reached)
        {
            isoAnim.UpdateAnimator(navigator.direction * .25f);
        }
        else
        {

            isoAnim.UpdateAnimator((navigator.speed / navigator.moveSpeed) * navigator.direction);
        }
    }


}
