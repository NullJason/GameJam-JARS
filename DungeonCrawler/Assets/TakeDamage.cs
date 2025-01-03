using UnityEngine;

public class TakeDamage : MonoBehaviour
{
    // simple click or collide to dmg. placeholder script.
    // script was added to test map change methods.
    // EnemyModule EnemyInfo;
    Transform tf;
    Rigidbody2D rb;
    GameObject Player;
    Transform PlayerTransform;

    float DeathSize = .5f;
    Vector2 knockbackForce = new Vector2(5f, 5f);
    float bounceBackForceMultiplier = 1.5f; // multiplier for fun
    float screenPadding = 1f; 

    void Start()
    {
        Player = GameObject.Find("Player");
        PlayerTransform = Player.transform;
        tf = gameObject.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckBounds();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.Equals(Player))
        {
            DamageEnemy(col.contacts[0].normal); 
        }
    }

    void DamageEnemy(Vector2 knockbackDirection)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse); // + knockback

        tf.localScale -= new Vector3(0.1f, 0.1f, 0);
        if (tf.localScale.x < DeathSize || tf.localScale.y < DeathSize)
        {
            Destroy(gameObject); // Destroy the enemy if it gets too small
        }
    }

    void CheckBounds()
    {
        if(rb.linearVelocity == Vector2.zero){rb.linearVelocity = new Vector2(1,1);}
        if(rb.linearVelocity.magnitude > new Vector2(5,5).magnitude) { 
            Vector2 lvn = rb.linearVelocity.normalized; 
            rb.linearVelocity = new Vector2(5*lvn.x,5*lvn.y);
            }
        // Get the screen bounds in world space
        Vector3 screenMin = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 screenMax = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        // Prevent enemy from going off screen horizontally
        if (tf.position.x < screenMin.x + screenPadding)
        {
            tf.position = new Vector2(screenMin.x + screenPadding, tf.position.y);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * -bounceBackForceMultiplier, rb.linearVelocity.y); 
        }
        else if (tf.position.x > screenMax.x - screenPadding)
        {
            tf.position = new Vector2(screenMax.x - screenPadding, tf.position.y); 
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * -bounceBackForceMultiplier, rb.linearVelocity.y); 
        }
        // Prevent enemy from going off screen vertically
        if (tf.position.y < screenMin.y + screenPadding)
        {
            tf.position = new Vector2(tf.position.x, screenMin.y + screenPadding); 
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * -bounceBackForceMultiplier); 
        }
        else if (tf.position.y > screenMax.y - screenPadding)
        {
            tf.position = new Vector2(tf.position.x, screenMax.y - screenPadding); 
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * -bounceBackForceMultiplier);
        }
    }
}
