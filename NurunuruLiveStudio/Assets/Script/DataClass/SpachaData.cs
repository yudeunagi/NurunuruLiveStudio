using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unage
{
    /// <summary>
    /// スパチャ（ギフト）の金額とそれに対応するイベントのデータを保持するクラス
    /// 
    /// </summary>
    public class SpachaData
    {
        //金額
        private decimal amount;

        //イベント名
        private string eventName;

        //以下アクセサ
        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public string EventName
        {
            get { return eventName; }
            set { eventName = value; }
        }
    }

}