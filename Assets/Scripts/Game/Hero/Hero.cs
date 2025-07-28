using GAME;
using UnityEngine;

public class Hero : HeroBase
{
    public override void init(DATA.HeroData data, int pos)
    {
        base.init(data, pos);
    }
    public override void attack()
    {
        Debug.Log("[Simulation] hero attack!");
    }
    public override void hit(float beforeHP, float damage, bool isCrit)
    {
    }
}
