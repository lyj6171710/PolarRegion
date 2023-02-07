using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileOperate
{
    void IWhenHoverTileFmOpera(IfoTilePick pick);
}

public class FieldTile : MonoBehaviour
{
    //Խֻ������ʾЧ��������踺��������д����
    //Tile��װ����ͼ��ĸ���
    //����ͼ�㹦��
    //���Ӧ��Ԫ��󶨣�������

    IfoTilePile mPile;
    Sprite mSpriteUse;//��ǰ������ͼ���زĵ���ò

    List<SpriteRenderer> mRenders;
    ITileOperate mCallback;
    FieldGrid mGrid;

    void OnMouseEnter()
    {
        mCallback.IWhenHoverTileFmOpera(mPile.GetPick(MapPlates.It.meCurLayer));
    }

    //�������============================

    public Service me;

    public void MakeReady(ITileOperate operater, FieldGrid grid)
    {
        mCallback = operater;
        mGrid = grid;
        mPile = new IfoTilePile().Initial();

        me = new Service(this);

        mRenders = new List<SpriteRenderer>();
        mRenders.Add(GbjAssist.AddCompSafe<SpriteRenderer>(gameObject));
        NuDrawTile(mPile);
        SpriteAssist.SetCollidBySprite(GetComponent<BoxCollider2D>(), mRenders[0]);
    }

    //������============================

    public void NuDrawTile(IfoTilePile pile)
    {
        pile.Initial();
        int vary = pile.meNumLayer - mPile.meNumLayer;
        UpdateRender(vary);//���ӵ���Ҫ��
        mPile = pile;
        for (int i = 0; i < mRenders.Count; i++)
            NuDrawTileAtLayer(i, pile.GetPick(i));
    }

    public void NuDrawTileAtLayer(int layer = 0, IfoTilePick pick = default(IfoTilePick))
    {
        int vary = mPile.SetPick(layer, pick);
        UpdateRender(vary);//ȷ����Ⱦ������
        UpdateShow(layer);
    }

    public void NuEraseTile(int layer = 0)
    {
        int vary = mPile.SetPick(layer, new IfoTilePick());
        if (vary < 0)
            UpdateRender(vary);//ɾ��������Ҫ����Ⱦ��
        else
            UpdateShow(layer);
    }

    //-----------------------------------------

    void UpdateShow(int layer)
    {
        if (mRenders.Count > layer)
        {
            mSpriteUse = MapPlates.It.SuGetSprite(mPile.GetPick(layer));
            mRenders[layer].sprite = mSpriteUse;
        }
    }

    void UpdateRender(int needVary)
    {
        Method.VaryUntilZero(needVary,
            (varyRest) =>
            {
                GameObject child = new GameObject();
                child.transform.SetParent(transform);
                child.transform.localPosition = Vector3.zero;
                mRenders.Add(child.AddComponent<SpriteRenderer>());
                int last = mRenders.Count - 1;
                mRenders[last].sortingOrder = last * 2;//����2���ܷ���������ھ���֮��
                needVary--;
            },
            (varyRest) =>
            {
                int last = mRenders.Count - 1;
                Destroy(mRenders[last].gameObject);
                mRenders.RemoveAt(last);
                needVary++;
            }
            );
    }

    public class Service : Srv<FieldTile>
    {
        public Service(FieldTile belong) : base(belong)
        {
        }

        public void ShowAllLayer()
        {
            foreach (SpriteRenderer renderer in j.mRenders) renderer.enabled = true;
        }

        public void HideAllLayerShow()
        {
            foreach (SpriteRenderer renderer in j.mRenders) renderer.enabled = false;
        }

        public void PerformLayerAt(int layer)
        {
            if (j.mRenders.Count > layer) j.mRenders[layer].enabled = true;
        }

        public void OnlyPerformOneLayerAt(int layer)
        {
            HideAllLayerShow();
            PerformLayerAt(layer);
        }
    }
}
