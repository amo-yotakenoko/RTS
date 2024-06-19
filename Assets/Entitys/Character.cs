using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
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
    void lineSet()
    {
        navmesh = GetComponent<NavMeshAgent>();
        line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Entity.teamColors[team];
        line.endColor = Entity.teamColors[team];
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
        // LineRenderer line = gameObject.AddComponent<LineRenderer>();
        // NavMeshAgent navmesh = GetComponent<NavMeshAgent>();
        navmesh.destination = targetpos;
        // NavMeshPath path = new NavMeshPath();


        do
        {
            // print(navmesh.pathPending + "," + navmesh.remainingDistance);

            // print(path.corners.Length);

            // navmesh.CalculatePath(targetpos, path);

            // line.SetVertexCount(path.corners.Length);
            // line.SetPositions(path.corners);
            yield return null;
            // print(navmesh.pathStatus);
        } while (navmesh.pathPending || navmesh.remainingDistance > 1f);
        // Destroy(line);

    }

    public IEnumerator moveToEntityCMD(Entity target)
    {
        // LineRenderer line = gameObject.AddComponent<LineRenderer>();
        NavMeshAgent navmesh = GetComponent<NavMeshAgent>();
        // NavMeshPath path = new NavMeshPath();

        do
        {
            navmesh.destination = target.transform.position;


            // navmesh.destination = target.transform.position;
            // navmesh.CalculatePath(target.transform.position, path);

            // // print(navmesh.pathPending + "," + navmesh.remainingDistance);

            // Vector3[] corners = new Vector3[10];
            // navmesh.CalculatePath(target.transform.position, path);
            // int cornerCount = path.GetCornersNonAlloc(corners);
            // line.SetVertexCount(cornerCount);
            // line.SetPositions(corners);
            // yield return new WaitForSeconds(1);
            yield return null;

        } while (navmesh.pathPending || navmesh.remainingDistance > 1f);
        // Destroy(line);
    }

    public IEnumerator AttackToEntityCMD(Entity target)
    {
        // LineRenderer line = gameObject.AddComponent<LineRenderer>();
        NavMeshAgent navmesh = GetComponent<NavMeshAgent>();


        do
        {
            navmesh.destination = target.transform.position;



            // NavMeshPath path = new NavMeshPath();
            // navmesh.CalculatePath(target.transform.position, path);
            // line.SetVertexCount(path.corners.Length);
            // line.SetPositions(path.corners);


            Entity enemy = getWithInReachEntity();
            if (enemy != null)
            {
                navmesh.isStopped = true;
                enemy.damage(2, team);
                yield return new WaitForSeconds(0.1f);//クールタイム
                navmesh.isStopped = false;
            }

            yield return null;


        } while (target != null);
        yield return null;
        // Destroy(line);


    }

    public IEnumerator AttackCMD(Entity target)//AI用
    {
        NavMeshAgent navmesh = GetComponent<NavMeshAgent>();
        Entity enemy = getWithInReachEntity();
        if (enemy != null)
        {
            navmesh.isStopped = true;
            enemy.damage(2, team);
            yield return new WaitForSeconds(1f);//クールタイム
            navmesh.isStopped = false;
        }
    }

    public Entity getWithInReachEntity(float r = 0.5f)//攻撃できるオブジェクトを返す
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * r * 2, r);
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
}
