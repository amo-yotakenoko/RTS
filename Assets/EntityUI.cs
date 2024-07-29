using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Entityの右上に出てくるTaskのListとか、空腹状態とか作るんだったらここ
public class EntityUI : MonoBehaviour
{
    // Start is called before the first frame update
    Entity entity;
    public TextMeshProUGUI Tasks;
    public Transform Canvas;
    public Slider hpBar;

    void Start()
    {
        entity = GetComponent<Entity>();
        hpBar.fillRect.GetComponent<Image>().color = Entity.teamColors[entity.team];
    }

    // Update is called once per frame
    void Update()
    {
        Canvas.rotation = Quaternion.Euler(90, 0, 0);
        Tasktext();
        hpBar.maxValue = entity.maxHp;
        hpBar.value = entity.hp;
    }

    //TODO:ココもっといい感じにする
    void Tasktext()
    {
        string tasktext = "";
        if (entity.Tasks.Count > 0)
        {
            foreach (var task in entity.Tasks)
            {
                tasktext += task.ToString() + "\n";
            }
        }
        Tasks.text = tasktext;
    }
}
