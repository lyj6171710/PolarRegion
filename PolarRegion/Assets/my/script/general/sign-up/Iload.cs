using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//SignUp用于显式表示所参与的协议，有些协议比较生僻，最好显式指定出来，
//然后通过引用追溯到这里，就能让读者很快明白具体情况
public interface ISignUpRead { int SignRead { get; } }

interface ISUReadStandard//用于降低游戏运行过程中的加载负担
{
    //假设这些函数在一个sub中

    void ReadReady();
    //由内部上级调用，作为正式加载内容的起点
    //外界访问附属部门时，内部会需要调用的接口，让部门内容准备就绪，进行ready，之后外界可以正常使用

    void InquirySubHave();
    //由内部自己在做自己的准备工作时调用
    //查询存在有哪些附属部门
    //这里的准备，对应自己本身具有的所有内容得到加载，对应由ReadReady引发

    void FormSubHave();
    //随InquiryHave的调用而调用，生成占位符性质的事物
    //查询到附属部门存在时，准备相关它的数据空间与基本信息，但未赋值有实际内容，也未进行Ready

    void AccessSub();
    //由外界调用，调用内部上级访问内部自己
    //外界访问附属部门的接口，外界可以认为数据早已存在，但实际上，当访问的时候才临时生成
    //（1）如果部门还未准备，则启动准备工作，调用ReadReady
    //（2）已经准备则直接返回该部门
    //（3）如果不存在这个部门，就报错，且理应报错，以为有什么部门，事先已经确定好了
}

interface ISUSaveLoad
{
    void MakeSave();

    void MakeLoad();
}