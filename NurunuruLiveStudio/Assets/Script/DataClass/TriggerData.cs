using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unage
{
    /// <summary>
    /// トリガー（イベント発動用の単語）に関するデータを保持するクラス
    /// </summary>
    public class TriggerData
    {

        /// <summary>
        /// イベントタイプ<br/>
        /// 金額、単語
        /// 
        /// </summary>
        public enum EVENT_TYPE
        {
            Amount
            ,Word
        }

        /// <summary>
        /// 検索タイプ<br/>
        /// 部分一致、前方一致、後方一致、完全一致
        /// 
        /// </summary>
        public enum FINDT_YPE
        {
            Partial
            ,Prefix
            ,Sufix
            ,Perfect
        }

        // ID
        private int id;

        // タイプ
        private EVENT_TYPE eventType;

        //トリガー金額
        private decimal amount;

        //トリガー単語
        [SerializeField]
        private string word;

        //検索タイプ
        [SerializeField]
        private FINDT_YPE findtype;

        //イベントリスト
        [SerializeField]
        private List<EventData> eventList;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public EVENT_TYPE EventType
        {
            get { return eventType; }
            set { eventType = value; }
        }

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public string Word
        {
            get { return word; }
            set { word = value; }
        }

        public FINDT_YPE Findtype
        {
            get { return findtype; }
            set { findtype = value; }
        }

        public List<EventData> EventList
        {
            get { return eventList; }
            set { eventList = value; }
        }


    }
}