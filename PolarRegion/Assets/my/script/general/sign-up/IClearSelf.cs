using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISignUpClearSelf
{
}

interface ISUClearSelf
{
    void DelSelfFile();//ɾ���洢�Լ���Ϣ��Ӳ���ļ�

    void DelRelaFile();//ɾ���������Լ������õ�Ӳ���ļ��е��ǲ�����Ϣ

    void DelOtherCite();//ɾ���ڴ��У��������Լ��Ķ����е��ǲ�����Ϣ

    void DelOtherUse();//�ر����й����У���Ϊ�Լ��������������Ч��
}
