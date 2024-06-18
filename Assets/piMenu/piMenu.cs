using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;
using UnityEngine.Events;
public class piMenu : MonoBehaviour
{
    // Item クラスを定義
    public class Item
    {
        public string CMDid;
        public GameObject UIobject;

        // Item クラスのコンストラクタ
        public Item(string _cmdid)
        {
            CMDid = _cmdid;
            executeEvent = new UnityEvent();
        }
        public UnityEvent executeEvent;
    }

    public GameObject itemPrefab;
    public List<Item> itemlis = new List<Item>();

    // Start は最初のフレーム更新前に呼び出されます
    void Start()
    {
        // add("1");
        // add("2");
        // add("3");
        // add("4");
    }

    void Update()
    {
        int i = 0;

        var selecting = itemlis.OrderBy(x => (Camera.main.ScreenToWorldPoint(Input.mousePosition) - x.UIobject.transform.position).magnitude).FirstOrDefault();
        foreach (var item in itemlis.Select(x => x.UIobject))
        {
            float angle = (float)i / (float)itemlis.Count * 2 * Mathf.PI;
            item.transform.localPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 50;
            i++;
            item.transform.eulerAngles = new Vector3(-90, 0, -angle * Mathf.Rad2Deg);

            item.transform.localScale = item == selecting.UIobject ? new Vector3(2, 2, 2) : new Vector3(1, 1, 1);
            if (item == selecting.UIobject && Input.GetMouseButtonDown(0))
            {
                selecting.executeEvent.Invoke();
            }
        }
    }

    // アイテムを追加するメソッド
    public void add(UnityAction action, string cmdid)
    {
        Item item = new Item(cmdid);
        itemlis.Add(item);
        item.UIobject = GameObject.Instantiate(itemPrefab, this.transform);
        item.UIobject.GetComponent<TextMeshProUGUI>().text = item.CMDid;
        item.executeEvent.AddListener(action);

    }
}
