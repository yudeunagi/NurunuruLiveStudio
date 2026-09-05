using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using Unage;

/// <summary>
/// トリガー設定データの永続化（JSON保存・読込）を担当する。
/// </summary>
public class SettingData : MonoBehaviour
{
    // 保存ファイル名
    private const string SettingsFileName = "triggerDatas.json";

    // 現在有効なトリガー設定
    private List<TriggerData> _triggerDatas = new List<TriggerData>();
    // 現行の保存先（永続データ領域）
    private string _settingsFilePath;
    // 旧保存先互換（Assets直下）
    private string _legacySettingsFilePath;

    [Serializable]
    private class TriggerDataFile
    {
        // ルート要素
        public List<TriggerDataRecord> triggerDatas = new List<TriggerDataRecord>();
    }

    [Serializable]
    private class TriggerDataRecord
    {
        // TriggerData 1件分のシリアライズ用DTO
        public int id;
        public string eventType;
        public string type;
        public string amount;
        public string word;
        public string findType;
        public List<EventDataRecord> eventList = new List<EventDataRecord>();
    }

    [Serializable]
    private class EventDataRecord
    {
        // EventData 1件分のシリアライズ用DTO
        public int id;
        public string path;
        public string prefabType;
        public ParameterRecord parameters = new ParameterRecord();
    }

    [Serializable]
    private class ParameterRecord
    {
        // EventData.Parameters のJSON表現
        public string lifeTime;
        public string size;
        public string posX;
        public string posY;
        public string movX;
        public string movY;
    }

    // 外部参照用アクセサ
    public List<TriggerData> TriggerDatas
    {
        get { return _triggerDatas; }
        set { _triggerDatas = value; }
    }

    /// <summary>
    /// 保存先パスを初期化する。
    /// </summary>
    private void Awake()
    {
        // 永続データ領域のパスを設定
        _settingsFilePath = Path.Combine(Application.persistentDataPath, SettingsFileName);
        _legacySettingsFilePath = Path.Combine(Application.dataPath, SettingsFileName);
    }

    /// <summary>
    /// 開始時に保存した各種設定を読み込む
    /// </summary>
    private void Start()
    {
        LoadEventData();
    }

    /// <summary>
    /// イベントデータのセーブ
    /// </summary>
    public void SaveEventData()
    {
        // _triggerDatas を保存用DTOに変換
        TriggerDataFile file = BuildFileFromTriggerDatas();
        // 保存用DTO を JSON 文字列に変換
        string json = JsonUtility.ToJson(file, true);

        // 保存ファイルディレクトリを取得
        string directory = Path.GetDirectoryName(_settingsFilePath);
        if (!string.IsNullOrEmpty(directory))
        {
            // ディレクトリが存在しない場合は作成
            Directory.CreateDirectory(directory);
        }

        // JSON文字列を保存ファイルに書き込む
        File.WriteAllText(_settingsFilePath, json);
    }

    /// <summary>
    /// イベントデータのロード
    /// </summary>
    public void LoadEventData()
    {
        // 設定ファイルのパス取得
        string readPath = ResolveSettingsReadPath();

        // 設定ファイルが存在するか確認
        if (string.IsNullOrEmpty(readPath))
        {
            // 設定ファイルが存在しない場合はモックデータをロード
            LoadMockTriggerData();
            return;
        }

        // 設定ファイルの内容を読み込む
        string json = File.ReadAllText(readPath);
        // 設定ファイルの内容が空かどうか確認
        if (string.IsNullOrWhiteSpace(json))
        {
            // 設定ファイルの内容が空の場合はモックデータをロード
            LoadMockTriggerData();
            return;
        }

        // JSON文字列をTriggerDataFileオブジェクトに変換
        TriggerDataFile file = JsonUtility.FromJson<TriggerDataFile>(json);
        if (file == null || file.triggerDatas == null)
        {
            // 設定ファイルの内容が不正な場合はモックデータをロード
            LoadMockTriggerData();
            return;
        }

        // ファイルから読み込んだデータをメモリ上の TriggerData 一式へ変換
        _triggerDatas = BuildTriggerDatasFromFile(file);
    }

    /// <summary>
    /// モックのイベントデータをロード
    /// </summary>
    public void LoadMockTriggerData()
    {
        TriggerDatas = DummyData.LoadNewMockTriggerData();
    }

