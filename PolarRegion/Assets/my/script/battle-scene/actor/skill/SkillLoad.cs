using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLoad : MonoBehaviour
{
    public string mBulletRefer;//技能子弹是预制体

    [HideInInspector] public AttrSkill meSkillAttr;

    protected FigureSkill mUser;
    protected bool mHaveReady;

    public virtual void MakeReady(FigureSkill user)
    {
        if (mHaveReady) return;

        mUser = user;

        meSkillAttr = GetComponent<AttrSkill>();

        //子类需要在准备后，把haveReady置true
    }
}
