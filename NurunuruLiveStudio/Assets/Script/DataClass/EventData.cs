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
        //イベント番号
        private int id;

        //イベント名
        private string name;

        //アクション
        private List<ActionData> actions = new List<ActionData>();

        //以下アクセサ
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<ActionData> Actions
        {
            get { return actions; }
            set { actions = value; }
        }

    }
}
