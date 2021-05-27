using UnityEngine;

public class StickmanDanceZoneBehaviour : ZoneBehaviour
{
    [SerializeField] private Animator animator;

    
    protected override void OnActivation()
    {
        base.OnActivation();
        animator.SetTrigger("Dance");
    }

    protected override void OnDeactivation()
    {
        base.OnDeactivation();
        animator.SetTrigger("Idle");
    }

}
