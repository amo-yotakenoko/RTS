using UnityEngine;

//建物の名前と値段を登録する
//参考:https://www.urablog.xyz/entry/2017/03/28/235739
[System.Serializable]
public class StructureData
{
    public string name;
    public GameObject prefab;
    public int cost;

    string ToString()
    {
        return $"{name}";
    }
}

[CreateAssetMenu(fileName = "StructureDatas", menuName = "Entitys/StructureDatas", order = 1)]
public class StructureDatas : ScriptableObject
{
    public StructureData[] structures;
}
