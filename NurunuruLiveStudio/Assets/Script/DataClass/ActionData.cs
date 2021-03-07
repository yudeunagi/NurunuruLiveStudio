using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unage
{
    public class ActionData
    {
        /**
         * アクションの種類の定義
         * Sound:効果音
         * Prefeb:オブジェクト（主に画像）表示
         */
         //memo:この定義って専用のクラスに持たせた方がよくない？
        public enum ActionType
        {
            Sound
            ,Prefeb
        }


        /// <summary>
        /// プレハブの種類の定義
        /// </summary>
        /// これも専用クラスに持たせるべきじゃ？
        public enum PrefabType
        {
            FallSprite
            , Prefeb
        }

        //アクション番号
        [SerializeField]
        private int id;

        //アクションの種類
        [SerializeField]
        private ActionType type;

        //画像、効果音のパス
        private string path;

        //プレハブ名
        private PrefabType name;

        //パラメータ
        private List<string> parameters = new List<string>();


        //以下アクセサ
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public ActionType Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public PrefabType Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<string> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }
    }
}