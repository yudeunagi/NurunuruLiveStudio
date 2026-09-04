using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Unage
{
    /// <summary>
    /// コメント内の単語を判定し、
    /// それによってイベントを呼び出すクラス
    /// 
    /// </summary>
    public class TriggerManager : MonoBehaviour
    {
        // 設定情報
        [SerializeField]
        private GameObject _SettingData;
        private SettingData data;

        // イベント実行クラス
        private EventManager _event;

        // Start is called before the first frame update
        void Start()
        {
            _event = this.gameObject.GetComponent<EventManager>();
            data = _SettingData.GetComponent<SettingData>();
        }


        /// <summary>
        ///
        /// コメント本文を受け取って金額が含まれているか判定する
        /// 含まれている場合該当するトリガーデータを返す
        /// 
        /// </summary>
        /// <param name="message">コメント本文</param>
        /// <returns>該当するトリガーデータ。該当なしの場合はnull。</returns>
        public List<EventData> getAmount(string message)
        {
            // 金額抽出
            decimal amount = ExtractionMoney(message);
            Debug.Log($"金額={amount.ToString()}");

            // イベント名を取得して返す
            return getEventName(amount);;
        }

        /// <summary>
        /// コメントに含まれる金額を抽出する
        /// todo:同じような処理で＄の分もそのうち作る
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private decimal ExtractionMoney (string message)
        {
            // \ から始まる数値を抜き出す、小数点はいったん忘れる
            var reg = new Regex("[￥¥\\\\][0-9０-９,]+");
            Match m = reg.Match(message);

            string s = m.Value;
            s = CommonUtils.ZtoH(s);

            //数値のみ抜き出す。
            var num = new Regex("[^0-9]");
            s = num.Replace(s, "");

            //decimalに変換
            decimal result;

            //変換に成功した場合変換した値を返す
            if(decimal.TryParse(s, out result))
            {
                return result;
            }

            //変換失敗したら0を返す
            return new decimal(0);
        }

        /// <summary>
        /// 金額
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>該当するイベントリスト。該当なしの場合はnull。</returns>
        public List<EventData> getEventName(decimal amount)
        {
            foreach (TriggerData tri in data.TriggerDatas)
            {
                // トリガータイプが金額でない場合はスキップ
                if(tri.EventType != TriggerData.EVENT_TYPE.Amount)
                {
                    continue;
                }

                Debug.Log($"設定情報の金額={tri.Amount}");
                //入力された金額が設定情報の金額以上の場合、設定されたイベント名を返す
                if(decimal.Compare(amount, tri.Amount) >= 0)
                {
                    Debug.Log($"入力された金額={amount} は設定情報の金額={tri.Amount} 以上です。");
                    return tri.EventList;
                }
            }
            return new List<EventData>();
        }



        /// <summary>
        /// コメント本文を受け取って、設定された条件に該当するトリガーデータを返す
        /// </summary>
        /// <param name="message">コメント本文</param>
        /// <returns>該当するトリガーデータ。該当なしの場合はnull。</returns>
        public List<EventData> Trigger(string message)
        {
            //単語検索
            List<EventData> eventList = FindWord(message);

            return eventList;
        }


        /// <summary>
        /// コメント本文が設定された条件に一致するか順次チェックを行い。
        /// 一致する場合イベント名を返却する。
        /// </summary>
        /// <param name="message">コメント本文</param>
        /// <returns>トリガーデータ</returns>
        private List<EventData> FindWord(string message)
        {
            //設定されたトリガーのリストから順次チェックを行う
            foreach (TriggerData trg in data.TriggerDatas)
            {
                // トリガータイプがワードでない場合はスキップ
                if(trg.EventType != TriggerData.EVENT_TYPE.Word)
                {
                    continue;
                }

                // ワードが設定されていない場合はスキップ
                if(trg.Word == null || trg.Word == "")
                {
                    continue;
                }

                switch (trg.Findtype)
                {
                    //部分一致
                    case TriggerData.FINDT_YPE.Partial:
                        if (findPartial(message, trg.Word))
                        {
                            return trg.EventList;
                        }
                        break;
                    //前方一致
                    case TriggerData.FINDT_YPE.Prefix:
                        if (findPrefix(message, trg.Word))
                        {
                            return trg.EventList;
                        }
                        break;
                    //後方一致
                    case TriggerData.FINDT_YPE.Sufix:
                        if (findSufix(message, trg.Word))
                        {
                            return trg.EventList;
                        }

                        break;
                    //完全一致
                    case TriggerData.FINDT_YPE.Perfect:
                        if (findPerfect(message, trg.Word))
                        {
                            return trg.EventList;
                        }
                        break;
                }
            }

            // 一致するものがない場合nullを返却
            return new List<EventData>();
        }

        /// <summary>
        /// 部分一致検索
        /// </summary>
        /// <param name="message">検索対象となるメッセージ（コメント本文）</param>
        /// <param name="keyword">検索する語句（予め設定したキーワード）</param>
        /// <returns></returns>
        private bool findPartial(string message, string keyword)
        {
            if(message.Contains(keyword))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 前方一致検索
        /// </summary>
        /// <param name="message">検索対象となるメッセージ（コメント本文）</param>
        /// <param name="keyword">検索する語句（予め設定したキーワード）</param>
        /// <returns></returns>
        private bool findPrefix(string message, string keyword)
        {
            if (message.StartsWith(keyword))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 後方一致検索
        /// </summary>
        /// <param name="message">検索対象となるメッセージ（コメント本文）</param>
        /// <param name="keyword">検索する語句（予め設定したキーワード）</param>
        /// <returns></returns>
        private bool findSufix(string message, string keyword)
        {
            if (message.EndsWith(keyword))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 完全一致検索
        /// </summary>
        /// <param name="message">検索対象となるメッセージ（コメント本文）</param>
        /// <param name="keyword">検索する語句（予め設定したキーワード）</param>
        /// <returns></returns>
        private bool findPerfect(string message, string keyword)
        {
            if (message.Equals(keyword))
            {
                return true;
            }
            return false;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}