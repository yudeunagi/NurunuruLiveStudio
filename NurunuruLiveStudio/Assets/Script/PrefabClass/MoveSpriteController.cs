﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpriteController : MonoBehaviour, PrefabBase
{
    private string[] _param = new string[7];

    // 生存時間
    private float _lifeTime;

    //移動方向X
    private float moveX = 0;

    //移動方向Y
    private float moveY = 0;

    /// <summary>
    /// 0:ライフタイム（未設定の場合5）
    /// 1:サイズ（未設定の場合1）
    /// 2:座標X（未設定の場合ランダム）
    /// 3:座標Y（未設定の場合ランダム）
    /// 4:回転（未設定の場合ランダムか0）
    /// 5:移動速度X（未設定の場合0）
    /// 6:移動速度Y（未設定の場合0）
    /// 
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    public void SetParameters(List<string> parameters)
    {
        int i = 0;
        foreach (string param in parameters)
        {
            _param[i] = param;
            i++;
        }

        //タイマーセット
        SetLifeTime(_param[0]);
        //サイズセット
        SetScale(_param[1]);
        //座標セット
        SetPosition(_param[2], _param[3]);
        //回転角度セット

        //移動量セット
        SetMovement(_param[5], _param[6]);

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
    private void SetPosition(string x, string y)
    {
        float fx, fy;

        //アス比を求める
        float aspect = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;

        //カメラサイズ（本来ならカメラコンポーネントのsizeをちゃんと取ってくるべきだけどめんどいので直で書く）
        float camSize = 4.50f;

        //基準値
        float scale = 0.7f;

        // x が未入力ならランダム
        if (!float.TryParse(x, out fx))
        {
            // Screen.widthで画面のサイズを上限、下限値にする
            fx = Random.Range(scale * -1.0f, scale);
        }

        // y が未入力ならランダム
        if (!float.TryParse(y, out fy))
        {
            fy = Random.Range(scale * -1.0f, scale);
        }

        //座標セット
        Vector3 pos = new Vector3(fx * camSize * aspect, fy * camSize);
        this.transform.position = pos;

    }

    /// <summary>
    /// 移動量セット
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void SetMovement(string x, string y)
    {
        Debug.Log(x);
        Debug.Log(y);

        // xが未入力 or Floatに変換できなかったら0を設定
        float fx;
        if (float.TryParse(x, out fx))
        {
            moveX = fx;
        }
        // yが未入力 or Floatに変換できなかったら0を設定
        float fy;
        if (float.TryParse(y, out fy))
        {
            moveY = fy;
        }
    }

    void Start()
    {
        //設定した時間経過後にオブジェクトを破棄
        Destroy(gameObject, _lifeTime);
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.Translate(moveX, moveY, 0);
    }
}
