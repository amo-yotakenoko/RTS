using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Reflection;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
    public GameObject commandMenuprefab;
    // Update is called once per frame
    void Update()
    {
        // selecting();
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hit))
            {
                GameObject hited = hit.collider.gameObject;

                Entity entity = hited.GetComponent<Entity>();
                if (Input.GetMouseButtonDown(0))
                {

                    if (entity != null && entity.team == this.team) entitySelect(entity);
                    if (entity != null && entity.team != this.team) enemySelect(entity);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (entity != null && entity.team == this.team) entityOption(entity);
                }


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
    void entitySelect(Entity entity)
    {


        if (!selectingEntity.Contains(entity))
        {
            if (!Input.GetKey(KeyCode.LeftShift)) selectingEntity.Clear();
            selectingEntity.Add(entity);
        }
        else
        {
            selectingEntity.Remove(entity);
        }


        //リフレクションでいろいろhttps://qiita.com/gushwell/items/91436bd1871586f6e663
        //メソッドを取得

    }
    commandMenu commandMenu;

    void entityOption(Entity entity)
    {

        // Destroy(commandMenu);
        if (commandMenu != null) commandMenu.destroy();

        if (!selectingEntity.Contains(entity))
        {
            if (!Input.GetKey(KeyCode.LeftShift)) selectingEntity.Clear();
            selectingEntity.Add(entity);
        }
        else
        {
            selectingEntity.Remove(entity);
        }

        commandMenu = Instantiate(commandMenuprefab, entity.transform.position + new Vector3(0, 20, 0), commandMenuprefab.transform.rotation).GetComponent<commandMenu>();

        // List<string> allMethodNames = new List<string>();
        // foreach (Entity e in selectingEntity)
        // {
        //     var methodNames = e.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
        //                                  .Where(x => x.Name.EndsWith("CMD"))
        //                                  .Where(x => x.GetParameters().Length == 0)
        //                                  .Select(x => x.Name);
        //     allMethodNames.AddRange(methodNames);
        // }
        // allMethodNames = allMethodNames.Distinct().ToList();

        List<(string name, int cost)> methods = new List<(string, int)>();

        foreach (Entity e in selectingEntity)
        {
            var methodInfos = e.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                         .Where(x => x.Name.EndsWith("CMD"))
                                         .Where(x => x.GetParameters().Length == 0);

            foreach (var methodInfo in methodInfos)
            {
                int cost = 0;
                var costAttribute = (CostAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(CostAttribute));
                if (costAttribute != null) cost = costAttribute.Cost;


                var existingMethod = methods.FirstOrDefault(m => m.name == methodInfo.Name);
                if (existingMethod.name != null)
                {
                    methods.Remove(existingMethod);
                    methods.Add((existingMethod.name, existingMethod.cost + cost));
                }
                else
                {
                    methods.Add((methodInfo.Name, cost));
                }
            }
        }

        foreach (var method in methods)
        {
            var button = commandMenu.add($"{method.name}", method.cost);
            button.onClick.AddListener(() =>
           {
               print($"実行{method.name}");
           });
            foreach (Entity e in selectingEntity)
            {


                UnityAction action = () => e.setTask(method.name, new object[] { });
                button.onClick.AddListener(action);

            }
        }
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
        commandMenu?.destroy();
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
