using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PerformChoose : MonoBehaviour
{
    //����UI����Ч������ǿ

    //��̬Ч�������ʹ�ã�Ӧ������ʱ���ӣ�Ҳ������ǰԤ�á�
    //����ڲ�Ч�������ã��Բ���Ϊ�ӿڡ�

    //��ʱʱ����������������
    //Ԥ��ʱ������ϵ�Ԥ��������Ϊ����
    
    protected bool mHaveReady;

    void Start()
    {
        if (!mHaveReady)//startʱ����δ׼��������˵�������ͨ��Ԥ�÷�ʽ��ʹ�õ�
        {
            MakeReadyWhenAsPreset();
            //mHaveReady = true;//�����޷�Ԥ�õ����ݣ�
            //��ô��ֻ��ͨ���������ⲿ�ֲ�������ɶԸö�Ч���������
        }
    }

    protected abstract void MakeReadyWhenAsPreset();
}
