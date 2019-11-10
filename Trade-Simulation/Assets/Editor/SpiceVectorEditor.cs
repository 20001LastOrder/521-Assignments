using UnityEditor;
using UnityEngine;
[UnityEditor.CustomEditor(typeof(SpiceVector))]
public class SpiceVectorEditor : UnityEditor.Editor
{
    SerializedProperty spices;

    void OnEnable()
    {
        spices = serializedObject.FindProperty("Spices");
        UnityEngine.Debug.Log("sadasdasd");
    }


    public void ShowArrayProperty(UnityEditor.SerializedProperty list)
    {
        UnityEditor.EditorGUI.indentLevel += 1;
        for (int i = 0; i < list.arraySize; i++)
        {
            UnityEditor.EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new UnityEngine.GUIContent("Bla" + (i + 1).ToString()));
        }
        UnityEditor.EditorGUI.indentLevel -= 1;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ShowArrayProperty(serializedObject.FindProperty("NameOfListToView"));
    }
}