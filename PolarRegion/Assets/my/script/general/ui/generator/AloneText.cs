using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AloneText : MonoBehaviour {
    
    Text text;
    string content;

    //内外机制=====================================

    public void MakeReady(string show)//接收并准备
    {
        content = show;
        //---------------------------
        text = GetComponent<Text>();
        text.text = show;
        text.resizeTextForBestFit = true;
    }
    
}
