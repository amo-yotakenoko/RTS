using System.Collections;
using System.Collections.Generic;
// using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class Character : Entity
{
    // Start is called before the first frame update


    LineRenderer line;
    NavMeshAgent navmesh;

    protected override void Start()
    {
        lineSet();
        // ChangeColor(Entity.teamColors[team]);
        base.Start();
    }

    //行先を示すためのLineRendererを更新
    void lineSet()
    {
        navmesh = GetComponent<NavMeshAgent>();
        line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Entity.teamColors[team];
        line.endColor = Entity.teamColors[team];
        line.startWidth = 0.5f;
        line.endWidth = 0.00f;
    }

    protected override void Update()
    {
        NavMeshPath path = new NavMeshPath();
        navmesh.CalculatePath(navmesh.destination, path);

        line.SetVertexCount(path.corners.Length);
        line.SetPositions(path.corners);
    }

    // public Vector3 targetpos;
    //lineの描画https://indie-du.com/entry/2016/05/21/080000
    public IEnumerator moveCMD(Vector3 targetpos)
    {
        // destinationに行先の座標を入れるとそこに行ってくれる
        navmesh.destination = targetpos;
        //到着するまで待つ、navmeshの更新をするためにdo while文で書いてます
        do
        {
            yield return null;
        } while (navmesh.pathPending || navmesh.remainingDistance > 1f);
    }

    public IEnumerator moveToEntityCMD(Entity target)
    {
        NavMeshAgent navmesh = GetComponent<NavMeshAgent>();

        //到着するまでターゲットを追尾、navmeshの更新をするためにdo while文で書いてます
        do
        {
            navmesh.destination = target.transform.position;
            yield return null;
        } while (navmesh.pathPending || navmesh.remainingDistance > 1f);
    }

    public IEnumerator AttackToEntityCMD(Entity target, float distance = 100)
    {
        NavMeshAgent navmesh = GetComponent<NavMeshAgent>();
        do
        {
            if (target == null || (this.transform.position - target.transform.position).magnitude > distance) break;
            navmesh.destination = target.transform.position;

            //リーチ内に敵がいたらアタック
            Entity enemy = getWithInReachEntity();
            if (enemy != null)
            {
                //アタック中は移動を停止
                navmesh.isStopped = true;
                enemy.damage(1, this);
                yield return new WaitForSeconds(0.2f); //クールタイム
                navmesh.isStopped = false;
            }
            yield return null;
        } while (target != null); //ターゲットが消滅するまで
        yield return null;
        // Destroy(line);
    }

    public IEnumerator AttackCMD(Entity target) //AI用、リーチ内にいたら自動で攻撃
    {
        NavMeshAgent navmesh = GetComponent<NavMeshAgent>();
        Entity enemy = getWithInReachEntity();
        if (enemy != null)
        {
            navmesh.isStopped = true;
            enemy.damage(2, this);
            yield return new WaitForSeconds(1f); //クールタイム
            navmesh.isStopped = false;
        }
    }

    public Entity getWithInReachEntity(float r = 0.5f) //攻撃できる敵オブジェクトを返す、オーバーライトしたら攻撃範囲の広いキャラとか作れるかも?
    {
        //自分の目の前の当たり判定をすべて取得
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position + transform.forward * r * 2,
            r
        );
        foreach (var hit in hitColliders)
        {
            Entity entity = hit.gameObject.GetComponent<Entity>();
            if (entity != null && entity.team != this.team)
            {
                return entity;
            }
        }
        return null;
    }

    public override void damage(int damage, Entity attacked = null)
    {
        //攻撃した北奴に反撃(優先度高)
        if (attacked != null)
            setTask("AttackToEntityCMD", new object[] { attacked, 5 }, priority: 10);
        base.damage(damage, attacked);
    }


}
