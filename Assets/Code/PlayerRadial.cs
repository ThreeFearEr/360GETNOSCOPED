using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRadial : MonoBehaviour {

    private CameraController cameraShaker;

    Image radial;
    Image ring;
    Image secRing;
    TextMeshProUGUI txtBox;

    struct Radial {
        public float time;
        public Vector3 dir;
    }
    Radial originRadial;
    Radial lastRadial;

    bool isClockwive = true;
    bool isOver180 = false;
    bool isWaiting = true;
    float originTime = 0;

    void Awake() {
        cameraShaker = Camera.main.GetComponent<CameraController>();
        radial = GetComponentsInChildren<Image>()[0];
        ring = GetComponentsInChildren<Image>()[1];
        secRing = GetComponentsInChildren<Image>()[2];
        txtBox = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start() {
        ResetRadial(Vector3.right);
    }

    [SerializeField] private float flickDuration = 1;
    [SerializeField] private float flickStartAngle = 1;
    [SerializeField] private float flickStartCheck = 0.1f;

    [Header("Debug")]
    [SerializeField] float zoomSize;
    [SerializeField] float zoomDuration;
    public void UpdateRadial(Vector3 curDirection) {
        Radial curRadial = new Radial { dir = curDirection, time = Time.time };
        float lastAngle = Vector3.SignedAngle(curDirection, lastRadial.dir, Vector3.forward);
        float originAngle = Vector3.SignedAngle(curDirection, originRadial.dir, Vector3.forward);
        
        float delay = curRadial.time - originRadial.time;

        if(isWaiting) {
            if(delay > flickStartCheck) {
                if(Mathf.Abs(originAngle) > flickStartAngle) {
                    isWaiting = false;
                }
                else {
                    ResetRadial(curDirection);
                }
            }
        }
        else if(delay > flickDuration) {
            ResetRadial(curDirection);
        }
        else if((lastAngle < 0 && isClockwive) || (lastAngle > 0 && !isClockwive)) {
            //clockwise flip handling
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1, -1, 1));
            txtBox.transform.localScale = Vector3.Scale(txtBox.transform.localScale, new Vector3(1, -1, 1));
            isClockwive = !isClockwive;
            ResetRadial(curDirection);
        }
        else {
            if(GameManager.NOfFlicks > 0 && curRadial.time - originTime > flickDuration) {
                //multiFlicks reset
                originTime = originRadial.time;
                GameManager.NOfMultiFlicks = 1;
            }
            if((originAngle < 0 && isClockwive)
            || (originAngle > 0 && !isClockwive)) {
                //handling of full circle (anlgles are in range (-180, 180> ) ouput Hf<0, 360)
                isOver180 = true;
                originAngle = 360 - Mathf.Abs(originAngle);//for originAngle < 0 it would be 360 + - originalAngle
            }
            else if(isOver180) {
                //flicked
                isOver180 = false;
                originRadial.time = curRadial.time;
                GameManager.NOfFlicks++;
                GameManager.NOfMultiFlicks++;
                cameraShaker.ZoomShake(Mathf.Clamp(-zoomSize - GameManager.NOfFlicks * 0.2f, -6, 0), Mathf.Clamp(zoomDuration - GameManager.NOfMultiFlicks * 0.04f, 0.1f, 1f));
            }

            if(lastAngle != 0) radial.fillAmount = Mathf.Abs(originAngle) / 360;
            ring.fillAmount = radial.fillAmount * delay / flickDuration;
            secRing.fillAmount = ring.fillAmount * (curRadial.time - originTime) / flickDuration;
        }

        lastRadial = curRadial;
    }

    public void ResetRadial(Vector3 curDirection) {
        Radial curRadial = new Radial { dir = curDirection, time = Time.time };
        float lastAngle = Vector3.SignedAngle(curDirection, lastRadial.dir, Vector3.forward);

        //reset
        originRadial = curRadial;
        float curAngle = Mathf.Atan2(originRadial.dir.y, originRadial.dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, curAngle));
        originTime = curRadial.time;
        isOver180 = false;
        isWaiting = true;
        GameManager.NOfFlicks = 0;
        GameManager.NOfMultiFlicks = 0;

        //stop 
        radial.fillAmount = 0;
        ring.fillAmount = 0;
        secRing.fillAmount = 0;
    }

    public void UpdateText() {
        txtBox.transform.rotation = Quaternion.identity;
        txtBox.fontSize = Mathf.Min(3 + GameManager.NOfFlicks * 0.1f, 5);
        txtBox.color = new Color32(255, (byte)Mathf.Max(255 - GameManager.NOfMultiFlicks * 48, 0), 0, 255);
        if(GameManager.NOfMultiFlicks > 1) txtBox.fontStyle = FontStyles.Underline;
        else txtBox.fontStyle = FontStyles.Normal;
        txtBox.text = GameManager.NOfFlicks.ToString();
    }

    private void OnDrawGizmos() {
        if(!Application.isPlaying) return;
        Vector3 center = new Vector3(20, 0, 0);
        Vector3 side = new Vector3(0, 10, 0);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + originRadial.dir * 10);
        if(isClockwive) Gizmos.color = Color.green;
        else Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + lastRadial.dir * 10);


        Gizmos.color = Color.red;
        for(int i = 0; i < GameManager.NOfFlicks; i++) {
            Gizmos.DrawLine(center - Vector3.right - side + Vector3.up * i, center + Vector3.right - side + Vector3.up * i);
        }
        Gizmos.color = Color.yellow;
        for(int i = 0; i < GameManager.NOfMultiFlicks; i++) {
            Gizmos.DrawLine(center - Vector3.right - side + Vector3.up * i + Vector3.up * 0.1f, center + Vector3.right - side + Vector3.up * i + Vector3.up * 0.1f);
        }
        Gizmos.color = Color.black;
        Gizmos.DrawLine(center + side - center / 20, center + side + center / 20);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(center - side, center - side + side * 2 * (Time.time - originRadial.time) / flickDuration);
    }
}
