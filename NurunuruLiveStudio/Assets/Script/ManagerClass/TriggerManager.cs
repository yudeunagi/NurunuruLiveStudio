using System;
using System.Collections;
using System.Collections.Generic;
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
        /// コメント本文を受け取って、設定された条件に該当する場合イベントを予約する
        /// </summary>
        /// <param name="message">コメント本文</param>
        public void Trigger(string message)
        {
            //単語検索
            string eventName = FindWord(message);

            //イベント名を取得できた場合呼び出しを行う
            if (!string.IsNullOrEmpty(eventName))
            {
                try
                {
                    //イベント予約
                    _event.enqueEvent(eventName);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }

        /// <summary>
        /// コメント本文が設定された条件に一致するか順次チェックを行い。
        /// 一致する場合イベント名を返却する。
        /// </summary>
        /// <param name="message">コメント本文</param>
        /// <returns>イベント名</returns>
        private string FindWord(string message)
        {
            //設定されたトリガーのリストから順次チェックを行う
            foreach (TriggerData trg in data.TriggerDatas)
            {
                switch (trg.Findtype)
                {
                    //部分一致
                    case TriggerData.FINDTYPE.Partial:
                        if (findPartial(message, trg.Word))
                        {
                            return trg.EventName;
                        }
                        break;
                    //前方一致
                    case TriggerData.FINDTYPE.Prefix:
                        if (findPrefix(message, trg.Word))
                        {
                            return trg.EventName;
                        }
                        break;
                    //後方一致
                    case TriggerData.FINDTYPE.Sufix:

                        break;
                    //完全一致
                    case TriggerData.FINDTYPE.Perfect:

                        break;
                }
            }

            // 一致するものがない場合空文字を返却
            return "";
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


        // Update is called once per frame
        void Update()
        {

        }
    }
}