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
        EventData event1 = new EventData();
        event1.ID = 1;
        event1.Name = "drink1";
        List<string> param1 = new List<string>();
        param1.Add("5");
        param1.Add("1");
        param1.Add("");
        param1.Add("1.2");

        ActionData action1 = createDummyAction(1, ActionType.Prefeb, PrefabType.FallSprite, "F:/picture/素材/drink_energy_can.png", param1);
        event1.Actions.Add(action1);

        _eventDatas.Add(event1.Name, event1);
        Debug.Log("mockset1完了");

        //イベント2
        EventData event2 = new EventData();
        event2.ID = 2;
        event2.Name = "drink2";
        List<string> param2 = new List<string>();
        param2.Add("5");
        param2.Add("0.5");
        param2.Add("");
        param2.Add("1.2");

        ActionData action2 = createDummyAction(2, ActionType.Prefeb, PrefabType.FallSprite, "F:/picture/素材/drink_energy_can.png", param2);

        event2.Actions.Add(action2);
        event2.Actions.Add(action2);
        event2.Actions.Add(action2);

        _eventDatas.Add(event2.Name, event2);
        Debug.Log("mockset2完了");


    }

    /// <summary>
    /// ダミーデータ作成
    /// </summary>
    /// <param name="id"></param>
    /// <param name="type"></param>
    /// <param name="pref"></param>
    /// <param name="path"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public ActionData createDummyAction(int id, ActionType type, PrefabType pref,string path, List<string>parameters)
    {
        ActionData action = new ActionData();
        action.ID = id;
        action.Type = type;
        action.Name = pref;
        action.Path = path;
        action.Parameters = parameters;

        return action;
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
        //サンプルデータ1
        SpachaData spa1 = new SpachaData();
        spa1.Amount = new decimal(500);
        spa1.EventName = "drink2";

        SpachaDatas.Add(spa1);

        //サンプルデータ2
        SpachaData spa2 = new SpachaData();
        spa2.Amount = new decimal(100);
        spa2.EventName = "drink1";

        SpachaDatas.Add(spa2);

        //金額の大きい順にソートする
        SpachaDatas.Sort((a, b) =>   decimal.Compare(b.Amount, a.Amount));
        Debug.Log(SpachaDatas);
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
