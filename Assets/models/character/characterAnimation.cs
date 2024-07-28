using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterAnimation : MonoBehaviour
{
    // Start is called before the first frame update

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    public float spead;
    public float direction;
    Vector3 prevpos;
    void Update()
    {
        animator.SetFloat("Speed", (prevpos - transform.position).magnitude * Time.deltaTime * spead);
        print((prevpos - transform.position).magnitude * Time.deltaTime * spead);
        prevpos = transform.position;
        animator.SetFloat("Direction", direction);
    }
}
