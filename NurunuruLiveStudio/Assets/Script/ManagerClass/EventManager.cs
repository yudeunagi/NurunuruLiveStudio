using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using UnityEditor;

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


        /// <summary>
        /// イベント予約キュー
        /// </summary>
        private Queue<List<EventData>> eventQueNeo = new Queue<List<EventData>>();

        /// <summary>
        /// 設定（イベント設定やトリガー設定など）に関するコンポーネントを取得する
        /// </summary>
        private void Start()
        {
            data = _SettingData.GetComponent<SettingData>();
        }

        /// <summary>
        /// イベント予約追加
        /// 指定されたイベントを予約キューへ追加する。
        /// このメソッドは外部のトリガークラスから呼び出して使う。
        /// </summary>
        /// <param name="eventName"></param>
        public void enqueEvent(List<EventData> eventList)
        {
            eventQueNeo.Enqueue(eventList);
        }

        /// <summary>
        /// Updateメソッド<br/>
        /// 予約キューにイベントがある場合イベントを実行する
        /// </summary>
        private void Update()
        {
            if (eventQueNeo.Count == 0)
            {
                return;
            }

            List<EventData> eventList = eventQueNeo.Dequeue();

            InvokeEvent(eventList);

        }

        /// <summary>
        /// イベント実行<br/>
        /// イベントデータリスト内に引数で渡されたイベント名が存在する場合、そのイベントを実行する。
        /// </summary>
        /// <param name="eventName"></param>
        private void InvokeEvent(List<EventData> eventList)
        {
            //イベントリスト内の各イベントについて処理を行う
            foreach(EventData ev in eventList)
            {
                CreateObject(ev);
                break;
            }
        }

        /// <summary>
        /// サウンドを鳴らす
        /// </summary>
        /// <param name="data"></param>
        private void PlaySound(ActionData data)
        {

        }

        /// <summary>
        /// オブジェクトを生成する
        /// 参考
        /// 
        /// 画像ファイルからスプライト作成の参考
        /// http://kainoshizuku.blog.fc2.com/blog-entry-51.html?sp
        /// 
        /// </summary>
        /// <param name="data"></param>
        private void CreateObject(EventData data)
        {
            //プレハブ名を取得
            string prefName = data.PrefabType.ToString();

            //プレハブを生成
            GameObject prefab = (GameObject)Resources.Load("Prefabs/" + prefName);
            GameObject obj = Instantiate(prefab);
            
            //パラメータをセット
            PrefabBase cnt = obj.GetComponent<PrefabBase>();
            cnt.SetParameters(data.Parameters);

            //画像をセット
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            Texture2D texture = ReadTexture(data.Path, 1000, 1000);
            Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            sr.sprite = sp;

            //表示フラグをONにする
            obj.SetActive(true);

        }

        /// <summary>
        /// テクスチャ読み込み
        /// 
        /// パスは以下の様な書式にする必要がある
        /// "F:/picture/prprLive/image/dousite.png"
        /// 
        /// ファイルダイアログ参考
        /// https://qiita.com/tomoi/items/ac0334dd990b6a558ceb
        /// 
        /// 外部ファイルからのスプライト作成参考
        /// http://anndoroido.blogspot.com/2016/03/unity.html
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private Texture2D ReadTexture(string path, int width ,int height)
        {
            //memo:パス指定が上手くいかなかった（先頭にunityのパスが付与されてしまう）ため書式が間違っていると思い、正しい書式を確認するためファイル読み込み処理を入れてみた。
            // var patht = EditorUtility.OpenFilePanel("Open png", "", "png");

            //memo: なんか File.ReadAllBytes(ppth) だと読み込みはするがテクスチャが表示されない
            // byte[] readBinary = File.ReadAllBytes(ppth);

            //memo: BinaryReaderだとうまくいく、よくわからん・・・
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            byte[] res = br.ReadBytes((int)br.BaseStream.Length);
            br.Close();

            //このwidthやheightの値に特に意味はないっぽい・・・？
            Texture2D texture = new Texture2D(width, height);
            texture.LoadImage(res);
            texture.filterMode = FilterMode.Point;

            return texture;

        }

    }
}