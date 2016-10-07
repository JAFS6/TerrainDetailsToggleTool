/*
MIT License

Copyright (c) 2016 Juan Antonio Fajardo Serrano

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace TerrainDetailsToggleTool
{
    public sealed class TerrainDetailsToggleToolWindow : EditorWindow
    {
        private const int WindowWidth = 400;
        private const int WindowHeight = 300;
        private const int AutoDetectTerrainsButtonWidth = 150;
        private const int ScrollHeight = 225;
        private Vector2 ScrollPosition;
        private const int TerrainTitleLabelWidth = 200;
        private const int MaxNameLabelWidth = 280;
        private const int ToggleWidth = 50;
        private const string FirstTimeDetectButtonLabel = "Auto detect Terrains";
        private const string DetectButtonLabel = "Refresh Terrain list";

        private static List<Terrain> Terrains;
        private static bool AllTerrainsDrawDetailsOn;

        [MenuItem("Window/TerrainDetailsToggle Tool")]
        private static void OpenToolWindow ()
        {
            TerrainDetailsToggleToolWindow window = EditorWindow.GetWindow<TerrainDetailsToggleToolWindow>();
            window.minSize = new Vector2(WindowWidth, WindowHeight);
            window.maxSize = new Vector2(WindowWidth, WindowHeight);
            window.autoRepaintOnSceneChange = true;
            window.Show();
        }

        private void OnGUI ()
        {
            if (Terrains == null)
            {
                Terrains = new List<Terrain>();
            }

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();

            if (GUILayout.Button((Terrains.Count > 0) ? DetectButtonLabel : FirstTimeDetectButtonLabel, GUILayout.Width(AutoDetectTerrainsButtonWidth)))
            {
                DetectTerrains();
                CheckAllTerrainsDrawStatus();
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Terrain(s)", EditorStyles.boldLabel, GUILayout.Width(TerrainTitleLabelWidth));
            EditorGUILayout.LabelField("Draw Tree & Detail Objects", EditorStyles.boldLabel, GUILayout.Width(MaxNameLabelWidth));
            EditorGUILayout.EndHorizontal();

            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, false, false, GUILayout.Height(ScrollHeight));

            bool repaintNeeded = false;
            bool drawDetailsBefore, drawDetailsAfter;

            if (Terrains.Count > 0)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField("All terrains", EditorStyles.boldLabel, GUILayout.Width(MaxNameLabelWidth));
                drawDetailsBefore = AllTerrainsDrawDetailsOn;
                drawDetailsAfter = EditorGUILayout.Toggle("", drawDetailsBefore, GUILayout.Width(ToggleWidth));
                AllTerrainsDrawDetailsOn = drawDetailsAfter;

                if (drawDetailsBefore != drawDetailsAfter)
                {
                    repaintNeeded = true;

                    bool newValue = AllTerrainsDrawDetailsOn;

                    for (int i = 0; i < Terrains.Count; i++)
                    {
                        Terrains[i].drawTreesAndFoliage = newValue;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            for (int i = 0; Terrains != null && i < Terrains.Count; i++)
            {
                if (Terrains[i] != null)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                    string name = Terrains[i].gameObject.name;

                    if (name == "")
                    {
                        name = "<GameObject without name>";
                    }

                    EditorGUILayout.LabelField(name, EditorStyles.label, GUILayout.Width(MaxNameLabelWidth));

                    drawDetailsBefore = Terrains[i].drawTreesAndFoliage;

                    drawDetailsAfter = EditorGUILayout.Toggle("", drawDetailsBefore, GUILayout.Width(ToggleWidth));

                    Terrains[i].drawTreesAndFoliage = drawDetailsAfter;

                    if (drawDetailsBefore != drawDetailsAfter)
                    {
                        repaintNeeded = true;
                        Selection.activeTransform = Selection.activeTransform;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            if (repaintNeeded)
            {
                SceneView.RepaintAll();
            }
        }

        private void DetectTerrains ()
        {
            // Clean list and detect all Terrain components

            Terrains.Clear();

            Terrain[] terrainsFound = FindObjectsOfType(typeof(Terrain)) as Terrain[];

            // Get a ordered list of terrains to show in the window

            SortedList orderedTerrains = new SortedList();

            for (int i = 0; i < terrainsFound.Length; i++)
            {
                orderedTerrains.Add(terrainsFound[i].gameObject.transform.GetSiblingIndex(), terrainsFound[i]);
            }

            for (int i = 0; i < terrainsFound.Length; i++)
            {
                Terrains.Add((Terrain)orderedTerrains.GetByIndex(i));
            }
        }

        private void CheckAllTerrainsDrawStatus ()
        {
            // This method sets AllTerrainsDrawDetailsOn to false if all terrains have the Draw details property set to false
            // In other case, AllTerrainsDrawDetailsOn will be true

            AllTerrainsDrawDetailsOn = false;

            for (int i = 0; !AllTerrainsDrawDetailsOn && i < Terrains.Count; i++)
            {
                if (Terrains[i].drawTreesAndFoliage)
                {
                    AllTerrainsDrawDetailsOn = true;
                }
            }
        }
    }
}
