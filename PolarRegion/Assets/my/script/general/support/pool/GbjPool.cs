using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GbjPool : MonoBehaviour
{
    //物体池，用来避免重复生成，降低对性能的耗费

    protected ObjectPool<GameObject> mPool;

    public Transform mSojournDp;

    protected void Initial(Action<GameObject> newComp = null, List<GameObject> preBuilds = null)
    {
        //预构体，需手动确保其状态符合机制需要，这里就不再费性能去检查了，本来这些预构体就是为了省性能而设置的

        Func<GameObject> newOne;
        if (newComp != null)
        {
            newOne = () =>
            {
                GameObject item = new GameObject("gbj-piece");
                newComp(item);
                return item;
            };
        }
        else
        {
            newOne = () => new GameObject("gbj-piece");
        }
        mPool = new ObjectPool<GameObject>(newOne, preBuilds);
    }

    //================================

    protected virtual void HowCountAsPure(GameObject item)
    {
        GbjAssist.ResetTransform(item.transform, false);
    }

    protected virtual void HowRecovery(GameObject item)
    {
        Component[] comps = item.GetComponents<Component>();
        for (int i = 1; i < comps.Length; i++)
        {//第一个组件是transform，不能销毁
            Destroy(comps[i]);
        }
    }

    //===================================

    public GameObject GetPure(Transform belong = null, string name = null)//肯定先于Revert调用
    {
        GameObject item = mPool.GetItem();
        HowCountAsPure(item);
        //创建与回收过程是自定义的
        item.transform.SetParent(belong);
        if (name != null) item.name = name;
        //这里有item引用，就更方便把名字设好
        item.SetActive(true);
        //Debug.Log(mPool.CountCapacity);
        return item;
    }

    public void Revert(GameObject item)//第一次回收后，才会确保闲置物体位于指定父物体下
    {
        if (item == null) return;
        item.SetActive(false);
        item.transform.SetParent(mSojournDp);
        GbjAssist.ClearChilds(item.transform);
        HowRecovery(item);
        mPool.ReleaseItem(item);
    }

}

//==============================================

public class PoolPiece<T>
{
    public PoolPiece(T item)
    {
        Item = item;
        meInUsed = false;//初始时应为未被使用状态
    }

    //---------------------------------

    // 容器对应对象
    public T Item { get; set; }

    // 容器对应对象是否已被使用的标记
    public bool meInUsed { get; private set; }

    // 标记为已被使用
    public void Consume()
    {
        meInUsed = true;
    }

    // 标记为未被使用
    public void Release()
    {
        meInUsed = false;
    }
}

public class ObjectPool<T>
{
    //并不能完成回收，只是处理回收到取得之间的逻辑与数据
    //所处理数据，与池子中具有的对象，类似并行式的关系

    //明确需要创建、取得、回收这三种过程的存在
    //外界自己应在调用该类取得与回收接口时，主动进行取得与回收上的加工(配合外界各种具体池子系统，来使用该类)
    //需要外界一开始，就将创建过程明确传给该类，该类会负责与控制创建工作的进行

    List<PoolPiece<T>> mPoolList;// 池中所有对象
    Dictionary<T, PoolPiece<T>> mInUseList;// 已被使用的对象(用于查询)
    Func<T> mNewOneProc;
    int mLastIndex;

    public int CountCapacity => mPoolList.Count;
    public int CountUsedItems => mInUseList.Count;

    public ObjectPool(Func<T> newOne, List<T> preBuilds = null)
    {
        // 该委托用于返回一个（新创建的）对象实例
        mNewOneProc = newOne;

        mPoolList = new List<PoolPiece<T>>();
        mInUseList = new Dictionary<T, PoolPiece<T>>();

        if (preBuilds != null)
            foreach (T item in preBuilds)
                AcceptToPiece(item);
    }

    //--------------------------------

    public T GetItem()
    {
        PoolPiece<T> piece = null;

        // 若在list中找得到还未被使用的对象，则使用该对象作为返回值
        for (int i = 0; i < mPoolList.Count; i++)
        {
            mLastIndex++;
            // 最可能没使用的就是上次使用了的下一个。若上次使用的是最后1个，则这次只要从第0个开始查
            if (mLastIndex > mPoolList.Count - 1)
                mLastIndex = 0;

            if (mPoolList[mLastIndex].meInUsed)
                continue;
            else
            {
                piece = mPoolList[mLastIndex];
                break;
            }
        }

        // list中所有对象都被使用了，新创建一个对象
        if (piece == null)
            piece = CreateOnePiece();

        // 标记为已使用
        piece.Consume();
        mInUseList.Add(piece.Item, piece);

        return piece.Item;
    }

    public void ReleaseItem(T item)
    {
        if (mInUseList.ContainsKey(item))
        {
            // 标记为未使用
            PoolPiece<T> piece = mInUseList[item];
            piece.Release();
            mInUseList.Remove(item);
        }
    }

    //-------------------------

    PoolPiece<T> CreateOnePiece() => AcceptToPiece(mNewOneProc());

    PoolPiece<T> AcceptToPiece(T newOne)
    {
        PoolPiece<T> container = new PoolPiece<T>(newOne);
        mPoolList.Add(container);
        return container;
    }
}
