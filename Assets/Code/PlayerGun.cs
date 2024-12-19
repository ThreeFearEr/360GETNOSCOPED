using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGun : MonoBehaviour {

    [SerializeField] private GameObject bulletPrefab;

    private Transform firePoint;
    private Transform bulletBin;
    private CameraController cameraShaker;
    private Animator animator;

    private bool isReloading = false;
    private float reloadTime = 1;

    [SerializeField] float recoilPower;
    [SerializeField] float recoilDuration;

    public void Awake() {
        bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet");
        firePoint = GetComponentsInChildren<Transform>()[1];//first is this secon SHOULD be firePoint
        cameraShaker = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();

        bulletBin = new GameObject("BulletBin").transform;
        bulletBin.parent = GameObject.Find("Environment").transform;
    }
    
    public void Fire() {
        if(isReloading) return;

        BulletController bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation, bulletBin).GetComponent<BulletController>();
        int flickValue = 1 + GameManager.NOfFlicks * Mathf.Max(GameManager.NOfMultiFlicks, 1);
        bullet.nOfBounces = flickValue;
        bullet.scoreBounty = flickValue;
        GameManager.UIManager.Radial.ResetRadial(transform.right);
        cameraShaker.Shake(-firePoint.right, recoilPower + GameManager.NOfFlicks * 0.6f, Mathf.Min(recoilDuration + GameManager.NOfMultiFlicks * 0.04f, 0.4f));
        animator.Play("GunFire");

        StartCoroutine(countDown());
    }

    IEnumerator countDown() {
        GameManager.UIManager.SetCursorToReload();
        isReloading = true;
        for(float f = 0; f < reloadTime; f += Time.deltaTime) {
            GameManager.UIManager.Reloader.UpdateReloader(f, reloadTime);
            yield return null;
        }
        isReloading = false;
        GameManager.UIManager.SetCursorToIddle();
        GameManager.UIManager.Reloader.HideReloader();
    }

    //screen shake on flicks, hitmarker cursor?

    private void OnDrawGizmos() {
        if(!firePoint) return;
        Gizmos.color = Color.white;
        Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.right * 10);
    }
}
