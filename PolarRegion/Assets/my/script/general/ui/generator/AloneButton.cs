using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AloneButton : MonoBehaviour {//由generator生成的每个按钮，都会挂载一个该组件，辅助控制

    int id;
    string content;
    IClick call;
    
    Button button;
    Text text;

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
        
    }

    //内外机制================================

    public void AcceptBase(int id, string show, IClick callback)
    {
        this.id = id;
        content = show;
        call = callback;
        //------------------
        button.onClick.AddListener(delegate () { call.when_click_(id); });//这里返回自己的引用，可以方便在点击后移除该按键
        text.text = content;
    }
    
    public interface IClick
    {
        void when_click_(int id);
    }
}
