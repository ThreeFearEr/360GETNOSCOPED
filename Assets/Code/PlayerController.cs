using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public PlayerBody Body;
    public PlayerGun Gun;
    PlayerRadial radial;
    Animator animator;

    private bool isFacingLeft = false;

    public float RotationSpeed = 5.0f; // Speed of rotation

    void Awake() {
        Body = GetComponentInChildren<PlayerBody>();
        Gun = GetComponentInChildren<PlayerGun>();
        animator = GetComponent<Animator>();
    }

    private void Start() {
        radial = GameManager.UIManager.GetComponentInChildren<PlayerRadial>();
    }

    void Update() {
        if(!GameManager.isPlaying) return;

        Vector3 mouseWorldPosition = GetMouseWorldPosition();
        RotateGun(mouseWorldPosition);
        CheckMouseSide(mouseWorldPosition);
        radial.UpdateRadial(Gun.transform.right);

        if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse3) || Input.GetKeyDown(KeyCode.W)) {
            Gun.Fire();
        }
    }
    /// <summary>
    /// Returns the cursor position in world, using Camera and and Cameras Z position
    /// </summary>
    /// <returns></returns>
    Vector3 GetMouseWorldPosition() {
        // Get mouse position and convert it to world space
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Camera.main.WorldToScreenPoint(transform.position).z; // Match object's depth
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0f; // Ensure Z is 0 in 2D
        return mouseWorldPosition;
    }
    /// <summary>
    /// Rotates Gun to chosen direction, using Slerp and Transform
    /// </summary>
    /// <param name="lookPosition">position to rotate towards</param>
    public void RotateGun(Vector3 lookPosition) {
        // Calculate the direction and rotation
        Vector3 direction = (lookPosition - Gun.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Smoothly rotate to the target angle
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        Gun.transform.rotation = Quaternion.Slerp(Gun.transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }

    void CheckMouseSide(Vector3 mousePos) {
        if((mousePos.x - transform.position.x < 0 && !isFacingLeft)//is left 
        || (mousePos.x - transform.position.x > 0 && isFacingLeft)) {//is right
            Body.transform.localScale = Vector3.Scale(Body.transform.localScale, new Vector3(-1, 1, 1));
            Gun.transform.localScale = Vector3.Scale(Gun.transform.localScale, new Vector3(1, -1, 1));
            isFacingLeft = !isFacingLeft;
        }
    }

    public void Die() {
        animator.Play("PlayerDie");
    }
}
