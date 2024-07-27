using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//平面上をドラッグアンドドロップする奴、structureの位置指定用だけど他にも使えそうなので別コンポーネントに分けた
public class drag : MonoBehaviour
{
    private Plane dragPlane;
    private Vector3 offset;
    private Vector3 screenPoint;

    void Start()
    {
        // Define a plane at y=0, facing up (normal vector pointing up)
        dragPlane = new Plane(Vector3.up, Vector3.zero);
    }

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (dragPlane.Raycast(ray, out distance))
        {
            Vector3 point = ray.GetPoint(distance);
            point.y = 0; // Ensure the point is on the plane at y=0
            transform.position = point + offset;
        }
    }

    public int isOverlapping()
    {
        Collider collider = GetComponent<Collider>();

        Vector3 center = collider.bounds.center;
        Vector3 halfExtents = collider.bounds.extents;
        Collider[] hitColliders = Physics.OverlapBox(center, halfExtents, collider.transform.rotation).Where(col => col != collider).Where(col => col.gameObject.name != "Terrain").ToArray();
        // print(hitColliders.Count==0);
        return hitColliders.Length;
    }
}
