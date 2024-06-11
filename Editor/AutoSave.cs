using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UnityAutoSave.AutoSave
{
    [InitializeOnLoad]
    static class AutoSaveExtension
    {
        private static DateTime lastSaveTime;
        private static readonly TimeSpan saveInterval = TimeSpan.FromMinutes(10);
        private static bool isAutoSaveEnabled;

        static AutoSaveExtension()
        {
            // Load the auto-save state from EditorPrefs
            isAutoSaveEnabled = EditorPrefs.GetBool("AutoSaveEnabled", true);

            // Subscribe to the playModeStateChanged event
            EditorApplication.playModeStateChanged -= AutoSaveWhenPlaymodeStarts;
            EditorApplication.playModeStateChanged += AutoSaveWhenPlaymodeStarts;

            // Initialize the last save time and subscribe to the update event
            lastSaveTime = DateTime.Now;
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }

        [MenuItem("Tools/Auto Save/Enable Auto Save")]
        private static void EnableAutoSave()
        {
            isAutoSaveEnabled = true;
            EditorPrefs.SetBool("AutoSaveEnabled", true);
            Debug.Log("Auto Save Enabled");
        }

        [MenuItem("Tools/Auto Save/Enable Auto Save", true)]
        private static bool EnableAutoSaveValidate()
        {
            return !isAutoSaveEnabled;
        }

        [MenuItem("Tools/Auto Save/Disable Auto Save")]
        private static void DisableAutoSave()
        {
            isAutoSaveEnabled = false;
            EditorPrefs.SetBool("AutoSaveEnabled", false);
            Debug.Log("Auto Save Disabled");
        }

        [MenuItem("Tools/Auto Save/Disable Auto Save", true)]
        private static bool DisableAutoSaveValidate()
        {
            return isAutoSaveEnabled;
        }

        private static void AutoSaveWhenPlaymodeStarts(PlayModeStateChange playModeStateChange)
        {
            if (isAutoSaveEnabled && playModeStateChange == PlayModeStateChange.ExitingEditMode)
            {
                Debug.Log("Saving Scenes and Assets");
                SaveAllOpenScenesAndAssets();
            }
        }

        private static void OnEditorUpdate()
        {
            if (isAutoSaveEnabled && DateTime.Now - lastSaveTime >= saveInterval)
            {
                Debug.Log("Auto-saving Scenes and Assets every 10 minutes");
                SaveAllOpenScenesAndAssets();
                lastSaveTime = DateTime.Now;
            }
        }

        private static void SaveAllOpenScenesAndAssets()
        {
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
        }
    }
}
