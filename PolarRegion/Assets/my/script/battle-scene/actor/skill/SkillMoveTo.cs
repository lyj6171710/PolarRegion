using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMoveTo :SkillLoad
{
    public override void MakeReady(FigureSkill user)
    {
        base.MakeReady(user);

        mHaveReady = true;
    }
}
