using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Unage;
using System;

namespace Unage//←パッケージ的なもの
{
    /// <summary>
    /// コメント内の金額を判定し、
    /// それによってイベントを呼び出すクラス
    /// 
    /// </summary>
    public class SuperChatManager : MonoBehaviour
    {
        // 設定情報
        [SerializeField]
        private GameObject _SettingData;
        private SettingData data;

        // イベント実行クラス
        private EventManager _event;

        void Start()
        {
            _event = this.gameObject.GetComponent<EventManager>();
            data = _SettingData.GetComponent<SettingData>();
        }


        /// <summary>
        ///
        /// コメント本文を受け取って金額が含まれているか判定する
        /// 
        /// </summary>
        /// <param name="message">コメント本文</param>
        /// <returns>金額</returns>
        public PayedMoney getAmount(string message)
        {
            // 金額抽出
            decimal amount = ExtractionMoney(message);
            Debug.Log(amount.ToString());

            try
            {
                // memo:ここで呼び出すのではなく上の階層から呼び出す方が良いか？
                // 金額からイベント取得
                string evName = getEventName(amount);
                // イベント予約
                _event.enqueEvent(evName);

            }
            catch(Exception e)
            {
                Debug.Log(e);
            }

            //金額を返却
            PayedMoney result = new PayedMoney();
            result.AMOUNT = amount;

            return result;

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
        /// <returns></returns>
        public String getEventName(decimal amount)
        {
            foreach (SpachaData spa in data.SpachaDatas)
            {
                //入力された金額が設定情報の金額以上の場合、設定されたイベント名を返す
                if(decimal.Compare(amount, spa.Amount) >= 0)
                {
                    return spa.EventName;
                }
            }
            return "";
        }

    }
}