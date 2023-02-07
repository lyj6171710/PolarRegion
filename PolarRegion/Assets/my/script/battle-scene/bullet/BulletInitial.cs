using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletInitial : ThrowInitial
{//��ǰ���ص������ϣ���Ҫ��̬���ɣ����������Ϊ�м����

    [ContextMenuItem("Build", "Build")]
    public bool mPresetDp;

    AttrSkillBody mAttr;

    public GameObject meBullet { get { return meShape; } }//������֮һ

    protected override bool mHavePreset { get { return mPresetDp; } }

    public override void Ready1ForBase()//���Ӧ���ε��ã����׼������
    {
        base.Ready1ForBase();

        mAttr = GetComponent<AttrSkillBody>();
    }

    MoveByTrackPreset mMotion;
    AlignOtherOneRot mFollowRot;

    public BulletInitial Ready31ForMoveByPreset(PresetTrack track, Action WhenFinish)
    {
        mMotion = gameObject.AddComponent<MoveByTrackPreset>();
        MoveByTrackPreset.Ifo ifoApply = new MoveByTrackPreset.Ifo();
        ifoApply.posApply = transform;
        ifoApply.scaleApply = transform;
        ifoApply.rotApply = meShadow.transform;
        mMotion.MakeReady(0, track, ifoApply);
        mMotion.SuWhenFinish = WhenFinish;

        mMoveCtrl.ReadyConsiderDir();

        mFollowRot = meBullet.AddComponent<AlignOtherOneRot>();
        mFollowRot.MakeReady(mAttr.angleSinceRight, meShadow.GetComponent<ImgFitToDir>());
        
        return this;
    }

    public void Ready32ForMoveByWorld(MoveReady.Ifo ifo, Vector3 start)
    {//���ӵ���
        mMotion.SuMakeLaunchByWorld(ifo, start);
        mFollowRot.SuAlignInstantly();
    }

    public void Ready32ForMoveByLocal(MoveReady.Ifo ifo)
    {
        mMotion.SuMakeLaunchByLocal(ifo);
        mFollowRot.SuAlignInstantly();
    }
}
