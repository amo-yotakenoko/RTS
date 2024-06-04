using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using System.Linq;
using System.Reflection;

public class Entity : MonoBehaviour
{
    public static readonly Color[] teamColors = new Color[]
   {
        Color.blue,
        Color.green,
        Color.red,
        Color.yellow,
        Color.magenta,
        Color.cyan,
        new Color(1.0f, 0.5f, 0.0f),  // オレンジ
        new Color(0.5f, 0.0f, 0.5f),  // パープル
        new Color(0.0f, 0.5f, 0.5f),  // ティール
        new Color(0.5f, 0.5f, 0.0f)   // オリーブ
   };
    // Start is called before the first frame update
    // public List<IEnumerator> Tasks;
    public int team;//0:中立,それ以外がそれぞれのチーム
    public int maxHp;
    public int hp;

    public List<IEnumerator> Tasks;
    protected virtual void Start()
    {
        Tasks = new List<IEnumerator>();
        StartCoroutine(taskExecute());
    }

    public virtual void damage(int damage)
    {
        hp -= damage;
        print("damageを受けた");
        if (hp <= 0)
        {
            killed();
        }
    }
    //オーバーライトして消滅せず壊れるだけにしてもいい
    protected virtual void killed()
    {
        Destroy(this.gameObject);
    }

    public bool setTask(string cmd, object[] arguments)
    {
        MethodInfo existTask = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                 .Where(x => x.Name == cmd).FirstOrDefault();

        if (existTask == null) return false;
        Tasks.Add((IEnumerator)existTask.Invoke(this, arguments));

        return true;
    }


    // Update is called once per frame

    protected virtual void Update()
    {
    }


    IEnumerator taskExecute()
    {
        while (true)
        {
            if (Tasks.Count > 0)
            {

                var task = Tasks[0];
                yield return task;
                Tasks.Remove(task);
            }
            yield return null;
        }
    }

    public IEnumerator Task1CMD()
    {
        Debug.Log("task1");
        yield return new WaitForSeconds(2);
        Debug.Log("task1end");
    }


    public IEnumerator Task2CMD()
    {
        Debug.Log("task2");
        yield return new WaitForSeconds(2);

    }


}
