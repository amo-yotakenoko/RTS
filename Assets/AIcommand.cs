using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
public class AIcommand : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public int team;

    // Update is called once per frame
    void Update()
    {
        for (int x = -20; x < 20; x += 2)
        {
            for (int z = -20; z < 20; z += 2)
            {
                Vector3 pos = new Vector3(x, 0, z);
                // var enemypower = getPower(pos, 1);

                // var power = getPower(pos, team);

            }
        }

        foreach (var character in GameObject.FindGameObjectsWithTag("entity").Select(x => x.GetComponent<Character>()).Where(x => x != null && x.team == team))
        {
            Vector3 pos = character.transform.position;
            var power = getPower(new Vector3(pos.x, 0, pos.z), team);
            (float power, Vector3 grad) enemypower = (0, new Vector3(0, 0, 0));
            for (int i = 0; i < 5; i++)
            {
                if (i == team) continue;
                var ep = getPower(new Vector3(pos.x, 0, pos.z), i);
                enemypower.power += ep.power;
                enemypower.grad += ep.grad;
            }



            Vector3 targetpos = character.transform.position;


            // targetpos += power.grad.normalized;//仲間と集まる

            // if (power.power + enemypower.power < 0)
            // {
            //     print("にげ");
            //     //自陣に逃げる
            targetpos = character.transform.position + power.grad.normalized;
            // }
            print(power.power + "," + enemypower.power);
            if (power.power > 0)
            {
                print("おっかけ");

                targetpos = character.transform.position + enemypower.grad.normalized;
            }
            else if (enemypower.power > 0)
            {
                print("逃げ");
                targetpos = character.transform.position + power.grad.normalized;
            }
            Entity attackEntity = character.getWithInReachEntity();
            if (attackEntity != null) character.setTask("AttackCMD", new object[] { null });

            character.GetComponent<NavMeshAgent>().destination = targetpos;
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
            // if (diff.magnitude <= 0) continue;
            float p = sigmoid(diff.magnitude);
            if (entity.GetComponent<Entity>().team == team)
            {
                power += p;

                grad += diff.normalized * p;
            }
            else
            {
                power -= p;
                grad -= diff.normalized * p;
            }
        }
        // print(power);

        Debug.DrawRay(pos, new Vector3(0, power * 20, 0), Entity.teamColors[team]);
        Debug.DrawRay(pos + new Vector3(0, power * 20, 0), grad, Entity.teamColors[team]);
        return (power, grad);
    }
    float sigmoid(float x)
    {
        return 1.0f / (1.0f + Mathf.Exp(0.5f * x));

    }
}
