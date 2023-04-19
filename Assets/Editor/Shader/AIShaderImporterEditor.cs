using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEditor.Experimental.AssetImporters;

using OpenAI;

namespace AIShader
{
    [CustomEditor(typeof(AIShaderImporter))]
    sealed class AIShaderImporterEditor : ScriptedImporterEditor
    {
        #region Private members

        SerializedProperty _prompt;
        SerializedProperty _cached;

        static string WrapPrompt(string input)
          => "Create an unlit shader for Unity. " + input +
             " Don't include any note nor explanation in the response." +
             " I only need the code body.";

        void Regenerate()
          => _cached.stringValue = OpenAIUtil.InvokeChat(WrapPrompt(_prompt.stringValue));

        #endregion

        #region ScriptedImporterEditor overrides

        public override void OnEnable()
        {
            base.OnEnable();
            _prompt = serializedObject.FindProperty("_prompt");
            _cached = serializedObject.FindProperty("_cached");
        }

        public override void OnInspectorGUI()
        {
            // Intro
            serializedObject.Update();

            // Prompt text area
            EditorGUILayout.PropertyField(_prompt);

            // "Generate" button
            EditorGUI.BeginDisabledGroup(!OpenAIUtil.IsApiKeyOk);
            if (GUILayout.Button("Generate")) Regenerate();
            EditorGUI.EndDisabledGroup();

            // Missing API key error
            if (!OpenAIUtil.IsApiKeyOk) EditorGUILayout.HelpBox(OpenAIUtil.ApiKeyErrorText, MessageType.Error);

            // Cached code text area
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_cached);

            // Outro
            serializedObject.ApplyModifiedProperties();
            ApplyRevertGUI();
        }

        #endregion
    }

} // namespace AIShader
