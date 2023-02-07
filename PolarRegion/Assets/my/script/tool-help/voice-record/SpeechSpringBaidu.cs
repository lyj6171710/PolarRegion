/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Baidu.Aip.Speech;//依赖第三方SDK
using System.IO;
using System;

public class SpeechSpringBaidu : MonoBehaviour
{
    [Header("官网上申请下来的凭证，不能随便给")]
    public string APP_ID = "25750403";
    public string API_KEY = "LneL4Kh6rGQnfZfGsp3N6Znj";
    public string SECRET_KEY = "ygpYce92PFuuhrcoWBd7h8HnPyGH25Zl";

    //public string UrlToSpeechSynthesis = "http://tsn.baidu.com/text2audio";// 语音合成地址

    //-------------------------------------

    Tts mAgent;

    void Start()
    {
        mAgent = new Tts(API_KEY, SECRET_KEY);
        mAgent.Timeout = 60000;//修改超时时间（心跳）
    }

    public AudioClip Get(string need)
    {
        var refer = new Dictionary<string, object>()
        {
            { "spd" , 4 },//语速
            { "vol" , 7 },//音量
            { "per" , 4 },//发音人
            { "aue" , 4 },//音频格式，4对应pcm、16k、16bit
            //tex String  合成的文本，使用UTF-8编码，请注意文本长度必须小于1024字节   是
            //cuid String  用户唯一标识，用来区分用户，填写机器 MAC 地址或 IMEI 码，长度为60以内 否
            //spd String  语速，取值0-9，默认为5中语速    否
            //pit String  音调，取值0-9，默认为5中语调    否
            //vol String  音量，取值0-15，默认为5中音量   否
            //per String  普通发音人选择：度小美=0(默认)，度小宇=1，，度逍遥（基础）=3，度丫丫=4    否
            //per String  精品发音人选择：度逍遥（精品）=5003，度小鹿=5118，度博文=106，度小童=110，度小萌=111，度米朵=103，度小娇=5
            //还存在有另外的参数，得去原生api接口列表中查
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

        //理解促进：
        //pcm存储量 = 采样频率(每秒多少个数据单位) * 时间 * (采样位数/8) * 声道
        //存储量单位为一个byte，采样频率指每秒多少个数据单位，采样位数指每个数据单位需用多少数据位存， 8bit为一个byte的长度

    }

    //-------------------------------------

    public static SpeechSpringBaidu It;

    void Awake()
    {
        It = this;
    }
}
*/