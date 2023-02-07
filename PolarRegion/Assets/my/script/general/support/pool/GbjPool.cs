using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GbjPool : MonoBehaviour
{
    //����أ����������ظ����ɣ����Ͷ����ܵĺķ�

    protected ObjectPool<GameObject> mPool;

    public Transform mSojournDp;

    protected void Initial(Action<GameObject> newComp = null, List<GameObject> preBuilds = null)
    {
        //Ԥ���壬���ֶ�ȷ����״̬���ϻ�����Ҫ������Ͳ��ٷ�����ȥ����ˣ�������ЩԤ�������Ϊ��ʡ���ܶ����õ�

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
        {//��һ�������transform����������
            Destroy(comps[i]);
        }
    }

    //===================================

    public GameObject GetPure(Transform belong = null, string name = null)//�϶�����Revert����
    {
        GameObject item = mPool.GetItem();
        HowCountAsPure(item);
        //��������չ������Զ����
        item.transform.SetParent(belong);
        if (name != null) item.name = name;
        //������item���ã��͸�������������
        item.SetActive(true);
        //Debug.Log(mPool.CountCapacity);
        return item;
    }

    public void Revert(GameObject item)//��һ�λ��պ󣬲Ż�ȷ����������λ��ָ����������
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
        meInUsed = false;//��ʼʱӦΪδ��ʹ��״̬
    }

    //---------------------------------

    // ������Ӧ����
    public T Item { get; set; }

    // ������Ӧ�����Ƿ��ѱ�ʹ�õı��
    public bool meInUsed { get; private set; }

    // ���Ϊ�ѱ�ʹ��
    public void Consume()
    {
        meInUsed = true;
    }

    // ���Ϊδ��ʹ��
    public void Release()
    {
        meInUsed = false;
    }
}

public class ObjectPool<T>
{
    //��������ɻ��գ�ֻ�Ǵ�����յ�ȡ��֮����߼�������
    //���������ݣ�������о��еĶ������Ʋ���ʽ�Ĺ�ϵ

    //��ȷ��Ҫ������ȡ�á����������ֹ��̵Ĵ���
    //����Լ�Ӧ�ڵ��ø���ȡ������սӿ�ʱ����������ȡ��������ϵļӹ�(��������־������ϵͳ����ʹ�ø���)
    //��Ҫ���һ��ʼ���ͽ�����������ȷ�������࣬����Ḻ������ƴ��������Ľ���

    List<PoolPiece<T>> mPoolList;// �������ж���
    Dictionary<T, PoolPiece<T>> mInUseList;// �ѱ�ʹ�õĶ���(���ڲ�ѯ)
    Func<T> mNewOneProc;
    int mLastIndex;

    public int CountCapacity => mPoolList.Count;
    public int CountUsedItems => mInUseList.Count;

    public ObjectPool(Func<T> newOne, List<T> preBuilds = null)
    {
        // ��ί�����ڷ���һ�����´����ģ�����ʵ��
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

        // ����list���ҵõ���δ��ʹ�õĶ�����ʹ�øö�����Ϊ����ֵ
        for (int i = 0; i < mPoolList.Count; i++)
        {
            mLastIndex++;
            // �����ûʹ�õľ����ϴ�ʹ���˵���һ�������ϴ�ʹ�õ������1���������ֻҪ�ӵ�0����ʼ��
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

        // list�����ж��󶼱�ʹ���ˣ��´���һ������
        if (piece == null)
            piece = CreateOnePiece();

        // ���Ϊ��ʹ��
        piece.Consume();
        mInUseList.Add(piece.Item, piece);

        return piece.Item;
    }

    public void ReleaseItem(T item)
    {
        if (mInUseList.ContainsKey(item))
        {
            // ���Ϊδʹ��
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
