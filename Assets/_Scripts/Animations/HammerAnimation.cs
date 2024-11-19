using UnityEngine;

public class HammerAnimation : MonoBehaviour
{
    public GameObject hammer; // Assign the hammer GameObject in the Inspector
    
    [Header("Animation Settings")]
    public float initialRotationAngle = -15f; // Angle for the first rotation
    public float initialRotationDuration = 0.3f; // Duration for the first rotation
    public float finalRotationAngle = 30f; // Angle for the second rotation
    public float finalRotationDuration = 2f; // Duration for the second rotation

    private void OnEnable()
    {
        // Subscribe to the AnimateHammer event
        AnimateHammer.OnAnimateHammer += TriggerHammerAnimation;
    }

    private void OnDisable()
    {
        // Unsubscribe from the AnimateHammer event to avoid memory leaks
        AnimateHammer.OnAnimateHammer -= TriggerHammerAnimation;
    }

    private void TriggerHammerAnimation()
    {
        // Rotate the hammer with LeanTween using customizable variables
        LeanTween.rotateZ(hammer, initialRotationAngle, initialRotationDuration).setOnComplete(() => {
        LeanTween.rotateZ(hammer, finalRotationAngle, finalRotationDuration);
        });
    }
}
