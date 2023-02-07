using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileOperate
{
    void IWhenHoverTileFmOpera(IfoTilePick pick);
}

public class FieldTile : MonoBehaviour
{
    //越只起表达显示效果，外界需负责给予与改写数据
    //Tile是装载有图块的格子
    //包含图层功能
    //与对应单元格绑定，不复用

    IfoTilePile mPile;
    Sprite mSpriteUse;//当前所承载图块素材的样貌

    List<SpriteRenderer> mRenders;
    ITileOperate mCallback;
    FieldGrid mGrid;

    void OnMouseEnter()
    {
        mCallback.IWhenHoverTileFmOpera(mPile.GetPick(MapPlates.It.meCurLayer));
    }

    //内外机制============================

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

    //外界可用============================

    public void NuDrawTile(IfoTilePile pile)
    {
        pile.Initial();
        int vary = pile.meNumLayer - mPile.meNumLayer;
        UpdateRender(vary);//增加到需要量
        mPile = pile;
        for (int i = 0; i < mRenders.Count; i++)
            NuDrawTileAtLayer(i, pile.GetPick(i));
    }

    public void NuDrawTileAtLayer(int layer = 0, IfoTilePick pick = default(IfoTilePick))
    {
        int vary = mPile.SetPick(layer, pick);
        UpdateRender(vary);//确保渲染器够用
        UpdateShow(layer);
    }

    public void NuEraseTile(int layer = 0)
    {
        int vary = mPile.SetPick(layer, new IfoTilePick());
        if (vary < 0)
            UpdateRender(vary);//删除不再需要的渲染器
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
                mRenders[last].sortingOrder = last * 2;//乘以2，能方便人物介于景物之间
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
