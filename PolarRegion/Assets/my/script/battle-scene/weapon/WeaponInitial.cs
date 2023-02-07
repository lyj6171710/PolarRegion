using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponInitial : ThrowInitial
{//��ǰ���ص������ϣ���Ҫ��̬���ɣ����������Ϊ�м����

    [ContextMenuItem("Build", "Build")]
    public bool mPresetDp;
    public GameObject meWeapon { get{ return meShape; } }

    public MoveSplitByDir meMotion { get; set; }

    public AttrWeapon meAttr { get; set; }

    public HitTryMake meHit { get { return mHitMake; } }

    protected override bool mHavePreset { get { return mPresetDp; } }

    public override void Ready1ForBase()
    {
        base.Ready1ForBase();

        meAttr = GetComponent<AttrWeapon>();
    }

    AlignOtherOneRot mFollowRot;

    public WeaponInitial Ready3ForMoveByDir()
    {
        meMotion = gameObject.AddComponent<MoveSplitByDir>();
        MoveReady.IfoImg ifo = new MoveReady.IfoImg();
        ifo.hold = meAttr.mDiagram;
        ifo.angleSelf = meAttr.angleSinceRight;
        ifo.mount = meWeapon.transform;
        meMotion.MakeReady(ifo, 0);
        
        mFollowRot = meShadow.AddComponent<AlignOtherOneRot>();
        mFollowRot.MakeReady(0, meWeapon.GetComponent<ImgFitToDir>());
        meMotion.SuWhenStartMove += () => { mFollowRot.SuAlignInstantly(); };
        mMoveCtrl.ReadyConsiderDir();

        return this;
    }
}
