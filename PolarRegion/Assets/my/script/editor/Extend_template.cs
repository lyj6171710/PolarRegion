//using UnityEngine;
//using UnityEditor;

////此类作为原始模板，不进行修改优化

//[CustomEditor(typeof(Def_tower_atker))]//关联之前的脚本
//public class Extend_tower : Editor
//{
//    private SerializedObject test;//序列法
//    private SerializedProperty enum_cast, tmp_dir, tmp_side, tmp_gap;//定义类型，变量a，变量b

//    private Def_tower_atker test2;//引用法
//    private float float_dir, float_side;

//    void OnEnable()
//    {
//        test = new SerializedObject(target);
//        enum_cast = test.FindProperty("cast");
//        tmp_dir = test.FindProperty("offset_dir");
//        tmp_side = test.FindProperty("offset_side");
//        tmp_gap = test.FindProperty("shoot_gap");

//        test2 = (Def_tower_atker)target;
//    }
    
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        //序列法===============================

//        test.Update();//更新test，没有此项时，两个同指向变量，一方变量的值不会随着另一方变量值的修改而修改
//        EditorGUILayout.Space();
//        if (enum_cast.enumValueIndex==1)
//            EditorGUILayout.PropertyField(tmp_gap);
//        else if (enum_cast.enumValueIndex == 2)
//            EditorGUILayout.PropertyField(tmp_side);
//        else if (enum_cast.enumValueIndex == 3)
//            EditorGUILayout.PropertyField(tmp_dir);
//        test.ApplyModifiedProperties();//应用，应该是将面板上的赋值，真正转化传递到游戏程序预置状态中去

//        //引用法==============================

//        //test2.cast = (B_Cast)EditorGUILayout.EnumPopup("cast", test2.cast);
//        //还不清楚该语句意义，只知道可以创建枚举变量的弹出菜单，如果cast属性在本类中是隐藏的，该语句可能就有用
//        //switch (test2.cast)
//        //{
//        //    case B_Cast.连发:
//        //        test2.shoot_gap = EditorGUILayout.DoubleField("shoot_gap", test2.shoot_gap); break;
//        //    case B_Cast.平行:
//        //        test2.offset_side = EditorGUILayout.FloatField("offset_side", test2.offset_side);//注意需再赋值给用来创建赋值域的变量
//        //        break;
//        //    case B_Cast.散发:
//        //        test2.offset_dir = EditorGUILayout.FloatField("offset_dir", test2.offset_dir);
//        //        break;
//        //} //这种方式，不知道为什么，重启编辑器后，就变回去了，暂时无解
//    }

//}