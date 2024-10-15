using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stageSet : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject character;

    void Start()
    {
        StartCoroutine(setCharacter(1, new Vector3(-50, 0, -50)));
        StartCoroutine(setCharacter(2, new Vector3(50, 0, -50)));
        StartCoroutine(setCharacter(3, new Vector3(-50, 0, 50)));
        StartCoroutine(setCharacter(4, new Vector3(50, 0, 50)));
    }

    IEnumerator setCharacter(int team, Vector3 position)
    {
        for (int i = 0; i < 100; i++)
        {
            var instance = Instantiate(character);

            instance.transform.position = position;
            instance.transform.position += new Vector3(
                Random.Range(-5.0f, 5.0f),
                0,
                Random.Range(-5.0f, 5.0f)
            );

            instance.GetComponent<Entity>().team = team;

            yield return null;
        }
    }

    // Update is called once per frame
    void Update() { }
}
