using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine.Events;
public class command : MonoBehaviour
{
    // Start is called before the first frame update
    public int team;//プレイヤーのチームid,
    void Start()
    {

    }
    public List<Entity> selectingEntity = new List<Entity>();
    public Mesh selectingMarkMesh;
    public Material selectingMarkMaterial;//TODO:ここシェーダグラフとか使っていい感じにしよう
    public GameObject piMenuprefab;
    // Update is called once per frame
    void Update()
    {
        // selecting();
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hited = hit.collider.gameObject;

                Entity entity = hited.GetComponent<Entity>();
                if (entity != null && entity.team == this.team) entitySelect(entity);
                if (entity != null && entity.team != this.team) enemySelect(entity);

                //TASK:ここの判定雑
                if (hited.name == "Terrain") groundSelect(hit.point);

            }

        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            selectingEntity.Clear();
        }
        showSelecting();
    }
    piMenu pimenu;
    void entitySelect(Entity entity)
    {

        Destroy(pimenu);
        if (!selectingEntity.Contains(entity))
        {
            if (!Input.GetKey(KeyCode.LeftShift)) selectingEntity.Clear();
            selectingEntity.Add(entity);
        }
        else
        {
            selectingEntity.Remove(entity);
        }

        foreach (Entity e in selectingEntity)
        {
            var existTasks = e.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
         .Where(x => x.Name.EndsWith("CMD"))
         .Where(x => x.GetParameters().Length == 0);
            pimenu = Instantiate(piMenuprefab, entity.transform.position + new Vector3(0, 20, 0), piMenuprefab.transform.rotation).GetComponent<piMenu>();
            foreach (var existTask in existTasks)
            {
                UnityAction action = () => e.setTask(existTask.Name, new object[] { });
                pimenu.add(action, $"{existTask.Name}");
            }

        }
        //リフレクションでいろいろhttps://qiita.com/gushwell/items/91436bd1871586f6e663
        //メソッドを取得

    }


    // public void TaskAdd(System.Reflection.MethodInfo task)
    // {
    //     print("taskadd");
    //     print(e.setTask(task.Name, new object[] { null }));
    // }



    void groundSelect(Vector3 targetPosition)
    {
        foreach (Entity e in selectingEntity)
        {

            // print(e.setTask("Task1CMD", new object[] { }));
            print(e.setTask("moveCMD", new object[] { targetPosition }));

            // e.targetpos = targetPosition;
        }
    }
    void enemySelect(Entity enemy)
    {
        foreach (Entity e in selectingEntity)
        {
            // print("moveToEntityCMD");
            print(e.setTask("AttackToEntityCMD", new object[] { enemy }));

        }
    }


    void showSelecting()
    {
        foreach (Entity e in selectingEntity)
        {
            var materix4x4 = Matrix4x4.TRS(e.transform.position, Quaternion.Euler(0f, Time.time * 10, 0f), new Vector3(1, 1, 1));
            Graphics.DrawMesh(selectingMarkMesh, materix4x4, selectingMarkMaterial, 0);
        }
    }
}
