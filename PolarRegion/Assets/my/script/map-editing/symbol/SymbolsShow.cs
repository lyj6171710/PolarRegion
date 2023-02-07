using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolsShow : MonoBehaviour
{
    //�Ѿ���������Ƭ�ϣ�һ�������ϣ�������Ƭ���Ƴ������򲻻ᱻж��

    //ֻ������ʾ���ݣ����ݵı䶯��Ӧ����縺��

    public EKindSymb meKind => mKind;
    public bool meHaveReady => mInReady;

    [HideInInspector]
    public IfoSymbsEachUnit mSymbsList;
    EKindSymb mKind;//�����������ʾ��һ����
    
    Transform mGather;//Ӧ�ø����Ӧ��Ҫ��������Ƭ��һ���������ϣ��������������Ǹ���ǵĸ�����
    List<GameObject> mShows;

    int mDivideNum;//������ƽ���ֳɼ���
    Vector2 mSizeUnit;//ʵ��ռ��
    Vector2 mSizeDivide;
    Vector2 mScaleSelf;

    bool mInReady;

    public void MakeReady(EKindSymb kind, int divide = 3)
    {
        mKind = kind;//�����ϣ�kind���������Ĵ��������ﲻ���ٸı���

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
        //ͼ����ʾ�ϵ�˳������紫������ǰ�Ͱ������������
        for (int i = 0; i < mSymbsList.symbs.Count; i++)
        {
            if (i == mDivideNum * mDivideNum)
                //�������ʾ����������ʾ�����ı�־����ʾ����
                //��ʱ����ֱ�Ӳ鿴��Ӧ��Ƭ��״̬
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
            Destroy(mGather.gameObject);//��ʱ��
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
        render.sortingOrder = MapRefer.cLayerForeGround;//ͼ������������ʾ
        render.color = Color.white;
        FitScale(symbol.transform, render.sprite);
        return symbol;
    }

    void FitScale(Transform symbol, Sprite icon)
    {
        Vector2 needSize = SpriteAssist.GetSizeInScene(icon);
        symbol.localScale = mSizeDivide / needSize / mScaleSelf;
    }

    void FitPos(Transform symbol, Vector2Int coord) //ÿһ�ݵ���������
    {
        Vector2 corner = mSizeUnit / 2 * mScaleSelf;
        Vector2 offset = mSizeDivide / 2 * mScaleSelf;
        Vector2 startLD = new Vector2(-corner.x, corner.y) + new Vector2(offset.x, -offset.y);
        Vector2 coordPos = startLD + 2 * new Vector2(coord.x * offset.x, -coord.y * offset.y);
        symbol.transform.position = coordPos.ToVector3() + transform.position;
    }

    Vector2Int GetFitCoord(int index) //ͼ��ᱻ�����ң����ϵ����Զ�����
    {
        int x = index % mDivideNum;
        int y = index / mDivideNum;
        return new Vector2Int(x, y);
    }

    struct Format
    {
        public int divideNum;//������ƽ���ֳɼ���
        public  Vector2 sizeUnit;//ʵ��ռ��
        public Vector2 sizeDivide;
        public Vector2 scaleSelf;
    }
}
