using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager {
    
    public static UIController UIController;
    public static SpawnerController SpawnerController;
    public static GameAudio AudioController;
    public static WebGate WebGate;

    public static string nickname;
    public static string deviceID;
    public static int curHighscore = 0;

    public static bool Intro = false;

    private static int nOfFlicks = 0;
    public static int NOfFlicks {
        get {
            return nOfFlicks;
        }
        set {
            nOfFlicks = value;
            UIController.Radial.UpdateText();
        }
    }

    private static int nOfMultiFlicks = 0;
    public static int NOfMultiFlicks {
        get {
            return nOfMultiFlicks;
        }
        set {
            nOfMultiFlicks = value;
            UIController.Radial.UpdateText();
        }
    }

    public static bool isPlaying = false;

    private static int score;
    public static int Score {
        get {
            return score;
        }
    }
    public static void AddScore(int value) {
        score += value;
        UIController.UpdateScore(value);
    }

    public static void Reset() {
        score = 0;
        nOfFlicks = 0;
        nOfMultiFlicks = 0;
        Intro = false;
        isPlaying = true;
        SpawnerController.StartSpawnCycle();
    }
}