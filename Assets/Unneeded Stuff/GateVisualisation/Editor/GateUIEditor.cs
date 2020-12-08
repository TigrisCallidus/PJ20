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
// that they have been altered from the originals.

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GateUIController))]
//Custom Editor for the QuantumBlurUnity class, adding some buttons and a representation of the Maze
public class GateUIEditor : Editor {

    GateUIController targetScript;

    void OnEnable() {
        targetScript = target as GateUIController;
    }

    public override void OnInspectorGUI() {

        // Let the default inspecter draw all the values
        DrawDefaultInspector();

        // Spawn buttons

        if (GUILayout.Button("Test")) {
            targetScript.Test();
        }

        if (GUILayout.Button("Clear")) {
            targetScript.Clear();
        }

    }
}