using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridOperate
{
    void IWhenHoverGridFmOpera(Vector2Int coord);
}

public interface IGridIfo
{
    Vector2Int iCoordInField { get; }
}

public class FieldGrid : MonoBehaviour,IGridIfo
{
    public Vector2Int iCoordInField => mCoord;
    Vector2Int mCoord; //相对场地，自己的位置

    BoxCollider2D collid;
    IGridOperate callback;

    private void Awake()
    {
        collid = GbjAssist.AddCompSafe<BoxCollider2D>(gameObject);
        collid.isTrigger = true;
    }

    void OnMouseEnter()
    {
        callback.IWhenHoverGridFmOpera(mCoord);
    }

    public void MakeReady(Vector2Int coord, IGridOperate operater)
    {
        mCoord = coord;
        callback = operater;
    }

}
