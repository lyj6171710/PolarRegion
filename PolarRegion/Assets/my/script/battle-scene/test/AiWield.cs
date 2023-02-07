using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiWield : MonoBehaviour
{
    public ActionCanDo act;
    public FigureProfile profile;

    // Update is called once per frame
    void Update()
    {
        Vector3 cursorPos = UnifiedCursor.It.meCursorPos;

        //act.SuTakeMoveBy(Vector2.left, false);

    }
}
