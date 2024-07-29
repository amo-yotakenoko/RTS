using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        Vector3 movepos = new Vector3(0, 0, 0);
        movepos +=
            transform.right
            * Input.GetAxis("Horizontal")
            / Vector3.Dot(transform.right, new Vector3(1, 0, 0));
        movepos +=
            transform.up
            * Input.GetAxis("Vertical")
            / Vector3.Dot(transform.up, new Vector3(0, 0, 1));

        // movepos /= movepos.magnitude;
        movepos.y = 0;
        if (movepos != new Vector3(0, 0, 0))
        {
            movepos *= Time.deltaTime * 30;
        }

        this.transform.position += movepos;

        drag();
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            var c = GetComponent<Camera>();

            c.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * 10;
            c.orthographicSize = Mathf.Clamp(c.orthographicSize, 5, 100);
        }
    }

    bool isDragging = false;
    Vector3 dragOrigin;

    void drag()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isDragging = true;
            dragOrigin = GetMouseWorldPosition();
        }

        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 currentMousePosition = GetMouseWorldPosition();
            transform.position += dragOrigin - currentMousePosition;
            dragOrigin = GetMouseWorldPosition();
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, 0);
        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}
