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
        /// 検索タイプ<br/>
        /// 部分一致、前方一致、後方一致、完全一致
        /// 
        /// </summary>
        public enum FINDTYPE
        {
            Partial
            ,Prefix
            ,Sufix
            ,Perfect
        }

        //トリガー番号
        [SerializeField]
        private string word;

        //イベント名
        [SerializeField]
        private string eventName;

        //検索タイプ
        [SerializeField]
        private FINDTYPE findtype;

        //以下アクセサ
        public string Word
        {
            get { return word; }
            set { word = value; }
        }

        public string EventName
        {
            get { return eventName; }
            set { eventName = value; }
        }

        public FINDTYPE Findtype
        {
            get { return findtype; }
            set { findtype = value; }
        }

    }
}