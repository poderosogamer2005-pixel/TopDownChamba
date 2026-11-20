using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaveSO))]
public class WaveSOEditor : Editor
{
    SerializedProperty _enemies;
    SerializedProperty _waveType;
    SerializedProperty _staticPositions;
    SerializedProperty _instantWave;
    SerializedProperty _timeBetweenEnemies;

    private bool waveGroup = false;
    private bool timeGroup = false;

    private void OnEnable()
    {
        _enemies = serializedObject.FindProperty("enemies");
        _waveType = serializedObject.FindProperty("waveType");
        _staticPositions = serializedObject.FindProperty("staticPositions");
        _instantWave = serializedObject.FindProperty("instantWave");
        _timeBetweenEnemies = serializedObject.FindProperty("timeBetweenEnemies");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        WaveSO targetReference = (WaveSO)target;

        //Variables Enemigo
        EditorGUILayout.PropertyField(_enemies);


        //Variable Wave
        waveGroup = EditorGUILayout.BeginFoldoutHeaderGroup(waveGroup, "Wave");
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (waveGroup)
        {
            EditorGUILayout.PropertyField(_waveType);
            if (targetReference.waveType == WaveType.Static)
            {
                EditorGUILayout.PropertyField(_staticPositions);
            }

        }

        //Variable tiempo
        timeGroup = EditorGUILayout.BeginFoldoutHeaderGroup(timeGroup, "time");
        EditorGUILayout.EndFoldoutHeaderGroup();
        if (timeGroup)
        {
            EditorGUILayout.PropertyField(_instantWave);
            if (targetReference.instantWave)
            {
                EditorGUILayout.PropertyField(_staticPositions);
            }

        }

        serializedObject.ApplyModifiedProperties();
    }
}
