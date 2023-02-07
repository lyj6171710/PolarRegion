using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSetRender : MonoBehaviour{
    //播放一组图像，构成动画
    //存疑bug，协程第一次的yield return使得无限等待，不论是null，还是waitsecond，建议将协程改为fixedupdate，具体机制不清楚，怎么都会徒劳
    
    public List<Sprite> sprites;
    public List<float> times;//times1[i]赋值为第i秒出现sprites[i]的图片，times[0]的值对应了最后一张图片转到第一张图片需要的时间
    
    public bool same_interval;//如果图片间隔时间都相同，可以设置same_interval为true,interval为每次时间，就不需要赋值times123了
    public float interval;
    SpriteRenderer render;
    IEnumerator runing;
    public Sprite 原精灵;//手动备份原有精灵

    [Header("下方各变量值，不需手动变换")]
    public bool loop;
    
    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
    }

    public void play_anim_(float speed = 1, bool loop = false)
    {
        render = GetComponent<SpriteRenderer>();//避免万一，外界可能先于start调用该函数
        stop_play_();//如果正播放有动画，则停止它并准备播放当前动画
        runing = play_(speed, loop);//赋值协程，填充内容
        StartCoroutine(runing);//启动协程
    }

    private void stop_play_()
    {
        render = GetComponent<SpriteRenderer>();
        render.sprite = 原精灵;
        if (runing != null)
            StopCoroutine(runing);
        //一个游戏对象同一时间只会有一个精灵被渲染，协程能代表当前的精灵状态
    }
    
    IEnumerator play_(float speed, bool loop)
    {//speed表示播放速度，会对原有所设定的时间全部同比例变化
     //loop表示如果播放则循环播放，否则播放一遍结束
        while (true)
        {
            for (int i = 0; i < sprites.Count; ++i)
            {//从0到size-1，共size张图
                render.sprite = sprites[i];
                if (same_interval)
                    yield return new WaitForSeconds(interval);
                else
                {
                    if (i < sprites.Count - 1)//没有到最后一张图时
                        yield return new WaitForSeconds((times[i + 1] - times[i]) / speed);//切换到下一张所需的时隔
                    else//最后一张图时，准备切换到第一张
                        yield return new WaitForSeconds(times[0] / speed);
                }
            }
            if (!loop) break;
        }
        render.sprite = 原精灵;
    }

}
