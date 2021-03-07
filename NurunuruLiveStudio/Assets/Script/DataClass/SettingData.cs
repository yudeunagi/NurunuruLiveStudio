using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unage;
using static Unage.ActionData;

/// <summary>
/// 各種設定を扱う
/// </summary>
public class SettingData : MonoBehaviour
{
    //トリガーに関するデータ
//    private List<TriggerData> triggerDatas;

    //イベントに関するデータ
    private Dictionary<string,EventData> _eventDatas = new Dictionary<string, EventData>();

    //以下アクセサ
    public Dictionary<string, EventData> EventDatas
    {
        get { return _eventDatas; }
        set { _eventDatas = value; }
    }

    //スパチャ金額に関するデータ
    private List<SpachaData> _spachaDatas = new List<SpachaData>();

    //以下アクセサ
    public List<SpachaData> SpachaDatas
    {
        get { return _spachaDatas; }
        set { _spachaDatas = value; }
    }


    /// <summary>
    /// イベントデータのセーブ
    /// </summary>
    public void SaveEventData()
    {
        //eventDatasのデータをJSONファイルへ保存する
    }

    /// <summary>
    /// イベントデータのロード
    /// </summary>
    public void LoadEventData()
    {
        //JSONファイルからイベント情報を読み込みeventDatasへセットする
    }

    /// <summary>
    /// イベントデータのロード
    /// </summary>
    public void LoadMockEventData()
    {
        //JSONファイルからイベント情報をロードする
        EventData event1 = DummyData.getDummyEvent1();
        _eventDatas.Add(event1.Name, event1);

        //イベント2
        EventData event2 = DummyData.getDummyEvent2();
        _eventDatas.Add(event2.Name, event2);

        //イベント3
        EventData event3 = DummyData.getDummyEvent3();
        _eventDatas.Add(event3.Name, event3);

        //イベント4
        EventData event4 = DummyData.getDummyEvent4();
        _eventDatas.Add(event4.Name, event4);

        //イベント5 お茶x3
        EventData event5 = DummyData.getDummyEvent5();
        _eventDatas.Add(event5.Name, event5);

        //イベント6 花束
        EventData event6 = DummyData.getDummyEvent6();
        _eventDatas.Add(event6.Name, event6);

        //イベント7 チキン
        EventData event7 = DummyData.getDummyEvent7();
        _eventDatas.Add(event7.Name, event7);

        //イベント8 ピザ
        EventData event8 = DummyData.getDummyEvent8();
        _eventDatas.Add(event8.Name, event8);

        //イベント9 ケーキ
        EventData event9 = DummyData.getDummyEvent9();
        _eventDatas.Add(event9.Name, event9);

        //イベント10 おすし
        EventData event10 = DummyData.getDummyEvent10();
        _eventDatas.Add(event10.Name, event10);
        Debug.Log("mockset完了");

    }

    /// <summary>
    /// 
    /// 
    /// ソート参考：
    /// https://qiita.com/tetsu8/items/96b8b889c57eb55125d1
    /// 
    /// </summary>
    public void LoadMockSpachaData()
    {
        DummyData.LoadMockSpachaData(SpachaDatas);
    }

    /// <summary>
    /// 開始時に保存した各種設定を読み込む
    /// </summary>
    void Start()
    {
        Debug.Log("start");

        LoadMockEventData();
        LoadMockSpachaData();
    }

}
