using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class hashSearch : MonoBehaviour
{
    // Start is called before the first frame update
    const int bucketSize = 50;
    public static new List<Transform>[,] hashTable;

    void Start()
    {
        StartCoroutine(tableSetTimer());
    }

    IEnumerator tableSetTimer()
    {
        while (true)
        {
            tableSet();
            yield return new WaitForSeconds(1);
        }
    }

    public float gridSize;

    void tableSet()
    {
        hashTable = new List<Transform>[bucketSize, bucketSize];

        // 各要素に新しい List<Transform> を割り当てる
        for (int x = 0; x < bucketSize; x++)
        {
            for (int y = 0; y < bucketSize; y++)
            {
                hashTable[x, y] = new List<Transform>();
            }
        }

        foreach (var entity in GameObject.FindGameObjectsWithTag("entity"))
        {
            var pos = entity.transform.position;
            var hashpos = getHash(pos);
            // if (hashpos == (3, 2))
            // {

            //     Debug.DrawLine(pos, pos + new Vector3(0, 5, 0), Color.red, 1f);
            // }
            hashTable[hashpos.x, hashpos.y].Add(entity.transform);
        }
    }

    (int x, int y) getHash(Vector3 pos)
    {
        return (
            remainder((int)Mathf.Round(pos.x / gridSize)),
            remainder((int)Mathf.Round(pos.z / gridSize))
        );
    }

    int remainder(int x)
    {
        return (x % bucketSize + bucketSize) % bucketSize;
    }

    // Update is called once per frame
    void Update()
    {
        // tableSet();
        // foreach (var item in searchEntity(search, r))
        // {
        //     Debug.DrawLine(
        //         search + new Vector3(0, 1, 0),
        //         item.transform.position + new Vector3(0, 1, 0),
        //         Color.red,
        //         1f
        //     );
        // }
    }

    public Vector3 search;
    public float r;

    public IEnumerable<Transform> searchEntity(Vector3 pos, float r)
    {
        Vector3 offset = new Vector3(0, 0, 0);

        int range = (int)Mathf.Round(r / gridSize);
        // print(range);
        for (offset.x = -range; offset.x <= range; offset.x += 1)
        {
            for (offset.z = -range; offset.z <= range; offset.z += 1)
            {
                var hashpos = getHash(pos + offset * gridSize);
                // print($"{hashpos.x},{hashpos.y}");
                foreach (var candidate in hashTable[hashpos.x, hashpos.y])
                {
                    if (candidate == null)
                        continue;
                    yield return candidate;
                    //     Debug.DrawLine(
                    //         search + new Vector3(0, 1, 0),
                    //         item.transform.position + new Vector3(0, 1, 0),
                    //         Color.red,
                    //         1f
                    //     );
                    // if ((pos - candidate.transform.position).magnitude < r)
                    // {


                    // Debug.DrawLine(
                    //     pos + new Vector3(0, 0, 0),
                    //     candidate.transform.position + new Vector3(0, 0, 0),
                    //     Color.white,
                    //     0.01f
                    // );


                    // }
                    // else
                    // {
                    //     Debug.DrawLine(
                    //         candidate.transform.position + new Vector3(-1f, 1, -1f),
                    //         candidate.transform.position + new Vector3(1, 1, 1),
                    //         Color.black,
                    //         0.1f
                    //     );
                    //     Debug.DrawLine(
                    //         candidate.transform.position + new Vector3(1, 1, -1),
                    //         candidate.transform.position + new Vector3(-1, 1, 1),
                    //         Color.black,
                    //         0.1f
                    //     );
                    // }
                }
            }
        }
    }

    void searchEntityall(Vector3 pos)
    {
        foreach (var candidate in GameObject.FindGameObjectsWithTag("entity"))
        {
            if ((pos - candidate.transform.position).magnitude < 10)
            {
                Debug.DrawLine(
                    pos + new Vector3(0, 1, 0),
                    candidate.transform.position + new Vector3(0, 1, 0),
                    Color.red,
                    1f
                );
            }
            else
            {
                Debug.DrawLine(
                    candidate.transform.position + new Vector3(-1f, 1, -1f),
                    candidate.transform.position + new Vector3(1, 1, 1),
                    Color.black,
                    0.1f
                );
                Debug.DrawLine(
                    candidate.transform.position + new Vector3(1, 1, -1),
                    candidate.transform.position + new Vector3(-1, 1, 1),
                    Color.black,
                    0.1f
                );
            }
        }
    }
}
