using System.Collections;
using System.Collections.Generic;
using Unage;
using UnityEngine;

public class FallSpriteController : MonoBehaviour, PrefabBase
{
    private string[] _param = new string[7];

    // 生存時間
    private float _lifeTime;
    

    /// <summary>
    /// パラメーターを設定する
    /// </summary>
    /// <param name="parameters">設定するパラメーターのマップ</param>
    public void SetParameters(Dictionary<string, string> parameters)
    {
        // パラメーターに格納された値を取得
        string lifetime = parameters[EventData.PARAMETER_KEY.LIFETIME.ToString()];
        string size = parameters[EventData.PARAMETER_KEY.SIZE.ToString()];
        // 座標は使用しない、なんとなく取得しているだけ
        string posX = parameters[EventData.PARAMETER_KEY.POSITION_X.ToString()];
        string posY = parameters[EventData.PARAMETER_KEY.POSITION_Y.ToString()];

        //タイマーセット
        SetLifeTime(lifetime);
        //サイズセット
        SetScale(size);
        //座標セット
        SetPosition(posX, posY);

    }

    /// <summary>
    /// 生存時間セット
    /// </summary>
    /// <param name="time"></param>
    private void SetLifeTime(string time)
    {
        // sizeが未入力 or Floatに変換できなかったらデフォルト値でリターン
        float ftime;
        if (!float.TryParse(time, out ftime))
        {
            _lifeTime = 5.0f;
            return;
        }

        _lifeTime = ftime;

    }

    /// <summary>
    /// サイズセット
    /// </summary>
    /// <param name="size"></param>
    private void SetScale(string size)
    {
        // sizeが未入力 or Floatに変換できなかったらデフォルト値でリターン
        float fsize;
        if (!float.TryParse(size, out fsize))
        {
            this.transform.localScale = new Vector3(1, 1, 1); ;
            return;
        }

        this.transform.localScale = new Vector3(fsize, fsize, fsize);
    }

    /// <summary>
    /// 
    /// 
    /// 
    /// カメラサイズと座標についての参考
    /// https://pengoya.net/unity/aspect/
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <remarks>
    /// 現在は引数を使用せず、ランダムなX座標と固定のY座標を設定する
    /// </remarks>
    private void SetPosition(string x, string y)
    {
        float fx, fy;

        //アス比を求める
        float aspect = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;

        //カメラサイズ（本来ならカメラコンポーネントのsizeをちゃんと取ってくるべきだけどめんどいので直で書く）
        // TODO: カメラコンポーネントから正しいサイズを取得するように修正する
        float camSize = 4.50f;

        // X位置をランダム設定
        fx = Random.Range(-1.0f, 1.0f);
        // Y位置は固定
        fy = 1.2f;

        //座標セット
        Vector3 pos = new Vector3(fx * camSize * aspect, fy * camSize);
        this.transform.position = pos;

    }

    void Start()
    {
        //座標表示
        //        Debug.Log("x;" + transform.position.x + ", y;" + transform.position.y);
        //設定した時間経過後にコライダーを破棄
        Destroy(GetComponent<CapsuleCollider2D>(), _lifeTime);
        //設定した時間経過 * 2 後にオブジェクトを破棄
        Destroy(gameObject, _lifeTime * 2.0f);

    }

    void Update()
    {
        //オブジェクト座標が0以下＆画面外に出たら削除
        /*
        if (transform.position.y < 0 && !GetComponent<Renderer>().isVisible)
        {
            Destroy(this.gameObject);
        }
        */

    }
}
