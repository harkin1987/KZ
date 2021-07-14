using UnityEngine;
using System.Collections;

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
    protected PlayerCameraController aPlayerCameraController;

    private void Start()
    {
        aMC = GetComponent<MovementController>();
        cC = GetComponent<CharacterController>();
        aPlayerCameraController = GetComponent<PlayerCameraController>();
    }

    public virtual void Initialize()
    {

    }
    public virtual void TriggerAbility()
    {

    }

    public virtual void StopAbility()
    {

    }
}
