using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ERoomMsg { del };

public enum ERoomType { root, trunk };

public abstract class MapTreeRoom : MonoBehaviour, ISignUpRoomRoot, ISignUpRead
{
    public int SignRoomRoot => throw new System.NotImplementedException();

    public int SignRead => throw new System.NotImplementedException();

    public Dictionary<Vector2Int, MapTreeRoom>.KeyCollection meRooms => mRooms.Keys;

    //============================

    protected Dictionary<Vector2Int, MapTreeRoom> mRooms;

    protected MapTreeRoom mRoot;

    protected List<Vector2Int> mRoute;

    public bool mHaveReady { get; protected set; }

    protected virtual void ReadReady(MapTreeRoom root, List<Vector2Int> route) 
    {
        //根主动调用该函数，叶子被动调用该函数

        mRoot = root;
        mRoute = route.CopyValueToNew();
        mRooms = new Dictionary<Vector2Int, MapTreeRoom>();
    }

    protected abstract void FormSubHave(Vector2Int coord);

    protected MapTreeRoom AccessRoom(List<Vector2Int> route, bool force = false)
    {
        if (route.Count == 0) return mRoot;
        else if (route.Count == mRoute.Count) return this;//已抵终点

        Vector2Int at = route.GetFirstDifferFromLeft(mRoute);
        if (at == Vector2Int.zero){ Debug.LogError("访问未知区域"); return null; }

        if (mRooms.ContainsKey(at))
        {
            if (!mRooms[at].mHaveReady)
            {
                mRooms[at].ReadReady(mRoot, mRoute.CopyValueToNew().SelfAddLast(at));
            }
            return mRooms[at].AccessRoom(route, force);
        }
        else
        {
            if (force)
            {
                FormSubHave(at);
                return AccessRoom(route, force);
            }
            else { Debug.LogError("访问未知区域"); return null; }
        }

    }

    protected void BackFromRoom(Vector2Int at, ERoomMsg msg)
    {
        MapTreeRoom super = AccessRoom(mRoute.NewRemoveLast());
        if (msg == ERoomMsg.del) super.mRooms.Remove(at);
    }
}