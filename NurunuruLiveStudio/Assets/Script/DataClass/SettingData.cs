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
    //イベントに関するデータ
    private Dictionary<string, EventData> _eventDatas = new Dictionary<string, EventData>();

    //トリガーに関するデータ
    private List<TriggerData> _triggerDatas = new List<TriggerData>();

    //スパチャ金額に関するデータ
    private List<SpachaData> _spachaDatas = new List<SpachaData>();

    //以下アクセサ
    public Dictionary<string, EventData> EventDatas
    {
        get { return _eventDatas; }
        set { _eventDatas = value; }
    }

    public List<TriggerData> TriggerDatas
    {
        get { return _triggerDatas; }
        set { _triggerDatas = value; }
    }

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
        //JSONファイルからイベント情報をロードする 骨
        EventData event1 = DummyData.getDummyEvent1();
        _eventDatas.Add(event1.Name, event1);

        //イベント2 おにぎり
        EventData event2 = DummyData.getDummyEvent2();
        _eventDatas.Add(event2.Name, event2);

        //イベント3 じょうず星人
        EventData event3 = DummyData.getDummyEvent3();
        _eventDatas.Add(event3.Name, event3);

        //イベント4 お茶
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

        //イベント11 草
        EventData event11 = DummyData.getDummyEvent11();
        _eventDatas.Add(event11.Name, event11);

        //イベント12 よっちゃん
        EventData event12 = DummyData.getDummyEvent12();
        _eventDatas.Add(event12.Name, event12);

        //イベント13 50円
        EventData event13 = DummyData.getDummyEvent13();
        _eventDatas.Add(event13.Name, event13);

        //イベント14 100円
        EventData event14 = DummyData.getDummyEvent14();
        _eventDatas.Add(event14.Name, event14);

        //イベント15 ぷいにゅー
        EventData event15 = DummyData.getDummyEvent15();
        _eventDatas.Add(event15.Name, event15);

        //イベント16 にゃんぱすー
        EventData event16 = DummyData.getDummyEvent16();
        _eventDatas.Add(event16.Name, event16);

        //イベント17 黒板消し
        EventData event17 = DummyData.getDummyEvent17();
        _eventDatas.Add(event17.Name, event17);

        //イベント18 たらい
        EventData event18 = DummyData.getDummyEvent18();
        _eventDatas.Add(event18.Name, event18);

        //イベント19 壺
        EventData event19 = DummyData.getDummyEvent19();
        _eventDatas.Add(event19.Name, event19);

        //イベント20 モンスターボール
        EventData event20 = DummyData.getDummyEvent20();
        _eventDatas.Add(event20.Name, event20);

        //イベント21 身代わり人形
        EventData event21 = DummyData.getDummyEvent21();
        _eventDatas.Add(event21.Name, event21);

        //イベント22 ペカコイン1
        EventData event22 = DummyData.getDummyEvent22();
        _eventDatas.Add(event22.Name, event22);

        //イベント23 ペカコイン100
        EventData event23 = DummyData.getDummyEvent23();
        _eventDatas.Add(event23.Name, event23);

        //イベント24 だんご
        EventData event24 = DummyData.getDummyEvent24();
        _eventDatas.Add(event24.Name, event24);

        //イベント25 ハンバーガー
        EventData event25 = DummyData.getDummyEvent25();
        _eventDatas.Add(event25.Name, event25);

        //イベント26 うな丼
        EventData event26 = DummyData.getDummyEvent26();
        _eventDatas.Add(event26.Name, event26);

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

    public void LoadMockTriggerData()
    {
        DummyData.LoadMockTriggerData(TriggerDatas);
    }

    /// <summary>
    /// 開始時に保存した各種設定を読み込む
    /// </summary>
    void Start()
    {
        Debug.Log("start");

        LoadMockEventData();
        LoadMockSpachaData();
        LoadMockTriggerData();
    }

}
