using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRespondPart : MonoBehaviour
{
    //������ײ�����������ڶ����ײ�壬����ͬһ��������Ӧ
    //����������ص�����ײ��������ϣ�Ȼ��Ὣ��ײ�����ָ������������Ӧ��

    public GameObject meBelongGbj { get { return mBelong.gameObject; } }

    HitRespond mBelong;//�ڲ�ֱ�������������
    public Collider2D neCollid { get; set; }//�Լ��϶�������һ����ײ�壬���ﱣ������

    Rigidbody2D mRigid;//�����ʱ��������Ч��
    //��Ȼ������ײ��һ��������ײǱ����������ײ�ߣ���һ�����Լ��ܵ���ײ
    //��������ײ������װ���������࣬����ѹ������Ը������ײ�ߵĸ����ֲ���װ
    
    public void MakeReady(HitRespond belong)
    {
        mBelong = belong;

        neCollid = GetComponent<Collider2D>();

        mRigid = GbjAssist.AddCompSafe<Rigidbody2D>(gameObject);
        mRigid.gravityScale = 0;
    }

    public bool MakeHit(HitTryMake comer)
    {
        return mBelong.MakeHit(comer, this);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {//ûʹ��Enter������Ϊ�������һ�����֣����ܾ��Ѿ��ڴ������У��ͻ�������
        mBelong.OnTriggerIn(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        mBelong.OnTriggerOut(collision);
    }

}
