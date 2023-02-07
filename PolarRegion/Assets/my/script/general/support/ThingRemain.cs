using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThingRemain : MonoBehaviour
{//����Ҫһֱ�����ڳ����е����壬��Ϊ�����˸�����������������
 //��Ҫ��Ϊ�˿糡����Ҳ���ִ��ڣ�ͬʱ�ٿ����ʱ��Ҳ������ֶ��ͬ����
 //ע���κ������壬�����ҪAwake��������漰�Թ������ݵ��޸ģ���̳и�����ṩ�Ľӿڣ�������ܳ����⣬�տ�Խ��ʱ�򣬻�ͬʱ������thingRemain

    static ThingRemain it;
    public static ThingRemain It
    {
        get
        {
            if (!it)
                it = GameObject.Find("thing-remain").GetComponent<ThingRemain>();
            return it;
        }
    }

    void Awake()
    {
        //�ȱ�֤�����ȫ����
        if (it != null) Destroy(gameObject);//��ֹ���ڶ��
        else
        {
            if (It)//����ߴٸ���ֵ���������˳û�ռ����
                DontDestroyOnLoad(gameObject);//Ψһʱ�����ڴ���
            AwakeOther();
        }
    }

    int mLastSceneIndex;
    ISwitchScene[] mSwitches;

    void AwakeOther()
    {
        mSwitches = GetComponentsInChildren<ISwitchScene>();
        foreach (ISwitchScene sw in mSwitches) sw.WhenAwake();
        //��Ҫawake��ִ�У���Ϊ���ڿ��������
        mLastSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        if (mLastSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {//�����������л�ʱ
            foreach (ISwitchScene sw in mSwitches) sw.WhenSwitchScene();
            mLastSceneIndex = SceneManager.GetActiveScene().buildIndex;
        }
    }

}

//��Ҫ�糡������ģ����̳иýӿ�
public interface ISwitchScene
{
    void WhenAwake();
    void WhenSwitchScene();
}