using System;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VoxSupport.Editor
{
    [CustomEditor(typeof(VoxAssetImporter))]
    [CanEditMultipleObjects]
    public class VoxAssetImporterEditor : ScriptedImporterEditor
    {
        private int _selectedOptions;
        
        private static readonly GUIContent[] ToolbarValues =
        {
            new GUIContent("Model"), new GUIContent("Materials"), new GUIContent("Stats")
        };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            switch (DrawToolbar())
            {
                case 0:
                    DrawModelOptions();
                    break;

                case 1:
                    DrawMaterialOptions();
                    break;

                case 2:
                    DrawStats();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();
        }

        private int DrawToolbar()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _selectedOptions = GUILayout.Toolbar(_selectedOptions, ToolbarValues, GUI.skin.button,
                                                 GUI.ToolbarButtonSize.FitToContents, GUILayout.Height(25));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            return _selectedOptions;
        }

        private void DrawModelOptions()
        {
            string prefix = nameof(VoxAssetImporter.modelOptions) + ".";

            DrawProperty(prefix + nameof(ImportModelOptions.orientation));
            DrawProperty(prefix + nameof(ImportModelOptions.scale));
            DrawProperty(prefix + nameof(ImportModelOptions.transformMesh), "Apply transform");
            DrawProperty(prefix + nameof(ImportModelOptions.moveToFloor));
        }

        private void DrawMaterialOptions()
        {
            string prefix = nameof(VoxAssetImporter.materialsOptions) + ".";
            DrawProperty(prefix + nameof(ImportMaterialsOptions.material));

            var voxAssetImporter = (VoxAssetImporter) target;
            if (targets.Length > 1 || !voxAssetImporter.materialsOptions.material)
            {
                return;
            }

            int materialId = voxAssetImporter.materialsOptions.material.GetInstanceID();

            if (EditorPrefs.HasKey("VoxAssetImporterDefaultMaterial")
                && EditorPrefs.GetInt("VoxAssetImporterDefaultMaterial") == materialId)
            {
                return;
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Set as default", GUILayout.Width(96)))
            {
                EditorPrefs.SetInt("VoxAssetImporterDefaultMaterial", materialId);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawProperty(string propertyName)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName));
        }

        private void DrawProperty(string propertyName, string editorName)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propertyName), new GUIContent(editorName));
        }

        private void DrawStats()
        {
            string prefix = targets.Length > 1 ? "~" : "";
            ImportStats importStats = targets.Length > 1 ? GetMeanImportStats() : ((VoxAssetImporter) target).stats;

            EditorGUILayout.LabelField("Time", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Import time", $"{prefix}{Math.Round(importStats.importTime, 3)}s");
            EditorGUILayout.LabelField("Convert time", $"{prefix}{Math.Round(importStats.convertTime, 3)}s");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Vertex count", $"{prefix}{importStats.vertexCount}");
            EditorGUILayout.LabelField("Triangle count", $"{prefix}{importStats.triangleCount}");
            EditorGUILayout.LabelField("Face count", $"{prefix}{importStats.faceCount}");
        }

        private ImportStats GetMeanImportStats()
        {
            var meanImportStats = new ImportStats();

            foreach (Object nTarget in targets)
            {
                ImportStats importStats = ((VoxAssetImporter) nTarget).stats;
                meanImportStats.importTime += importStats.importTime;
                meanImportStats.convertTime += importStats.convertTime;
                meanImportStats.vertexCount += importStats.vertexCount;
                meanImportStats.triangleCount += importStats.triangleCount;
                meanImportStats.faceCount += importStats.faceCount;
            }

            meanImportStats.importTime /= targets.Length;
            meanImportStats.convertTime /= targets.Length;
            meanImportStats.vertexCount /= targets.Length;
            meanImportStats.triangleCount /= targets.Length;
            meanImportStats.faceCount /= targets.Length;

            return meanImportStats;
        }
    }
}