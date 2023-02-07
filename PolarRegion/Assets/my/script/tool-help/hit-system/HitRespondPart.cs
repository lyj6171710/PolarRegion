using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRespondPart : MonoBehaviour
{
    //接收碰撞，但经常存在多个碰撞体，都由同一个事物响应
    //将该组件挂载到有碰撞体的物体上，然后会将碰撞传达给指定用来接收响应者

    public GameObject meBelongGbj { get { return mBelong.gameObject; } }

    HitRespond mBelong;//内部直接拿这个变量用
    public Collider2D neCollid { get; set; }//自己肯定伴随有一个碰撞体，这里保存起来

    Rigidbody2D mRigid;//激活触发时机函数的效果
    //虽然发起碰撞者一定持有碰撞潜力，接收碰撞者，不一定它自己受到碰撞
    //但是让碰撞发起者装，由于量多，性能压力大，宁愿接收碰撞者的各个局部来装
    
    public void MakeReady(HitRespond belong)
    {
        mBelong = belong;

        neCollid = GetComponent<Collider2D>();

        mRigid = GbjAssist.AddCompSafe<Rigidbody2D>(gameObject);
        mRigid.gravityScale = 0;
    }

    public bool MakeHit(HitTryMake comer)
    {
        return mBelong.MakeHit(comer, this);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {//没使用Enter，是因为外界物体一经出现，可能就已经在触发体中，就会有问题
        mBelong.OnTriggerIn(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        mBelong.OnTriggerOut(collision);
    }

}
