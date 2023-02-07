using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigureSkill : MonoBehaviour,ITreeBranch
{
    List<OneSkill> mSkills;

    void Ready()
    {
        mSkills = new List<OneSkill>();
        for (int i = 0; i < transform.childCount; i++)
        {
            OneSkill skill = new OneSkill();
            skill.attr = transform.GetChild(i).GetComponent<AttrSkill>();
            skill.load = transform.GetChild(i).GetComponent<SkillMoveByDir>();
            mSkills.Add(skill);
            mSkills[i].load.MakeReady(this);
        }
    }

    public void SuReleaseSkill(Vector2 to)
    {
        mSkills[0].load.SuEffect(to);
    }

    //===============================

    public TreeBranch branch;

    public object RespondReadFromUp(InfoSeal seal, out int source)
    {
        source = 0;
        return null;
    }
    public void ReceiveNotifyFromUp(InfoSeal seal)
    {

    }
    public void SelfReady(TreeBranch shelf)
    {
        branch = shelf;
        Ready();
    }

    public struct OneSkill
    {
        public AttrSkill attr;
        public SkillMoveByDir load;
    }

}
