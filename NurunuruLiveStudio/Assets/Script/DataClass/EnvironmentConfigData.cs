using System;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Unage
{
    /// <summary>
    /// 環境設定（受信ポート・転送ポート・転送有無）の保持と永続化を担当する。
    /// </summary>
    public class EnvironmentConfigData : MonoBehaviour
    {
        private const string ConfigFileName = "config.json";
        private const int DefaultMyPortNumber = 50002;
        private const int DefaultSendPortNumber = 50001;
        private const bool DefaultIsTransport = true;

        [SerializeField]
        private int _myPortNumber = DefaultMyPortNumber;
        [SerializeField]
        private int _sendPortNumber = DefaultSendPortNumber;
        [SerializeField]
        private bool _isTransport = DefaultIsTransport;

        private string _configFilePath;
        private string _legacyConfigFilePath;

        [Serializable]
        private class ConfigFileRecord
        {
            public string myPortNumber = DefaultMyPortNumber.ToString(CultureInfo.InvariantCulture);
            public string sendPortNumber = DefaultSendPortNumber.ToString(CultureInfo.InvariantCulture);
            public bool isTransport = DefaultIsTransport;
        }

        public int MyPortNumber
        {
            get { return _myPortNumber; }
            set { _myPortNumber = value; }
        }

        public int SendPortNumber
        {
            get { return _sendPortNumber; }
            set { _sendPortNumber = value; }
        }

        public bool IsTransport
        {
            get { return _isTransport; }
            set { _isTransport = value; }
        }

        /// <summary>
        /// シーン内に本コンポーネントが存在しない場合、自動生成する。
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstance()
        {
            if (UnityEngine.Object.FindFirstObjectByType<EnvironmentConfigData>() != null)
            {
                return;
            }

            GameObject dataObject = new GameObject("EnvironmentConfigData");
            dataObject.AddComponent<EnvironmentConfigData>();
        }

        private void Awake()
        {
            _configFilePath = Path.Combine(Application.persistentDataPath, ConfigFileName);
            _legacyConfigFilePath = Path.Combine(Application.dataPath, ConfigFileName);
            LoadConfig();
        }

        /// <summary>
        /// 現在の環境設定を config.json に保存する。
        /// </summary>
        public void SaveConfig()
        {
            ConfigFileRecord record = new ConfigFileRecord();
            record.myPortNumber = _myPortNumber.ToString(CultureInfo.InvariantCulture);
            record.sendPortNumber = _sendPortNumber.ToString(CultureInfo.InvariantCulture);
            record.isTransport = _isTransport;

            string json = JsonUtility.ToJson(record, true);
            string directory = Path.GetDirectoryName(_configFilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(_configFilePath, json);
        }

        /// <summary>
        /// config.json を読み込み、メモリ上の環境設定に反映する。
        /// </summary>
        public void LoadConfig()
        {
            string readPath = ResolveConfigReadPath();
            if (string.IsNullOrEmpty(readPath))
            {
                ApplyDefaultValues();
                return;
            }

            string json = File.ReadAllText(readPath);
            if (string.IsNullOrWhiteSpace(json))
            {
                ApplyDefaultValues();
                return;
            }

            ConfigFileRecord record = JsonUtility.FromJson<ConfigFileRecord>(json);
            if (record == null)
            {
                ApplyDefaultValues();
                return;
            }

            _myPortNumber = ParsePortOrDefault(record.myPortNumber, DefaultMyPortNumber);
            _sendPortNumber = ParsePortOrDefault(record.sendPortNumber, DefaultSendPortNumber);
            _isTransport = record.isTransport;
        }

        private string ResolveConfigReadPath()
        {
            if (File.Exists(_configFilePath))
            {
                return _configFilePath;
            }

            if (File.Exists(_legacyConfigFilePath))
            {
                return _legacyConfigFilePath;
            }

            return string.Empty;
        }

        private int ParsePortOrDefault(string value, int defaultValue)
        {
            int parsed;
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed) &&
                parsed > 0 &&
                parsed <= 65535)
            {
                return parsed;
            }

            return defaultValue;
        }

        private void ApplyDefaultValues()
        {
            _myPortNumber = DefaultMyPortNumber;
            _sendPortNumber = DefaultSendPortNumber;
            _isTransport = DefaultIsTransport;
        }
    }
}
