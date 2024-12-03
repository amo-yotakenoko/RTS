using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Clustering : MonoBehaviour
{
    // Start is called before the first frame update
    public Mesh mesh;
    public Material material;
    hashSearch hashSearch;

    // public List<Vector3> positions = new List<Vector3>();
    void Start()
    {
        hashSearch = GetComponent<hashSearch>();
        StartCoroutine(loop());
    }

    public int team;

    // Update is called once per frame
    void Update()
    {
        // print(groups.Count());
        if (groups.Count > 0)
            foreach (var group in groups)
            {
                // print(group);
                // Graphics.DrawMesh(mesh, group.center + new Vector3(0, 1, 0), Quaternion.identity, material, 0);
                Debug.DrawRay(
                    group.center,
                    Vector3.up * 5f,
                    new Color(Random.value, Random.value, Random.value)
                );

                foreach (var entity in group.GetEntities())
                {
                    Debug.DrawLine(
                        group.center + new Vector3(0, 1, 0),
                        entity.transform.position + new Vector3(0, 1, 0),
                        Entity.teamColors[team]
                    );
                }
            }
    }

    public class Group
    {
        public List<Entity> entitys { get; set; }

        public IEnumerable<Entity> GetEntities()
        {
            return entitys.Where(entity => entity != null && entity.gameObject != null);
        }

        public Vector3 center;

        public float deviation; //標準偏差

        public float calculateDeviation()
        {
            deviation = 0;
            foreach (var entity in GetEntities())
            {
                deviation += Mathf.Pow(Vector3.Distance(entity.transform.position, center), 2);
            }
            deviation /= GetEntities().Count();
            deviation = Mathf.Sqrt(deviation);
            return deviation;
        }
    }

    public List<Group> groups = new List<Group>();

    private IEnumerator loop()
    {
        yield return new WaitForSeconds(1f);
        var myTeamCharacters = GameObject
            .FindGameObjectsWithTag("entity")
            .Select(x => x.GetComponent<Entity>())
            .Where(x => x != null && x.team == team);
        //   .Where(x => x != null && x.team == team)
        //   .ToList();



        // foreach (var entity in myTeamCharacters)
        // {
        //     Group group = new Group
        //     {
        //         // transform = pick.transform,
        //         entitys = new List<Entity> { entity },
        //         center = entity.transform.position
        //     };

        //     groups.Add(group);
        // }


        //k means++っぽい感じで初期値を設定
        while (groups.Count < 10)
        {
            var pick = myTeamCharacters
                .Where(character => !groups.Any(group => group.GetEntities().Contains(character)))
                .OrderBy(character =>
                    groups.Sum(group =>
                        -Vector3.Distance(character.transform.position, group.center)
                    )
                )
                .FirstOrDefault();

            Group group = new Group
            {
                // transform = pick.transform,
                entitys = new List<Entity> { },
                center = pick.transform.position
            };

            // グループをリストに追加
            groups.Add(group);
        }

        StartCoroutine(groupMarge());
        // StartCoroutine(addGroupLoop());

        while (true)
        {
            centerUpdate();

            memberUpdate();
            groupDevision();

            yield return new WaitForSeconds(1f);
        }
    }

    // IEnumerator addGroupLoop()
    // {
    //     yield return new WaitForSeconds(1f);
    //     while (true)
    //     {
    //         var myTeamCharacters = GameObject
    //                             .FindGameObjectsWithTag("entity")
    //                             .Select(x => x.GetComponent<Entity>());

    //         var pick = myTeamCharacters
    //                                  .Where(character => !groups.Any(group => group.entitys.Contains(character)))
    //                              .OrderBy(character =>
    //                                    groups.Sum(group => -Vector3.Distance(character.transform.position, group.center))
    //                                ).FirstOrDefault();
    //         if (pick != null)
    //         {

    //             Group group = new Group
    //             {
    //                 // transform = pick.transform,
    //                 entitys = new List<Entity> { },
    //                 center = pick.transform.position
    //             };
    //             // グループをリストに追加
    //             groups.Add(group);
    //             print("add");
    //         }
    //         yield return null;

    //     }
    // }



    //ミーンシフトで中心を更新
    // https://qiita.com/chiba1sonny/items/2f518a8eb676329bd5e1
    void centerUpdate()
    {
        foreach (var group in groups)
        {
            var nearEntitys = hashSearch
                .searchEntity(group.center, 10)
                .Select(x => x.GetComponent<Entity>())
                .Where(x => x != null && x.team == team);

            print(nearEntitys.Count());
            if (nearEntitys.Count() > 0)
            {
                Vector3 totalPosition = Vector3.zero;
                Vector3 averagePosition = Vector3.zero;
                foreach (var entity in nearEntitys)
                {
                    averagePosition += entity.transform.position;
                }
                averagePosition /= nearEntitys.Count();

                group.center = group.center * 0.0f + averagePosition * 1f;
            }
            group.calculateDeviation();
        }
    }

    //G-meansもどきでグループを分割
    // https://knowledge.insight-lab.co.jp/aidx/k-means-g-means-x-means
    void groupDevision()
    {
        List<Group> newGroups = new List<Group>();

        foreach (var group in groups)
        {
            foreach (var entity in group.GetEntities())
            {
                print(Vector3.Distance(group.center, entity.transform.position));

                if (Vector3.Distance(group.center, entity.transform.position) > group.deviation * 1) //ここ適当
                {
                    // 外れ値
                    var outlier = group
                        .GetEntities()
                        .OrderBy(entity =>
                            -Vector3.Distance(group.center, entity.transform.position)
                        )
                        .FirstOrDefault();

                    // 新しいグループを一時的なリストに追加
                    groups.Add(
                        new Group
                        {
                            entitys = new List<Entity> { outlier },
                            center = outlier.transform.position
                        }
                    );
                    return;
                }
            }
        }
    }

    //k-meansっぽい感じで中身を設定
    void memberUpdate()
    {
        foreach (var group in groups)
        {
            group.entitys.Clear();
        }

        var myTeamCharacters = GameObject
            .FindGameObjectsWithTag("entity")
            .Select(x => x.GetComponent<Entity>())
            .Where(x => x != null && x.team == team);

        foreach (var entity in myTeamCharacters)
        {
            var nearestGroup = groups
                .OrderBy(group => Vector3.Distance(entity.transform.position, group.center))
                .FirstOrDefault();

            nearestGroup?.entitys.Add(entity);
        }
    }

    IEnumerator groupMarge()
    {
        yield return new WaitForSeconds(1f);
        List<Group> groupsToRemove = new List<Group>();
        List<Group> groupsToAdd = new List<Group>();
        List<(Group, Group)> processedPairs = new List<(Group, Group)>();
        while (true)
        {
            yield return null;
            for (int i = 0; i < groups.Count; i++)
            {
                for (int j = i + 1; j < groups.Count; j++)
                {
                    var group1 = groups[i];
                    var group2 = groups[j];

                    float distance = Vector3.Distance(group1.center, group2.center);

                    if (distance < 5)
                    {
                        print("マージ");

                        // 新しいグループを作成して追加
                        var newGroup = new Group
                        {
                            entitys = group1.entitys.Concat(group2.entitys).ToList(),
                            center = (group1.center + group2.center) / 2
                        };

                        groups.Add(newGroup);

                        groups.RemoveAt(j);
                        groups.RemoveAt(i);
                        yield return null;
                        break;
                    }

                    yield return null;
                }
            }
        }
    }
}
