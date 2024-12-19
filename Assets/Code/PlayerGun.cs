using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour {

    [SerializeField] private GameObject bulletPrefab;

    private Transform firePoint;
    private PlayerRadial radial;
    private Transform bulletBin;
    private CameraShaker cameraShaker;
    private Animator animator;

    private bool isReloading = false;
    public float ReloadTime = 1;

    [SerializeField] float recoilPower;
    [SerializeField] float recoilDuration;

    public void Awake() {
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
        radial = transform.parent.GetComponentInChildren<PlayerRadial>();
        firePoint = GetComponentsInChildren<Transform>()[1];//first is this secon SHOULD be firePoint
        cameraShaker = Camera.main.GetComponent<CameraShaker>();
        animator = GetComponent<Animator>();

        bulletBin = new GameObject("BulletBin").transform;
        bulletBin.parent = GameObject.Find("Environment").transform;
    }
    
    public void Fire() {
        if(isReloading) return;
        BulletController bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, bulletBin).GetComponent<BulletController>();
        bullet.nOfBounces = 1 + radial.nOfFlicks * radial.nOfMultiFlicks;
        bullet.nOfHits= 1 + radial.nOfFlicks * radial.nOfMultiFlicks;
        cameraShaker.Shake(-firePoint.right, recoilPower + radial.nOfFlicks * 0.6f, recoilDuration + Mathf.Clamp(radial.nOfMultiFlicks * 0.4f, 0.1f, 0.4f));
        animator.Play("GunFire");
        StartCoroutine(countDown());

    }

    IEnumerator countDown() {
        isReloading = true;
        yield return new WaitForSeconds(ReloadTime);
        isReloading = false;
    }

    //screen shake on flicks, hitmarker cursor?

    private void OnDrawGizmos() {
        if(!firePoint) return;
        Gizmos.color = Color.white;
        Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.right * 10);
    }
}
