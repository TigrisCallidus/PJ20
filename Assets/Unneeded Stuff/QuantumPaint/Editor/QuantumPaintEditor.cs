// -*- coding: utf-8 -*-

// This code is part of Qiskit.
//
// (C) Copyright IBM 2020.
//
// This code is licensed under the Apache License, Version 2.0. You may
// obtain a copy of this license in the LICENSE.txt file in the root directory
// of this source tree or at http://www.apache.org/licenses/LICENSE-2.0.
//
// Any modifications or derivative works of this code must retain this
// copyright notice, and modified files need to carry a notice indicating
// that they have been altered from the originals.using System;

using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(QuantumPaint))]
//Custom Editor for the QuantumBlurUnity class, adding some buttons and a representation of the Maze
public class QuantumPaintEditor : Editor {

    QuantumPaint targetScript;

    SerializedProperty size;
    SerializedProperty color;
    SerializedProperty type;
    SerializedProperty strength;
    //SerializedProperty axisToReflect;

    SerializedProperty showAdvanced;

    SerializedProperty useSimpleEncoding;
    SerializedProperty renormalizeImage;
    SerializedProperty Circuit;
    SerializedProperty Gates;

    SerializedProperty FileName;



    void OnEnable() {
        
        targetScript = target as QuantumPaint;

        size = serializedObject.FindProperty(nameof(targetScript.Size));
        color = serializedObject.FindProperty(nameof(targetScript.PaintColor));
        type = serializedObject.FindProperty(nameof(targetScript.Type));
        strength = serializedObject.FindProperty(nameof(targetScript.Strength));
        //axisToReflect = serializedObject.FindProperty(nameof(targetScript.AxisToReflect));

        showAdvanced = serializedObject.FindProperty(nameof(targetScript.ShowAdvanced));

        useSimpleEncoding = serializedObject.FindProperty(nameof(targetScript.UseSimpleEncoding));
        renormalizeImage = serializedObject.FindProperty(nameof(targetScript.RenormalizeImage));
        Circuit = serializedObject.FindProperty(nameof(targetScript.Circuit));
        Gates = serializedObject.FindProperty(nameof(targetScript.Gates));

        FileName = serializedObject.FindProperty(nameof(targetScript.FileName));

        
    }


    public override void OnInspectorGUI() {


        // Let the default inspecter draw all the values
        //DrawDefaultInspector();

        int lastAxis = targetScript.AxisToReflect;
        QuantumPaint.ReflectionType lastType = targetScript.Type;


        EditorGUILayout.PropertyField(size);

        EditorGUILayout.Space();

        if (GUILayout.Button("Initialize Image")) {
            targetScript.InitializeImage();
        }
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(type);
        EditorGUILayout.PropertyField(color);
        EditorGUILayout.PropertyField(strength);
        targetScript.AxisToReflect= EditorGUILayout.IntSlider("Axis to Reflect", targetScript.AxisToReflect, 1, targetScript.logSize);

        EditorGUILayout.Space();

        if (GUILayout.Button("Add Reflection")) {
            targetScript.AddGate();
        }
        if (GUILayout.Button("Undo")) {
            targetScript.Undo();
        }

        EditorGUILayout.PropertyField(showAdvanced);

        if (targetScript.ShowAdvanced) {
            EditorGUILayout.PropertyField(useSimpleEncoding);
            EditorGUILayout.PropertyField(renormalizeImage);
            EditorGUILayout.PropertyField(Circuit);
            EditorGUILayout.PropertyField(Gates);

        }


        EditorGUILayout.PropertyField(FileName);
        EditorGUILayout.Space();

        if (GUILayout.Button("Save File")) {
            targetScript.SaveFile();
            AssetDatabase.Refresh();

        }
        serializedObject.ApplyModifiedProperties();

        if (lastAxis!=targetScript.AxisToReflect || lastType !=targetScript.Type) {
            targetScript.ShowPreview();
        }



        // Spawn buttons






    }








}