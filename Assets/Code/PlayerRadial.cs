using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRadial : MonoBehaviour {
    Image radial;
    Image ring;
    Image secRing;

    struct Radial {
        public float time;
        public Vector3 dir;
    }
    Radial originRadial = new Radial { dir = Vector3.zero, time = 0 };
    Radial lastRadial = new Radial { dir = Vector3.zero, time = 0 };

    bool isClockwive = true;
    bool isOver180 = false;
    float originTime = 0;

    public int nOfFlicks = 0;
    public int nOfMultiFlicks = 0;

    void Awake() {
        radial = GetComponentsInChildren<Image>()[0];
        ring = GetComponentsInChildren<Image>()[1];
        secRing = GetComponentsInChildren<Image>()[2];
    }

    [SerializeField] private float flickDuration = 1;
    [SerializeField] private float flickStartAngle = 1;
    public void UpdateRadial(Vector3 curDirection) {
        Radial curRadial = new Radial { dir = curDirection, time = Time.time};
        float lastAngle = Vector3.SignedAngle(curDirection, lastRadial.dir, Vector3.forward);

        float delay = curRadial.time - originRadial.time;
        if(nOfFlicks > 0 && curRadial.time - originTime > flickDuration) {
            originTime = originRadial.time;
            nOfMultiFlicks = 0;
        }
        if(delay > flickDuration || ((lastAngle < 0 && isClockwive) || (lastAngle > 0 && !isClockwive))) {
            if(Mathf.Abs(lastAngle) > flickStartAngle) {
                //reset
                originRadial = curRadial;
                float curAngle = Mathf.Atan2(originRadial.dir.y, originRadial.dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, curAngle));
                originTime = curRadial.time;

                //clockwise flip handling
                if((lastAngle < 0 && isClockwive) || (lastAngle > 0 && !isClockwive)) {
                    transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1, -1, 1));
                    isClockwive = !isClockwive;
                }
            } else {
                radial.fillAmount = 0;
                ring.fillAmount = 0;
                secRing.fillAmount = 0;
            }
            isOver180 = false;
            nOfFlicks = 0;
            nOfMultiFlicks= 0;
        } else {
            float originAngle = Vector3.SignedAngle(curDirection, originRadial.dir, Vector3.forward);

            //handling of full circle (anlgles are in range (-180, 180> ) ouput Hf<0, 360)
            if((originAngle < 0 && isClockwive)
            || (originAngle > 0 && !isClockwive)) {
                isOver180 = true;
                originAngle = 360 - Mathf.Abs(originAngle);//for originAngle < 0 it would be 360 + - originalAngle
            } else if(isOver180) {
                isOver180 = false;
                originRadial.time = curRadial.time;
                nOfFlicks++;
                nOfMultiFlicks++;
            }

            if(lastAngle != 0) radial.fillAmount = Mathf.Abs(originAngle) / 360;
            ring.fillAmount = radial.fillAmount * delay / flickDuration;
            secRing.fillAmount = ring.fillAmount * (curRadial.time - originTime) / flickDuration;
        }

        lastRadial = curRadial;
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
        for(int i = 0; i < nOfFlicks; i++) {
            Gizmos.DrawLine(center - Vector3.right - side + Vector3.up * i, center + Vector3.right - side + Vector3.up * i);
        }
        Gizmos.color = Color.yellow;
        for(int i = 0; i < nOfMultiFlicks; i++) {
            Gizmos.DrawLine(center - Vector3.right - side + Vector3.up * i + Vector3.up * 0.1f, center + Vector3.right - side + Vector3.up * i + Vector3.up * 0.1f);
        }
        Gizmos.color = Color.black;
        Gizmos.DrawLine(center + side - center/20, center + side + center / 20);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(center - side, center - side + side * 2 * (Time.time - originRadial.time) / flickDuration);
    }
}
