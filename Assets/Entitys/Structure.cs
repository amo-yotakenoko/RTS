using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Structure : Entity
{
    // Start is called before the first frame update
    public bool isCompletion;//竣工したか
    void Start()
    {
        if (!isCompletion)
        {
            hp = 0;

        }
        base.Start();
    }
    void Update()
    {

        if (!isCompletion)
        {
            callBuilder();
        }
        base.Update();
    }


    void callBuilder() //Builderを呼ぶ関数
    {

        List<Builder> myTeamBuildes = GameObject.FindGameObjectsWithTag("entity")
        .Select(x => x.GetComponent<Builder>())
        .Where(x => x != null && x.team == team).ToList();
        foreach (var builder in myTeamBuildes)
        {
            if (builder.Tasks.Count == 0)
            {
                print("Builder呼ぼう");
                builder.setTask("buildStructureCMD", new object[] { this });
            }
        }
    }


    public void construction(int h)
    {
        hp += h;
        if (hp >= maxHp)
        {
            hp = maxHp;
            isCompletion = true;
            //TODO:竣工した時のエフェクトとか
        }
    }




    // // Update is called once per frame
    // void Update()
    // {

    // }
}