    /// <summary>
    /// メモリ上の TriggerData 一式を保存用DTOへ変換する。
    /// </summary>
    private TriggerDataFile BuildFileFromTriggerDatas()
    {
        TriggerDataFile file = new TriggerDataFile();

        // メモリ上の TriggerData 一式を保存用DTOへ変換
        foreach (TriggerData trigger in _triggerDatas)
        {
            TriggerDataRecord triggerRecord = new TriggerDataRecord();
            triggerRecord.id = trigger.Id;
            // eventType を文字列に変換
            triggerRecord.eventType = trigger.EventType.ToString().ToLowerInvariant();
            // triggerRecord.eventType = trigger.EventType == TriggerData.EVENT_TYPE.Amount ? "amount" : "word";
            triggerRecord.amount = trigger.Amount.ToString(CultureInfo.InvariantCulture);
            triggerRecord.word = trigger.Word ?? string.Empty;
            triggerRecord.findType = trigger.Findtype.ToString().ToLowerInvariant();
            triggerRecord.eventList = new List<EventDataRecord>();

            if (trigger.EventList != null)
            {
                // イベントデータを保存用DTOへ変換
                for (int i = 0; i < trigger.EventList.Count; i++)
                {
                    EventData eventData = trigger.EventList[i];
                    EventDataRecord eventRecord = new EventDataRecord();
                    eventRecord.id = i + 1;
                    eventRecord.path = eventData.Path ?? string.Empty;
                    eventRecord.prefabType = eventData.PrefabType.ToString();
                    // parameters を保存用DTOへ変換
                    eventRecord.parameters = BuildParameterRecord(eventData);
                    triggerRecord.eventList.Add(eventRecord);
                }
            }

            file.triggerDatas.Add(triggerRecord);
        }

        return file;
    }

    /// <summary>
    /// EventData.Parameters を保存用の固定キー構造へ変換する。
    /// </summary>
    private ParameterRecord BuildParameterRecord(EventData eventData)
    {
        // EventData.Parameters を辞書形式に変換
        Dictionary<string, string> parameters = BuildParameterMap(eventData.Parameters);

        ParameterRecord record = new ParameterRecord();
        record.lifeTime = parameters[EventData.PARAMETER_KEY.LIFETIME.ToString()];
        record.size = parameters[EventData.PARAMETER_KEY.SIZE.ToString()];
        record.posX = parameters[EventData.PARAMETER_KEY.POSITION_X.ToString()];
        record.posY = parameters[EventData.PARAMETER_KEY.POSITION_Y.ToString()];
        record.movX = parameters[EventData.PARAMETER_KEY.MOVEMENT_X.ToString()];
        record.movY = parameters[EventData.PARAMETER_KEY.MOVEMENT_Y.ToString()];
        return record;
    }

    /// <summary>
    /// 読み込んだ保存DTOを実行時の TriggerData 構造へ復元する。
    /// </summary>
    private List<TriggerData> BuildTriggerDatasFromFile(TriggerDataFile file)
    {
        List<TriggerData> triggers = new List<TriggerData>();

        // ファイルから読み込んだ TriggerDataFile を実行時の TriggerData 構造へ復元
        foreach (TriggerDataRecord record in file.triggerDatas)
        {
            TriggerData trigger = new TriggerData();
            trigger.Id = record.id;
            trigger.EventType = ParseEventType(record.eventType, record.type);
            trigger.Amount = ParseAmount(record.amount);
            trigger.Word = record.word ?? string.Empty;
            trigger.Findtype = ParseFindType(record.findType);
            trigger.EventList = new List<EventData>();

            if (record.eventList != null)
            {
                foreach (EventDataRecord eventRecord in record.eventList)
                {
                    EventData eventData = new EventData();
                    eventData.ID = eventRecord.id;
                    eventData.Path = eventRecord.path ?? string.Empty;
                    eventData.PrefabType = ParsePrefabType(eventRecord.prefabType);
                    eventData.Parameters = LoadAndBuildParameterMap(eventRecord.parameters);
                    trigger.EventList.Add(eventData);
                }
            }

            RenumberEvents(trigger);
            triggers.Add(trigger);
        }

        return triggers;
    }

    /// <summary>
    /// ファイルロード時に使用するメソッド
    /// JSONの parameters オブジェクトを辞書へ展開する。
    /// </summary>
    private Dictionary<string, string> LoadAndBuildParameterMap(ParameterRecord record)
    {

        // デフォルトパラメータを設定
        Dictionary<string, string> parameters = CreateDefaultParameterMap();
        if (record == null)
        {
            return parameters;
        }

        parameters[EventData.PARAMETER_KEY.LIFETIME.ToString()] = record.lifeTime ?? string.Empty;
        parameters[EventData.PARAMETER_KEY.SIZE.ToString()] = record.size ?? string.Empty;
        parameters[EventData.PARAMETER_KEY.POSITION_X.ToString()] = record.posX ?? string.Empty;
        parameters[EventData.PARAMETER_KEY.POSITION_Y.ToString()] = record.posY ?? string.Empty;
        parameters[EventData.PARAMETER_KEY.MOVEMENT_X.ToString()] = record.movX ?? string.Empty;
        parameters[EventData.PARAMETER_KEY.MOVEMENT_Y.ToString()] = record.movY ?? string.Empty;
        return parameters;
    }

