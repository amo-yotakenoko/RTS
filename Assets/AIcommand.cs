using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

//敵AI用
public class AIcommand : MonoBehaviour
{
    public float sumAddDamage;

    public int countCharacters()
    {
        return GameObject
            .FindGameObjectsWithTag("entity")
            .Select(x => x.GetComponent<Character>())
            .Where(x => x != null && x.team == team)
            .ToList()
            .Count();
    }

    hashSearch hashSearch;

    [System.Serializable]
    public class Parameter
    {
        public float attack; // 攻撃する閾値
        public float escape; // 逃げる閾値

        public float scattering; //拡散

        public float attackVector; // 攻撃ベクトル
        public float escapeVector; // 逃げるベクトル
        public float backVector; // バックベクトル

        public override string ToString()
        {
            return $"{attack},{escape},{scattering},{attackVector},{escapeVector},{backVector}";
        }

        public void RandomizeParameters(float maxDelta = 1f)
        {
            switch (Random.Range(0, 6))
            {
                case 0:
                    attack += UnityEngine.Random.Range(-maxDelta, maxDelta);
                    break;
                case 1:
                    escape += UnityEngine.Random.Range(-maxDelta, maxDelta);
                    break;
                case 2:
                    scattering += UnityEngine.Random.Range(-maxDelta, maxDelta);
                    break;
                case 3:
                    attackVector += UnityEngine.Random.Range(-maxDelta, maxDelta);
                    break;
                case 4:
                    escapeVector += UnityEngine.Random.Range(-maxDelta, maxDelta);
                    break;
                case 5:
                    backVector += UnityEngine.Random.Range(-maxDelta, maxDelta);
                    break;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        hashSearch = GetComponent<hashSearch>();
        StartCoroutine(loop());
    }

    public int team;

    // public float bravery;
    public Parameter parameter;

    // Update is called once per frame
    void Update()
    {
        // return;
        if (Input.GetKey(KeyCode.Space))
        {
            for (int x = -80; x < 80; x += 3)
            {
                for (int z = -80; z < 80; z += 3)
                {
                    Vector3 pos = new Vector3(x, 0, z);

                    var enemypower = getPower(pos, team);
                }
            }
        }
    }

    IEnumerator loop()
    {
        while (true)
        {
            List<Character> myTeamCharacters = GameObject
                .FindGameObjectsWithTag("entity")
                .Select(x => x.GetComponent<Character>())
                .Where(x => x != null && x.team == team)
                .ToList();
            foreach (var character in myTeamCharacters)
            {
                if (character.Tasks.Count <= 0)
                {
                    Vector3 pos = character.transform.position;
                    var power = getPower(new Vector3(pos.x, 0, pos.z), team);
                    (float power, Vector3 grad) enemypower = (0, new Vector3(0, 0, 0));
                    for (int i = 0; i < 5; i++)
                    {
                        if (i == team)
                            continue;
                        var ep = getPower(new Vector3(pos.x, 0, pos.z), i);
                        enemypower.power += ep.power;
                        enemypower.grad += ep.grad;
                    }

                    if (power.power + parameter.attack > enemypower.power)
                    {
                        //攻め
                        var attackCandidate = new List<Entity>();
                        foreach (
                            var enemy in hashSearch
                                .searchEntity(pos, 20)
                                .Select(x => x.GetComponent<Entity>())
                                .Where(x => x != null && x.team != team)
                        )
                        {
                            var ep = getPower(
                                new Vector3(
                                    enemy.transform.position.x,
                                    0,
                                    enemy.transform.position.z
                                ),
                                enemy.team
                            );
                            if (ep.power < power.power)
                            {
                                attackCandidate.Add(enemy);
                            }
                            else
                            {
                                // destination += power.grad.normalized * 5;
                            }
                        }
                        // if (character.Tasks.Count() == 0)
                        // {

                        var sortedAttackCandidate = attackCandidate
                            .OrderBy(enemy =>
                                Vector3.Distance(transform.position, enemy.transform.position)
                            )
                            .Where(enemy => !(enemy is Wall))
                            .FirstOrDefault();
                        if (sortedAttackCandidate != null)
                        {
                            character.setTask(
                                "AttackToEntityCMD",
                                new object[] { sortedAttackCandidate, 100 },
                                place: sortedAttackCandidate.transform
                            );
                            // return; //一気に目標が変わって重くならない様に1フレームに1キャラだけ指示
                            break;
                        }
                        // }
                    }
                    else
                    {
                        //逃げ
                        Vector3 destination = new Vector3(0, 0, 0);
                        destination -= enemypower.grad.normalized * parameter.escapeVector;
                        destination += power.grad.normalized * parameter.backVector;
                        character.GetComponent<NavMeshAgent>().destination =
                            character.transform.position + destination.normalized * 1;
                    }

                    // Vector3 destination = new Vector3(0, 0, 0);
                    // foreach (var otherMyTeamEntitys in myTeamCharacters.Where(x => x != character))
                    // {
                    //     Vector3 diff = character.transform.position - otherMyTeamEntitys.transform.position;
                    //     destination += diff.normalized * (1 / diff.magnitude) * parameter.scattering;
                    // }

                    // if (power.power + parameter.attack > enemypower.power)
                    // {
                    //     destination += enemypower.grad.normalized * parameter.attackVector;
                    //     // print("おっかけ");

                    // }
                    // else if (power.power + parameter.escape < enemypower.power)
                    // {
                    //     destination -= enemypower.grad.normalized * parameter.escapeVector;
                    //     destination += power.grad.normalized * parameter.backVector;
                    // }


                    // else if (enemypower.power > 0)
                    // {
                    //     print("逃げ");
                    //     destination += power.grad.normalized * 5;
                    // }


                    Entity attackEntity = character.getWithInReachEntity();
                    if (attackEntity != null)
                    {
                        sumAddDamage += 1;
                        character.setTask("AttackCMD", new object[] { null });
                    }

                    // character.GetComponent<NavMeshAgent>().destination = character.transform.position + destination.normalized * 2;
                }
                yield return null;
            }
            yield return null;
        }
    }

    //勢力を取得する、英語だとforceの方があってたりする?
    public (float power, Vector3 grad) getPower(Vector3 pos, int team)
    {
        float power = 0;
        Vector3 grad = new Vector3(0, 0, 0);
        // GameObject.FindGameObjectsWithTag("entity")から変更
        foreach (var entity in hashSearch.searchEntity(pos, 10))
        {
            Vector3 diff = entity.position - pos;

            // if (diff.magnitude <= 0) continue;
            Entity EntityComponent = entity.GetComponent<Entity>();
            float sig = sigmoid(diff.magnitude) * EntityComponent.hp;
            if (EntityComponent.team == team && team != 0)
            {
                power += sig * EntityComponent.hp * EntityComponent.team != 0 ? 1f : 0f;

                grad += diff.normalized * (1.0f - sig) * sig * EntityComponent.hp;
            }
            // else
            // {
            //     power -= p;
            //     grad -= diff.normalized * p;
            // }
        }
        // print(power);

        Debug.DrawRay(pos, new Vector3(0, power * 0.2f, 0), Entity.teamColors[team]);
        Debug.DrawRay(
            pos + new Vector3(0, power * 0.2f, 0),
            grad.normalized * 3f,
            Entity.teamColors[team]
        );
        return (power, grad);
    }

    float sigmoid(float x)
    {
        return 1.0f / (1.0f + Mathf.Exp(0.5f * x));
    }
}
