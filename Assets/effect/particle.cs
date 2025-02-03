using UnityEngine;

public class particle : MonoBehaviour
{


    public GameObject attackEffectPrefab;
    public static GameObject attackEffectPrefabStatic;
    public void Start()
    {
        attackEffectPrefabStatic = attackEffectPrefab;
        destroyEffectPrefabStatic = destroyEffectPrefab;
    }


    public GameObject destroyEffectPrefab;
    public static GameObject destroyEffectPrefabStatic;




    // エフェクトを再生するstatic関数
    public static void PlayAttackEffect(Vector3 position)
    {
        if (attackEffectPrefabStatic == null)
            return;


        GameObject effect = Instantiate(attackEffectPrefabStatic, position + new Vector3(0, 1, 0), Quaternion.identity);
        Destroy(effect, 2f);
    }



    public static void PlayDestroyEffect(Vector3 position)
    {
        if (destroyEffectPrefabStatic == null)
            return;


        GameObject effect = Instantiate(destroyEffectPrefabStatic, position + new Vector3(0, 3, 0), Quaternion.identity);
        Destroy(effect, 2f);
    }
}
