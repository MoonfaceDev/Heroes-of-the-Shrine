/*
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using UnityEditor;
using UnityEngine;

namespace Borodar.RainbowFolders.Editor
{
    public class RainbowFoldersPreferences
    {
        private const string HomeFolderPrefKey = "Borodar.RainbowFolders.HomeFolder.";
        private const string HomeFolderDefault = "Assets/RainbowFolders";
        private const string HomeFolderHint = "Change this setting to the new location of the \"Rainbow Folders\" if you move the folder around in your project.";

        public static readonly EditorPrefsString HomeFolder = new(HomeFolderPrefKey + ProjectName, "Folder Location", HomeFolderDefault);

        //---------------------------------------------------------------------
        // Messages
        //---------------------------------------------------------------------

        private class EditorPreferencesSettingsProvider : SettingsProvider
        {
            public EditorPreferencesSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project) : base(path, scopes)
            {
                
            }

            public override void OnGUI(string searchContext)
            {
                EditorGUILayout.HelpBox(HomeFolderHint, MessageType.Info);
                EditorGUILayout.Separator();
                HomeFolder.Draw();
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("Version " + AssetInfo.VERSION, EditorStyles.centeredGreyMiniLabel);
            }
        }

        [SettingsProvider]
        private static SettingsProvider EditorPreferences()
        {
            return new EditorPreferencesSettingsProvider("Rainbow Folders");
        }

        //---------------------------------------------------------------------
        // Helpers
        //---------------------------------------------------------------------

        private static string ProjectName
        {
            get
            {
                var s = Application.dataPath.Split('/');
                var p = s[^2];
                return p;
            }
        }

        //---------------------------------------------------------------------
        // Nested
        //---------------------------------------------------------------------

        public abstract class EditorPrefsItem<T>
        {
            protected readonly string key;
            protected readonly string label;
            protected readonly T defaultValue;

            protected EditorPrefsItem(string key, string label, T defaultValue)
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentNullException(nameof(key));
                }

                this.key = key;
                this.label = label;
                this.defaultValue = defaultValue;
            }

            public abstract T Value { get; set; }
            public abstract void Draw();

            public static implicit operator T(EditorPrefsItem<T> s)
            {
                return s.Value;
            }
        }

        public class EditorPrefsString : EditorPrefsItem<string>
        {
            public EditorPrefsString(string key, string label, string defaultValue)
                : base(key, label, defaultValue)
            {
            }

            public override string Value
            {
                get => EditorPrefs.GetString(key, defaultValue); 
                set => EditorPrefs.SetString(key, value);
            }

            public override void Draw()
            {
                EditorGUIUtility.labelWidth = 100f;
                Value = EditorGUILayout.TextField(label, Value);
            }
        }
    }
}