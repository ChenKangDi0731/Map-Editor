using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitConfig))]
public class UnitConfigEditor : CustomsInspector
{

    UnitConfig config;
    int preHp = -1;

    private void OnEnable()
    {
        config = (UnitConfig)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(config!=null && config.unit != null)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Unit State : " + config.unit.unitActiveState);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("UnitID : " + config.unit.unitData.unitID);//id

            EditorGUILayout.LabelField("UnitType : " + config.unit.unitData.unitType);//type

            EditorGUILayout.LabelField("UnitHp : " + config.unit.unitData.curHp + " / " + config.unit.unitData.maxHP);//hp
            config.unit.unitData.curHp = EditorGUILayout.IntField("Set UnitHp", config.unit.unitData.curHp);//set hp

            EditorGUILayout.LabelField("UnitAtk : " + config.unit.unitData.Atk);//atk

            EditorGUILayout.EndVertical();

            if (preHp != config.unit.unitData.curHp)
            {
                preHp = config.unit.unitData.curHp;
                Repaint();
            }

        }

    }

}
