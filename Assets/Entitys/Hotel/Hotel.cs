using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotel : Structure
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

    protected override void Update()
    {
        base.Update();
        if (status == Status.Complete && Tasks.Count == 0)
        {
            int cost = 5;
            if (teamParameter.getteamParameter(team).money > cost)
            {
                teamParameter.getteamParameter(team).money -= cost;
                setTask("SexForSoldierCMD", new object[] { }, priority: 0);
            }
        }
    }
}
