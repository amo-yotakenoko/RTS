using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stageSet : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject character;
    public GameObject builder;
    public GameObject baseStructure;

    void Start()
    {
        StartCoroutine(setCharacter(1, new Vector3(-1, 0, -1) * 40));
        StartCoroutine(setCharacter(2, new Vector3(1, 0, -1) * 40));
        StartCoroutine(setCharacter(3, new Vector3(-1, 0, 1) * 40));
        StartCoroutine(setCharacter(4, new Vector3(1, 0, 1) * 40));
    }

    IEnumerator setCharacter(int team, Vector3 position)
    {
        for (int i = 0; i < 100; i++)
        {
            var instance = Instantiate(i < 90 ? character : builder);

            instance.transform.position = position;
            instance.transform.position += new Vector3(
                Random.Range(-1.0f, 1.0f),
                0,
                Random.Range(-1.0f, 1.0f)
            );

            instance.GetComponent<Entity>().team = team;

            yield return null;
        }

        var baseInstance = Instantiate(baseStructure);
        baseInstance.GetComponent<Entity>().team = team;
        baseInstance.transform.position = position;
    }

    // Update is called once per frame
    void Update() { }
}
