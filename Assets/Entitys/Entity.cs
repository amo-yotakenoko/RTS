using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

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
        new Color(1.0f, 0.5f, 0.0f), // オレンジ
        new Color(0.5f, 0.0f, 0.5f), // パープル
        new Color(0.0f, 0.5f, 0.5f), // ティール
        new Color(0.5f, 0.5f, 0.0f) // オリーブ
    };

    // Start is called before the first frame update
    // public List<IEnumerator> Tasks;
    public int team; //0:中立,それ以外がそれぞれのチーム
    public int maxHp;
    public int hp;

    public class Task
    {
        public IEnumerator task;
        public int priority;

        public string ToString()
        {
            return $"{task}:{priority}";
        }
    }

    //ここに実行させたいTaskを追加する
    public List<Task> Tasks;

    protected virtual void Start()
    {
        Tasks = new List<Task>();
        StartCoroutine(taskExecute());
    }

    public virtual void damage(int damage, int attackedTeam = 0)
    {
        hp -= damage;
        // print("damageを受けた");
        if (hp <= 0)
        {
            killed(attackedTeam);
        }
    }

    //オーバーライトして消滅せず壊れるだけにしてもいい
    protected virtual void killed(int attackedTeam = 0)
    {
        Destroy(this.gameObject);
        // team = attackedTeam;
        // hp = maxHp;
    }

    public bool setTask(string cmd, object[] arguments, int priority = 0)
    {
        MethodInfo existTask = this.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            .Where(x => x.Name == cmd)
            .FirstOrDefault();

        if (existTask == null)
            return false;
        Task task = new Task();
        task.task = (IEnumerator)existTask.Invoke(this, arguments);
        task.priority = priority;
        Tasks.Add(task);

        return true;
    }

    // Update is called once per frame

    protected virtual void Update() { }

    //タスクを下から順番に実行する処理、TODO:優先度的なシステムを付けたい
    IEnumerator taskExecute()
    {
        while (true)
        {
            if (Tasks.Count > 0)
            {
                Tasks.OrderBy(x => x.priority);
                var task = Tasks[0];
                // Coroutine cr = StartCoroutine(task.task);
                while (true)
                {
                    // yield return null;
                    yield return task.task;
                    print("コルーチン" + task.task.Current);
                }
                Tasks.Remove(task);
            }
            yield return null;
        }
    }

    //テスト用Task
    public IEnumerator Task1CMD()
    {
        Debug.Log("task1");
        yield return new WaitForSeconds(2);
        Debug.Log("task1end");
    }

    //テスト用Task
    public IEnumerator Task2CMD()
    {
        Debug.Log("task2");
        yield return new WaitForSeconds(2);
    }

    //オブジェクトの削除
    public IEnumerator DestroyCMD()
    {
        Destroy(this.gameObject);
        yield return null;
    }
}

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
sealed class CostAttribute : Attribute
{
    public int Cost { get; }

    public CostAttribute(int cost)
    {
        Cost = cost;
    }
}
