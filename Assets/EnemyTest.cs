using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//敵が適当に動くテスト、多分もう使わない
public class EnemyTest : Entity
{
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(Mathf.Sin(Time.time), 1f / 5f, Mathf.Cos(Time.time)) * 5 + new Vector3(5, 0, 0);
    }
}
