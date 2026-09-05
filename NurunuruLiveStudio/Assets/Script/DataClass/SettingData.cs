using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using Unage;

/// <summary>
/// 各種設定を扱う
/// </summary>
public class SettingData : MonoBehaviour
{
    private const string SettingsFileName = "triggerDatas.json";

    // トリガーに関するデータ
    private List<TriggerData> _triggerDatas = new List<TriggerData>();
    private string _settingsFilePath;
    private string _legacySettingsFilePath;

    [Serializable]
    private class TriggerDataFile
    {
        public List<TriggerDataRecord> triggerDatas = new List<TriggerDataRecord>();
    }

    [Serializable]
    private class TriggerDataRecord
    {
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
        public int id;
        public string path;
        public string prefabType;
        public ParameterRecord parameters = new ParameterRecord();
    }

    [Serializable]
    private class ParameterRecord
    {
        public string lifeTime;
        public string size;
        public string posX;
        public string posY;
        public string movX;
        public string movY;
    }

    // 以下アクセサ
    public List<TriggerData> TriggerDatas
    {
        get { return _triggerDatas; }
        set { _triggerDatas = value; }
    }

    private void Awake()
    {
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
        TriggerDataFile file = BuildFileFromTriggerDatas();
        string json = JsonUtility.ToJson(file, true);

        string directory = Path.GetDirectoryName(_settingsFilePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(_settingsFilePath, json);
    }

    /// <summary>
    /// イベントデータのロード
    /// </summary>
    public void LoadEventData()
    {
        string readPath = ResolveSettingsReadPath();
        if (string.IsNullOrEmpty(readPath))
        {
            LoadMockTriggerData();
            return;
        }

        string json = File.ReadAllText(readPath);
        if (string.IsNullOrWhiteSpace(json))
        {
            LoadMockTriggerData();
            return;
        }

        TriggerDataFile file = JsonUtility.FromJson<TriggerDataFile>(json);
        if (file == null || file.triggerDatas == null)
        {
            LoadMockTriggerData();
            return;
        }

        _triggerDatas = BuildTriggerDatasFromFile(file);
    }

    /// <summary>
    /// イベントデータのロード
    /// </summary>
    public void LoadMockEventData()
    {
        LoadMockTriggerData();
    }

    public void LoadMockTriggerData()
    {
        TriggerDatas = DummyData.LoadNewMockTriggerData();
    }

    private TriggerDataFile BuildFileFromTriggerDatas()
    {
        TriggerDataFile file = new TriggerDataFile();

        foreach (TriggerData trigger in _triggerDatas)
        {
            TriggerDataRecord triggerRecord = new TriggerDataRecord();
            triggerRecord.id = trigger.Id;
            triggerRecord.eventType = trigger.EventType == TriggerData.EVENT_TYPE.Amount ? "amount" : "word";
            triggerRecord.amount = trigger.Amount.ToString(CultureInfo.InvariantCulture);
            triggerRecord.word = trigger.Word ?? string.Empty;
            triggerRecord.findType = trigger.Findtype.ToString().ToLowerInvariant();
            triggerRecord.eventList = new List<EventDataRecord>();

            if (trigger.EventList != null)
            {
                for (int i = 0; i < trigger.EventList.Count; i++)
                {
                    EventData eventData = trigger.EventList[i];
                    EventDataRecord eventRecord = new EventDataRecord();
                    eventRecord.id = i + 1;
                    eventRecord.path = eventData.Path ?? string.Empty;
                    eventRecord.prefabType = eventData.PrefabType.ToString();
                    eventRecord.parameters = BuildParameterRecord(eventData);
                    triggerRecord.eventList.Add(eventRecord);
                }
            }

            file.triggerDatas.Add(triggerRecord);
        }

        return file;
    }

    private ParameterRecord BuildParameterRecord(EventData eventData)
    {
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

    private List<TriggerData> BuildTriggerDatasFromFile(TriggerDataFile file)
    {
        List<TriggerData> triggers = new List<TriggerData>();

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
                    eventData.Parameters = BuildParameterMap(eventRecord.parameters);
                    trigger.EventList.Add(eventData);
                }
            }

            RenumberEvents(trigger);
            triggers.Add(trigger);
        }

        return triggers;
    }

    private Dictionary<string, string> BuildParameterMap(ParameterRecord record)
    {
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

    private Dictionary<string, string> CreateDefaultParameterMap()
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters[EventData.PARAMETER_KEY.LIFETIME.ToString()] = "5";
        parameters[EventData.PARAMETER_KEY.SIZE.ToString()] = "0.8";
        parameters[EventData.PARAMETER_KEY.POSITION_X.ToString()] = string.Empty;
        parameters[EventData.PARAMETER_KEY.POSITION_Y.ToString()] = string.Empty;
        parameters[EventData.PARAMETER_KEY.MOVEMENT_X.ToString()] = string.Empty;
        parameters[EventData.PARAMETER_KEY.MOVEMENT_Y.ToString()] = string.Empty;
        return parameters;
    }

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

    private TriggerData.EVENT_TYPE ParseEventType(string eventType, string legacyType)
    {
        string raw = !string.IsNullOrEmpty(eventType) ? eventType : legacyType;
        string value = (raw ?? string.Empty).ToLowerInvariant();
        if (value == "amount")
        {
            return TriggerData.EVENT_TYPE.Amount;
        }

        return TriggerData.EVENT_TYPE.Word;
    }

    private decimal ParseAmount(string raw)
    {
        decimal amount;
        if (decimal.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out amount))
        {
            return amount;
        }

        return 0m;
    }

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

    private EventData.PREFAB_TYPE ParsePrefabType(string raw)
    {
        if (string.IsNullOrEmpty(raw))
        {
            return EventData.PREFAB_TYPE.FallSprite;
        }

        object parsed;
        if (Enum.TryParse(typeof(EventData.PREFAB_TYPE), raw, true, out parsed))
        {
            return (EventData.PREFAB_TYPE)parsed;
        }

        return EventData.PREFAB_TYPE.FallSprite;
    }

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
