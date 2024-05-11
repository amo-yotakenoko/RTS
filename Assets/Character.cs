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
        while (navmesh.remainingDistance > 1f)
        {
            NavMeshPath path = new NavMeshPath();

            navmesh.CalculatePath(targetpos, path);

            line.SetVertexCount(path.corners.Length);
            line.SetPositions(path.corners);
            yield return null;

        }
        Destroy(line);
        yield return null;
    }

    public IEnumerator moveToEntityCMD(Entity target)
    {
        NavMeshAgent navmesh = GetComponent<NavMeshAgent>();

        do
        {
            navmesh.destination = target.transform.position;
            yield return null;
        } while (navmesh.remainingDistance > 1f);

    }
}
