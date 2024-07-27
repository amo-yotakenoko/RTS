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
        this.transform.position += new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        );
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
