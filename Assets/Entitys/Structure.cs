using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.AI;
public class Structure : Entity
{
    //建物のステータス(列挙型)
    public enum Status
    {

        LocationChoseing,//場所を選んでる
        Constaracting,//建ててもらってる
        Complete//建て終わった
    }
    public int constractionCost;
    protected internal NavMeshObstacle navMeshObstacle;

    // Start is called before the first frame update
    // public bool isCompletion;//竣工したか
    public Status status;
    drag drag;
    protected override void Start()
    {
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        if (status != Status.Complete)
        {
            hp = 0;
        }


        if (status == Status.LocationChoseing)
        {
            gameObject.tag = "Untagged";
            okButton = transform.Find("UI/positionSet/ok").GetComponent<Button>();
            cancelButton = transform.Find("UI/positionSet/cancel").GetComponent<Button>();
            // ボタンに関数を設定
            okButton.onClick.AddListener(ok);
            cancelButton.onClick.AddListener(cancel);
            navMeshObstacle.enabled = false;
            if (drag == null) drag = this.gameObject.AddComponent<drag>();

        }
        else
        {
            destroyPositionsetUI();
        }

        base.Start();
    }
    void destroyPositionsetUI()
    {
        Destroy(transform.Find("UI/positionSet").gameObject);
    }
    //okボタンが押されたときに実行される
    public virtual void ok()
    {
        print("ok");
        status = Status.Constaracting;
        if (navMeshObstacle == null) navMeshObstacle = GetComponent<NavMeshObstacle>();
        navMeshObstacle.enabled = true;
        Destroy(drag);
        destroyPositionsetUI();
        callBuilder();
        gameObject.tag = "entity";

    }
    //cancelボタンが押されたときに実行される
    public void cancel()
    {
        print("cancel");
        Destroy(this.gameObject);
    }
    Button okButton;
    Button cancelButton;
    protected override void Update()
    {

        if (status == Status.LocationChoseing)
        {
            // 他オブジェクトと被ってたらOKボタンを無効化
            okButton.interactable = drag.isOverlapping() == 0;
        }
        base.Update();
    }


    public void callBuilder() //Builderを呼ぶ関数,TODO:あんま遠い奴呼んでもしょうがないので距離or人数とかにする?
    {
        print("Builder呼びたい");
        List<Builder> myTeamBuildes = GameObject.FindGameObjectsWithTag("entity")
        .Select(x => x.GetComponent<Builder>())
        .Where(x => x != null && x.team == team).ToList();
        foreach (var builder in myTeamBuildes)
        {
            // if (builder.Tasks.Count == 0)
            // {
            print("Builder呼ぼう");
            builder.setTask("buildStructureCMD", new object[] { this });
            // }
        }
    }


    public virtual void construction(int h)//builderが呼ぶ奴、引数分だけ建築が進んで良い感じにお金が引かれる
    {
        h = Mathf.Clamp(hp + h, 0, maxHp) - hp;
        print("construction");
        float consumptionMoney = (float)h / (float)maxHp * (float)constractionCost;
        if (consumptionMoney > teamParameter.getteamParameter(team).money)
        {
            print("お金がたりない");
            return;
        }
        teamParameter.getteamParameter(team).money -= consumptionMoney;


        hp += h;
        if (hp >= maxHp)
        {
            hp = maxHp;
            status = Status.Complete;

            //TODO:竣工した時のエフェクトとか
        }
    }



}