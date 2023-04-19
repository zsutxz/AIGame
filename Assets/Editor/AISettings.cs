using UnityEngine;
using UnityEditor;
namespace OpenAI
{
    [FilePath("UserSettings/AISettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public sealed class AISettings : ScriptableSingleton<AISettings>
    {
        public string apiKey = null;
        public int timeout = 0;
        public void Save() => Save(true);
        void OnDisable() => Save();
    }

    sealed class AISettingsProvider : SettingsProvider
    {
        public AISettingsProvider()
          : base("Project/AI Setting", SettingsScope.Project) { }

        public override void OnGUI(string search)
        {
            var settings = AISettings.instance;

            var key = settings.apiKey;
            var timeout = settings.timeout;

            EditorGUI.BeginChangeCheck();

            key = EditorGUILayout.TextField("API Key", key);
            timeout = EditorGUILayout.IntField("Timeout", timeout);

            if (EditorGUI.EndChangeCheck())
            {
                settings.apiKey = key;
                settings.timeout = timeout;
                settings.Save();
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateCustomSettingsProvider()
          => new AISettingsProvider();
    }
}

