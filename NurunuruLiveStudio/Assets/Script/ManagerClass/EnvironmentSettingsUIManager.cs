using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unage
{
    /// <summary>
    /// UI Toolkit で環境設定画面を表示・編集するマネージャ。
    /// </summary>
    public class EnvironmentSettingsUIManager : MonoBehaviour
    {
        private const int SaveMessageDurationMilliseconds = 1500;

        private EnvironmentConfigData _configData;
        private UIDocument _document;

        private bool _isWindowVisible;
        private bool _isDirty;

        private int _editingMyPortNumber;
        private int _editingSendPortNumber;
        private bool _editingIsTransport;

        private VisualElement _windowRoot;
        private TextField _myPortField;
        private TextField _sendPortField;
        private Toggle _transportToggle;

        private VisualElement _modalOverlay;
        private Label _modalMessageLabel;
        private VisualElement _modalButtonsContainer;
        private Button _modalYesButton;
        private Button _modalNoButton;
        private Button _modalCancelButton;
        private Action _modalYesAction;
        private Action _modalNoAction;
        private Action _modalCancelAction;
        private int _modalStateToken;
        private bool _isAutoCloseModal;

        /// <summary>
        /// シーン内に本コンポーネントが存在しない場合、自動生成する。
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstance()
        {
            if (UnityEngine.Object.FindFirstObjectByType<EnvironmentSettingsUIManager>() != null)
            {
                return;
            }

            GameObject managerObject = new GameObject("EnvironmentSettingsUIManager");
            managerObject.AddComponent<EnvironmentSettingsUIManager>();
        }

        private void Awake()
        {
            ResolveConfigData();
            _document = SettingsUiCommon.EnsureDocument(this);
            BuildUi();
            UpdateVisibility();
        }

        /// <summary>
        /// 環境設定画面を開き、現在設定値のワークコピーを編集欄へ反映する。
        /// </summary>
        public void OpenWindow()
        {
            ResolveConfigData();
            if (_configData == null)
            {
                Debug.LogError("EnvironmentConfigData が見つかりません。");
                return;
            }

            _editingMyPortNumber = _configData.MyPortNumber;
            _editingSendPortNumber = _configData.SendPortNumber;
            _editingIsTransport = _configData.IsTransport;
            _isDirty = false;
            _isWindowVisible = true;

            _myPortField.SetValueWithoutNotify(_editingMyPortNumber.ToString(CultureInfo.InvariantCulture));
            _sendPortField.SetValueWithoutNotify(_editingSendPortNumber.ToString(CultureInfo.InvariantCulture));
            _transportToggle.SetValueWithoutNotify(_editingIsTransport);
            HideModal();
            UpdateVisibility();
        }

        private void ResolveConfigData()
        {
            if (_configData == null)
            {
                _configData = UnityEngine.Object.FindFirstObjectByType<EnvironmentConfigData>();
            }

            if (_configData == null)
            {
                GameObject dataObject = new GameObject("EnvironmentConfigData");
                _configData = dataObject.AddComponent<EnvironmentConfigData>();
            }
        }

        private void BuildUi()
        {
            VisualElement root = _document.rootVisualElement;
            root.Clear();
            root.style.flexGrow = 1f;

            SettingsUiCommon.WindowElements window = SettingsUiCommon.CreateWindow(root, "環境設定", 0.9f);
            _windowRoot = window.WindowRoot;
            window.Body.style.flexDirection = FlexDirection.Column;

            _myPortField = new TextField("受信ポート番号");
            _myPortField.isDelayed = true;
            SettingsUiCommon.ApplyTextFieldStyle(_myPortField);
            window.Body.Add(_myPortField);

            _sendPortField = new TextField("転送ポート番号");
            _sendPortField.isDelayed = true;
            SettingsUiCommon.ApplyTextFieldStyle(_sendPortField);
            window.Body.Add(_sendPortField);

            _transportToggle = new Toggle("棒読みちゃんへの転送");
            window.Body.Add(_transportToggle);

            _myPortField.RegisterValueChangedCallback(delegate(ChangeEvent<string> evt)
            {
                int nextPort;
                if (TryParsePort(evt.newValue, out nextPort))
                {
                    if (_editingMyPortNumber != nextPort)
                    {
                        _editingMyPortNumber = nextPort;
                        _isDirty = true;
                    }
                    return;
                }

                _myPortField.SetValueWithoutNotify(_editingMyPortNumber.ToString(CultureInfo.InvariantCulture));
            });

            _sendPortField.RegisterValueChangedCallback(delegate(ChangeEvent<string> evt)
            {
                int nextPort;
                if (TryParsePort(evt.newValue, out nextPort))
                {
                    if (_editingSendPortNumber != nextPort)
                    {
                        _editingSendPortNumber = nextPort;
                        _isDirty = true;
                    }
                    return;
                }

                _sendPortField.SetValueWithoutNotify(_editingSendPortNumber.ToString(CultureInfo.InvariantCulture));
            });

            _transportToggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> evt)
            {
                if (_editingIsTransport != evt.newValue)
                {
                    _editingIsTransport = evt.newValue;
                    _isDirty = true;
                }
            });

            Button saveButton = new Button(OnSaveClicked);
            saveButton.text = "保存";
            saveButton.style.width = 96f;
            saveButton.style.marginRight = 6f;
            window.Footer.Add(saveButton);

            Button closeButton = new Button(OnCloseClicked);
            closeButton.text = "閉じる";
            closeButton.style.width = 96f;
            window.Footer.Add(closeButton);

            SettingsUiCommon.ModalElements modal = SettingsUiCommon.CreateModal(root, OnModalYesClicked, OnModalNoClicked, OnModalCancelClicked);
            _modalOverlay = modal.Overlay;
            _modalMessageLabel = modal.MessageLabel;
            _modalButtonsContainer = modal.ButtonsContainer;
            _modalYesButton = modal.YesButton;
            _modalNoButton = modal.NoButton;
            _modalCancelButton = modal.CancelButton;
            HideModal();
        }

        private void OnSaveClicked()
        {
            if (SaveEditingConfig())
            {
                ShowSavedMessageModal();
            }
        }

        private bool SaveEditingConfig()
        {
            ResolveConfigData();
            if (_configData == null)
            {
                Debug.LogError("EnvironmentConfigData が見つかりません。");
                return false;
            }

            int myPort;
            int sendPort;
            if (!TryParsePort(_myPortField.value, out myPort) || !TryParsePort(_sendPortField.value, out sendPort))
            {
                ShowModal("ポート番号は1～65535の整数を入力してください。", "OK", string.Empty, string.Empty, HideModal, null, null);
                return false;
            }

            _editingMyPortNumber = myPort;
            _editingSendPortNumber = sendPort;

            _configData.MyPortNumber = _editingMyPortNumber;
            _configData.SendPortNumber = _editingSendPortNumber;
            _configData.IsTransport = _editingIsTransport;
            _configData.SaveConfig();
            ApplyConfigToServers();
            _isDirty = false;
            return true;
        }

        private void OnCloseClicked()
        {
            if (_isDirty)
            {
                ShowModal(
                    "設定が変更されています、閉じる前に保存しますか？",
                    "はい",
                    "いいえ",
                    "キャンセル",
                    delegate
                    {
                        if (SaveEditingConfig())
                        {
                            CloseWindow();
                        }
                    },
                    delegate
                    {
                        CloseWindow();
                    },
                    HideModal);
                return;
            }

            CloseWindow();
        }

        private void CloseWindow()
        {
            HideModal();
            _isWindowVisible = false;
            UpdateVisibility();
        }

        private void ApplyConfigToServers()
        {
            TestTCPServer[] servers = UnityEngine.Object.FindObjectsByType<TestTCPServer>(FindObjectsSortMode.None);
            for (int i = 0; i < servers.Length; i++)
            {
                servers[i].ApplyEnvironmentConfig(_configData);
            }
        }

        private bool TryParsePort(string raw, out int port)
        {
            return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out port) &&
                   port > 0 &&
                   port <= 65535;
        }

        private void ShowSavedMessageModal()
        {
            _modalStateToken += 1;
            int token = _modalStateToken;
            _isAutoCloseModal = true;

            _modalMessageLabel.text = "保存しました";
            _modalButtonsContainer.style.display = DisplayStyle.None;
            _modalYesButton.style.display = DisplayStyle.None;
            _modalNoButton.style.display = DisplayStyle.None;
            _modalCancelButton.style.display = DisplayStyle.None;

            _modalYesAction = null;
            _modalNoAction = null;
            _modalCancelAction = null;
            _modalOverlay.style.display = DisplayStyle.Flex;

            _modalOverlay.schedule.Execute((Action)delegate
            {
                if (_isAutoCloseModal && _modalStateToken == token)
                {
                    HideModal();
                }
            }).StartingIn(SaveMessageDurationMilliseconds);
        }

        private void ShowModal(string message, string yesText, string noText, string cancelText, Action yesAction, Action noAction, Action cancelAction)
        {
            _modalStateToken += 1;
            _isAutoCloseModal = false;
            _modalMessageLabel.text = message;
            _modalYesButton.text = string.IsNullOrEmpty(yesText) ? "はい" : yesText;
            _modalNoButton.text = string.IsNullOrEmpty(noText) ? "いいえ" : noText;
            _modalCancelButton.text = string.IsNullOrEmpty(cancelText) ? "キャンセル" : cancelText;
            _modalButtonsContainer.style.display = DisplayStyle.Flex;
            _modalYesButton.style.display = yesAction == null ? DisplayStyle.None : DisplayStyle.Flex;
            _modalNoButton.style.display = noAction == null ? DisplayStyle.None : DisplayStyle.Flex;
            _modalCancelButton.style.display = cancelAction == null ? DisplayStyle.None : DisplayStyle.Flex;

            _modalYesAction = yesAction;
            _modalNoAction = noAction;
            _modalCancelAction = cancelAction;
            _modalOverlay.style.display = DisplayStyle.Flex;
        }

        private void HideModal()
        {
            _modalStateToken += 1;
            _isAutoCloseModal = false;
            _modalOverlay.style.display = DisplayStyle.None;
            _modalYesAction = null;
            _modalNoAction = null;
            _modalCancelAction = null;
        }

        private void OnModalYesClicked()
        {
            Action action = _modalYesAction;
            if (action != null)
            {
                action();
            }
        }

        private void OnModalNoClicked()
        {
            Action action = _modalNoAction;
            if (action != null)
            {
                action();
            }
        }

        private void OnModalCancelClicked()
        {
            Action action = _modalCancelAction;
            if (action != null)
            {
                action();
            }
        }

        private void UpdateVisibility()
        {
            _windowRoot.style.display = _isWindowVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
