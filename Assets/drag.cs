using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class drag : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;

    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(
            new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
        transform.position = cursorPosition;
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
