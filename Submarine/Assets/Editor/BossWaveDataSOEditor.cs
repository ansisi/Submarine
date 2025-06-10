using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BossWaveDataSO))]
public class BossWaveDataSOEditor : WaveDataSOEditor
{
    private SerializedProperty bossPrefabProp;
    private SerializedProperty spawnPointProp;
    private SerializedProperty bossDelayTimeProp;

    protected override void OnEnable()
    {
        base.OnEnable(); // 부모 에디터의 OnEnable 호출

        bossPrefabProp = serializedObject.FindProperty("bossPrefab");
        spawnPointProp = serializedObject.FindProperty("spawnPoint");
        bossDelayTimeProp = serializedObject.FindProperty("bossDelayTime");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI(); // 기본 웨이브 설정 UI 출력

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("보스 웨이브 추가 설정", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(bossPrefabProp, new GUIContent("보스 프리팹"));
        EditorGUILayout.PropertyField(spawnPointProp, new GUIContent("보스 소환 위치"));
        EditorGUILayout.PropertyField(bossDelayTimeProp, new GUIContent("보스 등장 딜레이"));

        serializedObject.ApplyModifiedProperties();
    }
}
