using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private float initialWidth;
    private Player player = null;
    private RectTransform size;
    void Start()
    {
      size = GetComponent<RectTransform>();
      initialWidth = size.sizeDelta.x;
    }

    // Update is called once per frame
    void Update()
    {
      if(player == null) player = GameplayManager.hunter.GetComponent<Hunter>().GetPlayer().GetComponent<Player>();
      size.sizeDelta = new Vector2(initialWidth / player.GetMaxHp() * player.GetHp(), size.sizeDelta.y);
      Debug.Log(player.GetHp());
//      size.sizeDelta += new Vector2(0, 20);
    }
}
