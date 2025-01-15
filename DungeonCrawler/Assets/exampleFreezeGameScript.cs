using System.Collections;
using UnityEngine;

public class exampleFreezeGameScript : MonoBehaviour
{
    // test if timescale actually works.
    private bool GameFrozen = false;
    
    void Update()
    {
       if(Input.GetKeyDown("return") || Input.GetKeyDown(KeyCode.Return)){
        if (GameFrozen) Time.timeScale = 1; else{Time.timeScale = 0;}
        GameFrozen = !GameFrozen;
        StartCoroutine(TestPrint());
       }
    }
    IEnumerator TestPrint(){
        yield return new WaitForSecondsRealtime(1);
        print("Delays working with unscaled time! This means UI Animations and Events can work with paused Timescale!");
    }
}
