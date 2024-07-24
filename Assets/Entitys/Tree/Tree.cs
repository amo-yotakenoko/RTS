using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Entity
{
    protected override void Start()
    {
        GetComponent<Renderer>().material.color = Color.green; //仮対応
        base.Start();
    }

    protected override void killed(Entity attacked = null)
    {
        teamParameter.getteamParameter(attacked.team).money += 5;

        base.killed(attacked);
    }
    // public override void damage(int damage)
    // {

    //     base.damage(damage);
    // }
}
