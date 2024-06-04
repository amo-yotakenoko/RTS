using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Entity
{
   protected  override void killed(){
        print("復活!");
        hp += 10;
    }
    public override void damage(int damage){
        // https://futabazemi.net/unity/random_color
        // ダメージを受けると色が変わる
        GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value, 1.0f);

        //元あったプログラムを上書きしてるので必要な奴は書き足して
        hp -= damage;
        if(hp<=0){
            killed();
        }
    }
}
