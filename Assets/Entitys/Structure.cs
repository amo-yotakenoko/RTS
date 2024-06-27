using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
public class Structure : Entity
{
    public enum Status
    {

        LocationChoseing,
        InProgress,
        Complete
    }

    // Start is called before the first frame update
    // public bool isCompletion;//竣工したか
    public Status status;
    protected override void Start()
    {
        if (status != Status.Complete)
        {
            hp = 0;
        }
        // ボタンに関数を設定

        Button okButton = transform.Find("UI/positionSet/ok").GetComponent<Button>();
        Button cancelButton = transform.Find("UI/positionSet/cancel").GetComponent<Button>();

        if (status == Status.LocationChoseing)
        {
            okButton.onClick.AddListener(ok);
            cancelButton.onClick.AddListener(cancel);
        }
        else
        {
            Destroy(okButton.gameObject);
            Destroy(cancelButton.gameObject);
        }

        base.Start();
    }

    public void ok()
    {
        print("ok");
        status = Status.InProgress;
        callBuilder();
    }
    public void cancel()
    {
        print("cancel");
        Destroy(this.gameObject);
    }
    protected override void Update()
    {
        // print("建てられた");
        // if (status != Status.Complete)
        // {
        //     callBuilder();
        // }
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
            status = Status.Complete;
            //TODO:竣工した時のエフェクトとか
        }
    }




    // // Update is called once per frame
    // void Update()
    // {

    // }
}