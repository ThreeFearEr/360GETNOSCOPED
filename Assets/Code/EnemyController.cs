using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private Rigidbody2D rb;  // The Rigidbody2D component of the object
    private Vector3 targetPosition = Vector3.zero;  // The target position to move towards
    private Vector3 curDirection = Vector3.right;  // The target position to move towards
    public float speed = 5f;  // Movement speed

    public bool isMoving = false;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if(!isMoving) return;

        Vector3 currentPosition = rb.position;
        if(curDirection != Vector3.zero) {
            targetPosition = currentPosition + curDirection * speed;
        }
        Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.fixedDeltaTime);

        rb.MovePosition(newPosition);
    }

    public IEnumerator Born(Vector3 direction) {
        gameObject.SetActive(true);
        curDirection = direction;
        yield return new WaitForSeconds(1f);
        isMoving = true;
        yield return new WaitForSeconds(5f);
        curDirection = Vector3.zero;//pipipupo but it is what it is $$$
        targetPosition = Vector3.zero;
    }

    public void Die() {
        isMoving = false;
        gameObject.SetActive(false);
    }
}
