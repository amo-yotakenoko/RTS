using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class AIcommandV2 : MonoBehaviour
{
    hashSearch hashSearch;
    public Clustering clustaring;

    // hashSearch hashSearch;
    void Start()
    {
        hashSearch = GetComponent<hashSearch>();
        StartCoroutine(loop());
        clustaring = gameObject.AddComponent<Clustering>();
        clustaring.team = team;
    }

    public int team;

    // Update is called once per frame
    void Update() { }

    IEnumerator loop()
    {
        print("loop");
        yield return new WaitForSeconds(1f);
        while (true)
        {
            yield return constraction();
            try
            {
                foreach (var group in clustaring.groups)
                {
                    var sortedGroups = GetOtherGroups()
                        .OrderBy(x => Vector3.Distance(group.center, x.center));

                    Vector3 destination = new Vector3(0, 0, 0);
                    foreach (
                        var othergroup in sortedGroups.OrderBy(g =>
                            Vector3.Distance(group.center, g.center)
                        )
                    )
                    {
                        if (group.power + 5 >= othergroup.power)
                        {
                            //勝てるので突進
                            Vector3 direction = (othergroup.center - group.center).normalized;
                            float moveDistance = Mathf.Min(
                                Vector3.Distance(group.center, othergroup.center),
                                5f
                            );
                            destination = -(group.center + direction * moveDistance);

                            break;
                        }
                        else
                        {
                            // Ally:味方
                            var nearAllyGroup = clustaring
                                .groups.Where(g => g != group) // 自分自身を除外
                                .OrderBy(g => Vector3.Distance(group.center, g.center)) // 距離でソート
                                .FirstOrDefault(); // 最も近いグループを取得

                            if (nearAllyGroup != null)
                            {
                                destination = (nearAllyGroup.center - group.center);
                                break;
                            }
                        }
                    }

                    foreach (var entity in group.entitys)
                    {
                        if (entity is Character character)
                        {
                            // `entity` が `Character` 型の場合に処理を続行
                            Entity attackEntity = character.getWithInReachEntity();

                            if (attackEntity != null)
                            {
                                entity.setTask("AttackCMD", new object[] { null });
                            }
                        }

                        if (entity.Tasks.Count() > 0)
                            continue;

                        NavMeshAgent agent = entity.GetComponent<NavMeshAgent>();
                        if (agent != null)
                        {
                            Vector3 nowDestination = entity
                                .GetComponent<NavMeshAgent>()
                                .destination;

                            Vector3 nowVector = entity.transform.position - nowDestination;

                            entity.GetComponent<NavMeshAgent>().destination =
                                entity.transform.position + destination;
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("エラーが発生しました: " + e.Message);
            }
            yield return null;
        }
    }

    [SerializeField]
    private StructureDatas structureDatabase;

    IEnumerator constraction()
    {
        print("たてたい" + teamParameter.getteamParameter(team).money);
        yield return null;

        if (teamParameter.getteamParameter(team).money > 120)
        {
            bool allComplete = GameObject
                .FindGameObjectsWithTag("entity")
                .Select(x => x.GetComponent<Structure>())
                .Where(x => x != null && x.team == team)
                .All(s => s.status == Structure.Status.Complete);

            if (allComplete)
            {
                print("たてる");
                var structureData = structureDatabase
                    .structures.Where(x => x.name == "hotel")
                    .FirstOrDefault();

                var strongestGroup = clustaring
                    .groups.OrderByDescending(group => group.power)
                    .FirstOrDefault();

                // print($" {structureData.name}を建築");
                yield return constractionPlace(
                    structureData,
                    strongestGroup.center + new Vector3(0, 0, 0)
                );
            }
        }
    }

    IEnumerator constractionPlace(StructureData structureData, Vector3 position)
    {
        print($" {team}{structureData.name}をたてる");
        var instantiatedStructure = Instantiate(
                structureData.prefab,
                position,
                structureData.prefab.transform.rotation
            )
            .GetComponent<Structure>();
        instantiatedStructure.status = Structure.Status.LocationChoseing;
        instantiatedStructure.team = team;
        instantiatedStructure.constractionCost = structureData.cost;
        // instantiatedStructure.gameObject.name = "自動で建てた";
        yield return null;
        float randumRange = 5;
        while (instantiatedStructure.isoverLap())
        {
            if (instantiatedStructure == null)
                yield break;
            randumRange += 1f;
            instantiatedStructure.transform.position =
                position
                + new Vector3(
                    Random.Range(-randumRange, randumRange),
                    0f,
                    Random.Range(-randumRange, randumRange)
                );
            yield return null;
        }
        instantiatedStructure.ok();
    }

    public List<Clustering.Group> GetOtherGroups()
    {
        // 全てのグループを格納するリスト
        List<Clustering.Group> allGroups = new List<Clustering.Group>();

        // Clustering コンポーネントを取得してループ
        foreach (var otherClustering in GetComponents<Clustering>())
        {
            if (otherClustering != clustaring)
                if (otherClustering.groups != null)
                {
                    allGroups.AddRange(otherClustering.groups);
                }
        }

        return allGroups;
    }
}
