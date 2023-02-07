using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRegionShow : FieldWall
{
    IfoRegion mData;

    public void MakeUse(IfoRegion data)
    {
        MakeUse(data.wall);
        mData = data;
    }

    //====================================

    public string mKeyCreateRoom = "r";
    public System.Action<MapRegionShow,Vector2Int> mWhenNeedRoom;

    private void Update()
    {
        if (Input.GetKeyDown(mKeyCreateRoom))
        {
            if (mPaint != null && mPaint.meInUse)
            {
                if (mPaint.neSelect.me.IsHoverSelect)
                {
                    Vector2Int coordIn = mPaint.neSelect.me.CurSelect;
                    mWhenNeedRoom(this, coordIn);//可能是进入房间，也可能是创建房间
                }
            }
        }
    }
}
