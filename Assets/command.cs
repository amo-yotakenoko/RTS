using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

//プレイヤーがキャラクターに指示を出す奴、デカくなってきたのでわけてもいいかも
public class command : MonoBehaviour
{
    // Start is called before the first frame update
    public int team; //プレイヤーのチームid,

    void Start() { }

    public List<Entity> selectingEntity = new List<Entity>();
    public Mesh selectingMarkMesh;
    public Material selectingMarkMaterial; //TODO:ここシェーダグラフとか使っていい感じにしよう
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
                    if (entity != null && entity.team == this.team)
                        entitySelect(entity);
                    if (entity != null && entity.team != this.team)
                        enemySelect(entity);

                    if (hited.name == "Terrain")
                        StartCoroutine(groundSelect(hit.point));
                    ;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (entity != null && entity.team == this.team)
                        entityOption(entity);
                    if (hited.name == "Terrain")
                        groundOption(hit.point);
                }
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
            if (!Input.GetKey(KeyCode.LeftShift))
                selectingEntity.Clear();
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

    [SerializeField]
    private StructureDatas structureDatabase;

    //建築のUIが出る
    void groundOption(Vector3 position)
    {
        print("groundOption");
        if (commandMenu != null)
            commandMenu.destroy();
        commandMenu = Instantiate(commandMenuprefab, position + new Vector3(0, 1, 0), commandMenuprefab.transform.rotation)
            .GetComponent<commandMenu>();
        commandMenu.team = team;

        foreach (StructureData structureData in structureDatabase.structures)
        {
            var button = commandMenu.add(
                $" {structureData.name}",
                cost: structureData.cost,
                isMoneyConsumption: false
            );
            button.onClick.AddListener(() =>
            {
                print($" {structureData.name}を建築");
                var instantiatedStructure = Instantiate(
                        structureData.prefab,
                        position,
                        structureData.prefab.transform.rotation
                    )
                    .GetComponent<Structure>();
                instantiatedStructure.status = Structure.Status.LocationChoseing;
                instantiatedStructure.team = team;
                instantiatedStructure.constractionCost = structureData.cost;
            });
        }
    }

    void entityOption(Entity entity)
    {
        // Destroy(commandMenu);
        if (commandMenu != null)
            commandMenu.destroy();

        if (!selectingEntity.Contains(entity))
        {
            if (!Input.GetKey(KeyCode.LeftShift))
                selectingEntity.Clear();
            selectingEntity.Add(entity);
        }
        else
        {
            selectingEntity.Remove(entity);
        }

        commandMenu = Instantiate(
                commandMenuprefab,
                entity.transform.position + new Vector3(0, 20, 0),
                commandMenuprefab.transform.rotation
            )
            .GetComponent<commandMenu>();

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

        //Methodの名前とCostを計算
        List<(string name, int cost)> methods = new List<(string, int)>();
        foreach (Entity e in selectingEntity)
        {
            //EntityにあるTaskのメソッドを取得、whereでいろいろ絞り込み
            var methodInfos = e.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.Name.EndsWith("CMD"))
                .Where(x => x.GetParameters().Length == 0);

            foreach (var methodInfo in methodInfos)
            {
                int cost = 0;
                //コストの属性を取得
                var costAttribute = (CostAttribute)
                    Attribute.GetCustomAttribute(methodInfo, typeof(CostAttribute));
                if (costAttribute != null)
                    cost = costAttribute.Cost;

                var existingMethod = methods.FirstOrDefault(m => m.name == methodInfo.Name);
                // 複数Entityにも同時に送れるように重複をひとまとめにする
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

        //commandMenuをつくってそこにボタンを追加
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


    public Mesh selectRectMesh;

    //範囲選択
    IEnumerator groundSelect(Vector3 targetPosition)
    {
        List<Entity> addedSelectingEntity = new List<Entity>();

        Plane plane = new Plane(Vector3.up, 0);
        Vector3 endPosition;
        Vector3 size = new Vector3(0, 0, 0);
        bool selectingEntityClearedChack = false;

        Vector3 lastPosition = new Vector3(-100, -100, -100);
        while (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float enter;

            if (plane.Raycast(ray, out enter))
            {
                endPosition = ray.GetPoint(enter);
                if (
                    selectingEntityClearedChack == false
                    && lastPosition != new Vector3(-100, -100, -100)
                    && lastPosition != endPosition
                )
                {
                    if (!Input.GetKey(KeyCode.LeftShift))
                        selectingEntity.Clear();
                    selectingEntityClearedChack = true;
                }
                lastPosition = endPosition;
                size = endPosition - targetPosition;
                size.x = Mathf.Abs(size.x);
                size.y = Mathf.Abs(size.y) + 1;
                size.z = Mathf.Abs(size.z);
                print(size);
                Vector3 center = (endPosition + targetPosition) / 2;

                Matrix4x4 matrix4x4 = Matrix4x4.TRS(center, Quaternion.Euler(0f, 0f, 0f), size);
                Graphics.DrawMesh(selectRectMesh, matrix4x4, selectingMarkMaterial, 0);

                foreach (
                    var entity in GameObject
                        .FindGameObjectsWithTag("entity")
                        .Select(x => x.GetComponent<Entity>())
                        .Where(x => x != null && x.team == team)
                )
                {
                    if (isRectIn(center, size, entity.transform))
                    {
                        if (!selectingEntity.Contains(entity))
                        {
                            selectingEntity.Add(entity);
                            addedSelectingEntity.Add(entity);
                        }
                    }
                }
                List<Entity> removeSelectingEntity = new List<Entity>();
                foreach (var added in addedSelectingEntity)
                {
                    if (!isRectIn(center, size, added.transform))
                    {
                        removeSelectingEntity.Add(added);
                    }
                }
                foreach (var remove in removeSelectingEntity)
                {
                    selectingEntity.Remove(remove);
                    addedSelectingEntity.Remove(remove);
                }
            }

            yield return null;
        }
        print(size);
        if (size.magnitude < 1.01f)
        {
            print("移動");
            //選択してるEntityをそこに移動
            foreach (Entity e in selectingEntity)
            {
                // print(e.setTask("Task1CMD", new object[] { }));

                e.Tasks.RemoveAll(x => $"{x.task}".Contains("moveCMD"));
                print(e.setTask("moveCMD", new object[] { targetPosition }, priority: 50));
                // print($"名前{e.Tasks.Select(x => x.task).FirstOrDefault()} ");
                // e.targetpos = targetPosition;
            }
            commandMenu?.destroy();
        }
    }

    bool isRectIn(Vector3 center, Vector3 size, Transform entity)
    {
        if (
            (center.x - size.x + 1) < entity.transform.position.x
            && entity.transform.position.x < (center.x + size.x - 1)
        )
        {
            if (
                (center.z - size.z + 1) < entity.transform.position.z
                && entity.transform.position.z < (center.z + size.z - 1)
            )
            {
                return true;
            }
        }
        return false;
    }

    void enemySelect(Entity enemy)
    {
        //選択してるEntityがそこに攻撃
        foreach (Entity e in selectingEntity)
        {
            // print("moveToEntityCMD");
            print(e.setTask("AttackToEntityCMD", new object[] { enemy, 100 }));
        }
    }

    //選択されてる奴を強調
    void showSelecting()
    {
        selectingEntity.RemoveAll(e => e == null);
        foreach (Entity e in selectingEntity)
        {
            var materix4x4 = Matrix4x4.TRS(
                e.transform.position,
                Quaternion.Euler(0f, Time.time * 10, 0f),
                new Vector3(1, 1, 1)
            );
            Graphics.DrawMesh(selectingMarkMesh, materix4x4, selectingMarkMaterial, 0);
        }
    }
}
