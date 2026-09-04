using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unage
{

    /**
     * イベントに関するデータを保持するクラス
     * 
     * 
     */
    public class EventData
    {

        /// <summary>
        /// プレハブの種類の定義
        /// </summary>
        /// これも専用クラスに持たせるべきじゃ？
        public enum PREFAB_TYPE
        {
            FallSprite    //落下
            , MoveSprite  //移動
            , PopupSprite //ポップアップ
        }

        /// <summary>
        /// パラメータのキー定義
        /// </summary>
        public enum PARAMETER_KEY
        {
            LIFETIME //オブジェクトの生存時間
            , SIZE //オブジェクトのサイズ
            , POSITION_X //オブジェクトのX座標
            , POSITION_Y //オブジェクトのY座標
            , MOVEMENT_X //オブジェクトのX方向の移動速度
            , MOVEMENT_Y //オブジェクトのY方向の移動速度    

        }

        //イベントID
        private int id;

        //画像のパス
        [SerializeField]
        private string path;

        //プレハブ名
        [SerializeField]
        private PREFAB_TYPE prefabType;

        //パラメータ
        [SerializeField]
        private Dictionary<string, string> parameters = new Dictionary<string, string>();

        //以下アクセサ
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public PREFAB_TYPE PrefabType
        {
            get { return prefabType; }
            set { prefabType = value; }
        }

        public Dictionary<string, string> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

    }
}
