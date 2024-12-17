using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Developer : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKey(KeyCode.F2)) {
            if(Input.GetKeyDown(KeyCode.Alpha1)) Time.timeScale = 1.0f;
            if(Input.GetKeyDown(KeyCode.Alpha2)) Time.timeScale /= 2f;
            if(Input.GetKeyDown(KeyCode.Alpha3)) Time.timeScale *= 2f;
        }
    }
}
