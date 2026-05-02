using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    //must have reference to player. use this to make cards with effects. isRare is also required, rare version of cards should have (SLIGHTLY) more powerful effects.
    public abstract void Apply(GameObject player, bool isRare);
}
