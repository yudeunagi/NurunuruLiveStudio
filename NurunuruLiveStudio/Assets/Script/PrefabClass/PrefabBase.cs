using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unage;

/// <summary>
/// 
/// 参考
/// 【Unity】C# インターフェースを使う実例 (備忘録)
/// https://qiita.com/Nekomasu/items/fe175fbb2cd4282a0e1c
/// 
/// </summary>
public interface PrefabBase
{
    /// <summary>
    /// パラメーターセット
    /// </summary>
    /// <param name="parameters"></param>
    void SetParameters(List<string> parameters);
}
