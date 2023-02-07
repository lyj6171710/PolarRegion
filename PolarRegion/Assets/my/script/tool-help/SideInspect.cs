using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kind_colliding
{
    public string tag = "empty";
    public bool have_just = true;
    //public Type_of type;
}

public class SideInspect : MonoBehaviour
{
    public bool is_colliding = false;
    public List<Kind_colliding> kind_colliding = new List<Kind_colliding>();
    bool have_kind = false;
    
    private void Update()
    {
        if (kind_colliding.Count > 0)
            is_colliding = true;
        else
            is_colliding = false;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < kind_colliding.Count; i++)
            if (kind_colliding[i].have_just)
                StartCoroutine(off_delay_(kind_colliding[i]));
    }

    IEnumerator off_delay_(Kind_colliding will_off)
    {
        yield return new WaitForSeconds(0.2f);
        will_off.have_just = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        have_kind = false;
        for (int i = 0; i < kind_colliding.Count; i++)
        {
            if (kind_colliding[i].tag == collision.transform.tag)
            {
                have_kind = true;
                break;
            }
        }
        if (!have_kind)
            add_node_(collision.transform);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        for (int i = 0; i < kind_colliding.Count; i++)
        {
            if (kind_colliding[i].tag == collision.transform.tag)
            { kind_colliding.RemoveAt(i); break; }
        }
    }

    void add_node_(Transform crash)
    {
        Kind_colliding node = new Kind_colliding();
        node.tag = crash.tag;
        node.have_just = true;
        //node.type = crash.GetComponent<Type_of>();
        kind_colliding.Add(node);
    }
}
