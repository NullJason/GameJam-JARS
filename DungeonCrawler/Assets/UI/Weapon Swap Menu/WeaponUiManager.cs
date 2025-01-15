using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

//TODO: Store a reference to each added weapon in a dictionary or hashSet! That way, duplicate weapons can be automatically detected and fused for levelups!

public class WeaponUiManager : MonoBehaviour
{
  private HashSet<Weapon> weapons;
  [SerializeField] private protected GameObject blankEntry;
  [SerializeField] private protected RectTransform ui;
  private protected Weapon currentlyDisplayed;
  private protected Button selectWeaponButton;
  private protected RectTransform scrollScreen;
  private protected RectTransform infoPanelText;
  private protected RectTransform infoPanelTitle;
  private protected RectTransform bosslockText;
  private protected WeaponUiTab holding;
  private protected WeaponUiTab mostRecent;
  private const int SIZE_OF_ONE_ENTRY = 60;
  public static WeaponUiManager main;
  private protected bool bossLock; //Whether you can open the menu to switch weapons. Locks when you enter the boss chamber, and unlocks once you defeat the boss.
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    if(main == null) main = this;
    scrollScreen = ui.GetChild(0).Find("Weapon List").GetChild(0).Find("Viewport").GetChild(0).GetComponent<RectTransform>();
    infoPanelText = ui.GetChild(0).Find("Info Panel").Find("Description").GetComponent<RectTransform>();
    infoPanelTitle = infoPanelText.transform.parent.Find("Title").GetComponent<RectTransform>();
    selectWeaponButton = infoPanelText.transform.parent.Find("Button").GetComponent<Button>();
    bosslockText = infoPanelText.transform.parent.Find("Bosslock Display panel").GetChild(0).GetComponent<RectTransform>();
    selectWeaponButton.onClick.AddListener(HoldWeapon);
    Switch();
    Switch();//For some reason, adding weapons to the UI display doesn't like to work until it's shown at least once.
    weapons = new HashSet<Weapon>();
  }

  // Update is called once per frame
  void Update()
  {
    if(!bossLock){
      if(Input.GetKeyDown(KeyCode.Return)) Switch();
    }
    else if(ui.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Return)) Switch();
  }
  //Returns true if BossLock was false and now true.
  public bool BossLock(string name){
    bool result = !bossLock;
    bossLock = true;
    DisplayBossScreen(name);
    bosslockText.parent.gameObject.SetActive(true);
    bosslockText.gameObject.GetComponent<TMP_Text>().SetText(name);
    return result;
  }
  private protected void DisplayBossScreen(string name){
    if(!ui.gameObject.activeSelf) Switch();

  }
  //Returns true if BossLock was true and now false.
  public bool BossUnlock(){
    bool result = bossLock;
    bossLock = false;
    bosslockText.parent.gameObject.SetActive(false);
    return result;
  }
  //Swaps whether or not the screen is to be showed or not.
  private protected bool Switch(){
    ui.gameObject.SetActive(!ui.gameObject.activeSelf);
    if(ui.gameObject.activeSelf) PlayingFieldObject.Pause();
    else PlayingFieldObject.Unpause();
    return ui.gameObject.activeSelf;
  }
  //Adds an entry for a new weapon onto the menu.
  private protected void Add(Weapon weapon){
    GameObject tab = Instantiate(blankEntry, scrollScreen.position + new Vector3(scrollScreen.rect.width / 2, -scrollScreen.sizeDelta.y, 0), new Quaternion(0, 0, 0, 0), scrollScreen);
    scrollScreen.sizeDelta += new Vector2(0, SIZE_OF_ONE_ENTRY);
    tab.transform.GetChild(0).GetComponent<WeaponUiTab>().SetWeapon(weapon);
  }

  //Displays info on the tab to the right.
  public void Display(Weapon weapon, WeaponUiTab tab){
    infoPanelText.GetComponent<TMP_Text>().SetText(weapon.GetInfo());
    infoPanelTitle.GetComponent<TMP_Text>().SetText(weapon.GetNameAndLevel());
    currentlyDisplayed = weapon;
    mostRecent = tab;
  }

  public void PickUpWeapon(Weapon weapon){
    Weapon currentCopy;
    if(weapons.TryGetValue(weapon, out currentCopy)){
      currentCopy.LevelUp();
    }
    else{
      weapons.Add(weapon);
      weapon.transform.SetParent(PlayerMovement.mainPlayer.transform.Find("Weapons"));
      weapon.transform.localPosition = new Vector3(0, 0, weapon.transform.position.z);
      Add(weapon);
      PlayerMovement.mainPlayer.InitializeWeapon(weapon);
    }
  }
  public void HoldWeapon(Weapon weapon){
    PlayerMovement.mainPlayer.SetWeapon(weapon);
    Highlight(mostRecent);
  }
  public void HoldWeapon(){
    HoldWeapon(currentlyDisplayed);
  }
  public void Highlight(WeaponUiTab next){
    if(holding != null) holding.transform.parent.GetComponent<Image>().color = new Color(1, 1, 1, 0.392f); //TODO: Remove Magic Numbers!
    next.transform.parent.GetComponent<Image>().color = new Color(0, 1, 1, 255);
    holding = next;
  }
}