    /// <summary>
    /// 既存辞書を不足キー補完つきで正規化する。
    /// </summary>
    private Dictionary<string, string> BuildParameterMap(Dictionary<string, string> source)
    {
        Dictionary<string, string> parameters = CreateDefaultParameterMap();
        if (source == null)
        {
            return parameters;
        }

        foreach (KeyValuePair<string, string> entry in source)
        {
            parameters[entry.Key] = entry.Value ?? string.Empty;
        }

        return parameters;
    }

    /// <summary>
    /// Parameters の初期値マップを生成する。
    /// </summary>
    private Dictionary<string, string> CreateDefaultParameterMap()
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters[EventData.PARAMETER_KEY.LIFETIME.ToString()] = "5";
        parameters[EventData.PARAMETER_KEY.SIZE.ToString()] = "1";
        parameters[EventData.PARAMETER_KEY.POSITION_X.ToString()] = string.Empty;
        parameters[EventData.PARAMETER_KEY.POSITION_Y.ToString()] = string.Empty;
        parameters[EventData.PARAMETER_KEY.MOVEMENT_X.ToString()] = string.Empty;
        parameters[EventData.PARAMETER_KEY.MOVEMENT_Y.ToString()] = string.Empty;
        return parameters;
    }

    /// <summary>
    /// EventList のIDを1始まりの連番で採番し直す。
    /// </summary>
    private void RenumberEvents(TriggerData trigger)
    {
        if (trigger.EventList == null)
        {
            return;
        }

        for (int i = 0; i < trigger.EventList.Count; i++)
        {
            trigger.EventList[i].ID = i + 1;
        }
    }

    /// <summary>
    /// 保存文字列を TriggerData.EVENT_TYPE に変換する。
    /// </summary>
    private TriggerData.EVENT_TYPE ParseEventType(string eventType, string legacyType)
    {
        // eventType が空の場合は legacyType を使用
        string raw = !string.IsNullOrEmpty(eventType) ? eventType : legacyType;
        // 小文字に変換して比較するための値を取得
        string value = (raw ?? string.Empty).ToLowerInvariant();

        // "amount" の場合は金額イベント、それ以外は単語イベントとみなす
        // TODO: 将来的に他のイベントタイプの判定を追加する可能性あり
        if (value == "amount")
        {
            return TriggerData.EVENT_TYPE.Amount;
        }

        return TriggerData.EVENT_TYPE.Word;
    }

    /// <summary>
    /// 保存文字列を金額(decimal)へ変換する。失敗時は0。
    /// </summary>
    private decimal ParseAmount(string raw)
    {
        decimal amount;
        if (decimal.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out amount))
        {
            return amount;
        }

        return 0m;
    }

    /// <summary>
    /// 保存文字列を FindType へ変換する（suffix/sufix表記ゆれを許容）。
    /// </summary>
    private TriggerData.FINDT_YPE ParseFindType(string raw)
    {
        string value = (raw ?? string.Empty).ToLowerInvariant();
        switch (value)
        {
            case "prefix":
                return TriggerData.FINDT_YPE.Prefix;
            case "sufix":
            case "suffix":
                return TriggerData.FINDT_YPE.Sufix;
            case "perfect":
                return TriggerData.FINDT_YPE.Perfect;
            case "partial":
            default:
                return TriggerData.FINDT_YPE.Partial;
        }
    }

    /// <summary>
    /// 保存文字列を PrefabType へ変換する。失敗時は FallSprite。
    /// </summary>
    private EventData.PREFAB_TYPE ParsePrefabType(string raw)
    {
        // 文字が空の場合はデフォルトの FallSprite を返す
        if (string.IsNullOrEmpty(raw))
        {
            return EventData.PREFAB_TYPE.FallSprite;
        }

        object parsed;
        if (Enum.TryParse(typeof(EventData.PREFAB_TYPE), raw, true, out parsed))
        {
            return (EventData.PREFAB_TYPE)parsed;
        }

        // Enumに変換できなかった場合もデフォルトの FallSprite を返す
        return EventData.PREFAB_TYPE.FallSprite;
    }

    /// <summary>
    /// 設定読込時に利用可能な保存ファイルパスを返す。
    /// </summary>
    private string ResolveSettingsReadPath()
    {
        if (File.Exists(_settingsFilePath))
        {
            return _settingsFilePath;
        }

        if (File.Exists(_legacySettingsFilePath))
        {
            return _legacySettingsFilePath;
        }

        return string.Empty;
    }
}
