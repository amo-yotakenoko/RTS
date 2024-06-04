using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Analysis : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int x = -50; x < 50; x++)
        {
            for (int z = -50; z < 50; z++)
            {
                Vector3 pos = new Vector3(x, 0, z);
                var power = getPower(pos, 1);
                if (power.power > 0)
                {

                    Debug.DrawRay(pos, new Vector3(0, power.power * 5, 0), Color.green);
                    Debug.DrawRay(pos + new Vector3(0, power.power * 5, 0), power.grad, Color.green);
                }
            }
        }
    }
    //勢力を取得する、英語だとforceの方があってたりする?
    public (float power, Vector3 grad) getPower(Vector3 pos, int team)
    {
        float power = 0;
        Vector3 grad = new Vector3(0, 0, 0);
        foreach (var entity in GameObject.FindGameObjectsWithTag("entity"))
        {
            Vector3 diff = entity.transform.position - pos;
            if (entity.GetComponent<Entity>().team == team)
            {
                power += 1 / Mathf.Pow(diff.magnitude, 2);
                grad += diff.normalized * (1 / diff.magnitude);
            }
            else
            {
                power -= 1 / Mathf.Pow(diff.magnitude, 2);
                grad -= diff.normalized * (1 / diff.magnitude);
            }
        }
        return (power, grad);
    }
}
