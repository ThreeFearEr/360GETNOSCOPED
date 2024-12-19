using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIIndicatorController : MonoBehaviour {

    TextMeshPro scoreTxt;
    //TextMeshPro flickCountTxt;

    private void Awake() {
        GameManager.UIIndicators = this;
        
        //scoreTxt = GetComponentsInChildren<TextMeshPro>()[0];
        //flickCountTxt = GetComponentsInChildren<TextMeshPro>()[1];
    }

    public void UpdateScore(float value) {
        scoreTxt.text = value.ToString();
    }
}
