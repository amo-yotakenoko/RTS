using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;
using UnityEngine.Events;
public class commandMenu : MonoBehaviour
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
        // int i = 0;

        // var selecting = itemlis.OrderBy(x => (Camera.main.ScreenToWorldPoint(Input.mousePosition) - x.UIobject.transform.position).magnitude).FirstOrDefault();
        // var selecting = itemlis
        //     .Select(x => new { Item = x, Distance = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - x.UIobject.transform.position).magnitude })
        //     .Where(x => x.Distance < 5)
        //     .OrderBy(x => x.Distance)
        //     .Select(x => x.Item)
        //     .FirstOrDefault();
        // if (selecting == null && Input.GetMouseButtonDown(0)) Destroy(this.gameObject);
        // foreach (var item in itemlis)
        // {
        //     float angle = (float)i / (float)itemlis.Count * 2 * Mathf.PI;
        //     item.UIobject.transform.localPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 50;
        //     i++;
        //     // item.transform.eulerAngles = new Vector3(-90, 0, -angle * Mathf.Rad2Deg);

        //     // item.transform.localScale = item == selecting?.UIobject ? new Vector3(2, 2, 2) : new Vector3(1, 1, 1);

        // }
    }

    // アイテムを追加するメソッド
    public Button add(string cmdid, int cost = 0)
    {
        Item item = new Item(cmdid);

        item.UIobject = GameObject.Instantiate(itemPrefab, this.transform);
        item.UIobject.transform.Find("text").GetComponent<TextMeshProUGUI>().text = $"{item.CMDid}/{cost}";
        var button = item.UIobject.GetComponent<Button>();
        button.onClick.AddListener(destroy);

        itemlis.Add(item);
        setItemPosition();
        return button;

    }

    void setItemPosition()
    {
        int j = 0;
        foreach (var i in itemlis)
        {
            float angle = (float)j / (float)itemlis.Count * 2 * Mathf.PI;
            i.UIobject.transform.localPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 100;
            j++;

        }

    }
    public void destroy()
    {
        print("destroy");
        Destroy(this.gameObject);
    }
}
