using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//====================================

public interface Ifo { }//����ʱ��һ���Ը���������Ϣ��

public interface Sto { }//��Ҫ�洢���ݵĴ洢�ṹ

public interface IForIfo<T> where T : Ifo //���Խ���ת���Ѷȣ�ֻҪһ�����ÿ����Ա���̳иýӿڣ��Ϳ��Էֲ�ת�������Ҹ����ڼ��
{
    T ToIfo();//��sto��Ϣ����ifo��Ϣ����Ӧȡ��һ���¶���
}

public interface ICanSto<T> where T : Sto
{
    T ToSto();//��ifo��Ϣ����sto��Ϣ����Ӧȡ��һ���¶���
}

//====================================

public interface IGetIfo<T, F>//����ȡĳ�������Ϣ
{
    T GetIfo(F refer);
    //����ĳ����ҪF
    //��ifo��Ϣ������һ��ifo��Ϣ
}

public interface IGetIfo<T>
{
    T GetIfo();
}

//====================================