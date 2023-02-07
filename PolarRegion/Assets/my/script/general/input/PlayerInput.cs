using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour//管理玩家当前所操控角色所受的输入
{
    //外界可操作--------------------------------------------

    public static PlayerInput Single { get { return single; } }
    public Collider optionClickTarget { get { return click_target; } }
    public Vector3 moveInput { get { return movement.normalized; } }
    public bool isMoveInput { get { return !Mathf.Approximately(moveInput.magnitude, 0); } }
    public float distanceToInteractWithNpc = 2.0f;

    //私用变量-----------------------------------------

    static PlayerInput single;
    Collider click_target;
    Vector3 movement;

    void Awake()
    {
        single = this;
    }

    void Update()
    {
        movement.Set(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        );

        if (Input.GetMouseButtonDown(0)) HandleLeftMouseBtnDown();
        if (Input.GetMouseButtonDown(1)) HandleRightMouseBtnDown();
            
    }
        
    //============================================================

    void HandleLeftMouseBtnDown()
    {//暂时空置
    }

    void HandleRightMouseBtnDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//交互识别
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit);
        if (hasHit) StartCoroutine(TriggerOptionTarget(hit.collider));
    }

    bool IsPointerOverUIElement()
    {
        var eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }

    IEnumerator TriggerOptionTarget(Collider other)
    {
        click_target = other;
        yield return new WaitForSeconds(0.03f);
        click_target = null;
    }

}