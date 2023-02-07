using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MapRoomShow : FieldWall
{
    IfoRoom mData;

    public void MakeUse(IfoRoom data)
    {
        MakeUse(data.wall);
        mData = data;
    }

    //====================================

    const string mKeyCreateRoom = "r";
    public System.Action<Vector2Int> mWhenNeedRoom;

    private void Update()
    {
        if (Input.GetKeyDown(mKeyCreateRoom))
        {
            if (mPaint != null && mPaint.meInUse)
            {
                if (mPaint.neSelect.me.IsHoverSelect)
                {
                    Vector2Int CoordIn = mPaint.neSelect.me.CurSelect;
                    mWhenNeedRoom(CoordIn);//可能是进入房间，也可能是创建房间
                }
            }
        }
    }
}
