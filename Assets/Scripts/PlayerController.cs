using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed;
    public float jumpForce;
    private bool grounded = false;
    private bool canDoubleJump = true;
    private bool faceToRight = true;
    private Rigidbody2D rb;
    private Animator anim;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update() {
        

        // Movement input 
        float horizontalVelocity = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
        rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);
        
        // Flip sprite
        if(faceToRight && horizontalVelocity < 0) {
            faceToRight = false;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        } else if(!faceToRight && horizontalVelocity > 0) {
            faceToRight = true;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }

        if (Input.GetButtonDown("Jump")) {
            if (grounded) {
                anim.SetTrigger("Jump");
                rb.AddForce(Vector2.up * jumpForce);
            } else if (canDoubleJump) {
                anim.SetTrigger("DoubleJump");
                rb.AddForce(Vector2.up * jumpForce);
                canDoubleJump = false;
            }
        }

        // Run animation
        if ( (horizontalVelocity < -0.01 || 0.01 < horizontalVelocity) && grounded) {
            anim.SetBool("Run", true);
        } else {
            anim.SetBool("Run", false);
        }

        anim.SetBool("Grounded", grounded);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Debug.Log("Enter collision");
        grounded = true;
        canDoubleJump = true;
    }

    private void OnCollisionExit2D(Collision2D collision) {
        Debug.Log("Exit collision");
        grounded = false;
    }
}