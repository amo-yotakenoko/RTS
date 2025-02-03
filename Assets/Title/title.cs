using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
public class title : MonoBehaviour
{
    void Update()
    {
        // "A" キーが押された場合にシーンを追加
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // "NewScene" という名前のシーンを追加で読み込む
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
            // SceneManager.LoadScene("SampleScene");

            StartCoroutine(FadeOutCoroutine());
        }
    }

    public RawImage rawImage;  // InspectorでRawImageを設定する
    public float fadeDuration = 2f;  // フェードアウトにかける時間

    public GameObject text;
    public GameObject cam;
    IEnumerator FadeOutCoroutine()
    {
        Scene newScene = SceneManager.GetSceneByName("SampleScene");
        SceneManager.MoveGameObjectToScene(this.gameObject, newScene);
        // Destroy(this.gameObject);
        Destroy(cam);
        SceneManager.UnloadSceneAsync("title");

        Destroy(text);
        float timeElapsed = 0f;

        // 初期の色（アルファ値が1の状態）
        Color initialColor = rawImage.color;

        while (timeElapsed < fadeDuration)
        {
            // 経過時間に応じてアルファ値を減少させる
            float alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeDuration);
            rawImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            // 経過時間を更新
            timeElapsed += Time.deltaTime;

            // 次のフレームまで待機
            yield return null;
        }

        // 完全に透明にする
        rawImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);




        // SceneManager.SetActiveScene(newScene);

        // // 現在のシーンに存在するすべてのゲームオブジェクトを取得
        // GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // // 各オブジェクトを新しいシーンに移動
        // foreach (var obj in allObjects)
        // {

        //     SceneManager.MoveGameObjectToScene(obj, newScene);

        // }
        // // string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("SampleScene");

    }
}
