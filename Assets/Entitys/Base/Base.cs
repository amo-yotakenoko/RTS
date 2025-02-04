using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : Structure
{
    public GameObject soldier;

    [Cost(10)]
    public IEnumerator SexForSoldierCMD()
    {
        print("sex!!!");
        //TODO:進捗ゲージ作る?
        yield return new WaitForSeconds(3);
        var instance = Instantiate(soldier);
        instance.GetComponent<Entity>().team = this.team;
        instance.transform.position =
            this.transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        ;
        yield return null;
    }

    public GameObject builder;

    [Cost(15)]
    public IEnumerator SexForBuilderCMD()
    {
        print("sex!!!");
        //TODO:進捗ゲージ作る?
        yield return new WaitForSeconds(3);
        var instance = Instantiate(builder);
        instance.GetComponent<Entity>().team = this.team;
        instance.transform.position =
            this.transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        ;
        yield return null;
    }

    public void Update()
    {
        base.Update();
    }

    public void Start()
    {
        base.Start();
        // StartCoroutine(continueSex());
    }

    IEnumerator continueSex()
    {
        print("sexはじめ");
        while (true)
        {
            if (Tasks.Count <= 0)
            {
                print("sex" + Tasks.Count);
                setTask("SexForSoldierCMD", arguments: null, priority: 0);

                yield return new WaitForSeconds(1);
            }

            yield return null;
        }
    }
}
