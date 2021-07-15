using UnityEngine;
using System.Collections;
using PlayerMovement;

public class Ability : MonoBehaviour
{
    public KeyCode aAbilityKey = KeyCode.F;
    public string aName = "New Ability";
    public Sprite aSprite;
    public AudioClip aSound;
    public float aBaseCoolDown = 1f;
    protected float aLastStartTime = 0f;
    protected bool aExectuting = false;
    protected MovementController aMC;
    protected CharacterController cC;
    protected PlayerCameraController aPlayerCam;
    protected PlayerControllerCC aPlayerCharCont;
    protected AbilitiesManager abilitiesMan;
    protected Rigidbody aRB;

    private void Start()
    {
        aMC = GetComponent<MovementController>();
        cC = GetComponent<CharacterController>();
        aPlayerCam = GetComponent<PlayerCameraController>();
        abilitiesMan = GetComponent<AbilitiesManager>();
        aPlayerCharCont = GetComponent<PlayerControllerCC>();
        aRB = GetComponent<Rigidbody>();
        Initialize();
    }

    public virtual void Initialize(){
        abilitiesMan.abilities.Add(this);
    }
    public virtual void TriggerAbility()
    {

        abilitiesMan.StopAllAbilities();
    }
    public virtual void StopAbility(){ }
    public virtual void ResetAbility() { }
}
