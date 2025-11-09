using System;
using Editor.Helpers;
using UnityEditor;
using UnityEngine;

namespace Project.Expressions
{
    [CustomPropertyDrawer(typeof(BoolExpression))]
    [CustomPropertyDrawer(typeof(FloatExpression))]
    [CustomPropertyDrawer(typeof(IntExpression))]
    [CustomPropertyDrawer(typeof(ComponentExpression))]
    [CustomPropertyDrawer(typeof(GameObjectExpression))]
    public class ExpressionDrawer : PropertyDrawer
    {
        const string s_PropertyName = "m_ExpressionRef";


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var conditionProp = property.FindPropertyRelative(s_PropertyName);

            if (conditionProp.managedReferenceValue == null)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUI.GetPropertyHeight(conditionProp, true);

        }


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var conditionProp = property.FindPropertyRelative(s_PropertyName);

            // position = EditorGUI.PrefixLabel(new Rect(position) { x = 0, y = 0 }, label); this does not work with the BeginClip() below
            if (label != GUIContent.none)
            {
                var labelRect = position.CutLeft(EditorGUIUtility.labelWidth, out position);
                labelRect.height = EditorGUIUtility.singleLineHeight;
                GUI.Label(labelRect, label);
            }

            
            var left = position.CutLeft(8, out position);
            left.height = EditorGUIUtility.singleLineHeight;
            var hover = left.Contains(Event.current.mousePosition);
            using (new GUI.ClipScope(left))
            {
                var buttonRect = new Rect(-4, 1, 16, left.height);
                if (GUI.Button(buttonRect, EditorGUIUtility.IconContent("d_more"), EditorStyles.iconButton))
                {
                    var menu = ManagedReferenceDrawerHelper.TypeSelectorMenu(conditionProp, buttonRect);

                    // TODO: find all IWrapable implementations for type

                    menu.AddSeparator("");

                    var wrappables = ReflectionHelper.GetAllGenericImplementationsTypes(conditionProp.managedReferenceValue, typeof(IWrapable<>));

                    foreach (var wrappableTypes in wrappables)
                    {
                        var scopedWrappableType = wrappableTypes;
                        menu.AddItem(new GUIContent("Wrap with " + scopedWrappableType.Name), false, () =>
                        {
                            var method = typeof(IWrapable<>).MakeGenericType(scopedWrappableType).GetMethod("Wrap");
                            conditionProp.managedReferenceValue = method.Invoke(conditionProp.managedReferenceValue, Array.Empty<object>());
                            conditionProp.serializedObject.ApplyModifiedProperties();

                        });
                    }

                    menu.DropDown(buttonRect);
                }
            }

            if (conditionProp.managedReferenceValue == null)
            {
                EditorGUI.HelpBox(position, "null", MessageType.Info);
                return;
            }

            EditorGUI.PropertyField(position, conditionProp, GUIContent.none);

            if (hover)
            {
                EditorGUI.DrawRect(position, new Color(0.2f, 0.6f, 1f, 0.1f));
            }
        }

    }


}
