using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;
using System.ComponentModel;
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
        hpBar.fillRect.GetComponent<Image>().color = new Color[] { Color.blue, Color.green, Color.red }[entity.team];
    }

    // Update is called once per frame
    void Update()
    {
        Canvas.rotation = Quaternion.Euler(90, 0, 0);
        Tasktext();
        hpBar.maxValue = entity.maxHp;
        hpBar.value = entity.hp;
    }
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
