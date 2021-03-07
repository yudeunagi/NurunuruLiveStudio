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
        //JSONファイルからイベント情報を保存する
        EventData event1 = new EventData();
        event1.ID = 1;
        event1.Name = "drink1";
        List<string> param1 = new List<string>();
        param1.Add("3");
        param1.Add("1");
        param1.Add("");
        param1.Add("1.2");

        ActionData action1 = createDummyAction(1, ActionType.Prefeb, PrefabType.FallSprite, "‪F:\\picture\\素材\\drink_energy_can.png", param1);

        event1.Actions.Add(action1);

        _eventDatas.Add(event1.Name, event1);
        Debug.Log("mockset完了");

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
    /// 開始時に保存した各種設定を読み込む
    /// </summary>
    void Start()
    {
        Debug.Log("start");

        LoadMockEventData();

    }

}
