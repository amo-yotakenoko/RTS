using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using System.IO;
public class AIcommand : MonoBehaviour
{
    [System.Serializable]
    public class Parameter
    {
        public float attack;      // 攻撃する閾値
        public float escape;      // 逃げる閾値
        private float _scattering; // 拡散
        public float scattering
        {
            get { return _scattering; }
            set { _scattering = Mathf.Clamp(value, -1f, 2f); }
        }
        public float attackVector; // 攻撃ベクトル
        public float escapeVector; // 逃げるベクトル
        public float backVector;   // バックベクトル

        public override string ToString()
        {
            return $"{attack},{escape},{scattering},{attackVector},{escapeVector},{backVector}";
        }

        public void RandomizeParameters(float maxDelta = 1f)
        {
            attack += UnityEngine.Random.Range(-maxDelta, maxDelta);
            escape += UnityEngine.Random.Range(-maxDelta, maxDelta);
            scattering += UnityEngine.Random.Range(-maxDelta, maxDelta);
            attackVector += UnityEngine.Random.Range(-maxDelta, maxDelta);
            escapeVector += UnityEngine.Random.Range(-maxDelta, maxDelta);
            backVector += UnityEngine.Random.Range(-maxDelta, maxDelta);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    public int team;
    // public float bravery;
    public Parameter parameter;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {

            for (int x = -20; x < 20; x += 2)
            {
                for (int z = -20; z < 20; z += 2)
                {
                    Vector3 pos = new Vector3(x, 0, z);
                    for (int i = 0; i < 5; i++)
                    {
                        var enemypower = getPower(pos, i);

                    }

                }
            }
        }
        List<Character> myTeamCharacters = GameObject.FindGameObjectsWithTag("entity").Select(x => x.GetComponent<Character>()).Where(x => x != null && x.team == team).ToList();
        foreach (var character in myTeamCharacters)
        {
            if (character.Tasks.Count <= 0)
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



                Vector3 destination = new Vector3(0,0,0);
                foreach (var otherMyTeamEntitys in myTeamCharacters.Where(x => x != character))
                {
                    Vector3 diff = character.transform.position - otherMyTeamEntitys.transform.position;
                    destination += diff.normalized * (1 / diff.magnitude) * parameter.scattering;
                }

                // destination += power.grad.normalized;//仲間と集まる

                // if (power.power + enemypower.power < 0)
                // {
                //     print("にげ");
                //     //自陣に逃げる
                // destination = character.transform.position + power.grad.normalized;
                // }
                // print(power.power + "," + enemypower.power);
                if (power.power + parameter.attack > enemypower.power)
                {
                    destination += enemypower.grad.normalized * parameter.attackVector;
                    // print("おっかけ");

                }
                else if (power.power + parameter.escape < enemypower.power)
                {
                    destination -= enemypower.grad.normalized * parameter.escapeVector;
                    destination += power.grad.normalized * parameter.backVector;
                }


                // else if (enemypower.power > 0)
                // {
                //     print("逃げ");
                //     destination += power.grad.normalized * 5;
                // }


                Entity attackEntity = character.getWithInReachEntity();
                if (attackEntity != null) character.setTask("AttackCMD", new object[] { null });


                character.GetComponent<NavMeshAgent>().destination = character.transform.position+destination.normalized*2;
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
            // if (diff.magnitude <= 0) continue;
            float p = sigmoid(diff.magnitude) * entity.GetComponent<Entity>().hp;
            if (entity.GetComponent<Entity>().team == team)
            {
                power += p;

                grad += diff.normalized * p;
            }
            // else
            // {
            //     power -= p;
            //     grad -= diff.normalized * p;
            // }
        }
        // print(power);

        Debug.DrawRay(pos, new Vector3(0, power*5, 0), Entity.teamColors[team]);
        Debug.DrawRay(pos + new Vector3(0, power*5, 0), grad / 20f, Entity.teamColors[team]);
        return (power, grad);
    }
    float sigmoid(float x)
    {
        return 1.0f / (1.0f + Mathf.Exp(0.5f * x));

    }
}
