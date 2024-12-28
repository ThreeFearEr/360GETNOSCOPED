using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class WebGate : MonoBehaviour {
    private string serverUrl = "https://taborbobri.cz/UnityOnlines/noscoped.php";

    private void Awake() {
        GameManager.WebGate = this;

        if(!PlayerPrefs.HasKey("DeviceID")) {
            PlayerPrefs.SetString("DeviceID", SystemInfo.deviceUniqueIdentifier);
        }
        else GameManager.deviceID = PlayerPrefs.GetString("DeviceID");
        if(!PlayerPrefs.HasKey("Nickname")) {
            PlayerPrefs.SetString("Nickname", "Player_" + UnityEngine.Random.Range(1000, 9999));
        }
        else GameManager.nickname = PlayerPrefs.GetString("Nickname");
        if(!PlayerPrefs.HasKey("Highscore")) {
            PlayerPrefs.SetInt("Highscore", 0);
        }
        else {
            GameManager.curHighscore = PlayerPrefs.GetInt("Highscore");
        }
        GameManager.UIController.UpdateHighscore(GameManager.curHighscore);
        GameManager.UIController.UpdateNickname();
    }
    public void UpdateHighscore(int highscore) {
        GameManager.curHighscore = highscore;
        PlayerPrefs.SetInt("Highscore", highscore);
        GameManager.UIController.UpdateHighscore(highscore);
    }

    //SetHighscore
    public void SetHighscore() {
        if(GameManager.Score <= GameManager.curHighscore) return;
        GameManager.curHighscore = GameManager.Score;
        StartCoroutine(PostHighscore(GameManager.curHighscore));//posts only if ingame score is biger than score from prefs (thus rewriting the prefs wont change the web value)
        UpdateHighscore(GameManager.curHighscore);
    }
    private IEnumerator PostHighscore(int score) {
        WWWForm form = new WWWForm();
        form.AddField("deviceID", GameManager.deviceID);  // Send the unique device ID
        form.AddField("nickname", GameManager.nickname);  // Send the player's username
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
        string url = serverUrl + "?deviceID=" + GameManager.deviceID;
        Debug.Log(url);
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success) {
            string jsonResponse = request.downloadHandler.text;
            Debug.LogWarning(jsonResponse);
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