using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Text;
using Unity.EditorCoroutines.Editor;
using Newtonsoft.Json.Linq;
using System.Linq;

public class GoogleSheetDownloader : EditorWindow
{
    private const string apiKey = "AIzaSyAKS-p-SMyIRO8JY5t40aBompWApR7Uzoc";
    private const string sheetId = "1EhEbfvnNCpOwv7cx4O6EXhy4GU1GmpXnTguDf5Jkjro";
    private static readonly string savePath = "Assets/Resources/Data/";

    [MenuItem("Tools/Load Sheet Data")]
    public static void DownloadSheets()
    {
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        EditorCoroutineUtility.StartCoroutineOwnerless(DownloadSheetsRoutine());
    }

    private static IEnumerator DownloadSheetsRoutine()
    {
        string metaUrl = $"https://sheets.googleapis.com/v4/spreadsheets/{sheetId}?key={apiKey}";
        UnityWebRequest metaReq = UnityWebRequest.Get(metaUrl);

        EditorUtility.DisplayProgressBar("GoogleSheetDownloader", "Fetching sheet metadata...", 0f);
        yield return metaReq.SendWebRequest();

        if (metaReq.result != UnityWebRequest.Result.Success)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError($"[SheetLoader] Failed to get sheet meta: {metaReq.error}");
            yield break;
        }

        JObject json = JObject.Parse(metaReq.downloadHandler.text);
        var sheets = json["sheets"];
        int total = sheets.Count();
        int index = 0;

        foreach (var sheet in sheets)
        {
            string title = sheet["properties"]["title"].ToString();
            string gid = sheet["properties"]["sheetId"].ToString();

            string csvUrl = $"https://docs.google.com/spreadsheets/d/{sheetId}/export?format=csv&gid={gid}";
            string filePath = Path.Combine(savePath, $"{title}.csv");

            float progress = (float)index / total;
            EditorUtility.DisplayProgressBar("GoogleSheetDownloader", $"Downloading: {title}", progress);

            yield return DownloadAndSave(csvUrl, filePath);
            Debug.Log($"[SheetLoader] Saved: {filePath}");

            index++;
        }

        EditorUtility.DisplayProgressBar("GoogleSheetDownloader", "Refreshing assets...", 1f);
        AssetDatabase.Refresh();

        EditorUtility.ClearProgressBar();
        Debug.Log("[SheetLoader] All sheets downloaded successfully.");

        if (Application.isPlaying)
        {
            DataManager.Instance.ForceReload();
            Debug.Log("[SheetLoader] DataManager cache cleared, will reload new data.");
        }
    }

    private static IEnumerator DownloadAndSave(string url, string path)
    {
        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError($"[SheetLoader] Failed to download {url}: {req.error}");
            yield break;
        }

        File.WriteAllText(path, req.downloadHandler.text, Encoding.UTF8);
    }
}
