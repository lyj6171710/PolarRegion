using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Fsm<O,E,S>:MonoBehaviour where O:class where S:class,new()
{//提供状态机，专门方便AI控制
    //泛型O是描述子类的类型，也是各状态的从属者，方便各状态获取
    //泛型E是使用状态机时，区分不同状态的变量的类型
    //泛型S是描述并集合不同状态共用变量的变量的类型

    //子类可用===================================
        
    protected O belong;
    protected S Share;
        
    //私用变量===================================

    StateMachine<E> fsm;//状态机，子类不用知道该状态机的存在

    //内部机制===================================
    
    void Awake() { Awake_(); }
    void Start() { Start_(); }
    void Update() {  Update_(); }
    void FixedUpdate() { FixedUpdate_(); }
    void OnEnable() {  OnEnable_(); }
    void OnDisable() {  OnDisable_(); }

    protected virtual void Awake_()
    {
        belong = this as O;
    }

    protected virtual void Start_()
    {
        Share = new S();
        start_to_share_();
    }

    protected virtual void Update_()
    {
        update_to_share_();
        fsm.Update();
    }

    protected virtual void FixedUpdate_()
    {
        fsm.FixedUpdate();
    }

    protected virtual void OnEnable_()
    {
        fsm.Restart();
    }

    protected virtual void OnDisable_()
    { }
    
    //状态机使用（子类中调用）===================================

    protected virtual void start_to_share_()
    {

    }

    protected virtual void update_to_share_()
    {

    }

    protected void build_states_(params StateBase<E>[] states)//子类应在start中使用，且最后使用
    {
        foreach (StateBase<E> state in states)
        {
            StateTemplate<E, O, S> new_state = state as StateTemplate<E, O, S>;
            new_state.Com = Share;
            new_state.Owner = this as O;
        }
        fsm = new StateMachine<E>(states);
    }
        

    //==============================================
}