using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Entity
{
    EntityUI entityUI;

    protected override void Start()
    {
        GetComponent<Renderer>().material.color = Color.green; //仮対応
        entityUI = GetComponent<EntityUI>();
        base.Start();
    }

    protected override void Update()
    {
        entityUI.Canvas.gameObject.SetActive(entityUI.hpBar.value != entityUI.hpBar.maxValue);
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
