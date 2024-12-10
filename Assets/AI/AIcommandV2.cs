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
        yield return new WaitForSeconds(2f);
        while (true)
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
                    if (group.power >= othergroup.power)
                    {
                        //勝てるので突進
                        destination = othergroup.center - group.center;
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
                        entity.GetComponent<NavMeshAgent>().destination =
                            entity.transform.position + destination;
                    }
                }
            }
            yield return null;
        }
    }

    public List<Clustering.Group> GetOtherGroups()
    {
        // 全てのグループを格納するリスト
        List<Clustering.Group> allGroups = new List<Clustering.Group>();

        // Clustering コンポーネントを取得してループ
        foreach (var otherClustering in GetComponents<Clustering>())
        {
            if (otherClustering != clustaring)
                // 各 Clustering の groups を null チェックして追加
                if (otherClustering.groups != null)
                {
                    allGroups.AddRange(otherClustering.groups);
                }
        }

        return allGroups;
    }
}
