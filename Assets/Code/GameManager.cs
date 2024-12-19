using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager {
    
    public static UIIndicatorController UIIndicators;
    
    public static float GameTime = 0;
    private static float score;

    public static bool isPlaying = false;

    public static float Score {
        get {
            return score;
        }
        set {
            score = value;
            UIIndicators.UpdateScore(value);
        }
    }
}