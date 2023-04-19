using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Text;

namespace OpenAI
{
    static class OpenAIUtil
    {
        public static bool IsApiKeyOk  => !string.IsNullOrEmpty(AISettings.instance.apiKey);

        public const string ApiKeyErrorText =
          "API Key hasn't been set. Please check the project settings " +
          "(Edit > Project Settings > AI Setting> API Key).";

        static string CreateChatRequestBody(string prompt)
        {
            var msg = new OpenAI.RequestMessage();
            msg.role = "user";
            msg.content = prompt;

            var req = new OpenAI.Request();
            req.model = "gpt-3.5-turbo";
            req.messages = new[] { msg };

            // string jsstr = "{\"model\":\"gpt-3.5-turbo\",\"messages\":[{\"role\":\"user\",\"content\":\"how old are you\"}]}";
            return JsonUtility.ToJson(req);
        }

        public static string InvokeChat(string prompt)
        {
            var settings = AISettings.instance;

            // // POST
            // using var post = UnityWebRequest.Post(OpenAI.Api.Url, CreateChatRequestBody(prompt), "application/json");
            // // Request timeout setting
            // post.timeout = settings.timeout;
            // // API key authorization
            // post.SetRequestHeader("Authorization", "Bearer " + settings.apiKey);

            //another post way
            var post = new UnityWebRequest(OpenAI.Api.Url, "POST");

            post.timeout = settings.timeout;
            post.SetRequestHeader("Content-Type", "application/json");
            post.SetRequestHeader("Authorization", string.Format("Bearer {0}", settings.apiKey));

            string str = CreateChatRequestBody(prompt);
            Debug.Log(str);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(str);
            post.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            post.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();


            // Request start
            var req = post.SendWebRequest();

            // Progress bar (Totally fake! Don't try this at home.)
            for (var progress = 0.0f; !req.isDone; progress += 0.01f)
            {
                EditorUtility.DisplayProgressBar("AI Command", "Generating...", progress);
                System.Threading.Thread.Sleep(100);
                progress += 0.01f;
            }

            EditorUtility.ClearProgressBar();

            if (post.isHttpError || post.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("isHttpError:"+post.isHttpError);
                Debug.Log("isNetworkError:"+post.isNetworkError);
                Debug.Log(post.error);
            }
            // Debug.Log("post.result:"+post.result);
            Debug.Log(post.downloadHandler.text);

            // Response extraction
            var json = post.downloadHandler.text;
            var data = JsonUtility.FromJson<OpenAI.Response>(json);

            return data.choices[0].message.content;
        }
    }

} // namespace OpenAI
