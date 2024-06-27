using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Builder : Character
{
    // Start is called before the first frame update
    protected override void Start()
    {
        GetComponent<Renderer>().material.color = Color.blue;//仮対応
        base.Start();
    }
    public IEnumerator buildStructureCMD(Structure target)
    {

        UnityEngine.AI.NavMeshAgent navmesh = GetComponent<UnityEngine.AI.NavMeshAgent>();


        do
        {
            float r = 0.5f;
            navmesh.destination = target.transform.position;
            if (Physics.OverlapSphere(transform.position + transform.forward * r * 2, r).Select(x => x.gameObject).Contains(target.gameObject))
            {
                navmesh.isStopped = true;
                target.construction(5);
                yield return new WaitForSeconds(1f);//クールタイム
                navmesh.isStopped = false;
            }
            if (target.status == Structure.Status.Complete) break;

            yield return null;



        } while (navmesh.pathPending || navmesh.remainingDistance > 1f);
        // Destroy(line);
    }

}
