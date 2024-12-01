using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Kmeans : MonoBehaviour
{
    // Start is called before the first frame update
    public Mesh mesh;
    public Material material;
    public int team;
    // public List<Vector3> positions = new List<Vector3>();
    void Start()
    {
        StartCoroutine(kMeans());
    }

    // Update is called once per frame
    void Update()
    {
        if (groups.Count > 0)
            foreach (var group in groups)
            {
                // print(group);
                Graphics.DrawMesh(mesh, group.center + new Vector3(0, 1, 0), Quaternion.identity, material, 0);
                Debug.DrawRay(group.center, Vector3.up * 5f, new Color(Random.value, Random.value, Random.value));


                foreach (var entity in group.entitys)
                {
                    Debug.DrawLine(group.center + new Vector3(0, 1, 0), entity.transform.position + new Vector3(0, 1, 0), Color.red);
                }
            }
    }




    public class Group
    {
        // グループの中心点や位置を表す Transform
        public Transform transform { get; set; }


        public List<Entity> entitys { get; set; }
        public Vector3 center;

        public void centerUpdate()
        {
            if (entitys.Count <= 0) return;


            center = Vector3.zero;
            foreach (var entity in entitys)
            {
                center += entity.transform.position;
            }
            center /= entitys.Count;

        }

    }
    public List<Group> groups;

    private IEnumerator kMeans()
    {
        yield return new WaitForSeconds(1f);
        var myTeamCharacters = GameObject
                       .FindGameObjectsWithTag("entity")
                       .Select(x => x.GetComponent<Entity>());
        //   .Where(x => x != null && x.team == team)
        //   .ToList();
        groups = new List<Group>();

        while (groups.Count < 4)
        {
            var pick = myTeamCharacters
           .OrderBy(character =>
                 groups.Sum(group => -Vector3.Distance(character.transform.position, group.center))
             ).FirstOrDefault();

            Group group = new Group
            {
                transform = pick.transform,
                entitys = new List<Entity> { pick },
                center = pick.transform.position
            };

            // グループをリストに追加
            groups.Add(group);

        }
        // foreach (var randomCharacter in myTeamCharacters
        // .OrderBy(x => Random.value).Take(4))
        // {

        //     // ランダムにキャラクターを選択
        //     print(randomCharacter);
        //     // 新しいグループを作成し、Transform を設定
        //     Group group = new Group
        //     {
        //         transform = randomCharacter.transform,
        //         entitys = new List<Entity> { randomCharacter },
        //         center = randomCharacter.transform.position
        //     };

        //     // グループをリストに追加
        //     groups.Add(group);
        // }



        while (true)
        {

            myTeamCharacters = GameObject
                      .FindGameObjectsWithTag("entity")
                      .Select(x => x.GetComponent<Entity>());
            foreach (var group in groups)
            {
                group.entitys.Clear();
            }



            foreach (var entity in myTeamCharacters)
            {
                var nearestGroup = groups
                    .OrderBy(group => Vector3.Distance(entity.transform.position, group.center))
                    .FirstOrDefault();

                nearestGroup?.entitys.Add(entity);
            }
            foreach (var group in groups)
            {
                group.centerUpdate();
            }


            yield return new WaitForSeconds(1f);
        }


    }

}
