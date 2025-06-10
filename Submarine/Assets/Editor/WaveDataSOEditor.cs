using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveDataSO))]
public class WaveDataSOEditor : Editor
{
    private SerializedProperty waveDurationProp;
    private SerializedProperty subWavesProp;
    private SerializedProperty clearRewardItemProp;
    private SerializedProperty clearRewardQuantityProp;

    protected virtual void OnEnable()
    {
        waveDurationProp = serializedObject.FindProperty("waveDuration");
        subWavesProp = serializedObject.FindProperty("subWaves");
        clearRewardItemProp = serializedObject.FindProperty("clearRewardItem");
        clearRewardQuantityProp = serializedObject.FindProperty("clearRewardQuantity");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("웨이브 설정", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(waveDurationProp);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("웨이브 클리어 보상", EditorStyles.boldLabel); 
        EditorGUILayout.PropertyField(clearRewardItemProp, new GUIContent("보상 아이템")); 
        EditorGUILayout.PropertyField(clearRewardQuantityProp, new GUIContent("보상 수량")); 

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("서브 웨이브 리스트", EditorStyles.boldLabel);

        for (int i = 0; i < subWavesProp.arraySize; i++)
        {
            SerializedProperty subWaveProp = subWavesProp.GetArrayElementAtIndex(i);
            SerializedProperty spawnTimeProp = subWaveProp.FindPropertyRelative("spawnTime");
            SerializedProperty enemySpawnDatasProp = subWaveProp.FindPropertyRelative("enemySpawnDatas");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"서브웨이브 {i + 1}", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(spawnTimeProp, new GUIContent("스폰 시간"));

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("적 스폰 데이터", EditorStyles.miniBoldLabel);

            for (int j = 0; j < enemySpawnDatasProp.arraySize; j++)
            {
                SerializedProperty dataProp = enemySpawnDatasProp.GetArrayElementAtIndex(j);
                SerializedProperty prefabProp = dataProp.FindPropertyRelative("enemyPrefab");
                SerializedProperty countProp = dataProp.FindPropertyRelative("spawnCount");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(prefabProp, GUIContent.none);
                EditorGUILayout.PropertyField(countProp, GUIContent.none);
                if (GUILayout.Button("삭제", GUILayout.Width(50)))
                {
                    enemySpawnDatasProp.DeleteArrayElementAtIndex(j);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("적 스폰 추가"))
            {
                enemySpawnDatasProp.InsertArrayElementAtIndex(enemySpawnDatasProp.arraySize);
            }

            if (GUILayout.Button("이 서브웨이브 삭제"))
            {
                subWavesProp.DeleteArrayElementAtIndex(i);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (GUILayout.Button("서브웨이브 추가"))
        {
            subWavesProp.InsertArrayElementAtIndex(subWavesProp.arraySize);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
