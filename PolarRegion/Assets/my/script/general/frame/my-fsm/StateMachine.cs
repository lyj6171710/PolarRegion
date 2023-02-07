using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<E>
{
    public E state_need;//当前需要处在的状态，或保持或切换

    //用来存储当前机器所控制的所有状态
    public Dictionary<E, StateBase<E>> state_cache;
    
    public StateBase<E> previous_state;//定义上一个状态
    public StateBase<E> current_state;//定义当前状态
    //状态机不负责下一个状态是什么，外界自己负责组织

    //内部机制=======================================================
    
    public StateMachine(params StateBase<E>[] states)//一开始就加入进全部状态，暂时不考虑动态增删状态的问题
    {//默认第一个传来的状态为初始状态
        if (states.Length > 0)
        {
            AddBeginState(states[0]);//状态机的使用，至少伴随一个初始状态
            for (int i = 1; i < states.Length; i++)
                AddState(states[i]);
        }
        else
            Debug.Log("不能没有状态，状态机初始化失败");
    }

    void AddBeginState(StateBase<E> state)
    {
        state_cache = new Dictionary<E, StateBase<E>>();
        AddState(state);//把状态添加到集合中

        previous_state = null;
        current_state = state;
        state_need = state.ID;

        EnterState(current_state);
    }

    //内外机制========================================================
    
    //状态保持
    public void FixedUpdate()
    {
        current_state.OnStayFixed();
    }

    public void Update()
    {
        current_state.OnStayEach();
        TranslateState(state_need);//等待OnStay()内容全部完结后才做转换最好(外面逻辑的不当，可能造成translate的频繁执行)
                                   //立即随需要切换的话，可能造成无限来回切换，卡死程序
                                   //而且切换后的原有逻辑，照样会得到一次执行，还得考虑各自顺序的问题
    }

    public void Restart()
    {
        current_state.OnEnter();//状态机的执行可能会被外界干扰
                                //外界就可能需要重新进入一下状态机当前所在的状态
    }
    
    //内外接口=======================================================

    //通过Id来切换状态
    public void TranslateState(E id)
    {
        if (state_cache[id] == current_state) return;
        if (!state_cache.ContainsKey(id)) return;

        previous_state = current_state;
        current_state = state_cache[id];

        previous_state.OnExit();
        EnterState(current_state);
    }
    
    //内部接口===========================================
    
    void EnterState(StateBase<E> state)
    {
        if (state.have_ready_inner)
            state.OnEnter();
        else
        {
            state.OnFirst();
            state.have_ready_inner = true;
            state.OnEnter();
        }
    }
    
    void AddState(StateBase<E> state)
    {
        if (!state_cache.ContainsKey(state.ID))
        {
            state_cache.Add(state.ID, state);
            state.machine_inner = this;
        }
    }

}
