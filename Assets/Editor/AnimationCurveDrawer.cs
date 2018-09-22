using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimationCurve))]
public sealed class AnimationCurveDrawer : PropertyDrawer
{
    private static AnimationCurve m_copiedCurve;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var curveFieldPos = position;
        curveFieldPos.width -= 18;

        label = EditorGUI.BeginProperty(position, label, property);
        EditorGUI.CurveField(curveFieldPos, property, Color.green, Rect.zero);

        var menuPos = position;
        menuPos.xMin = menuPos.xMax - 13;

        if (GUI.Button(menuPos, GUIContent.none, "ShurikenDropdown"))
        {
            var content1 = new GUIContent("Copy");
            var content2 = new GUIContent("Paste");

            var genericMenu = new GenericMenu();
            genericMenu.AddItem(content1, false, OnCopy, property);
            genericMenu.AddItem(content2, false, OnPaste, property);

            if (m_copiedCurve == null)
            {
                genericMenu.AddDisabledItem(content2);
            }

            genericMenu.DropDown(menuPos);
        }

        EditorGUI.EndProperty();
    }

    private static void OnCopy(object data)
    {
        var property = data as SerializedProperty;
        m_copiedCurve = property.animationCurveValue;
    }

    private static void OnPaste(object data)
    {
        if (m_copiedCurve == null) return;

        var property = data as SerializedProperty;
        property.serializedObject.Update();
        property.animationCurveValue = m_copiedCurve;
        property.serializedObject.ApplyModifiedProperties();
    }
}