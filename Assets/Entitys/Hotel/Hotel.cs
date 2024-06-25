using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotel : Structure
{
    public IEnumerator SexForSoldierCMD()
    {
        print("sex!!!");
        //TODO:進捗ゲージ作る?
        yield return new WaitForSeconds(3);
        var instance = Instantiate(soldier);
        instance.GetComponent<Entity>().team = this.team;
        instance.transform.position = this.transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)); ;
        yield return null;
    }
    public GameObject soldier;
}
