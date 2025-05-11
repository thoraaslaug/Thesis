using UnityEngine;
using MalbersAnimations.HAP;
using UnityEngine.InputSystem;
using MalbersAnimations.InputSystem;

public class HorseControlManager : MonoBehaviour
{
    [Header("Assign Riders")]
    public MRider activeRider;         // e.g. the man in front
    public MRider passiveRider;        // e.g. the woman behind

    [Header("Optional")]
    public PlayerInput activeInput;    // Optional override if auto not working

    void Start()
    {
        ActivateRider(activeRider);
        DeactivateRider(passiveRider);
    }

    public void ActivateRider(MRider rider)
    {
        if (rider == null) return;

        var inputLink = rider.GetComponent<MInputLink>();
        var playerInput = rider.GetComponent<PlayerInput>();
       // var controller = rider.GetComponent<ThirdPersonController>();
        var animal = rider.GetComponent<MalbersAnimations.Controller.MAnimal>();

        if (inputLink) inputLink.Enable(true);
        if (playerInput) playerInput.enabled = true;
       // if (controller) controller.enabled = true;
        if (animal) animal.InputSource?.Enable(true);

        // Optional: Force set the input source if things are funky
        if (inputLink && activeInput)
            inputLink.PlayerInput_Set(activeInput);
    }

    public void DeactivateRider(MRider rider)
    {
        if (rider == null) return;

        var inputLink = rider.GetComponent<MInputLink>();
        var playerInput = rider.GetComponent<PlayerInput>();
        //var controller = rider.GetComponent<ThirdPersonController>();
        var animal = rider.GetComponent<MalbersAnimations.Controller.MAnimal>();

        if (inputLink) inputLink.Enable(false);
        if (playerInput) playerInput.enabled = false;
        //if (controller) controller.enabled = false;
        if (animal) animal.InputSource?.Enable(false);
    }
}