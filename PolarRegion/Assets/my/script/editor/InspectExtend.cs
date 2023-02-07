//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//public class InspectExtend : Editor {//为修改inspect显示内容的基本工具提供

//    protected SerializedObject self;//序列法
//    private SerializedProperty[] vars;//这个不支持显示类变量
//                                      //如果想要支持显示类变量，通过反射获取类的所有属性的字符串表示
//                                      //同时全部包装进一个变量组中，在需要显示时一并显示在一个折叠栏下foldout
//    private string[] vars_name;

//    //内外机制===================================================

//    private void OnEnable()
//    {
//        self = new SerializedObject(target);
//        OnEnable_pre_();//铺垫var_name
//        vars = new SerializedProperty[vars_name.Length];
//        for (int i = 0; i < vars_name.Length; i++)
//            vars[i] = self.FindProperty(vars_name[i]);

//        OnEnable_post_();
//    }
//    protected virtual void OnEnable_pre_() { }
//    protected virtual void OnEnable_post_() { }

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
        
//        self.Update();//更新test，没有此项时，两个同指向变量，一方变量的值不会随着另一方变量值的修改而修改
        
//        alter_();

//        self.ApplyModifiedProperties();//应用，应该是将面板上的赋值，真正转化传递到游戏程序预置状态中去
//    }
//    protected virtual void alter_() { }

//    //继承工具============================================

//    protected void make_space_()
//    {
//        EditorGUILayout.Space();
//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("以下是当前需补充填写的属性");
//    }

//    protected void load_var_(string[] vars){
//        vars_name = vars;
//    }

//    protected SerializedProperty get_var_(string need)
//    {
//        for (int i = 0; i < vars_name.Length; i++) {
//            if (vars_name[i] == need) return vars[i];
//        }
//        return null;
//    }

//    protected void get_show_(string[] vars)
//    {
//        foreach (string var in vars)
//            EditorGUILayout.PropertyField(get_var_(var));
//    }

//    protected void get_show_(string var)
//    {
//        EditorGUILayout.PropertyField(get_var_(var), true);
//    }

//    //===================================================
//}

///*使用举例
//using UnityEditor;

//[CustomEditor(typeof(Def_role_act))]//关联之前的脚本
//public class Extend_role_act : InspectExtend
//{
//    protected override void OnEnable_pre_()
//    {
//        load_var_(new string[3] { "trim_collid", "offset", "size" });
//    }

//    protected override void alter_()
//    {
//        make_space_();

//        if (get_var_("trim_collid").boolValue == true)
//            get_show_(new string[2] { "offset", "size" });
//    }
//}
// */
