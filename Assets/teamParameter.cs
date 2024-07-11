using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teamParameter : MonoBehaviour
{
    static private Dictionary<int, teamParameter> parameter = new Dictionary<int, teamParameter>();
    static public teamParameter getteamParameter(int team)
    {
        return parameter[team];
    }

    public int team;
    //建設にかかるお金をconstractionが呼ばれるごとにしたのでmoneyは少数値をとらせます、表示するときは整数値にしよう
    public float money;



    void Start()
    {
        parameter[team] = this;
    }
}
