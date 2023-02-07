using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessCtrl : MonoBehaviour {//相对某一个场景


    public List<GameObject> need_active;//需要一开始保证激活状态的事物

    private void Awake()
    {
        need_active.ForEach(obj => obj.SetActive(true));
    }
}
