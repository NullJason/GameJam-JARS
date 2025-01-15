using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private float initialWidth;
    private PlayerMovement player = null;
    private RectTransform size;
    void Start()
    {
      size = GetComponent<RectTransform>();
      initialWidth = size.sizeDelta.x;
    }

    // Update is called once per frame
    void Update()
    {
      if(player == null) player = PlayerMovement.mainPlayer.GetComponent<PlayerMovement>();
      size.sizeDelta = new Vector2(initialWidth / 100 * player.GetHealth(), size.sizeDelta.y);
//      size.sizeDelta += new Vector2(0, 20);
    }
}
