using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//敵のテスト
public class EnemyTest : Entity
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(Mathf.Sin(Time.time), 0, Mathf.Cos(Time.time)) * 5 + new Vector3(5, 0, 0);
    }
}
