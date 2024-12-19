using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerReloader : MonoBehaviour {

    Image bar;

    private void Awake() {
        bar = GetComponentsInChildren<Image>()[1];
        HideReloader();
    }

    public void UpdateReloader(float value, float reloadTime) {
        if(!bar.transform.parent.gameObject.activeSelf) bar.transform.parent.gameObject.SetActive(true);

        Vector3 cursorScreenPosition = Input.mousePosition;
        Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(cursorScreenPosition);
        cursorWorldPosition.z = transform.position.z;
        transform.position = cursorWorldPosition;

        bar.fillAmount = value / reloadTime;
    }

    public void HideReloader() {
        bar.transform.parent.gameObject.SetActive(false);
    }
}
