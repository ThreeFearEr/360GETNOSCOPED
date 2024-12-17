using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBorders : MonoBehaviour {

    [SerializeField] GameObject borderPrefab;
    Transform[] borders = new Transform[4];

    void Awake() {
        borderPrefab = Resources.Load<GameObject>("Prefabs/Border");
        for(int i = 0; i < borders.Length; i++) {
            borders[i] = Instantiate(borderPrefab).transform;
            borders[i].name = "Border" + (i + 1);
            borders[i].transform.parent = transform;
        }

        BorderUpdate();
    }

    public void BorderUpdate() {
        borders[0].transform.position = new Vector3(0, 0, 0);
        borders[0].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        float orthoSize = Camera.main.orthographicSize;
        float aspectRatio = Camera.main.aspect;

        // The width of the viewport is twice the aspect ratio times the orthographic size
        float worldHeight = orthoSize * 2;
        float worldWidth = worldHeight * aspectRatio;

        // Top border
        borders[0].position = new Vector3(0, orthoSize, 0);
        borders[0].localScale = new Vector3(worldWidth, 1, 1);  // Scale to match the width of the camera's viewport

        // Bottom border
        borders[1].position = new Vector3(0, -orthoSize, 0);
        borders[1].localScale = new Vector3(worldWidth, 1, 1);  // Scale to match the width of the camera's viewport

        // Left border
        borders[2].position = new Vector3(-worldWidth / 2, 0, 0);
        borders[2].localScale = new Vector3(1, worldHeight, 1);  // Scale to match the height of the camera's viewport

        // Right border
        borders[3].position = new Vector3(worldWidth / 2, 0, 0);
        borders[3].localScale = new Vector3(1, worldHeight, 1);  // Scale to match the height of the camera's viewport
    }
}
