using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WallRefarence : Structure
{
    // Start is called before the first frame update
    public Transform wallEnd;
    public Mesh wallMesh;
    public Material wallMaterial;
    protected override void Start()
    {

        base.Start();
        if (status != Status.LocationChoseing) Destroy(wallEnd.gameObject);
        wallEnd.SetParent(null);
    }

    protected override void Update()
    {
        if (status == Status.LocationChoseing)
        {
            // GetComponent<MeshRenderer>().enabled = false;
            transform.LookAt(wallEnd.transform.position);
            Vector3 pos = this.transform.position;
            float pitch = 1.5f;
            for (float i = 0; i < (this.transform.position - wallEnd.transform.position).magnitude; i += pitch)
            {
                pos += this.transform.forward * pitch;
                Matrix4x4 matrix = Matrix4x4.TRS(pos, this.transform.rotation, this.transform.lossyScale);

                // Draw the mesh using the transformation matrix
                Graphics.DrawMesh(wallMesh, matrix, wallMaterial, 0);
            }
        }
        base.Update();

    }
    public override void ok()
    {
        craeteWall();
        // GetComponent<MeshRenderer>().enabled = true;
        // Destroy(wallEnd.gameObject);
        // base.ok();
        Destroy(this.gameObject);
    }
    public GameObject wallPrefab;
    void craeteWall()
    {
        Vector3 pos = this.transform.position;
        float pitch = 1.5f;
        for (float i = 0; i < (this.transform.position - wallEnd.transform.position).magnitude; i += pitch)
        {
            pos += this.transform.forward * pitch;
            var wall = Instantiate(wallPrefab, pos, this.transform.rotation);
            wall.GetComponent<Structure>().team = this.team;
            wall.GetComponent<Structure>().ok();
            // wall.GetComponent<Structure>().callBuilder();
            // Draw the mesh using the transformation matrix
            // Graphics.DrawMesh(wallMesh, matrix, wallMaterial, 0);
        }
    }


    void OnDestroy()
    {
        if (wallEnd != null) Destroy(wallEnd.gameObject);
    }
}
