using GAME;
using UnityEngine;

public class Hero : HeroBase
{
    public override void init(DATA.HeroData data, int pos)
    {
        animator = transform.GetComponent<Animator>();

        base.init(data, pos);
    }
    public override void attack(bool isCrit)
    {
        Debug.Log("[Simulation] hero attack!");
        if (animator)
        {
            string attackState = isCrit == true ? "hero9_critical" : "hero9_normal";
            animator.SetTrigger(attackState);
        }
    }
    public override void hit(float beforeHP, float damage, bool isCrit)
    {
    }
}
