using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomsInspector : Editor
{
    protected void DrawPropertyFiled(string filedName)
    {
        DrawPropertyFiled(filedName, true, true);

    }

    protected void DrawPropertyFiled(string filedName,bool isValid,bool isValidWarning)
    {

        WrapWithValidationColor(() =>
        {
            SerializedProperty propety = serializedObject.FindProperty(filedName);
            EditorGUILayout.PropertyField(propety);
        },isValid,isValidWarning);

    }

    protected void WrapWithValidationColor(System.Action method,bool isValid,bool isValidWarning)
    {
        Color colorBackup = GUI.color;
        if (isValid == false)
        {
            GUI.color = Color.red;
        }else if (isValidWarning == false)
        {
            GUI.color = Color.yellow;
        }

        method?.Invoke();
        GUI.color = colorBackup;
    }
}
