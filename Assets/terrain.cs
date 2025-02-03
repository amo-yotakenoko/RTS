using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrain : MonoBehaviour
{
    void Update()
    {
        // makeGround();
    }

    void Start()
    {
        StartCoroutine(makeGround());
    }

    public float scale;
    public float freq;
    public float seed = 0;
    public GameObject treePrefab;
    public float bias;

    IEnumerator makeGround()
    {
        yield return new WaitForSeconds(3);
        TerrainData genTerrain = GetComponent<Terrain>().terrainData;
        var heights = new float[genTerrain.heightmapResolution, genTerrain.heightmapResolution];

        // テレイン平面をパーリンノイズによって隆起させる。
        for (int x = -75; x < 75; x += 3)
        {
            for (int y = -75; y < 75; y += 3)
            {

                if (Vector3.Distance(new Vector3(x, 0, y), new Vector3(-40, 0, -40)) <= 15 ||
                    Vector3.Distance(new Vector3(x, 0, y), new Vector3(40, 0, -40)) <= 15 ||
                    Vector3.Distance(new Vector3(x, 0, y), new Vector3(-40, 0, 40)) <= 15 ||
                    Vector3.Distance(new Vector3(x, 0, y), new Vector3(40, 0, 40)) <= 15)
                    // Instantiate(treePrefab, new Vector3(x, 0, y), transform.rotation);
                    continue;

                // Terrainの高さをセット
                // heights[x, y] = perlinNoiseHeight(x, y);
                // heights[x, y] = 0;
                float noise = perlinNoiseHeight(x, y);
                if (Mathf.Abs(x) + Mathf.Abs(y) < 100 && 0 < noise)
                {
                    Instantiate(treePrefab, new Vector3(x, 0, y), transform.rotation);
                }
            }
            yield return null;
        }
    }

    private float perlinNoiseHeight(int x, int y)
    {
        // パーリンノイズから高さのベースを算出
        float height = Mathf.PerlinNoise(x * freq + seed, y * freq + (seed / 2)) - bias;

        // パーリンノイズを加工 0.35以下をすべて0.35にする。
        // if (height < 0f)
        //     height = 0f;

        return height * scale;
    }
}
