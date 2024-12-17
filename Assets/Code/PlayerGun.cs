using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour {

    [SerializeField] private GameObject bulletPrefab;

    private Transform firePoint;
    private PlayerRadial radial;
    private Transform bulletBin;

    private bool isReloading = false;
    public float ReloadTime = 1;

    public void Awake() {
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
        radial = transform.parent.GetComponentInChildren<PlayerRadial>();
        firePoint = transform.GetComponentInChildren<Transform>();

        bulletBin = new GameObject("BulletBin").transform;
        bulletBin.parent = GameObject.Find("Environment").transform;
    }
    
    public void Fire() {
        if(isReloading) return;
        Debug.Log(bulletPrefab);
        Debug.Log(firePoint.position);
        Debug.Log(firePoint.rotation);
        Debug.Log(bulletBin);
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, bulletBin);
        StartCoroutine(countDown());
    }

    IEnumerator countDown() {
        isReloading = true;
        yield return new WaitForSeconds(ReloadTime);
        isReloading = false;
    }

    //screen shake on flicks, hitmarker cursor?
}
