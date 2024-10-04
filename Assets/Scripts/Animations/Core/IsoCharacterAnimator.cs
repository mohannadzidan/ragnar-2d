using UnityEngine;

public class IsoCharacterAnimator : MonoBehaviour
{

    public string xAnimationParamName = "Horizontal";
    public string yAnimationParamName = "Vertical";
    public string movementStateTag = "Movement";

    public string deathTriggerName = "Death";
    public string hitTriggerName = "Hit";
    public string attackTriggerName = "Attack";

    public bool flipWhenRotate = false;




    private Animator animator;
    private Vector3 initialScale;
    private int xAnimHash;
    private int yAnimHash;
    private int animDeath;
    private int animHit;
    private int animAttack;
    private int animMovementState;



    void Start()
    {
        // Get reference to the Animator component
        animator = GetComponent<Animator>();
        initialScale = transform.localScale;
        xAnimHash = Animator.StringToHash(xAnimationParamName);
        yAnimHash = Animator.StringToHash(yAnimationParamName);
        animDeath = Animator.StringToHash(deathTriggerName);
        animHit = Animator.StringToHash(hitTriggerName);
        animAttack = Animator.StringToHash(attackTriggerName);
        animMovementState = Animator.StringToHash(movementStateTag);

    }

    // Update animator parameters based on movement direction
    public void UpdateAnimator(Vector2 movement)
    {
        animator.SetFloat(xAnimHash, movement.x);
        animator.SetFloat(yAnimHash, movement.y);
        if (flipWhenRotate)
        {
            transform.localScale = new Vector3(-Mathf.Sign(movement.x) * initialScale.x, initialScale.y, initialScale.z);
        }
    }

    public void LookAt(Vector3 position)
    {
        var delta = position - transform.position;
        delta.z = 0;
        UpdateAnimator(delta.normalized * 0.25f);
    }

    public void Hit()
    {
        animator.SetTrigger(animHit);
    }

    public void Attack()
    {
        animator.SetTrigger(animAttack);
    }
    public void AbortAttack()
    {
        animator.ResetTrigger(animAttack);
    }


    public void Death()
    {
        animator.SetTrigger(animDeath);
    }

    public bool IsMovementAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).tagHash == animMovementState;
    }


    public void UpdateVelocity(Vector3 velocity)
    {
        // animator.SetFloat(xAnimHash, velocity.x);
        // animator.SetFloat(yAnimHash, velocity.y);
        // var delta = transform.posvelocity.x
        // var distance = delta.magnitude;
        // var actualVelocity = distance / Time.deltaTime;
        // velocity = Mathf.SmoothDamp(velocity, actualVelocity, ref acceleration, 0.3f);
        // if (actualVelocity > 0)
        // {
        //     direction = distance > 0 ? delta / distance : Vector2.zero;
        //     var factor = velocity / speed;

        //     if (flipWhenRotate)
        //     {
        //         transform.localScale = new Vector3(-Mathf.Sign(direction.x) * initialScale.x, initialScale.y, initialScale.z);
        //     }
        // }

        // previousPosition = transform.position;

    }

}
