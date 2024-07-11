using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Entity
{
    protected override void Start()
    {
        GetComponent<Renderer>().material.color = Color.green;//仮対応
        base.Start();
    }
    protected override void killed(int attackedTeam = 0)
    {
        teamParameter.getteamParameter(attackedTeam).money += 5;

        base.killed(attackedTeam);
    }
    // public override void damage(int damage)
    // {

    //     base.damage(damage);
    // }
}
