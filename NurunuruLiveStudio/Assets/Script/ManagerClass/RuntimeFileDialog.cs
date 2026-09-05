using System;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace Unage
{
    /// <summary>
    /// Windowsランタイム向けのファイル選択ダイアログを提供する。
    /// </summary>
    internal static class RuntimeFileDialog
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        /// <summary>
        /// 画像ファイル選択ダイアログを表示する。
        /// </summary>
        public static bool TrySelectImageFile(out string selectedPath)
        {
            selectedPath = string.Empty;

            Type dialogType = Type.GetType("System.Windows.Forms.OpenFileDialog, System.Windows.Forms");
            Type dialogResultType = Type.GetType("System.Windows.Forms.DialogResult, System.Windows.Forms");
            if (dialogType == null || dialogResultType == null)
            {
                Debug.LogError("System.Windows.Forms が見つからないため、ファイル選択ダイアログを表示できません。");
                return false;
            }

            ConstructorInfo constructor = dialogType.GetConstructor(Type.EmptyTypes);
            PropertyInfo titleProperty = dialogType.GetProperty("Title");
            PropertyInfo filterProperty = dialogType.GetProperty("Filter");
            PropertyInfo fileNameProperty = dialogType.GetProperty("FileName");
            PropertyInfo checkFileExistsProperty = dialogType.GetProperty("CheckFileExists");
            PropertyInfo checkPathExistsProperty = dialogType.GetProperty("CheckPathExists");
            MethodInfo showDialogMethod = dialogType.GetMethod("ShowDialog", Type.EmptyTypes);
            object okResult = Enum.Parse(dialogResultType, "OK");

            if (constructor == null || titleProperty == null || filterProperty == null || fileNameProperty == null ||
                checkFileExistsProperty == null || checkPathExistsProperty == null || showDialogMethod == null)
            {
                Debug.LogError("OpenFileDialog の必要メンバーを解決できませんでした。");
                return false;
            }

            string resolvedPath = string.Empty;
            bool isAccepted = false;
            Exception dialogException = null;

            Thread dialogThread = new Thread(delegate ()
            {
                object dialog = null;
                try
                {
                    dialog = constructor.Invoke(null);
                    titleProperty.SetValue(dialog, "画像ファイルを選択", null);
                    filterProperty.SetValue(dialog, "画像ファイル (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|すべてのファイル (*.*)|*.*", null);
                    checkFileExistsProperty.SetValue(dialog, true, null);
                    checkPathExistsProperty.SetValue(dialog, true, null);

                    object result = showDialogMethod.Invoke(dialog, null);
                    if (Equals(result, okResult))
                    {
                        string fileName = fileNameProperty.GetValue(dialog, null) as string;
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            resolvedPath = fileName;
                            isAccepted = true;
                        }
                    }
                }
                catch (TargetInvocationException ex)
                {
                    dialogException = ex.InnerException ?? ex;
                }
                catch (MemberAccessException ex)
                {
                    dialogException = ex;
                }
                catch (ArgumentException ex)
                {
                    dialogException = ex;
                }
                finally
                {
                    IDisposable disposable = dialog as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
            });

            dialogThread.SetApartmentState(ApartmentState.STA);
            dialogThread.Start();
            dialogThread.Join();

            if (dialogException != null)
            {
                Debug.LogError("OpenFileDialog の表示に失敗しました: " + dialogException.Message);
                return false;
            }

            if (!isAccepted)
            {
                return false;
            }

            selectedPath = resolvedPath;
            return true;
        }
#else
        /// <summary>
        /// 非対応プラットフォーム向けのスタブ実装。
        /// </summary>
        public static bool TrySelectImageFile(out string selectedPath)
        {
            selectedPath = string.Empty;
            return false;
        }
#endif
    }
}
