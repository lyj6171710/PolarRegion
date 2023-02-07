using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMoveByDir : SkillLoad
{//�����ӵ�����ͬʱ������������Բ���Ҫά��

    List<PresetTrack> mTracks;//�����ӵ��Ĺ켣��Ԥ��

    public override void MakeReady(FigureSkill user)
    {
        base.MakeReady(user);

        mTracks = new List<PresetTrack>();
        for (int i = 0; i < transform.childCount; i++)
        {
            mTracks.Add(transform.GetChild(i).GetComponent<PresetTrack>());
            mTracks[i].MakeReady();
        }

        mHaveReady = true;
    }

    public void SuEffect(Vector2 to)
    {
        for (int i = 0; i < mTracks.Count; i++)
        {
            GameObject entity;
            if (meSkillAttr.mMechanismDp.moveLocal)
                entity = DataLibrary.It.SuGetThing(mBulletRefer, transform);
            else
                entity = DataLibrary.It.SuGetThing(mBulletRefer, null);

            //��������������ص����ݣ������������𴫵�
            
            BulletInitial bullet = entity.GetComponent<BulletInitial>();

            bullet.Ready1ForBase();

            string against = (mUser.branch.meSuper.Su<FigureInitial>().mCamp
                == ECamp.amity) ? GlobalConfig.sign_enemy : GlobalConfig.sign_amity;
            bullet.Ready2ForHit(true, against);

            bullet.Ready31ForMoveByPreset(mTracks[i], () => { Destroy(entity); });
            MoveReady.Ifo ifo = new MoveReady.Ifo();
            ifo.dir = to;
            ifo.speed = 1;
            if (meSkillAttr.mMechanismDp.moveLocal)
                bullet.Ready32ForMoveByLocal(ifo);
            else
                bullet.Ready32ForMoveByWorld(ifo, transform.position);

            //��������ص���Ϊ���������Լ�����

            SkillActionBase action = bullet.meBullet.AddComponent<SkillActionBase>();
            action.Ready(bullet.GetComponent<AttrSkillBody>(), mUser.branch.SuLeader<FigureProfile>());

            FromOrBelong from = bullet.meBullet.AddComponent<FromOrBelong>();
            from.fromGOjbect = mUser.branch.SuFind<ActionBalance>(EFgrNode.balance).gameObject;
            from.fromTrans = transform;
            from.fromMono = this;
        }
    }
}
