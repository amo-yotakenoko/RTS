using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
public class Character : Entity
{
    // Start is called before the first frame update


    public Vector3 targetpos;
    //lineの描画https://indie-du.com/entry/2016/05/21/080000
    public IEnumerator moveCMD(Vector3 targetpos)
    {
        LineRenderer line = gameObject.AddComponent<LineRenderer>();
        NavMeshAgent navmesh = GetComponent<NavMeshAgent>();
        navmesh.destination = targetpos;
        NavMeshPath path = new NavMeshPath();


        do
        {
            // print(navmesh.pathPending + "," + navmesh.remainingDistance);

            // print(path.corners.Length);

            navmesh.CalculatePath(targetpos, path);

            line.SetVertexCount(path.corners.Length);
            line.SetPositions(path.corners);
            yield return null;
            // print(navmesh.pathStatus);
        } while (navmesh.pathPending || navmesh.remainingDistance > 1f);
        Destroy(line);

    }

    public IEnumerator moveToEntityCMD(Entity target)
    {
        LineRenderer line = gameObject.AddComponent<LineRenderer>();
        NavMeshAgent navmesh = GetComponent<NavMeshAgent>();
        NavMeshPath path = new NavMeshPath();

        navmesh.destination = target.transform.position;
        do
        {


            navmesh.destination = target.transform.position;
            navmesh.CalculatePath(target.transform.position, path);

            // print(navmesh.pathPending + "," + navmesh.remainingDistance);


            line.SetVertexCount(path.corners.Length);
            line.SetPositions(path.corners);
            // yield return new WaitForSeconds(1);
            yield return null;

        } while (navmesh.pathPending || navmesh.remainingDistance > 1f);
        Destroy(line);
    }

    public IEnumerator AttackToEntityCMD(Entity target)
    {
        LineRenderer line = gameObject.AddComponent<LineRenderer>();
        NavMeshAgent navmesh = GetComponent<NavMeshAgent>();
        NavMeshPath path = new NavMeshPath();

        navmesh.destination = target.transform.position;
        do
        {


            // navmesh.CalculatePath(target.transform.position, path);

            // print(navmesh.pathPending + "," + navmesh.remainingDistance);

            //一番近い点から https://docs.unity3d.com/ja/2019.4/ScriptReference/AI.NavMesh.SamplePosition.html
            // NavMeshHit hit;
            // if (NavMesh.SamplePosition(target.transform.position, out hit, 1.0f, NavMesh.AllAreas))
            // {
            //     navmesh.destination = hit.position;
            // }

            navmesh.destination = target.transform.position;
            navmesh.CalculatePath(target.transform.position, path);
            // print(navmesh.pathStatus);
            print(path.corners.Length);




            Vector3[] corners = new Vector3[10];
            navmesh.CalculatePath(targetpos, path);
            int cornerCount = path.GetCornersNonAlloc(corners);
            line.SetVertexCount(cornerCount);
            line.SetPositions(corners);
            // yield return new WaitForSeconds(1);
            // if ()
            // print(navmesh.pathStatus);

        Entity enemy= getWithInReachEntity();
        if(enemy!=null){
                navmesh.isStopped = true;
           enemy.damage(5);
                yield return new WaitForSeconds(0.1f);//クールタイム
            navmesh.isStopped = false;
        }

            yield return null;


        } while (target != null);
        Destroy(line);
    }

    Entity getWithInReachEntity(float r=0.5f){
        Collider[] hitColliders = Physics.OverlapSphere(transform.position+transform.forward*r*2, r);
        foreach (var hit in hitColliders)
        {
            Entity  entity = hit.gameObject.GetComponent<Entity>();
            if(entity!=null&&entity.team!=this.team)  {
                return entity;
            }  
        }
        return null;
    }
}
