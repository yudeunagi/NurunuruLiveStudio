using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unage
{
    /// <summary>
    /// 設定画面間で共有する UI Toolkit 補助処理を提供する。
    /// </summary>
    internal static class SettingsUiCommon
    {
        internal sealed class WindowElements
        {
            public VisualElement WindowRoot;
            public VisualElement Body;
            public VisualElement Footer;
        }

        internal sealed class ModalElements
        {
            public VisualElement Overlay;
            public Label MessageLabel;
            public VisualElement ButtonsContainer;
            public Button YesButton;
            public Button NoButton;
            public Button CancelButton;
        }

        /// <summary>
        /// UIDocument と PanelSettings を保証して返す。
        /// </summary>
        public static UIDocument EnsureDocument(MonoBehaviour owner)
        {
            UIDocument document = owner.GetComponent<UIDocument>();
            if (document == null)
            {
                document = owner.gameObject.AddComponent<UIDocument>();
            }

            if (document.panelSettings == null)
            {
                PanelSettings panelSettings = FindReusablePanelSettings(document);
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

                document.panelSettings = panelSettings;
            }

            return document;
        }

        /// <summary>
        /// 設定画面呼び出し用ランチャーボタンを生成する。
        /// </summary>
        public static Button CreateLauncherButton(string text, Action onClick, float left)
        {
            Button button = new Button(onClick);
            button.text = text;
            button.style.position = Position.Absolute;
            button.style.left = left;
            button.style.top = 10f;
            button.style.width = 60f;
            button.style.height = 30f;
            return button;
        }

        /// <summary>
        /// 設定画面の標準ウィンドウ（タイトル・本文・フッター）を生成する。
        /// </summary>
        public static WindowElements CreateWindow(VisualElement root, string title, float backgroundAlpha)
        {
            WindowElements window = new WindowElements();

            window.WindowRoot = new VisualElement();
            window.WindowRoot.style.position = Position.Absolute;
            window.WindowRoot.style.left = Length.Percent(8);
            window.WindowRoot.style.top = Length.Percent(8);
            window.WindowRoot.style.width = Length.Percent(84);
            window.WindowRoot.style.height = Length.Percent(84);
            window.WindowRoot.style.backgroundColor = new Color(1f, 1f, 1f, backgroundAlpha);
            ApplyFramedPanelStyle(window.WindowRoot, 8f);
            root.Add(window.WindowRoot);

            Label titleLabel = new Label(title);
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            titleLabel.style.fontSize = 18;
            titleLabel.style.marginBottom = 8f;
            window.WindowRoot.Add(titleLabel);

            window.Body = new VisualElement();
            window.Body.style.flexDirection = FlexDirection.Row;
            window.Body.style.flexGrow = 1f;
            window.WindowRoot.Add(window.Body);

            window.Footer = new VisualElement();
            window.Footer.style.flexDirection = FlexDirection.Row;
            window.Footer.style.justifyContent = Justify.FlexEnd;
            window.Footer.style.marginTop = 8f;
            window.WindowRoot.Add(window.Footer);

            return window;
        }

        /// <summary>
        /// 標準の確認モーダルを生成する。
        /// </summary>
        public static ModalElements CreateModal(VisualElement root, Action onYesClick, Action onNoClick, Action onCancelClick)
        {
            ModalElements modal = new ModalElements();

            modal.Overlay = new VisualElement();
            modal.Overlay.style.position = Position.Absolute;
            modal.Overlay.style.left = 0f;
            modal.Overlay.style.top = 0f;
            modal.Overlay.style.right = 0f;
            modal.Overlay.style.bottom = 0f;
            modal.Overlay.style.backgroundColor = new Color(0f, 0f, 0f, 0.4f);
            root.Add(modal.Overlay);

            VisualElement modalPanel = new VisualElement();
            modalPanel.style.position = Position.Absolute;
            modalPanel.style.left = Length.Percent(32);
            modalPanel.style.top = Length.Percent(36);
            modalPanel.style.width = Length.Percent(36);
            modalPanel.style.height = 140f;
            modalPanel.style.backgroundColor = Color.white;
            ApplyFramedPanelStyle(modalPanel, 10f);
            modal.Overlay.Add(modalPanel);

            modal.MessageLabel = new Label();
            modal.MessageLabel.style.whiteSpace = WhiteSpace.Normal;
            modal.MessageLabel.style.flexGrow = 1f;
            modalPanel.Add(modal.MessageLabel);

            modal.ButtonsContainer = new VisualElement();
            modal.ButtonsContainer.style.flexDirection = FlexDirection.Row;
            modal.ButtonsContainer.style.justifyContent = Justify.FlexEnd;
            modalPanel.Add(modal.ButtonsContainer);

            modal.YesButton = new Button(onYesClick);
            modal.YesButton.text = "はい";
            modal.YesButton.style.width = 70f;
            modal.YesButton.style.marginRight = 6f;
            modal.ButtonsContainer.Add(modal.YesButton);

            modal.NoButton = new Button(onNoClick);
            modal.NoButton.text = "いいえ";
            modal.NoButton.style.width = 70f;
            modal.NoButton.style.marginRight = 6f;
            modal.ButtonsContainer.Add(modal.NoButton);

            modal.CancelButton = new Button(onCancelClick);
            modal.CancelButton.text = "キャンセル";
            modal.CancelButton.style.width = 100f;
            modal.ButtonsContainer.Add(modal.CancelButton);

            return modal;
        }

        /// <summary>
        /// TextField の見た目を明示し、入力文字の視認性を統一する。
        /// </summary>
        public static void ApplyTextFieldStyle(TextField field)
        {
            field.style.color = Color.black;
            field.style.unityTextAlign = TextAnchor.MiddleLeft;

            VisualElement input = field.Q("unity-text-input");
            if (input != null)
            {
                input.style.color = Color.black;
                input.style.backgroundColor = Color.white;
            }
        }

        private static void ApplyFramedPanelStyle(VisualElement panel, float padding)
        {
            panel.style.color = Color.black;
            panel.style.borderBottomColor = Color.gray;
            panel.style.borderTopColor = Color.gray;
            panel.style.borderLeftColor = Color.gray;
            panel.style.borderRightColor = Color.gray;
            panel.style.borderBottomWidth = 1f;
            panel.style.borderTopWidth = 1f;
            panel.style.borderLeftWidth = 1f;
            panel.style.borderRightWidth = 1f;
            panel.style.paddingLeft = padding;
            panel.style.paddingRight = padding;
            panel.style.paddingTop = padding;
            panel.style.paddingBottom = padding;
        }

        private static PanelSettings FindReusablePanelSettings(UIDocument currentDocument)
        {
            UIDocument[] sceneDocuments = UnityEngine.Object.FindObjectsByType<UIDocument>(FindObjectsSortMode.None);
            for (int i = 0; i < sceneDocuments.Length; i++)
            {
                UIDocument document = sceneDocuments[i];
                if (document == null || document == currentDocument)
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
                if (document == null || document == currentDocument)
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

        private static ThemeStyleSheet TryResolveThemeStyleSheet()
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
    }
}
