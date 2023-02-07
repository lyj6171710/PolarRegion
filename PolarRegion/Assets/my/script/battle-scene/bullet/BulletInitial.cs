using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletInitial : ThrowInitial
{//提前挂载到弹体上，不要动态生成，将该组件作为中间介质

    [ContextMenuItem("Build", "Build")]
    public bool mPresetDp;

    AttrSkillBody mAttr;

    public GameObject meBullet { get { return meShape; } }//子物体之一

    protected override bool mHavePreset { get { return mPresetDp; } }

    public override void Ready1ForBase()//外界应依次调用，完成准备工作
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
    {//紧接调用
        mMotion.SuMakeLaunchByWorld(ifo, start);
        mFollowRot.SuAlignInstantly();
    }

    public void Ready32ForMoveByLocal(MoveReady.Ifo ifo)
    {
        mMotion.SuMakeLaunchByLocal(ifo);
        mFollowRot.SuAlignInstantly();
    }
}
