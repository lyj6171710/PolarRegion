using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UiOption : MonoBehaviour
{//ѡ�������ڶֻ࣬����ʹ��ͬһ�ֽű�����ͬѡ��ֻ�ǲ�ͬʵ��
 //ѡ�����Ϸ����ľֲ�����
 //ѡ��ͬʱ���Դ���ѡ����л������û�رգ������ʹ�����Լ�Ԥ�ú�����
 //Ҳ����˵��UIҲ��������Ϸ������߼�����ֻ�ǽ�����Ϊ������޶�������

    [HideInInspector] public int mIndex;//����ѡ���Ѽ�ѡ��ʱ�Զ�¼��
    [HideInInspector] public UiList mSuper;//�Զ�¼��

    public UnityEvent mCallWhenPressDp;//��Ҫ�������߼���ָ
    public UiList mDivertToOtherDp;

    public void MakeSure()
    {
        if (mCallWhenPressDp != null)
            mCallWhenPressDp.Invoke();
        if (mDivertToOtherDp != null)
        {
            //mListBelong.CallOffBox();
            //mListBelong.TakeDivert(mDivertToOtherDp);
        }
    }
}
