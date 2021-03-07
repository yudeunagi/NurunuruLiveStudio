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

        private EventManager _event;

        void Start()
        {
            _event = this.gameObject.GetComponent<EventManager>();
        }


        /// <summary>
        ///
        /// コメント本文を受け取って金額が含まれているか判定する
        /// 戻り値 0:なし ,1以降の数値:金額ランク、
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public PayedMoney Hogehoge(string message)
        {
            decimal homo = ExtractionMoney(message);
            Debug.Log(homo.ToString());

            try
            {
                _event.enqueEvent("drink1");
            }catch(Exception e)
            {
                Debug.Log(e);
            }



            return new PayedMoney();

        }


        private decimal ExtractionMoney (string message)
        {
            // \ から始まる数値を抜き出す、小数点はいったん忘れる
            var reg = new Regex("[￥\\\\][0-9０-９,]+");
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

    }
}