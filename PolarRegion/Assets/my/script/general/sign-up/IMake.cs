using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISignUpMake { }
//make函数，全应被上级或外界调用，作用于子级
//子级自己最多只能在make函数中调用自己另外有的make函数

//make服务于从上到下全面控制的体系
//make也内含可选化，如果不是可选的，就不要用

interface ISUMakeReady
{
    //只存在预备或解除，全局常驻

    void MakeReady();
    //准备即使用，或者，准备仅仅是一些基础工作，对外低依赖或初始化后不会再变动的

    void MakeNeed();
    //开始工作后，也不一定会接到任务，而没事可做
    //这里就可以用于外界对自己分发任务，本地随时准备接受这些任务并完成它

    void MakeClean();
    //清理当前工作内容，但还是处于工作状态，
    //工作输出可能会继续产生，而不再处于clean状态。
    //只是部分需要做的内容已经做完或不需要做了，还有事要做的。

    void MakeRemove();
    //与准备工作相互对应，即解散自己需要做的工作，给外界一个交代
    //建议作为自毁程序的接口
}

//======================================

interface ISUMakeUse
{
    //不破坏使用环境，可以暂时使用与不使用
    //但每次使用，只存在开始和结束，无法中断

    void MakeOpen();
    //阶段上处于准备工作之后，投入使用之前。
    //先仅展示给外界，可有最基础的对外交流、影响
    //给外界一个准备开工的招呼，以及能吸引外界做好对接的准备与信心

    void MakeUse();
    //准备工作做好后，不一定会立即开工，从这里开工

    void MakeDown();
    //结束工作，但并不打算解散关闭，还可以通过MakeUse再次开工
    //不过一旦结束，下次使用时，就只有重新开做，
    //但至少不需要下次开工时还需要先做好准备工作

    void MakeClose();
    //如果已经处于close状态，就不会有什么效果
    //不论怎样，直通不可用状态，而且同时还偏向不展示给外界看了

    //----------------------------------------

    void MakeCome();
    //相对于MakeUse，还不确保已经ready以及open，
    //如果没有ready好，就会自动先ready再use
    //如果没有open好，就会自动先open再use
    //如果已经处于come状态，就不会有什么效果

    //简而言之，就是不论怎样，保证不出问题地直通use状态，
    //外界只管用，不需要理解内部状态，主要好处是，被使用者可以不忙着提前准备

    void MakeLeave();
    //在已经做好准备的前提下(有过至少一次come)，保证不出问题地直通close状态
    //有了Come和Leave，基本不需要MakeOpen和MakeClose的存在，只需有相关的状态标记就行了
}

//=====================================

interface ISUMakeStep
{
    //可暂停、中断的工作

    void MakeBegin();
    //开始行动

    void MakeContinue();
    //从上次停下来的地方继续做
    
    void MakePause();
    //停下、暂停当前工作，但工作产生的资料都还保留着，以便继续

    void MakeAppend();
    //在当前工作任务的基础上，保持原量并增加工作任务
}

//=====================================

public interface ISignUpTake { }
//take函数，被子级调用，不被自己亲自调用，但作用于子级或自己，自己负责把关

interface ISUTakeBack
{
    void TakeBack();//申请回溯一级

    void TakeForward();//申请向内进入某个直辖部门

    //----------------------------------
    enum ELevelDvt { Hard, Soft, Side }
    void TakeSubDvt(ELevelDvt lv);//dvt等同于Divert
    //Hard:子级部门间的转移，一定伴随被转移部门关闭，新轮到部门开启且启动
    //Soft:子级部门间的转移，伴随被转移部门失效，新轮到部门开启且启动
    //Side:子级部门要求启动与其同级同属的另一个部门
}

//=====================================

public interface ISignUpTurn { }
//turn函数，因机制需要而被自己调用，作用于自己，可能对外散发影响

interface ISUTurnOn
{
    void TurnOn();

    void TurnOff();
}

//===================================

public interface ISignUpFollow { }
//follow函数，被外界调用
//与make类似，但非可选，与机制强相关，
//但却被分拆出来了，为了继续体现强相关，使用follow来内含