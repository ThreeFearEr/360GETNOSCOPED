using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletController : MonoBehaviour {
    
    TrailRenderer trail;

    public float speed = 100;
    public int scoreBounty = 1;

    public int nOfBounces;

    LayerMask borderLayer;
    LayerMask enemyLayer;
    LayerMask playerLayer;

    private void Awake() {
        trail = GetComponent<TrailRenderer>();
        borderLayer = LayerMask.NameToLayer("Border");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        playerLayer = LayerMask.NameToLayer("Player");
    }

    private void Start() {
        StartCoroutine(BulletTravel(transform.position, transform.right, null));
    }

    public IEnumerator BulletTravel(Vector3 origin, Vector3 direction, Transform bouncer) {
        float maxDistance = getMaxDistance();
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, maxDistance);

        if(hits.Length < 1 || direction == Vector3.zero) {
            Debug.LogError("WHAT THE ACTuAL SHIT??! WHATT!!? [" + hits.Length + "]");
            yield break;
        }

        foreach (RaycastHit2D hit in hits) {
            if(hit.transform.gameObject.layer == playerLayer || hit.transform == bouncer) continue;
            if(hit.transform.gameObject.layer == enemyLayer) {
                hit.transform.GetComponent<EnemyController>().Die();
                GameManager.AddScore(scoreBounty);
            }
            float distance = Vector3.Distance(trail.transform.position, hit.point);//hit.distance?
            float wholeDistance = distance;
            while(distance > 0) {
                transform.position = Vector3.Lerp(origin, hit.point, 1 - (distance / wholeDistance));
                distance -= Time.deltaTime * speed;

                yield return new WaitForEndOfFrame();
            }
            if(nOfBounces == 0) {
                Destroy(gameObject, trail.time);
                yield break;
            }
            if(hit.transform.gameObject.layer == borderLayer) {
                nOfBounces--;
                GetComponent<TrailRenderer>().sortingOrder = 20;
                StartCoroutine(BulletTravel(hit.point, Vector3.Reflect(direction, hit.normal), hit.transform));
                break;
            }
        }
    }

    private float getMaxDistance() {
        float height = Camera.main.orthographicSize * 2;
        float width = height * Camera.main.aspect;
        return Mathf.Sqrt(width * width + height * height);
    }
}
