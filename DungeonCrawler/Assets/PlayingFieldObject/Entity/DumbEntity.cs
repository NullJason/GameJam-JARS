using UnityEngine;
// "...And there you have it, folks! An Entity that does literally nothing!"
// This Entity does not move or attack at all. It still does regular Entity stuff, like health. 
public class DumbEntity : Entity
{
    private protected override void Behave(){}
}
