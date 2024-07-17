using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//チームごとのお金とかのパラメーター、マルチプレイとかを見越して(作らないけど)スコアとか称号とかはココに入れて欲しい
public class teamParameter : MonoBehaviour
{
    //static変数にコンポーネントを入れてどこからでも参照しやすいようにしてる
    //取得するときはteamParameter.getteamParamater(team).moneyみたいにしてください
    static private Dictionary<int, teamParameter> parameter = new Dictionary<int, teamParameter>();
    //アクセス処理の関係と例外処理を後で追加する為にメソッドにしてます

    static public teamParameter getteamParameter(int team)
    {
        return parameter[team];
    }

    public int team;
    //建設にかかるお金をconstractionが呼ばれるごとにしたのでmoneyは少数値をとらせます、表示するときは整数値に直す予定
    public float money;



    void Start()
    {
        parameter[team] = this;
    }
}
