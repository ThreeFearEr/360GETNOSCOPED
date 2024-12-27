using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class WebGate : MonoBehaviour {
    private string serverUrl = "https://taborbobri.cz/UnityOnlines/noscoped.php";
    private string nickname;
    private string deviceID;
    private int curHighscore = 0;

    private void Awake() {
        GameManager.WebGate = this;

        if(!PlayerPrefs.HasKey("DeviceID")) {
            PlayerPrefs.SetString("DeviceID", SystemInfo.deviceUniqueIdentifier);
        }
        else deviceID = PlayerPrefs.GetString("DeviceID");
        if(!PlayerPrefs.HasKey("Nickname")) {
            PlayerPrefs.SetString("Nickname", "Player_" + UnityEngine.Random.Range(1000, 9999));
        }
        else nickname = PlayerPrefs.GetString("Nickname");
        GetCurHighscore(UpdateHighscore, error => Debug.LogError("Error: " + error));

        Debug.Log(PlayerPrefs.GetString("Nickname"));
        Debug.Log(PlayerPrefs.GetString("DeviceID"));
    }
    private void UpdateHighscore(int highscore) {
        curHighscore = highscore;
        Debug.Log(curHighscore);
        GameManager.UIController.UpdateHighscore(highscore);
    }
    public void UpdatePlayerPrefs(string username) {
        PlayerPrefs.SetString("Nickname", username);
    }

    //SetHighscore
    public void SetHighscore(int curHighscore) {
        if(GameManager.Score < curHighscore) return;
        StartCoroutine(PostHighscore(deviceID, nickname, curHighscore));
    }
    private IEnumerator PostHighscore(string deviceID, string username, int score) {
        WWWForm form = new WWWForm();
        form.AddField("deviceID", deviceID);  // Send the unique device ID
        form.AddField("nickname", username);  // Send the player's username
        form.AddField("highscore", score);   // Send the high score

        UnityWebRequest www = UnityWebRequest.Post(serverUrl, form);
        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.Success) {
            Debug.Log("Score submitted successfully!");
        }
        else {
            Debug.LogError("Error submitting score: " + www.error);
        }
    }

    //GetCurHighscore
    [System.Serializable]
    public class CurHighscoreResponse {
        public int highscore;
    }
    public void GetCurHighscore(Action<int> onSuccess, Action<string> onError) {
        StartCoroutine(WebGetCurHighscore(onSuccess, onError));
    }
    private IEnumerator WebGetCurHighscore(Action<int> onSuccess, Action<string> onError) {
        string url = serverUrl + "?deviceID=" + deviceID;

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success) {
            string jsonResponse = request.downloadHandler.text;
            CurHighscoreResponse response = JsonUtility.FromJson<CurHighscoreResponse>(jsonResponse);
            onSuccess?.Invoke(response.highscore);
        }
        else {
            onError?.Invoke("Error: " + request.error);
        }
    }

    //GetTopHighscore
    [System.Serializable]
    public class TopHighscoresContainer {
        public TopHighscoresResponse[] topScores;
    }
    [System.Serializable]
    public class TopHighscoresResponse {
        public string nickname;
        public float highscore;
    }
    public void GetTopHighscores(int top, Action<TopHighscoresResponse[]> onSuccess, Action<string> onError) {
        StartCoroutine(WebGetTopHighscores(top, onSuccess, onError));
    }
    private IEnumerator WebGetTopHighscores(int top, Action<TopHighscoresResponse[]> onSuccess, Action<string> onError) {
        string url = serverUrl + "?top=" + top;

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success) {
            string jsonResponse = request.downloadHandler.text;
            if(jsonResponse.Contains("error")) onError?.Invoke("Error: " + jsonResponse);
            else {
                //SUCCESS
                TopHighscoresContainer container = JsonUtility.FromJson<TopHighscoresContainer>("{\"topScores\":" + jsonResponse + "}");
                onSuccess?.Invoke(container.topScores);
            }
        }
        else onError?.Invoke("Error: " + request.error);
    }
}