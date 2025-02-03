using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
// using UnityEditor.Experimental.GraphView;
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
        public float priority;
        public Transform place;

        public float priorityCalculate(Vector3 pos)
        {
            if (place == null)
                return priority;
            float distance = 1.0f / (pos - place.position).magnitude;
            return priority + sigmoid(distance);
        }

        float sigmoid(float x)
        {
            return 1.0f / (1.0f + Mathf.Exp(-x));
        }

        public override string ToString()
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

    public virtual void damage(int damage, Entity attacked = null)
    {
        hp -= damage;
        // print("damageを受けた");
        if (hp <= 0)
        {
            killed(attacked);
        }
    }

    //オーバーライトして消滅せず壊れるだけにしてもいい
    protected virtual void killed(Entity attacked = null)
    {
        particle.PlayDestroyEffect(this.transform.position);
        Destroy(this.gameObject);
        // team = attackedTeam;
        // hp = maxHp;
    }

    public bool setTask(string cmd, object[] arguments, float priority = 0, Transform place = null)
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
        task.place = place;
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
                Tasks = Tasks.OrderBy(x => -x.priorityCalculate(transform.position)).ToList();
                var task = Tasks[0];
                // if (team == 1)
                // {
                //     print(task.task);
                // }
                // Coroutine cr = StartCoroutine(task.task);
                bool isRunning = task.task.MoveNext();
                // yield return null;
                // yield return task.task;
                // print("コルーチン" + task.task.Current);
                yield return task.task.Current;
                if (!isRunning)
                    Tasks.Remove(task);
            }
            yield return null;
        }
    }

    //テスト用Task
    public IEnumerator Task1CMD()
    {
        Debug.Log("task1");
        yield return new WaitForSeconds(10);
        Debug.Log("task1end");
    }

    //テスト用Task
    public IEnumerator Task2CMD()
    {
        for (int i = 0; i < 20; i++)
        {
            GetComponent<Renderer>().material.color = Color.black;
            yield return new WaitForSeconds(0.5f);
            GetComponent<Renderer>().material.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
    }

    //オブジェクトの削除
    public IEnumerator DestroyCMD()
    {
        Destroy(this.gameObject);
        particle.PlayDestroyEffect(this.transform.position);
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
