using System;
using System.Collections.Generic;
using UnityEngine;

public class RecordMake : MonoBehaviour
{//�����ȫ�ֹ���һ�������ɸ���

    public int meRestTime { get { return (int)mRestTime; } }
    public string meTip { get { return mTip; } }
    public bool meHaveEnd { get { if (!mDo && !mStart) return true; else return false; } }
    public AudioClip meOutput { get { if (meHaveEnd)  return mClip;  else return null; } }

    public string meOutputStr { 
        get {
            if (meHaveEnd)
            {
                string tmpStr = mClipStr;
                mClipStr = null;//ȡ���ļ������
                return tmpStr;
            }
            else
                return null;
        }
    }

    public static RecordMake It;

    //----------------------------------------

    Info mInfo;

    bool mDo;//����¼��
    bool mStart;//�ѿ�ʼ¼��
    MonoBehaviour mClient;//��ǰռ��¼�����ܵĲ��ţ������ǰδ¼���������Ա������豸��ռ

    string[] devices;

    float mRestTime;
    string mTip;

    AudioClip mClip;
    string mClipStr;

    void Awake()
    {
        It = this;

        devices = Microphone.devices;
        if (devices.Length != 0)
        {
            mTip = "�豸����˷磺" + devices[0];
        }
        else
        {
            mTip = "�豸û����˷�";
        }

        mStart = false;
        mDo = false;
        mClient = null;

        mClipStr = null;
    }

    void Update()
    {
        if (mDo)
        {
            if (!mStart)
            {
                mTip = "¼����ʼ";
                mStart = true;
                mRestTime = mInfo.limit;
                //����һ���豸���֣�nullΪĬ���豸�����������Ƿ�ѭ��¼�ƣ���������¼��ʱ�䣨�룩�������ģ���Ƶ��
                mClip = Microphone.Start(devices[0], false, mInfo.limit, mInfo.frequency);
                //��ʱ����ϵͳ��ôʵ�����¼��Ч���ģ���߱����ĺ�������ô��ϵͳ֪�����أ�
            }
            else
            {
                mTip = "����¼��";
                mRestTime -= Time.deltaTime;
                if (mRestTime <= 0)//�Զ�����
                {
                    EndRecord("ʱ���ֹ");
                    mInfo.WhenFinish();
                }
            }
        }
        else
        {
            if (mStart)//�����������
            {
                EndRecord("¼����ǰ����");
            }
            else
            {
                //û��ʼ¼������ʲô��������
            }
        }
    }

    bool IfCanTakeRecord(MonoBehaviour client)//¼��Ȩ��
    {
        if (client == null)
            return false;
        else if (!meHaveEnd)
        {
            if (client != mClient)
                return false;
            else
                return true;
        }
        else 
            return true;
    }

    public bool OnoffRecord(MonoBehaviour client, Info refer)//��翪��¼���Ľӿ�
    {
        if (IfCanTakeRecord(client))
        {
            if (!meHaveEnd)//¼����δ�����ͱ���ֹ
            {
                EndRecord("¼����ǰ����");
                return false;//����¼��ʱ������false
            }
            else
            {
                mClient = client;
                mInfo = refer;
                mDo = true;
                mClip = null;
                return true;//����¼��ʱ������true
            }
        }
        else
            return false;//����¼��ʱ������false
    }

    void EndRecord(string tip = "")
    {
        Microphone.End(devices[0]);
        mTip = tip;
        mClipStr = AudioToStr(mClip);
        mStart = false;
        mDo = false;
    }

    //----------------------------------------------------

    public static string AudioToStr(AudioClip clip) //ת�����ַ��������ڴ洢
    {
        return ValueConvert.BytesToStr(AudioToBytes(clip));
    }

    public static AudioClip StrToAudio(string carrier,int freqCorr)
    {
        return BytesToAudio(ValueConvert.StrToBytes(carrier), freqCorr);
    }

    public static byte[] AudioToBytes(AudioClip clip)//ת�����ֽ����飬���ڴ���
    {
        //AudioClip����ͨ��GetData��SetData��ȡ��������Ƶ���ݣ�����������-1��1֮���float����
        //���byte[]��ת��AudioClipʱ��Ҫ���������ų�-1��1֮ǰ��float
        //byte[]�������ֽڶ�ӦUnity��һ��float����

        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);
        short[] intData = new short[samples.Length];//converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]
        byte[] bytesData = new byte[samples.Length * 2];//bytesData array is twice the size of
                                                        //dataSource array because a float converted in Int16 is 2 bytes.
        int rescaleFactor = 32767;//to convert float to Int16
        //��Ϊfloat������-1��1֮ǰ��������Ҫ������ת�����з���2���ֽڵķ�Χ -32768~32767������������32767

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            byte[] byteArr = new byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        return bytesData;
    }

    public static AudioClip BytesToAudio(byte[] rawData,int freqCorr)
    {
        //ע�⣬Ӧ��ֻ֧������audio clip�����������ݶ�ת�������ֽ�������֧����Ƶ�ļ��ֽ�����clip��ת��
        //����clip������������ݣ����Ǵ���Ƶ����pcm��ʽֱ�Ӷ�Ӧ

        float[] samples = new float[rawData.Length / 2];
        float rescaleFactor = 32767;
        short st = 0;
        float ft = 0;

        for (int i = 0; i < rawData.Length; i += 2)
        {
            st = BitConverter.ToInt16(rawData, i);
            ft = st / rescaleFactor;
            samples[i / 2] = ft;
        }

        AudioClip audioClip = AudioClip.Create("mySound", samples.Length, 1, freqCorr, false);
        audioClip.SetData(samples, 0);

        return audioClip;
    }

    //------------------------------

    public struct Info
    {
        public int limit;//���ʹ�ø������¼������ǰ����Ҫ��ֵ
        public int frequency;//���������Ҫ���������
        public Action WhenFinish;//�����ÿɲ���

        public Info(int mFreq)
        {
            limit = 2;
            frequency = mFreq;
            WhenFinish = null;
        }
    }

}