using UnityEngine;

[System.Serializable]
public class StructureData
{
    public string name;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "StructureDatas", menuName = "Entitys/StructureDatas", order = 1)]
public class StructureDatas : ScriptableObject
{
    public StructureData[] structures;
}