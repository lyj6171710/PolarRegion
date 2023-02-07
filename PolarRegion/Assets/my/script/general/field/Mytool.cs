using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Reflection;

public class Mytool : MonoBehaviour,ISwitchScene
{
    public void WhenAwake()
    {
        if (cors != null)
            cors.Clear();
        else
            cors = new Dictionary<string, Coroutine>();

        if (news != null)
            news.Clear();
        else
            news = new Dictionary<int, Dictionary<GameObject, List<Delegate>>>();

        if (callCache != null)
            callCache.Clear();
        else
            callCache = new Dictionary<string, MethodInfo>();
    }

    public void WhenSwitchScene()
    {

    }

    //------------------------------------------------------

    //便捷地实现：开启一个协程时关闭之前开启过的同名协程（能保证程序规范），避免重复
    //暂时只能使用string来调用协程
    //这个功能应该写在需要该功能的代码页中，这里只是记录存放
    Dictionary<string, Coroutine> cors;
    void renew_cor_(string corName, object param = null)
    {
        Coroutine cor = null;
        if (cors.TryGetValue(corName, out cor))
        {
            if (cor != null)
                StopCoroutine(cor);
        }
        cors.Remove(corName);
        cors.Add(corName, StartCoroutine(corName, param));
    }

    //================================================================

    static Dictionary<string, MethodInfo> callCache = new Dictionary<string, MethodInfo>();//为提高性能用的

    static void save_call_(string call, MethodInfo cite)
    {
        MethodInfo info;
        if (callCache.TryGetValue(call, out info))
        {
            Debug.Log("已经存在");
            return;
        }
        callCache.Add(call, cite);
    }

    static MethodInfo get_call_(string call)
    {
        MethodInfo info;
        if (callCache.TryGetValue(call, out info))
            return info;
        return null;
    }

    static bool lastFound = false;
    static bool lastExist = false;
    static bool lastInGameObject = false;
    static bool haveFound = false;//临时标记是否找到了指定方法，用于有前后依赖关系的类
    static bool haveExist = false;//临时标记是否从组件中找到了指定方法
    static bool inGameObject = false;//用来标记当前函数的运行选择，让函数自身的后续处理有所准备
    public static object send_msg_(object target, string call, params object[] paras)//查找指定方法，且只会有一个同名方法被调用
    {//高配版sendmessage，若干个形式参数，还可以有返回，也不限类型，但性能比sendmessage还慢几倍，不能频繁调用
        lastExist = haveExist;//可能存在上一层的send_msg_，执行完毕后，得复原临时变量
        lastFound = haveFound;
        lastInGameObject = inGameObject; 
        haveExist = false;
        haveFound = false;
        inGameObject = false;//每一次执行，都是从false状态开始
        //debug_existing_();//检查是否被频繁调用，要尽力避免
        object result= send_message_(target, call, paras);//执行期间会使用inGameObject
        haveExist = lastExist;
        haveFound = lastFound;
        inGameObject = lastInGameObject;//外界每一次send_msg_可能会引起其它send_msg_，就可能重制该变量到false
        //但可以保证，所引起的其它send_msg_一定会先执行完，再回到上一层send_msg_中调用指定方法的地方，这里再重设到进入此层send_msg_时的状态即可
        return result;
    }
    static object send_message_(object target, string call, params object[] paras)//从对象查找方法
    {
        if (target != null && call != null && call != "")
        {
            Type type = target.GetType();//这个类型信息直接指向最子层的那个类

            if (type.Name == "GameObject")//适应并利用unity特殊编辑环境
            {
                inGameObject = true;//让后续查找时就算没找到，也不用报告
                GameObject gameobject = target as GameObject;
                MonoBehaviour[] components = gameobject.GetComponents<MonoBehaviour>();
                for (int i = 0; i < components.Length; i++)
                {
                    object feedback = send_message_(components[i], call, paras);
                    if (haveExist) return feedback;//找到了，就不需要继续执行了
                }
                Debug.Log("所有组件都不存在指定方法");
                return null;
            }
            else
            {
                object feedback = invoke_(type, type.Name, call, target, paras);
                if (haveFound)
                {
                    if (inGameObject)
                        haveExist = true;
                }
                else
                {
                    if (!inGameObject)//遍历中，当前没找到，不代表这一次遍历也找不到
                        Debug.Log("指定对象不存在指定方法");
                }
                return feedback;
            }
        }
        else
        {
            Debug.Log("参数错误");
            return null;
        }
    }
    static object invoke_(Type type, string leaf, string call, object target, params object[] paras)//从类调用方法，传给参数
    {//查询过程，包括所传给类的基类
        MethodInfo method = get_call_(leaf + call);//需要区分不同类的同名调用，但有父子关系的类不需要
        if (method != null)
        {
            haveFound = true;
            return method.Invoke(target, paras);
        }
        else
        {
            method = type.GetMethod(call, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //instance是指需要由对象调用出来的方法，静态方法就不需要经过对象;public和都应该可以，至于性能，只是开始那一下
            //但还无法获取到基类对象中的方法
            if (method != null)
            {
                save_call_(leaf + call, method);//保存已经查找过的方法，以后可以快捷调用出来
                haveFound = true;
                return method.Invoke(target, paras);
            }
            else
            {
                if (type.BaseType.Name == "MonoBehaviour" || type.BaseType.Name == "Object")
                {//查询到MonoBehaviour时，已经是系统类，不需要查找了
                    haveFound = false;
                    return null;
                }
                else
                    return invoke_(type.BaseType, leaf, call, target, paras);
            }
        }
    }
    
