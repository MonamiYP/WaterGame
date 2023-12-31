using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpAmount = 10f;
    [SerializeField] private float gravityWater = 0f;
    [SerializeField] private float gravityNormal = 2f;
    [SerializeField] private float gravityFalling = 4f;

    [SerializeField] private InputManager inputManager;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] public bool isUnderwater = false;

    private bool plunge = false;
    private bool isFacingRight = true;

    private void Start() {
        inputManager.OnJump += InputManager_OnJump;

        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        HandleMovement();
    }

    private void HandleMovement() {
        Vector2 inputVector = inputManager.GetMovementVector();
        
        if (isFacingRight && inputVector.x < -0f || !isFacingRight && inputVector.x > 0f) {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

        if (isUnderwater && !plunge) {
            rb.velocity = new Vector2(inputVector.x * moveSpeed, inputVector.y * moveSpeed); 
        } else {
            rb.velocity = new Vector2(inputVector.x * moveSpeed, rb.velocity.y); 
        }  
        
        HandleGravity();
        Plunge();
    }

    private void InputManager_OnJump(object sender, System.EventArgs e) {
        if (!isUnderwater && rb.velocity.y == 0) {
            rb.AddForce(Vector2.up * jumpAmount, ForceMode2D.Impulse);
        }
    }

    private void Plunge() {
        if (plunge) {
            if (Mathf.Abs(rb.velocity.y) <= 2) {
                plunge = false;
            } else {
                rb.AddForce(-rb.velocity * 0.2f, ForceMode2D.Force);
            }
        }
    }

    private void HandleGravity() {
        if (rb.velocity.y >= 0) {
            rb.gravityScale = gravityNormal;
        } else {
            rb.gravityScale = gravityFalling;
        }

        if (isUnderwater)
            rb.gravityScale = gravityWater;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Water") {
            isUnderwater = true;
            plunge = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "Water") {
            isUnderwater = false;
        }
    }
}