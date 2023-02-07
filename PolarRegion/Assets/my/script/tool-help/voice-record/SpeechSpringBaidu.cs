/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Baidu.Aip.Speech;//����������SDK
using System.IO;
using System;

public class SpeechSpringBaidu : MonoBehaviour
{
    [Header("����������������ƾ֤����������")]
    public string APP_ID = "25750403";
    public string API_KEY = "LneL4Kh6rGQnfZfGsp3N6Znj";
    public string SECRET_KEY = "ygpYce92PFuuhrcoWBd7h8HnPyGH25Zl";

    //public string UrlToSpeechSynthesis = "http://tsn.baidu.com/text2audio";// �����ϳɵ�ַ

    //-------------------------------------

    Tts mAgent;

    void Start()
    {
        mAgent = new Tts(API_KEY, SECRET_KEY);
        mAgent.Timeout = 60000;//�޸ĳ�ʱʱ�䣨������
    }

    public AudioClip Get(string need)
    {
        var refer = new Dictionary<string, object>()
        {
            { "spd" , 4 },//����
            { "vol" , 7 },//����
            { "per" , 4 },//������
            { "aue" , 4 },//��Ƶ��ʽ��4��Ӧpcm��16k��16bit
            //tex String  �ϳɵ��ı���ʹ��UTF-8���룬��ע���ı����ȱ���С��1024�ֽ�   ��
            //cuid String  �û�Ψһ��ʶ�����������û�����д���� MAC ��ַ�� IMEI �룬����Ϊ60���� ��
            //spd String  ���٣�ȡֵ0-9��Ĭ��Ϊ5������    ��
            //pit String  ������ȡֵ0-9��Ĭ��Ϊ5�����    ��
            //vol String  ������ȡֵ0-15��Ĭ��Ϊ5������   ��
            //per String  ��ͨ������ѡ�񣺶�С��=0(Ĭ��)����С��=1��������ң��������=3����ѾѾ=4    ��
            //per String  ��Ʒ������ѡ�񣺶���ң����Ʒ��=5003����С¹=5118���Ȳ���=106����Сͯ=110����С��=111�����׶�=103����С��=5
            //������������Ĳ�������ȥԭ��api�ӿ��б��в�
        };
        TtsResponse result = mAgent.Synthesis(need, refer);
        if (result.Success)
        {
             return SpecPCMToClip(result.Data);
        }
        else
            return null;
    }

    static AudioClip SpecPCMToClip(byte[] pcmData)
    {
        short[] samples_int16 = new short[pcmData.Length / 2];
        Buffer.BlockCopy(pcmData, 0, samples_int16, 0, pcmData.Length);
        float[] samples = new float[samples_int16.Length];
        for (int i = 0; i < samples.Length; i++) 
        {
            samples[i] = samples_int16[i] / (float)short.MaxValue;
        }

        AudioClip audioClip = AudioClip.Create("convert", samples.Length, 1, 16000, false);
        audioClip.SetData(samples, 0);

        return audioClip;

        //���ٽ���
        //pcm�洢�� = ����Ƶ��(ÿ����ٸ����ݵ�λ) * ʱ�� * (����λ��/8) * ����
        //�洢����λΪһ��byte������Ƶ��ָÿ����ٸ����ݵ�λ������λ��ָÿ�����ݵ�λ���ö�������λ�棬 8bitΪһ��byte�ĳ���

    }

    //-------------------------------------

    public static SpeechSpringBaidu It;

    void Awake()
    {
        It = this;
    }
}
*/