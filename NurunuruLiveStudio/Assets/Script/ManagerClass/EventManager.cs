using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unage
{
    /// <summary>
    /// イベント呼び出しを扱う
    /// オブジェクト生成などUnityのAPIはメインスレッドからの呼び出ししかできない
    /// （コメントデータ受信時のコールバックからは呼び出せない）
    /// ため、イベント名をキューへ追加し、Updateメソッド内で処理を行うようにしている。
    /// 
    /// 参考
    /// Unityな日々（Unity Geek）
    /// https://unitygeek.hatenablog.com/entry/2015/08/26/184239
    /// 
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _SettingData;
        private SettingData data;

        private Queue<string> eventQue = new Queue<string>();

        private void Start()
        {
            data = _SettingData.GetComponent<SettingData>();
        }

        private void Update()
        {
            if(eventQue.Count == 0)
            {
                return;
            }

            string eventName = eventQue.Dequeue();

            InvokeEvent(eventName);

        }

        /// <summary>
        /// イベント予約追加
        /// </summary>
        /// <param name="eventName"></param>
        public void enqueEvent(string eventName)
        {
            eventQue.Enqueue(eventName);
        }

        public void InvokeEvent(string eventName)
        {
            //イベント名からイベントを取得できなかった場合終了
            EventData ev;
            if (!data.EventDatas.TryGetValue(eventName,out ev))
            {
                return;
            }

            //設定されたアクションを実行する
            foreach(ActionData act in ev.Actions)
            {
                switch(act.Type)
                {
                    case ActionData.ActionType.Sound:
                        PlaySound(act);
                        break;

                    case ActionData.ActionType.Prefeb:
                        CreateObject(act);
                        break;

                }
            }
            
        }

        private void PlaySound(ActionData data)
        {

        }

        /// <summary>
        /// オブジェクトを生成する
        /// 参考
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void CreateObject(ActionData data)
        {
            string prefName = data.Name.ToString();

            //プレハブ名を取得
            //プレハブを生成

            GameObject prefab = (GameObject)Resources.Load("Prefabs/" + prefName);
            GameObject obj = Instantiate(prefab);
            //パラメータをセット
            PrefabBase cnt = obj.GetComponent<PrefabBase>();
            cnt.SetParameters(data.Parameters);

            //表示フラグをONにする
            obj.SetActive(true);

        }

    }
}