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
    protected override void killed()
    {
        //TODO:お金を手に入れる処理
        base.killed();
    }
    public override void damage(int damage)
    {

        base.damage(damage);
    }
}
