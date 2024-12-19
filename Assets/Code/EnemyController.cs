using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private Rigidbody2D rb;  // The Rigidbody2D component of the object
    private Vector3 targetPosition = Vector3.zero;  // The target position to move towards
    private Vector3 curDirection = Vector3.right;  // The target position to move towards

    public float speed = 5f;  // Movement speed
    public bool isMoving = false;
    [SerializeField] float attackRange = 24;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if(!isMoving || !GameManager.isPlaying) return;
        Vector3 currentPosition = rb.position;

        Vector2 newPosition = Vector2.zero;
        if(Vector2.Distance(currentPosition, targetPosition) > attackRange) {
            newPosition = currentPosition + curDirection * speed * Time.fixedDeltaTime;
        }
        else {
            newPosition = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.fixedDeltaTime);
        }
        rb.MovePosition(newPosition);
    }

    public IEnumerator Born(Vector3 direction) {
        gameObject.SetActive(true);
        curDirection = direction;
        yield return new WaitForSeconds(1f);
        isMoving = true;
    }

    public void Die() {
        isMoving = false;
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.layer != LayerMask.NameToLayer("Player") || !GameManager.isPlaying) return;
        
        GameManager.isPlaying = false;
        PlayerController player = collision.collider.GetComponent<PlayerController>();
        player.Die();
        Camera.main.GetComponent<CameraController>().Zoom((transform.position - collision.transform.position) / 2, 10, 1);
        GameManager.UIManager.Die();
    }
}
