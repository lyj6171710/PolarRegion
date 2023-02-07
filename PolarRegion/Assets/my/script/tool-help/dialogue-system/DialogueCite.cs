using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class DataTalkOne
{
    public string name;
    public string speech;
    [PreviewField] public Sprite head;//������ʵ��Ӧ�����ַ���������Ȼ�����Դ·��
    //��������Unity֧�������棬Ȼ��ԭ���ù�ϵ��ֻҪ��������Դ��ַû��
    public List<DataTalkOneSelect> selects;
}

[System.Serializable]
public class DataTalkOneSelect
{
    public string intent;//��ͼ
    public GameObject nextBranch;//��ת����һ����֧
                                 //ÿһ����֧��Ӧ��һ������е�
                                 //������ô������Ϊ������༭���Դ�����ת��ʾ���ܣ���Ȼ������ֱ�ۻ���ʾ���ݵ��໥���ù�ϵ��

    [HideInInspector]
    public string citeTo;//��֧�������������
                         //���ù�ϵ���Ǵ洢���˵ģ���Ϊ�����������Ƕ�̬�������ٵģ�����������Ҫ������
                         //�������Ӧ���壬�����໥���ɣ�ֱ�۲���ʱӦֻ�踳ֵ���壬�����������Ʊ���
}

[System.Serializable]
public class DataTalkBranch//����˳�����ĶԻ���ֱ�����ַ�֧����Ҳ��һ���ͻ����������֧��
{
    public List<DataTalkOne> seq;

    public int meLengh { get { return seq.Count; } }
}

public interface IDataTalkBranchGet
{
    public DataTalkBranch meDataTalkBranch { get;}
}

public class DialogueCite
{
}