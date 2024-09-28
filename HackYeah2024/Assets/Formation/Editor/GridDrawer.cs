using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// IngredientDrawerUIE
[CustomPropertyDrawer(typeof(FormationData.Grid))]
public class GridDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);
        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        GUILayout.BeginVertical();
        for(var y = 0; y < 10; y++)
        {
            GUILayout.BeginHorizontal();
            for(var x = 0; x < 10; x++)
            {
                var ind = y*10+x;
                var gridProp = property.FindPropertyRelative("Values").GetArrayElementAtIndex(ind).boolValue;
                GUI.color = gridProp ? Color.red : Color.white;
                if(GUILayout.Button(gridProp ? "X" : "", GUILayout.Width(25), GUILayout.Height(25)))
                    property.FindPropertyRelative("Values").GetArrayElementAtIndex(ind).boolValue = !gridProp;
            }
            GUILayout.EndHorizontal();

        }
        GUILayout.EndVertical();


        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}