using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public enum EKindSymb { site, room }

public class SymbolPreset : MonoBehaviour
{
    public Sprite mSymbRoomEntryDp;

    public Sprite mSymbDefaultSiteDp;
    public List<IfoSymbol> mPresetsSymbSiteDp;

    public string meSelectNow => mSelectNow;
    string mSelectNow;

    Dictionary<string, IfoSymbol> mSymbols;
    Dictionary<string, int> mSymbolsPrior;

    public Sprite GetIcon(EKindSymb kind, string sign) {
        if (kind == EKindSymb.site)
        {
            if (sign != null && mSymbols.ContainsKey(sign))
                return mSymbols[sign].icon;
            else
                return mSymbDefaultSiteDp;
        }
        else if (kind == EKindSymb.room)
        {
            return mSymbRoomEntryDp;
        }
        else
            return null;
    }

    public int GetPrior(string sign) { if (sign != null && mSymbolsPrior.ContainsKey(sign))  return mSymbolsPrior[sign]; else return 0; }

    //===============================

    public static SymbolPreset It;

    void Awake()
    {
        It = this;
        mSymbols = new Dictionary<string, IfoSymbol>();
        mSymbolsPrior = new Dictionary<string, int>();
        for (int i = 0; i < mPresetsSymbSiteDp.Count; i++)
        {
            mSymbols.Add(mPresetsSymbSiteDp[i].sign, mPresetsSymbSiteDp[i]);
            mSymbolsPrior.Add(mPresetsSymbSiteDp[i].sign, i);
        }
        IfoSymbol.callback = (s) => { mSelectNow = s; };
        mSelectNow = mPresetsSymbSiteDp[0].sign;
    }
}

[System.Serializable]
public class IfoSymbol
{
    public string sign;
    [PreviewField(30)]
    [InlineButton("Select")]
    public Sprite icon;

    public static Action<string> callback;
    void Select()
    {
        if (SysGeneral.meInGame)
            callback(sign);
        else
            Debug.Log("非运行状态选择无效");
    }
}