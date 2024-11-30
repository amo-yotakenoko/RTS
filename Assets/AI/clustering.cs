using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class clustering : MonoBehaviour
{
    // Start is called before the first frame update
    public Mesh mesh;
    public Material material;
    // public List<Vector3> positions = new List<Vector3>();
    void Start()
    {
        StartCoroutine(kMeans());
    }

    // Update is called once per frame
    void Update()
    {

    }




    public class Group
    {
        // グループの中心点や位置を表す Transform
        public Transform Transform { get; set; }

        // グループに属するキャラクターのリスト
        public List<Character> Characters { get; set; }
        public Vector3 center;

    }
    public List<Group> groups;

    private IEnumerator kMeans()
    {
        yield return new WaitForSeconds(5f);
        var myTeamCharacters = GameObject
                       .FindGameObjectsWithTag("entity")
                       .Select(x => x.GetComponent<Character>());
        //   .Where(x => x != null && x.team == team)
        //   .ToList();
        groups = new List<Group>();

        foreach (var randomCharacter in myTeamCharacters
        .OrderBy(x => Random.value).Take(2))
        {

            // ランダムにキャラクターを選択
            print(randomCharacter);
            // 新しいグループを作成し、Transform を設定
            Group group = new Group
            {
                Transform = randomCharacter.transform,
                Characters = new List<Character> { randomCharacter },
                center = randomCharacter.transform.position
            };

            // グループをリストに追加
            groups.Add(group);
        }



        while (true)
        {
            foreach (var group in groups)
            {
                print(group);
                Graphics.DrawMesh(mesh, group.center, Quaternion.identity, material, 0);
                Debug.DrawRay(group.center, Vector3.up * 5f, new Color(Random.value, Random.value, Random.value));
            }

            yield return null;
        }


    }

}