    public static object feedBack { get { object tmp = feedback; feedback = null; return tmp; } set { feedback = value; } }
    //想要通过sendmessage返回一个值并即时处理时，可以统一通过该属性来相互交流
    static object feedback;

    //委托式通知启用机构(这个相比sendMessage可以一改皆改，而且省性能一些)=================================

    static Dictionary<int, Dictionary<GameObject, List<Delegate>>> news;
    //int是消息名称，由统一的枚举转换得来，GameObject是注册者的唯一标识
    //总关系可以描述为，一个消息种类，对应多个具有该消息的事物，一个事物自身又可以有多个同时可响应该消息的部门
    //news意指那些想要从外界得知消息所需要经过的媒介

    public static T call_news_<T>(EMsgs need, GameObject target, params object[] paras)//适合外界自己也不做确定的时候
    {//T是想要返回结果的类型，不要写成委托类型
        int clue = (int)need;
        if (news.ContainsKey(clue))
        {
            if (news[clue].ContainsKey(target))
            {
                List<Delegate> list = news[clue][target];
                int the_last = list.Count - 1;
                for (int i = 0; i < the_last; i++)
                    list[i].Method.Invoke(list[i].Target, paras);
                Delegate call = list[the_last];//只管最后一个被调用者的返回值
                return (T)call.Method.Invoke(call.Target, paras);
            }
            else
            {
                //Debug.Log("指定对象不具有这种消息");
                return default(T);
            }
        }
        else
        {
            //Debug.Log("还不存在这种消息");
            return default(T);
        }
    }
    
    public static List<D> get_news_<D>(EMsgs need, GameObject target) where D : class//转换到外界相应需要的类型来调用，性能提升大，外界甚至可以不断调用
    {//D为一种具体的委托类型，外界得到返回值后，需要紧接加上后缀“(参数列表)”来使用
        List<Delegate> list = news[(int)need][target];
        if (list.Count == 0)
            return null;
        else
        {
            List<D> news_convert = new List<D>();
            list.ForEach((news) => { news_convert.Add(news as D); });
            return news_convert;
        }
    }//使用举例： if (Mytool.get_news_<Func<bool>>(Msgs.whether_alive_, com.hunt_object)[0]()) 
    
    public static void register_to_news_(EMsgs other_need, GameObject self, Delegate respond)
    {//GameObject变量对应有该委托变量，由此后面调用respond的时候，才会是所需的那个对象所具有的该委托
     //最后一个委托变量是委托基类变量，这样的话，同一个消息，可以有不同的委托类型
     //第一个变量是一种枚举，里面定义了各种特定预定需要得知的消息
        int clue = (int)other_need;
        if (!news.ContainsKey(clue))//有这个外界需求时
            apply_news_(clue);
        if (!news[clue].ContainsKey(self))
            apply_news_(clue, self);
        news[clue][self].Add(respond);
    }//使用举例：Mytool.register_to_news_(Msgs.whether_alive_, gameObject, new Func<bool>(tell_out_whether_alive_));
    
    public static void detach_from_news_(EMsgs other_need, GameObject self)
    {//由于方法相关于实例，外界需要在销毁该实例时，手动解除调用关系，得以降低性能负担
     //物体实例被消除时，相关委托都必然应被消除，所以不需要给具体是哪一个委托了
        int clue = (int)other_need;
        if (news.ContainsKey(clue))
            news[clue].Remove(self);
        //else
        //    Debug.Log("自己没有申请过该消息");
    }//使用举例：Mytool.detach_from_news_(Msgs.whether_alive_, gameObject);
    
    //-----------------------------------------

    static void apply_news_(int need)//这个参数仅是一种事件声明，其内容不会有作用
    {
        if (!news.ContainsKey(need))//同一种事件只会存在一个
            news.Add(need, new Dictionary<GameObject, List<Delegate>>());
    }

    static void apply_news_(int need, GameObject self)//需要先确保need已经申请到事件列表了
    {
        if (!news[need].ContainsKey(self))
            news[need][self] = new List<Delegate>();
    }

    //=======================================

}

