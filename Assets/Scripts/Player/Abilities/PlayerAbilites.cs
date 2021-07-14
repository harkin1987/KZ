using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilites : MonoBehaviour
{
    public List<Ability> abilities;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Ability a in abilities)
        {
            a.Initialize();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
