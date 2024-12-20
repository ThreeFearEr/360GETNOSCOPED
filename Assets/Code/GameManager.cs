using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager {
    
    public static UIManager UIManager;

    private static int nOfFlicks = 0;
    public static int NOfFlicks {
        get {
            return nOfFlicks;
        }
        set {
            nOfFlicks = value;
            UIManager.Radial.UpdateText();
        }
    }

    private static int nOfMultiFlicks = 0;
    public static int NOfMultiFlicks {
        get {
            return nOfMultiFlicks;
        }
        set {
            nOfMultiFlicks = value;
            UIManager.Radial.UpdateText();
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
        UIManager.UpdateScore(value);
    }

    public static void Reset() {
        score = 0;
        nOfFlicks = 0;
        nOfMultiFlicks = 0;
        isPlaying = true;
    }
}