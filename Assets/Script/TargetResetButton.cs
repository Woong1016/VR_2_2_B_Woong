using UnityEngine;

public class TargetResetButton : MonoBehaviour
{
    public TargetScore[] allTargets;

    public void ResetAllTargets()
    {
        foreach (TargetScore target in allTargets)
        {
            if (target != null)
            {
                target.ResetScore();
            }
        }

    }
}
