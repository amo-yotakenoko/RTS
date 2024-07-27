using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class teamParameterUI : MonoBehaviour
{
    // Start is called before the first frame update
    TextMeshProUGUI text;
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = teamParameter.getteamParamater(0).ToString();
    }
}
