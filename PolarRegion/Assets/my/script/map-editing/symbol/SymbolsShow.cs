using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolsShow : MonoBehaviour
{
    //已经挂载在瓦片上，一旦挂载上，除非瓦片被移除，否则不会被卸载

    //只负责显示数据，数据的变动，应由外界负责

    public EKindSymb meKind => mKind;
    public bool meHaveReady => mInReady;

    [HideInInspector]
    public IfoSymbsEachUnit mSymbsList;
    EKindSymb mKind;//该组件用于显示哪一类标记
    
    Transform mGather;//应该各标记应该要集中在瓦片的一个子物体上，而该子物体又是各标记的父物体
    List<GameObject> mShows;

    int mDivideNum;//将区域平均分成几份
    Vector2 mSizeUnit;//实际占长
    Vector2 mSizeDivide;
    Vector2 mScaleSelf;

    bool mInReady;

    public void MakeReady(EKindSymb kind, int divide = 3)
    {
        mKind = kind;//机制上，kind在这个组件的存在周期里不会再改变了

        mDivideNum = divide;
        mSizeUnit = FieldAssist.GetSizeCover(transform);
        mSizeDivide = mSizeUnit / mDivideNum;

        mScaleSelf = GbjAssist.GetSumScaleWhenSelf(transform);

        mShows = new List<GameObject>();

        mGather = new GameObject("symbs-" + mKind.ToString()).transform;
        mGather.SetParent(transform);

        mInReady = true;
    }

    public void MakeUse(IfoSymbsEachUnit symbsList)
    {
        mSymbsList = symbsList;
        //图标显示上的顺序，请外界传入数据前就按其需求排序好
        for (int i = 0; i < mSymbsList.symbs.Count; i++)
        {
            if (i == mDivideNum * mDivideNum)
                //有最大显示量，超过显示数量的标志不显示出来
                //此时建议直接查看相应瓦片的状态
                break;

            GameObject one = ProduceOneSymbol(mSymbsList.symbs[i].sign);
            FitPos(one.transform, GetFitCoord(i));
            mShows.Add(one);
        }
    }

    public void MakeDown()
    {
        foreach (GameObject one in mShows) Destroy(one);
    }

    public void MakeRemove()
    {
        if (mGather != null)
            Destroy(mGather.gameObject);//暂时的
        Destroy(this);
    }

    //===========================

    public void UpdateShow(IfoSymbsEachUnit symbsList)
    {
        MakeDown();
        MakeUse(symbsList);
    }

    //===========================

    GameObject ProduceOneSymbol(string sign)
    {
        GameObject symbol = new GameObject("symbol");
        symbol.transform.SetParent(mGather);
        SpriteRenderer render = symbol.AddComponent<SpriteRenderer>();
        render.sprite = SymbolPreset.It.GetIcon(mKind, sign);
        render.sortingOrder = MapRefer.cLayerForeGround;//图标总是优先显示
        render.color = Color.white;
        FitScale(symbol.transform, render.sprite);
        return symbol;
    }

    void FitScale(Transform symbol, Sprite icon)
    {
        Vector2 needSize = SpriteAssist.GetSizeInScene(icon);
        symbol.localScale = mSizeDivide / needSize / mScaleSelf;
    }

    void FitPos(Transform symbol, Vector2Int coord) //每一份的中心坐标
    {
        Vector2 corner = mSizeUnit / 2 * mScaleSelf;
        Vector2 offset = mSizeDivide / 2 * mScaleSelf;
        Vector2 startLD = new Vector2(-corner.x, corner.y) + new Vector2(offset.x, -offset.y);
        Vector2 coordPos = startLD + 2 * new Vector2(coord.x * offset.x, -coord.y * offset.y);
        symbol.transform.position = coordPos.ToVector3() + transform.position;
    }

    Vector2Int GetFitCoord(int index) //图标会被从左到右，从上到下自动排列
    {
        int x = index % mDivideNum;
        int y = index / mDivideNum;
        return new Vector2Int(x, y);
    }

    struct Format
    {
        public int divideNum;//将区域平均分成几份
        public  Vector2 sizeUnit;//实际占长
        public Vector2 sizeDivide;
        public Vector2 scaleSelf;
    }
}
