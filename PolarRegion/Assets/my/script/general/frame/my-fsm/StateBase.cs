using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase<E>{//每种状态的基础类，主要由状态机操作

    //外界可用===============================

    public E ID { get { return id; } }
    
    //一次性赋值=============================

    E id;//给每个状态设置一个ID，外界也通过这个ID来使用状态
    
    //子类可用===============================

    public StateMachine<E> machine_inner;//被添加到某一状态机时，由该状态机赋值

    //组合可用===============================

    //被当前机器所控制与使用
    public bool have_ready_inner = false;//由状态机使用，识别是否已经执行过OnFirst方法
    
    //内部机制=====================================

    public StateBase(E id){ this.id = id; }

    //给子类提供方法
    public abstract void OnEnter();//每次转变到该状态时
    public abstract void OnExit();//离开该状态的时候
    public abstract void OnFirst();//第一次进入到该状态时
    public abstract void OnStayFixed();//有些功能可能更适合定量帧
    public abstract void OnStayEach();//但有些外界功能会需要每帧都调用
}

public abstract class StateTemplate<E,O,S> : StateBase<E>
{//这个类比基类更相关专门情境一些，方便专门情境使用，不被状态机识别，但该类可以影响状态机运转

    //一次性赋值========================================

    public O Owner;//状态所描述与作用的对象
                   //由外界先主动、手动赋值
                   //使用泛型可以加快速度
    public S Com;//各具体状态都共用的参数集
                 //而且也建议作为外界与各状态沟通的主要媒介
                 //一样由外界先主动、手动赋值
                 
    //私用变量=========================================

    bool new_switch;//是否被制作者重写过Switch函数的标志

    //内部机制=========================================

    public StateTemplate(E id) : base(id) {
        new_switch = true;//得默认是重写过的
    }
    
    public override void OnFirst() { }//这里直接实现抽象函数，子类可以有选择地实现了
    public override void OnStayFixed() { }//子类不用也不要，去调用这个基类的这些函数
    public override void OnStayEach(){ }
    public override void OnEnter() { }
    public override void OnExit() { }
    protected virtual E SwitchWhen()//该状态什么时候什么情况切换到另外什么状态
    {//异步切换，这一帧OnStay执行完毕后才且立即做切换
        new_switch = false;//能被执行该语句，说明确实没重写过
        return default(E);
    }

    //子类可用===========================================

    protected void SwitchTake()//进行一次状态切换检测
    {//使用该函数，是为了节省性能，该切换的时候再做切换
        E should = SwitchWhen();//默认认为已经重写过该函数
        if (new_switch)//确实重写了
            machine_inner.state_need = should;//状态机会做异步切换
        else//发现没有重写过
            new_switch = true;
    }

    protected E SwitchNo{//外界在重写SwitchWhen()时，没有什么可以切换的时候，调用并返回该函数的执行结果
        get
        {
            new_switch = false;//执行到这里时，其实还是重写了，只是利用该变量完成该函数的目标功能
            return default(E);
        }
    }
    
}