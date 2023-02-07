using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWield : MonoBehaviour
{
    public ActionCanDo act;
    public FigureProfile profile;
    public FigureSkill skill;

    // Update is called once per frame
    void Update()
    {
        Vector3 cursorPos = UnifiedCursor.It.meCursorPos;
        profile.SuSwitchFaceTo(cursorPos.x - transform.position.x);

        if (UnifiedInput.It.meIsMoving())
        {
            act.SuTakeMoveBy(UnifiedInput.It.meMoveDir, false);
        }

        if (UnifiedInput.It.meTapConfirm())
        {
            //act.SuTakeLowAtk(cursorPos - transform.position);
            skill.SuReleaseSkill(cursorPos - transform.position);
        }

        if (UnifiedInput.It.SuWhenInPressJust("x"))
        {
            act.SuStopMove();
        }

        if (UnifiedInput.It.SuWhenInPressJust("space"))
        {
            act.SuTakeJump();
        }

    }

    float angleAcc;
}
