using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesManager : MonoBehaviour
{
    public List<Ability> abilities = new List<Ability>();

    public void StopAllAbilities()
    {
        foreach(Ability a in abilities)
        {
            a.StopAbility();
        }
    }

}
