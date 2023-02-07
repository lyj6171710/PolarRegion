using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillLoad : MonoBehaviour
{
    public string mBulletRefer;//�����ӵ���Ԥ����

    [HideInInspector] public AttrSkill meSkillAttr;

    protected FigureSkill mUser;
    protected bool mHaveReady;

    public virtual void MakeReady(FigureSkill user)
    {
        if (mHaveReady) return;

        mUser = user;

        meSkillAttr = GetComponent<AttrSkill>();

        //������Ҫ��׼���󣬰�haveReady��true
    }
}
