using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TakeDamage : MonoBehaviour
{
    // simple click or collide to dmg. placeholder script.
    // script was added to test map change methods.
    // EnemyModule EnemyInfo;
    Transform tf;
    Button b;
    GameObject Player;
    Transform PlayerTransform;

    float DeathSize = .1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("Player");
        PlayerTransform = Player.transform;
        tf = gameObject.transform;
        b = tf.GetComponent<Button>();
        b.onClick.AddListener(DamageEnemy);
    }
    void OnCollisionEnter2D(Collision2D col){
        // check if object is a projectile/child from the player, or the player itself.
        GameObject o = col.gameObject;
        if (col.transform.IsChildOf(PlayerTransform)){
            DamageEnemy();
        }
    }
    void DamageEnemy(){
        tf.localScale -= new Vector3(0.1f,0.1f);
        if (tf.localScale.x < DeathSize || tf.localScale.y < DeathSize || tf.localScale.z < DeathSize){
            GameObject.Destroy(gameObject);
        }
    }
}
