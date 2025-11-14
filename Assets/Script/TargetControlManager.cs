using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using System.Collections.Generic;

public class TargetControlManager : MonoBehaviour
{
    
    public XRBaseInteractable inspectionButton; 

    
    public Transform[] allTargets;  

     
    public Transform[] closeUpPoints;  

   
    public float moveSpeed = 1.0f;  

     
    private List<Vector3> originalPositions;  
    private bool areTargetsClose = false;
    private bool isMoving = false;

    void Start()
    {
        
        if (allTargets.Length != closeUpPoints.Length)
        {
            
            return;  
        }

        
        originalPositions = new List<Vector3>();
        foreach (Transform target in allTargets)
        {
            originalPositions.Add(target.position);
        }

        
        if (inspectionButton != null)
        {
            inspectionButton.selectEntered.AddListener(OnButtonPressed);
        }
    }

    
    private void OnButtonPressed(SelectEnterEventArgs arg)
    {
        if (isMoving) return;  
        if (allTargets.Length == 0) return;  

        areTargetsClose = !areTargetsClose;
        StartCoroutine(MoveAllTargets());
    }

    
    private IEnumerator MoveAllTargets()
    {
        isMoving = true;

        float timer = 0f;
        List<Vector3> startPositions = new List<Vector3>();
        List<Vector3> endPositions = new List<Vector3>();

        
        for (int i = 0; i < allTargets.Length; i++)
        {
            
            startPositions.Add(allTargets[i].position);

            Vector3 targetPos;
            if (areTargetsClose)  
            {
                 
                targetPos = closeUpPoints[i].position;
            }
            else  
            {
                 
                targetPos = originalPositions[i];
            }
            endPositions.Add(targetPos);
        }
        
        while (timer < 1f)
        {
            timer += Time.deltaTime * moveSpeed;

            for (int i = 0; i < allTargets.Length; i++)
            {
                allTargets[i].position = Vector3.Lerp(startPositions[i], endPositions[i], timer);
            }
            yield return null;  
        }

        
        for (int i = 0; i < allTargets.Length; i++)
        {
            allTargets[i].position = endPositions[i];
        }

        isMoving = false;
    }
}