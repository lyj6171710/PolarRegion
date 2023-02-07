using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExprNumItem : ExprReferInt,ExprNumItem.INowAble
{
    public interface INowAble
    {
        public List<GameObject> GetUsing();
    }

    public GameObject[] toHide;

    public List<GameObject> GetUsing()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < mNumNow; i++) list.Add(toHide[i]);
        return list;
    }

    //=================================

    int mNumNow;

    protected override void WhenIntChange(int state)
    {
        int indexLast = mNumNow - 1;//保留可能负数的情况，用来区分有选项和无选项
        mNumNow = MathNum.Clamp(state, 0, toHide.Length);//0是数量底线
        int indexNow = mNumNow - 1;

        Method.VaryToAndWithDo(indexLast, indexNow,
            (index) => {
                if (index < 0) return;//无对应选项
                toHide[index].SetActive(true);
            },
            (index) => {
                if (index < 0) return;//无对应选项
                toHide[index].SetActive(false);
            },
            (dir) => { if (dir && indexNow >= 0) toHide[indexNow].SetActive(true); }
        );
    }

    protected override void MakeReady()
    {
        base.MakeReady();

        NuWhenEnable += () =>
        {
            foreach (GameObject entity in toHide)
                entity.SetActive(true);
            mNumNow = toHide.Length;
            WhenIntChange(meInt);
        };
    }
}
