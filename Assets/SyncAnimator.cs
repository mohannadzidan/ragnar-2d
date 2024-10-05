using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent()]
public class SyncAnimator : MonoBehaviour
{

    public Animator reference;
    Animator animator;

    AnimatorControllerParameter[] parameters;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        parameters = reference.parameters;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var param in parameters)
        {
            var hash = param.nameHash;
            switch (param.type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(hash, reference.GetFloat(hash));
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(hash, reference.GetBool(hash));
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(hash, reference.GetInteger(hash));
                    break;
                case AnimatorControllerParameterType.Trigger:
                    if (reference.GetBool(hash)) animator.SetTrigger(hash);
                    else animator.ResetTrigger(hash);
                    break;

            }
        }

    }
}
