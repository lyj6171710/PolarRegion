using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MultEnum))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.intValue = EditorGUI.MaskField(
            position, label, property.intValue, property.enumNames);

        //Debug.Log("Í¼²ãµÄÖµ£º" + property.intValue);
    }
}
