using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unage
{
    public class EventSettingsUIManager : MonoBehaviour
    {
        private const float DoubleClickThresholdSeconds = 0.3f;

        private SettingData _settingData;
        private UIDocument _document;

        private bool _isSettingsButtonVisible;
        private bool _isWindowVisible;
        private bool _isDirty;
        private float _lastClickTime = -10f;

        private List<TriggerData> _editingTriggerDatas = new List<TriggerData>();
        private TriggerData _selectedTrigger;
        private EventData _selectedEvent;
        private TriggerData _pendingDeleteTrigger;

        private Button _settingsLauncherButton;
        private VisualElement _windowRoot;
        private ScrollView _triggerListView;
        private ScrollView _editorView;

        private VisualElement _modalOverlay;
        private Label _modalMessageLabel;
        private Button _modalYesButton;
        private Button _modalNoButton;
        private Button _modalCancelButton;
        private Action _modalYesAction;
        private Action _modalNoAction;
        private Action _modalCancelAction;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstance()
        {
            if (FindAnyObjectByType<EventSettingsUIManager>() != null)
            {
                return;
            }

            GameObject managerObject = new GameObject("EventSettingsUIManager");
            managerObject.AddComponent<EventSettingsUIManager>();
        }

        private void Awake()
        {
            ResolveSettingData();
            EnsureDocument();
            BuildUi();
            UpdateVisibility();
        }

        private void Update()
        {
            if (_isWindowVisible)
            {
                return;
            }

            if (!IsLeftClickDown())
            {
                return;
            }

            float now = Time.unscaledTime;
            if (now - _lastClickTime <= DoubleClickThresholdSeconds)
            {
                _isSettingsButtonVisible = !_isSettingsButtonVisible;
                if (!_isSettingsButtonVisible)
                {
                    _isWindowVisible = false;
                }

                UpdateVisibility();
                _lastClickTime = -10f;
                return;
            }

            _lastClickTime = now;
        }

        private void ResolveSettingData()
        {
            if (_settingData == null)
            {
                _settingData = FindAnyObjectByType<SettingData>();
            }
        }

        private void EnsureDocument()
        {
            _document = GetComponent<UIDocument>();
            if (_document == null)
            {
                _document = gameObject.AddComponent<UIDocument>();
            }

            if (_document.panelSettings == null)
            {
                PanelSettings panelSettings = FindReusablePanelSettings();
                if (panelSettings == null)
                {
                    panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
                    panelSettings.sortingOrder = 1000;
                    ThemeStyleSheet themeStyleSheet = TryResolveThemeStyleSheet();
                    if (themeStyleSheet != null)
                    {
                        panelSettings.themeStyleSheet = themeStyleSheet;
                    }
                    else
                    {
                        Debug.LogWarning("PanelSettings の Theme Style Sheet が見つかりません。UIDocument を作成して UnityDefaultRuntimeTheme.tss を生成するか、Theme Style Sheet を持つ PanelSettings を既存 UIDocument に設定してください。");
                    }
                }

                _document.panelSettings = panelSettings;
            }
        }

        private PanelSettings FindReusablePanelSettings()
        {
            UIDocument[] sceneDocuments = FindObjectsOfType<UIDocument>();
            for (int i = 0; i < sceneDocuments.Length; i++)
            {
                UIDocument document = sceneDocuments[i];
                if (document == null || document == _document)
                {
                    continue;
                }

                if (document.panelSettings != null)
                {
                    return document.panelSettings;
                }
            }

            UIDocument[] loadedDocuments = Resources.FindObjectsOfTypeAll<UIDocument>();
            for (int i = 0; i < loadedDocuments.Length; i++)
            {
                UIDocument document = loadedDocuments[i];
                if (document == null || document == _document)
                {
                    continue;
                }

                if (document.panelSettings != null)
                {
                    return document.panelSettings;
                }
            }

            return null;
        }

        private ThemeStyleSheet TryResolveThemeStyleSheet()
        {
            ThemeStyleSheet[] loadedThemes = Resources.FindObjectsOfTypeAll<ThemeStyleSheet>();
            if (loadedThemes != null && loadedThemes.Length > 0)
            {
                return loadedThemes[0];
            }

#if UNITY_EDITOR
            string[] priorityPaths = new[]
            {
                "Assets/UI Toolkit/UnityThemes/UnityDefaultRuntimeTheme.tss",
                "Packages/com.unity.ui/PackageResources/StyleSheets/Generated/DefaultCommonDark.tss",
                "Packages/com.unity.ui/PackageResources/StyleSheets/Generated/DefaultCommonLight.tss",
            };

            for (int i = 0; i < priorityPaths.Length; i++)
            {
                ThemeStyleSheet theme = UnityEditor.AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(priorityPaths[i]);
                if (theme != null)
                {
                    return theme;
                }
            }

            string[] themeGuids = UnityEditor.AssetDatabase.FindAssets("t:ThemeStyleSheet");
            for (int i = 0; i < themeGuids.Length; i++)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(themeGuids[i]);
                ThemeStyleSheet theme = UnityEditor.AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(assetPath);
                if (theme != null)
                {
                    return theme;
                }
            }
#endif

            return null;
        }

        private void BuildUi()
        {
            VisualElement root = _document.rootVisualElement;
            root.Clear();
            root.style.flexGrow = 1f;

            _settingsLauncherButton = new Button(OnSettingsLauncherClicked);
            _settingsLauncherButton.text = "設定";
            _settingsLauncherButton.style.position = Position.Absolute;
            _settingsLauncherButton.style.left = 10f;
            _settingsLauncherButton.style.top = 10f;
            _settingsLauncherButton.style.width = 60f;
            _settingsLauncherButton.style.height = 30f;
            root.Add(_settingsLauncherButton);

            _windowRoot = new VisualElement();
            _windowRoot.style.position = Position.Absolute;
            _windowRoot.style.left = Length.Percent(8);
            _windowRoot.style.top = Length.Percent(8);
            _windowRoot.style.width = Length.Percent(84);
            _windowRoot.style.height = Length.Percent(84);
            _windowRoot.style.backgroundColor = new Color(0.95f, 0.95f, 0.95f, 0.80f);
            _windowRoot.style.borderBottomColor = Color.gray;
            _windowRoot.style.borderTopColor = Color.gray;
            _windowRoot.style.borderLeftColor = Color.gray;
            _windowRoot.style.borderRightColor = Color.gray;
            _windowRoot.style.borderBottomWidth = 1f;
            _windowRoot.style.borderTopWidth = 1f;
            _windowRoot.style.borderLeftWidth = 1f;
            _windowRoot.style.borderRightWidth = 1f;
            _windowRoot.style.paddingLeft = 8f;
            _windowRoot.style.paddingRight = 8f;
            _windowRoot.style.paddingTop = 8f;
            _windowRoot.style.paddingBottom = 8f;
            root.Add(_windowRoot);

            Label title = new Label("イベント設定");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.fontSize = 18;
            title.style.marginBottom = 8f;
            _windowRoot.Add(title);

            VisualElement body = new VisualElement();
            body.style.flexDirection = FlexDirection.Row;
            body.style.flexGrow = 1f;
            _windowRoot.Add(body);

            VisualElement leftPane = new VisualElement();
            leftPane.style.width = Length.Percent(55);
            leftPane.style.marginRight = 8f;
            body.Add(leftPane);

            Button addTriggerButton = new Button(OnAddTriggerClicked);
            addTriggerButton.text = "トリガー追加";
            addTriggerButton.style.width = 120f;
            addTriggerButton.style.marginBottom = 6f;
            leftPane.Add(addTriggerButton);

            _triggerListView = new ScrollView();
            _triggerListView.style.flexGrow = 1f;
            leftPane.Add(_triggerListView);

            _editorView = new ScrollView();
            _editorView.style.flexGrow = 1f;
            body.Add(_editorView);

            VisualElement footer = new VisualElement();
            footer.style.flexDirection = FlexDirection.Row;
            footer.style.justifyContent = Justify.FlexEnd;
            footer.style.marginTop = 8f;
            _windowRoot.Add(footer);

            Button saveButton = new Button(OnSaveClicked);
            saveButton.text = "保存";
            saveButton.style.width = 96f;
            saveButton.style.marginRight = 6f;
            footer.Add(saveButton);

            Button closeButton = new Button(OnCloseClicked);
            closeButton.text = "閉じる";
            closeButton.style.width = 96f;
            footer.Add(closeButton);

            BuildModal(root);
        }

        private void BuildModal(VisualElement root)
        {
            _modalOverlay = new VisualElement();
            _modalOverlay.style.position = Position.Absolute;
            _modalOverlay.style.left = 0f;
            _modalOverlay.style.top = 0f;
            _modalOverlay.style.right = 0f;
            _modalOverlay.style.bottom = 0f;
            _modalOverlay.style.backgroundColor = new Color(0f, 0f, 0f, 0.4f);
            root.Add(_modalOverlay);

            VisualElement modalPanel = new VisualElement();
            modalPanel.style.position = Position.Absolute;
            modalPanel.style.left = Length.Percent(32);
            modalPanel.style.top = Length.Percent(36);
            modalPanel.style.width = Length.Percent(36);
            modalPanel.style.height = 140f;
            modalPanel.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            modalPanel.style.borderBottomColor = Color.gray;
            modalPanel.style.borderTopColor = Color.gray;
            modalPanel.style.borderLeftColor = Color.gray;
            modalPanel.style.borderRightColor = Color.gray;
            modalPanel.style.borderBottomWidth = 1f;
            modalPanel.style.borderTopWidth = 1f;
            modalPanel.style.borderLeftWidth = 1f;
            modalPanel.style.borderRightWidth = 1f;
            modalPanel.style.paddingLeft = 10f;
            modalPanel.style.paddingRight = 10f;
            modalPanel.style.paddingTop = 10f;
            modalPanel.style.paddingBottom = 10f;
            _modalOverlay.Add(modalPanel);

            _modalMessageLabel = new Label();
            _modalMessageLabel.style.whiteSpace = WhiteSpace.Normal;
            _modalMessageLabel.style.flexGrow = 1f;
            modalPanel.Add(_modalMessageLabel);

            VisualElement buttons = new VisualElement();
            buttons.style.flexDirection = FlexDirection.Row;
            buttons.style.justifyContent = Justify.FlexEnd;
            modalPanel.Add(buttons);

            _modalYesButton = new Button(OnModalYesClicked);
            _modalYesButton.text = "はい";
            _modalYesButton.style.width = 70f;
            _modalYesButton.style.marginRight = 6f;
            buttons.Add(_modalYesButton);

            _modalNoButton = new Button(OnModalNoClicked);
            _modalNoButton.text = "いいえ";
            _modalNoButton.style.width = 70f;
            _modalNoButton.style.marginRight = 6f;
            buttons.Add(_modalNoButton);

            _modalCancelButton = new Button(OnModalCancelClicked);
            _modalCancelButton.text = "キャンセル";
            _modalCancelButton.style.width = 100f;
            buttons.Add(_modalCancelButton);

            HideModal();
        }

        private void OnSettingsLauncherClicked()
        {
            ResolveSettingData();
            if (_settingData == null)
            {
                Debug.LogError("SettingData が見つかりません。");
                return;
            }

            _editingTriggerDatas = CloneTriggerDatas(_settingData.TriggerDatas);
            _selectedTrigger = null;
            _selectedEvent = null;
            _isDirty = false;
            _isWindowVisible = true;

            UpdateVisibility();
            RefreshTriggerList();
            RefreshEditor();
        }

        private void OnAddTriggerClicked()
        {
            TriggerData trigger = new TriggerData();
            trigger.Id = GenerateTriggerId();
            trigger.EventType = TriggerData.EVENT_TYPE.Word;
            trigger.Amount = 0m;
            trigger.Word = string.Empty;
            trigger.Findtype = TriggerData.FINDT_YPE.Partial;
            trigger.EventList = new List<EventData>();

            _editingTriggerDatas.Add(trigger);
            _selectedTrigger = trigger;
            _selectedEvent = null;
            _isDirty = true;

            RefreshTriggerList();
            RefreshEditor();
        }

        private void OnSaveClicked()
        {
            ResolveSettingData();
            if (_settingData == null)
            {
                Debug.LogError("SettingData が見つかりません。");
                return;
            }

            _settingData.TriggerDatas = CloneTriggerDatas(_editingTriggerDatas);
            _settingData.SaveEventData();
            _isDirty = false;
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
                        OnSaveClicked();
                        CloseWindow();
                    },
                    delegate
                    {
                        CloseWindow();
                    },
                    delegate
                    {
                        HideModal();
                    });
                return;
            }

            CloseWindow();
        }

        private void CloseWindow()
        {
            HideModal();
            _isWindowVisible = false;
            _pendingDeleteTrigger = null;
            _selectedTrigger = null;
            _selectedEvent = null;
            _editingTriggerDatas = new List<TriggerData>();
            UpdateVisibility();
        }

        private void RefreshTriggerList()
        {
            _triggerListView.Clear();

            for (int i = 0; i < _editingTriggerDatas.Count; i++)
            {
                TriggerData trigger = _editingTriggerDatas[i];
                _triggerListView.Add(CreateTriggerRow(trigger));

                if (_selectedTrigger == trigger)
                {
                    AddEventRows(trigger);
                }
            }
        }

        private VisualElement CreateTriggerRow(TriggerData trigger)
        {
            VisualElement row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.marginBottom = 4f;

            string summary = trigger.EventType == TriggerData.EVENT_TYPE.Amount
                ? trigger.Amount.ToString("0", CultureInfo.InvariantCulture)
                : trigger.Word ?? string.Empty;

            Button selectButton = new Button(delegate
            {
                _selectedTrigger = trigger;
                _selectedEvent = null;
                RefreshTriggerList();
                RefreshEditor();
            });
            selectButton.text = "T:" + summary;
            selectButton.style.flexGrow = 1f;
            row.Add(selectButton);

            Button addButton = new Button(delegate
            {
                AddEvent(trigger);
            });
            addButton.text = "追加";
            addButton.style.width = 64f;
            addButton.style.marginLeft = 4f;
            row.Add(addButton);

            Button deleteButton = new Button(delegate
            {
                _pendingDeleteTrigger = trigger;
                ShowModal(
                    "この TriggerData と紐づく EventData を削除しますか？",
                    "はい",
                    "いいえ",
                    string.Empty,
                    delegate
                    {
                        RemoveTrigger(_pendingDeleteTrigger);
                        HideModal();
                    },
                    delegate
                    {
                        _pendingDeleteTrigger = null;
                        HideModal();
                    },
                    null);
            });
            deleteButton.text = "削除";
            deleteButton.style.width = 64f;
            deleteButton.style.marginLeft = 4f;
            row.Add(deleteButton);

            return row;
        }

        private void AddEventRows(TriggerData trigger)
        {
            if (trigger.EventList == null)
            {
                trigger.EventList = new List<EventData>();
                return;
            }

            for (int i = 0; i < trigger.EventList.Count; i++)
            {
                EventData eventData = trigger.EventList[i];
                VisualElement row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.marginLeft = 16f;
                row.style.marginBottom = 2f;

                string name = Path.GetFileName(eventData.Path ?? string.Empty);
                Button selectButton = new Button(delegate
                {
                    _selectedTrigger = trigger;
                    _selectedEvent = eventData;
                    RefreshEditor();
                });
                selectButton.text = "E:" + name;
                selectButton.style.flexGrow = 1f;
                row.Add(selectButton);

                Button copyButton = new Button(delegate
                {
                    DuplicateEvent(trigger, eventData);
                });
                copyButton.text = "複製";
                copyButton.style.width = 64f;
                copyButton.style.marginLeft = 4f;
                row.Add(copyButton);

                Button deleteButton = new Button(delegate
                {
                    RemoveEvent(trigger, eventData);
                });
                deleteButton.text = "削除";
                deleteButton.style.width = 64f;
                deleteButton.style.marginLeft = 4f;
                row.Add(deleteButton);

                _triggerListView.Add(row);
            }
        }

        private void RefreshEditor()
        {
            _editorView.Clear();

            if (_selectedEvent != null)
            {
                BuildEventEditor(_selectedEvent);
                return;
            }

            if (_selectedTrigger != null)
            {
                BuildTriggerEditor(_selectedTrigger);
            }
        }

        private void BuildTriggerEditor(TriggerData trigger)
        {
            _editorView.Add(new Label("TriggerData"));
            _editorView.Add(new Label("id: " + trigger.Id));

            DropdownField typeField = new DropdownField("eventType", new List<string> { "Amount", "Word" }, trigger.EventType == TriggerData.EVENT_TYPE.Amount ? 0 : 1);
            _editorView.Add(typeField);

            TextField amountField = new TextField("amount");
            amountField.value = trigger.Amount.ToString("0", CultureInfo.InvariantCulture);
            _editorView.Add(amountField);

            TextField wordField = new TextField("word");
            wordField.value = trigger.Word ?? string.Empty;
            _editorView.Add(wordField);

            DropdownField findField = new DropdownField("findType", new List<string> { "Partial", "Perfect", "Prefix", "Sufix" }, FindTypeToIndex(trigger.Findtype));
            _editorView.Add(findField);

            typeField.RegisterValueChangedCallback(delegate(ChangeEvent<string> evt)
            {
                TriggerData.EVENT_TYPE nextType = evt.newValue == "Amount" ? TriggerData.EVENT_TYPE.Amount : TriggerData.EVENT_TYPE.Word;
                if (nextType != trigger.EventType)
                {
                    trigger.EventType = nextType;
                    _isDirty = true;
                    RefreshTriggerList();
                }
                UpdateTriggerFieldVisibility(trigger, amountField, wordField);
            });

            amountField.RegisterValueChangedCallback(delegate(ChangeEvent<string> evt)
            {
                int amount;
                if (int.TryParse(evt.newValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out amount) && amount >= 0)
                {
                    if (trigger.Amount != amount)
                    {
                        trigger.Amount = amount;
                        _isDirty = true;
                        RefreshTriggerList();
                    }
                    return;
                }

                amountField.SetValueWithoutNotify(trigger.Amount.ToString("0", CultureInfo.InvariantCulture));
            });

            wordField.RegisterValueChangedCallback(delegate(ChangeEvent<string> evt)
            {
                string next = evt.newValue ?? string.Empty;
                if (next != (trigger.Word ?? string.Empty))
                {
                    trigger.Word = next;
                    _isDirty = true;
                    RefreshTriggerList();
                }
            });

            findField.RegisterValueChangedCallback(delegate(ChangeEvent<string> evt)
            {
                TriggerData.FINDT_YPE nextType = ParseFindType(evt.newValue);
                if (nextType != trigger.Findtype)
                {
                    trigger.Findtype = nextType;
                    _isDirty = true;
                }
            });

            UpdateTriggerFieldVisibility(trigger, amountField, wordField);
        }

        private void BuildEventEditor(EventData eventData)
        {
            _editorView.Add(new Label("EventData"));
            _editorView.Add(new Label("id: " + eventData.ID));

            TextField pathField = new TextField("path");
            pathField.value = eventData.Path ?? string.Empty;
            _editorView.Add(pathField);

            Button pathButton = new Button(delegate
            {
                string selectedPath;
                if (TrySelectImagePath(out selectedPath))
                {
                    if (selectedPath != (eventData.Path ?? string.Empty))
                    {
                        eventData.Path = selectedPath;
                        pathField.SetValueWithoutNotify(selectedPath);
                        _isDirty = true;
                        RefreshTriggerList();
                    }
                }
            });
            pathButton.text = "ファイル選択";
            pathButton.style.width = 120f;
            _editorView.Add(pathButton);

            DropdownField prefabField = new DropdownField("prefabType", new List<string> { "FallSprite", "MoveSprite" }, eventData.PrefabType == EventData.PREFAB_TYPE.MoveSprite ? 1 : 0);
            _editorView.Add(prefabField);

            _editorView.Add(new Label("parameters"));
            EnsureEventParameters(eventData);

            TextField lifeTimeField = CreateIntegerParameterField(eventData, EventData.PARAMETER_KEY.LIFETIME, "lifeTime", false);
            TextField sizeField = CreateDecimalParameterField(eventData, EventData.PARAMETER_KEY.SIZE, "size", false, false);
            TextField posXField = CreateDecimalParameterField(eventData, EventData.PARAMETER_KEY.POSITION_X, "posX", true, true);
            TextField posYField = CreateDecimalParameterField(eventData, EventData.PARAMETER_KEY.POSITION_Y, "posY", true, true);
            TextField movXField = CreateDecimalParameterField(eventData, EventData.PARAMETER_KEY.MOVEMENT_X, "movX", true, true);
            TextField movYField = CreateDecimalParameterField(eventData, EventData.PARAMETER_KEY.MOVEMENT_Y, "movY", true, true);

            _editorView.Add(lifeTimeField);
            _editorView.Add(sizeField);
            _editorView.Add(posXField);
            _editorView.Add(posYField);
            _editorView.Add(movXField);
            _editorView.Add(movYField);

            pathField.RegisterValueChangedCallback(delegate(ChangeEvent<string> evt)
            {
                string next = evt.newValue ?? string.Empty;
                if (next != (eventData.Path ?? string.Empty))
                {
                    eventData.Path = next;
                    _isDirty = true;
                    RefreshTriggerList();
                }
            });

            prefabField.RegisterValueChangedCallback(delegate(ChangeEvent<string> evt)
            {
                EventData.PREFAB_TYPE next = evt.newValue == "MoveSprite" ? EventData.PREFAB_TYPE.MoveSprite : EventData.PREFAB_TYPE.FallSprite;
                if (eventData.PrefabType != next)
                {
                    eventData.PrefabType = next;
                    _isDirty = true;
                    RefreshTriggerList();
                }
                UpdateEventFieldVisibility(eventData, posXField, posYField, movXField, movYField);
            });

            UpdateEventFieldVisibility(eventData, posXField, posYField, movXField, movYField);
        }

        private void UpdateTriggerFieldVisibility(TriggerData trigger, VisualElement amountField, VisualElement wordField)
        {
            bool isAmount = trigger.EventType == TriggerData.EVENT_TYPE.Amount;
            amountField.style.display = isAmount ? DisplayStyle.Flex : DisplayStyle.None;
            wordField.style.display = isAmount ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void UpdateEventFieldVisibility(EventData eventData, VisualElement posXField, VisualElement posYField, VisualElement movXField, VisualElement movYField)
        {
            bool isFall = eventData.PrefabType == EventData.PREFAB_TYPE.FallSprite;
            DisplayStyle style = isFall ? DisplayStyle.None : DisplayStyle.Flex;
            posXField.style.display = style;
            posYField.style.display = style;
            movXField.style.display = style;
            movYField.style.display = style;
        }

        private TextField CreateIntegerParameterField(EventData eventData, EventData.PARAMETER_KEY key, string label, bool allowEmpty)
        {
            TextField field = new TextField(label);
            field.value = GetParameter(eventData, key);
            field.RegisterValueChangedCallback(delegate(ChangeEvent<string> evt)
            {
                string next = evt.newValue ?? string.Empty;
                if (allowEmpty && next.Length == 0)
                {
                    SetParameter(eventData, key, string.Empty);
                    return;
                }

                int parsed;
                if (int.TryParse(next, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed) && parsed >= 0)
                {
                    SetParameter(eventData, key, parsed.ToString(CultureInfo.InvariantCulture));
                    return;
                }

                field.SetValueWithoutNotify(GetParameter(eventData, key));
            });
            return field;
        }

        private TextField CreateDecimalParameterField(EventData eventData, EventData.PARAMETER_KEY key, string label, bool allowEmpty, bool allowNegative)
        {
            TextField field = new TextField(label);
            field.value = GetParameter(eventData, key);
            field.RegisterValueChangedCallback(delegate(ChangeEvent<string> evt)
            {
                string next = evt.newValue ?? string.Empty;
                if (allowEmpty && next.Length == 0)
                {
                    SetParameter(eventData, key, string.Empty);
                    return;
                }

                decimal parsed;
                if (!decimal.TryParse(next, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed))
                {
                    field.SetValueWithoutNotify(GetParameter(eventData, key));
                    return;
                }

                if (!allowNegative && parsed < 0)
                {
                    field.SetValueWithoutNotify(GetParameter(eventData, key));
                    return;
                }

                parsed = decimal.Round(parsed, 3, MidpointRounding.AwayFromZero);
                string formatted = parsed.ToString("0.###", CultureInfo.InvariantCulture);
                SetParameter(eventData, key, formatted);
                field.SetValueWithoutNotify(formatted);
            });
            return field;
        }

        private void AddEvent(TriggerData trigger)
        {
            if (trigger.EventList == null)
            {
                trigger.EventList = new List<EventData>();
            }

            EventData eventData = new EventData();
            eventData.ID = trigger.EventList.Count + 1;
            eventData.Path = string.Empty;
            eventData.PrefabType = EventData.PREFAB_TYPE.FallSprite;
            eventData.Parameters = CreateDefaultParameterMap();
            trigger.EventList.Add(eventData);

            _selectedTrigger = trigger;
            _selectedEvent = eventData;
            _isDirty = true;

            RefreshTriggerList();
            RefreshEditor();
        }

        private void DuplicateEvent(TriggerData trigger, EventData source)
        {
            if (trigger.EventList == null)
            {
                trigger.EventList = new List<EventData>();
            }

            EventData clone = new EventData();
            clone.ID = trigger.EventList.Count + 1;
            clone.Path = source.Path;
            clone.PrefabType = source.PrefabType;
            clone.Parameters = CloneParameterMap(source.Parameters);
            trigger.EventList.Add(clone);
            RenumberEvents(trigger);

            _selectedEvent = clone;
            _isDirty = true;

            RefreshTriggerList();
            RefreshEditor();
        }

        private void RemoveEvent(TriggerData trigger, EventData eventData)
        {
            if (trigger.EventList == null)
            {
                return;
            }

            trigger.EventList.Remove(eventData);
            RenumberEvents(trigger);

            if (_selectedEvent == eventData)
            {
                _selectedEvent = null;
            }

            _isDirty = true;
            RefreshTriggerList();
            RefreshEditor();
        }

        private void RemoveTrigger(TriggerData trigger)
        {
            if (trigger == null)
            {
                return;
            }

            _editingTriggerDatas.Remove(trigger);
            if (_selectedTrigger == trigger)
            {
                _selectedTrigger = null;
                _selectedEvent = null;
            }

            _pendingDeleteTrigger = null;
            _isDirty = true;
            RefreshTriggerList();
            RefreshEditor();
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

        private void EnsureEventParameters(EventData eventData)
        {
            if (eventData.Parameters == null)
            {
                eventData.Parameters = CreateDefaultParameterMap();
            }

            EnsureParameterKey(eventData, EventData.PARAMETER_KEY.LIFETIME);
            EnsureParameterKey(eventData, EventData.PARAMETER_KEY.SIZE);
            EnsureParameterKey(eventData, EventData.PARAMETER_KEY.POSITION_X);
            EnsureParameterKey(eventData, EventData.PARAMETER_KEY.POSITION_Y);
            EnsureParameterKey(eventData, EventData.PARAMETER_KEY.MOVEMENT_X);
            EnsureParameterKey(eventData, EventData.PARAMETER_KEY.MOVEMENT_Y);
        }

        private void EnsureParameterKey(EventData eventData, EventData.PARAMETER_KEY key)
        {
            string mapKey = key.ToString();
            if (!eventData.Parameters.ContainsKey(mapKey))
            {
                eventData.Parameters[mapKey] = string.Empty;
            }
        }

        private string GetParameter(EventData eventData, EventData.PARAMETER_KEY key)
        {
            EnsureEventParameters(eventData);
            return eventData.Parameters[key.ToString()] ?? string.Empty;
        }

        private void SetParameter(EventData eventData, EventData.PARAMETER_KEY key, string value)
        {
            EnsureEventParameters(eventData);
            string mapKey = key.ToString();
            string current = eventData.Parameters[mapKey] ?? string.Empty;
            string next = value ?? string.Empty;

            if (current == next)
            {
                return;
            }

            eventData.Parameters[mapKey] = next;
            _isDirty = true;
        }

        private Dictionary<string, string> CreateDefaultParameterMap()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            map[EventData.PARAMETER_KEY.LIFETIME.ToString()] = "5";
            map[EventData.PARAMETER_KEY.SIZE.ToString()] = "1";
            map[EventData.PARAMETER_KEY.POSITION_X.ToString()] = string.Empty;
            map[EventData.PARAMETER_KEY.POSITION_Y.ToString()] = string.Empty;
            map[EventData.PARAMETER_KEY.MOVEMENT_X.ToString()] = string.Empty;
            map[EventData.PARAMETER_KEY.MOVEMENT_Y.ToString()] = string.Empty;
            return map;
        }

        private Dictionary<string, string> CloneParameterMap(Dictionary<string, string> source)
        {
            Dictionary<string, string> map = CreateDefaultParameterMap();
            if (source == null)
            {
                return map;
            }

            foreach (KeyValuePair<string, string> pair in source)
            {
                map[pair.Key] = pair.Value ?? string.Empty;
            }

            return map;
        }

        private List<TriggerData> CloneTriggerDatas(List<TriggerData> source)
        {
            List<TriggerData> list = new List<TriggerData>();
            if (source == null)
            {
                return list;
            }

            foreach (TriggerData trigger in source)
            {
                TriggerData copy = new TriggerData();
                copy.Id = trigger.Id;
                copy.EventType = trigger.EventType;
                copy.Amount = trigger.Amount;
                copy.Word = trigger.Word;
                copy.Findtype = trigger.Findtype;
                copy.EventList = new List<EventData>();

                if (trigger.EventList != null)
                {
                    foreach (EventData eventData in trigger.EventList)
                    {
                        EventData eventCopy = new EventData();
                        eventCopy.ID = eventData.ID;
                        eventCopy.Path = eventData.Path;
                        eventCopy.PrefabType = eventData.PrefabType;
                        eventCopy.Parameters = CloneParameterMap(eventData.Parameters);
                        copy.EventList.Add(eventCopy);
                    }
                }

                RenumberEvents(copy);
                list.Add(copy);
            }

            return list;
        }

        private int GenerateTriggerId()
        {
            int id;
            if (!int.TryParse(DateTime.Now.ToString("MMddHHmmss", CultureInfo.InvariantCulture), out id))
            {
                id = UnityEngine.Random.Range(1, int.MaxValue - 1);
            }

            HashSet<int> usedIds = new HashSet<int>();
            foreach (TriggerData trigger in _editingTriggerDatas)
            {
                usedIds.Add(trigger.Id);
            }

            while (usedIds.Contains(id))
            {
                id += 1;
                if (id == int.MaxValue)
                {
                    id = 1;
                }
            }

            return id;
        }

        private int FindTypeToIndex(TriggerData.FINDT_YPE findType)
        {
            switch (findType)
            {
                case TriggerData.FINDT_YPE.Partial:
                    return 0;
                case TriggerData.FINDT_YPE.Perfect:
                    return 1;
                case TriggerData.FINDT_YPE.Prefix:
                    return 2;
                case TriggerData.FINDT_YPE.Sufix:
                    return 3;
                default:
                    return 0;
            }
        }

        private TriggerData.FINDT_YPE ParseFindType(string value)
        {
            switch ((value ?? string.Empty).ToLowerInvariant())
            {
                case "perfect":
                    return TriggerData.FINDT_YPE.Perfect;
                case "prefix":
                    return TriggerData.FINDT_YPE.Prefix;
                case "sufix":
                case "suffix":
                    return TriggerData.FINDT_YPE.Sufix;
                case "partial":
                default:
                    return TriggerData.FINDT_YPE.Partial;
            }
        }

        private bool TrySelectImagePath(out string selectedPath)
        {
#if UNITY_EDITOR
            selectedPath = UnityEditor.EditorUtility.OpenFilePanel("画像ファイルを選択", "", "png,jpg,jpeg");
            return !string.IsNullOrEmpty(selectedPath);
#else
            selectedPath = string.Empty;
            Debug.LogWarning("ファイル選択ダイアログは UNITY_EDITOR でのみ利用できます。");
            return false;
#endif
        }

        private void ShowModal(string message, string yesText, string noText, string cancelText, Action yesAction, Action noAction, Action cancelAction)
        {
            _modalMessageLabel.text = message;
            _modalYesButton.text = string.IsNullOrEmpty(yesText) ? "はい" : yesText;
            _modalNoButton.text = string.IsNullOrEmpty(noText) ? "いいえ" : noText;
            _modalCancelButton.text = string.IsNullOrEmpty(cancelText) ? "キャンセル" : cancelText;
            _modalCancelButton.style.display = cancelAction == null ? DisplayStyle.None : DisplayStyle.Flex;

            _modalYesAction = yesAction;
            _modalNoAction = noAction;
            _modalCancelAction = cancelAction;
            _modalOverlay.style.display = DisplayStyle.Flex;
        }

        private void HideModal()
        {
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
            _settingsLauncherButton.style.display = _isSettingsButtonVisible ? DisplayStyle.Flex : DisplayStyle.None;
            _windowRoot.style.display = _isWindowVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private bool IsLeftClickDown()
        {
#if ENABLE_INPUT_SYSTEM
            UnityEngine.InputSystem.Mouse mouse = UnityEngine.InputSystem.Mouse.current;
            if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            {
                return true;
            }
#endif
            return Input.GetMouseButtonDown(0);
        }
    }
}
