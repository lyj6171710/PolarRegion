using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ThrowInitial : MonoBehaviour
{
    protected GameObject meShape;//������֮һ
    Collider2D mCollid;
    ImgSideExpress mExpress;
    protected HitTryMake mHitMake;//����������ײ�뷴Ӧ���ɸ��������

    protected GameObject meShadow;//������֮��
    protected MoveOnPlane mMoveCtrl;//���崦��ƽ���ϵ�λ��

    protected abstract bool mHavePreset { get;}

    const string cNameShadow = "shadow";
    const string cNameShape = "shape";

    protected void Build()
    {
        GameObject shape = GbjAssist.AddChildNormal(transform, cNameShape);
        SpriteRenderer renderShape = shape.AddComponent<SpriteRenderer>();
        renderShape.sprite = gameObject.GetComponent<AttrGeneral>().mDiagram;

        GameObject shadow = GbjAssist.AddChildNormal(transform, cNameShadow);
        SpriteRenderer renderShadow = shadow.AddComponent<SpriteRenderer>();
        renderShadow.sprite = Resources.Load<Sprite>(AddrResource.path_shadow);
    }

    public virtual void Ready1ForBase()
    {
        if (mHavePreset)
        {
            meShape = transform.Find(cNameShape).gameObject;
            meShape.GetComponent<SpriteRenderer>().sprite = gameObject.GetComponent<AttrGeneral>().mDiagram;
        }
        else
        {
            meShape = new GameObject(cNameShape);
            meShape.transform.SetParent(transform);
            meShape.transform.localPosition = Vector3.zero;

            SpriteRenderer renderBody = GbjAssist.AddCompSafe<SpriteRenderer>(meShape);
            renderBody.sprite = gameObject.GetComponent<AttrGeneral>().mDiagram;
        }

        mExpress = GbjAssist.AddCompSafe<ImgSideExpress>(meShape);
        mExpress.MakeReady(null);
        mExpress.ReadyCountSize();

        mCollid = GbjAssist.AddCompSafe<BoxCollider2D>(meShape);
        mCollid.isTrigger = true;

        if (mHavePreset)
        {
            meShadow = transform.Find(cNameShadow).gameObject;
            mMoveCtrl = GbjAssist.AddCompSafe<MoveOnPlane>(meShadow);
            mMoveCtrl.MakeReady(meShape.transform, GlobalConfig.layer_actor);
        }
        else
        {
            meShadow = new GameObject(cNameShadow);
            meShadow.transform.SetParent(transform);
            meShadow.transform.localPosition = Vector3.zero;
            mMoveCtrl = GbjAssist.AddCompSafe<MoveOnPlane>(meShadow);
            SpriteAssist.Ifo ifo = new SpriteAssist.Ifo();
            ifo.scaleShrinkX = 1;
            ifo.scaleShrinkY = 3;
            mMoveCtrl.MakeReady(meShape.transform, GlobalConfig.layer_actor).ReadyBuildShapeSelf(ifo);
        }
        mMoveCtrl.SuTakeLiftInstant(0.25f);
    }

    public void Ready2ForHit(bool startHit,params string[] aims)
    {
        mHitMake = meShape.AddComponent<HitTryMake>();
        mHitMake.MakeReady(GlobalConfig.layer_atk, false, startHit).ReadyAgainst(aims);
    }

}
