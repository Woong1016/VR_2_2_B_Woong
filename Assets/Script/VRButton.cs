using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class VRButton : MonoBehaviour
{
    public UnityEvent onGrab;  

    private XRGrabInteractable interactable;

    void Start()
    {
        interactable = GetComponent<XRGrabInteractable>();

        interactable.selectEntered.AddListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs arg)
    {
        onGrab.Invoke();

        if (arg.interactorObject is XRBaseControllerInteractor controller)
        {
            controller.xrController.SendHapticImpulse(0.5f, 0.1f);
        }

        
    }
}