using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb;     //r�f�rence au rigidbody 
    private BoxCollider2D boxCollider; //r�f�rence au boxCollider
    [SerializeField] private LayerMask JumpableGround;  //r�f�rence au layer du sol
    [SerializeField] private float jumpForce = 14f;     //force du saut
    [SerializeField] private float moveSpeed = 7f;
    private bool canDoubleJump = false;
     private bool hasPickedUpItem = false; 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();    //r�cup�ration du rigidbody
        boxCollider = GetComponent<BoxCollider2D>();  //r�cup�ration du boxColliderß
        StartCoroutine(SimulateItemPickup());
    }

        IEnumerator SimulateItemPickup()
    {
        yield return new WaitForSeconds(5f); // Wait for 5 seconds

        // Simulate item pickup
        PickupItem();
    }
    // Update is called once per frame
    void Update()
    {
        float directionX = Input.GetAxisRaw("Horizontal");  //r�cup�ration de la direction en X
        rb.velocity = new Vector2(directionX * moveSpeed, rb.velocity.y);  //d�placement du joueur

            if (Input.GetButtonDown("Jump"))
                if(IsGrounded())
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    canDoubleJump = true; // Reset double jump ability when grounded
                }  //si on appuie sur la touche de saut et que le joueur est au sol
                else if (canDoubleJump && hasPickedUpItem) // Check if double jump is allowed and the item has been picked up
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    canDoubleJump = false; // Prevent further double jumps until grounded
                }
    }
    private bool IsGrounded(){
        // Create a box around the player that has the same shape of the box collider Second box over it , 0 is just rotation , vector 2. down - move the box down Checks if we are overlapping this jumpable ground
        return Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, .1f, JumpableGround);
    }
        public void PickupItem() // ****** Need to call this somwhere when the player picks up the item
    {
        hasPickedUpItem = true;
    }
}


