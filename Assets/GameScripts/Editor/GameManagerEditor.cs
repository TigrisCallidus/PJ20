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

[CustomEditor(typeof(GameManager))]
//Custom Editor for the QuantumBlurUnity class, adding some buttons and a representation of the Maze
public class GameManagerEditor : Editor {

    GameManager targetScript;

    void OnEnable() {
        targetScript = target as GameManager;
    }

    public override void OnInspectorGUI() {

        // Let the default inspecter draw all the values
        DrawDefaultInspector();

        // Spawn buttons

        if (GUILayout.Button("Set Player Settings")) {
            PlayerSettings.WebGL.emscriptenArgs = "-s ALLOW_MEMORY_GROWTH=1";
            PlayerSettings.WebGL.memorySize = 2048;
        }



    }
}