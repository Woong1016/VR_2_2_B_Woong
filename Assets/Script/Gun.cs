using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class Gun : MonoBehaviour
{
    [Header("레이캐스트 사격 설정")]
    public float maxDistance = 100f;
    public LayerMask hitLayers = -1;

    [Header("총알 구멍 설정")]
    public GameObject bulletHolePrefab;
    public float holeOffset = 0.01f;

    [Header("탄피 배출 관련")]
    public GameObject shellPrefab;
    public Transform ejectionPoint;
    public float ejectionForce = 2f;

    [Header("총구 위치")]
    public Transform muzzlePoint;

    [Header("오디오 관련")]
    private AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip emptyClickSound;
    public AudioClip magInsertSound;

    [Header("재장전 관련")]
    public XRSocketInteractor magazineSocket;
    private Magazine currentMagazine = null;
    private bool hasMagazine = false;

    [Header("진동 설정")]
    public float hapticAmplitude = 0.8f;
    public float hapticDuration = 0.1f;

    private Animator boltAnimator;
    private bool isBoltLocked = false;

    public GameObject BoomPos;

    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(Fire);

        audioSource = GetComponent<AudioSource>();

        if (magazineSocket != null)
        {
            magazineSocket.selectEntered.AddListener(MagazineInserted);
            magazineSocket.selectExited.AddListener(MagazineRemoved);
        }
    }

    public void Fire(ActivateEventArgs arg)
    {
        if (isBoltLocked)
        {
            ReleaseBolt();
            if (BoomPos != null)
            {
                Quaternion fixRotation = muzzlePoint.rotation * Quaternion.Euler(0, -90, 0);
                EffectManager.instance.PlayEffect("Boom", BoomPos.transform.position, fixRotation);
            }
            return;
        }

        if (hasMagazine)
        {
            if (currentMagazine.ammoCount > 0)
            {
                RaycastHit hit;

                if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, maxDistance, hitLayers, QueryTriggerInteraction.Ignore))
                {
                    TargetScore target = hit.collider.GetComponent<TargetScore>();
                    if (target != null) target.OnHit(hit.point);

                    ReactionTarget reactionTarget = hit.collider.GetComponent<ReactionTarget>();
                    if (reactionTarget != null) reactionTarget.OnHit();

                    if (bulletHolePrefab != null)
                    {
                        Vector3 spawnPos = hit.point + (hit.normal * holeOffset);
                        Quaternion spawnRot = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90, 0, 0);
                        GameObject hole = Instantiate(bulletHolePrefab, spawnPos, spawnRot);
                        hole.transform.SetParent(hit.collider.transform);
                        Destroy(hole, 10f);
                    }

                    if (hit.rigidbody != null) hit.rigidbody.AddForce(-hit.normal * 50f);
                }

                currentMagazine.ammoCount--;

                if (shellPrefab != null && ejectionPoint != null)
                {
                    GameObject spawnedShell = Instantiate(shellPrefab, ejectionPoint.position, ejectionPoint.rotation);
                    Rigidbody shellRb = spawnedShell.GetComponent<Rigidbody>();
                    shellRb.AddForce(ejectionPoint.right * ejectionForce, ForceMode.Impulse);
                    Destroy(spawnedShell, 3f);
                }

                audioSource.PlayOneShot(fireSound);


                if (BoomPos != null)
                {
                    Quaternion fixRotation = muzzlePoint.rotation * Quaternion.Euler(0, -90, 0);
                    EffectManager.instance.PlayEffect("Boom", BoomPos.transform.position, fixRotation);
                }

                if (arg.interactorObject is XRBaseControllerInteractor controllerInteractor)
                {
                    controllerInteractor.xrController.SendHapticImpulse(hapticAmplitude, hapticDuration);
                }

                if (currentMagazine.ammoCount <= 0) LockBolt();
            }
            else
            {
                audioSource.PlayOneShot(emptyClickSound);
                LockBolt();
            }
        }
        else
        {
            audioSource.PlayOneShot(emptyClickSound);
        }
    }

    private void MagazineInserted(SelectEnterEventArgs arg)
    {
        currentMagazine = arg.interactableObject.transform.GetComponent<Magazine>();
        if (currentMagazine != null)
        {
            hasMagazine = true;
            audioSource.PlayOneShot(magInsertSound);
            if (currentMagazine.ammoCount > 0 && isBoltLocked) ReleaseBolt();
        }
    }

    private void MagazineRemoved(SelectExitEventArgs arg)
    {
        currentMagazine = null;
        hasMagazine = false;
    }

    private void LockBolt()
    {
        if (boltAnimator != null && !isBoltLocked)
        {
            isBoltLocked = true;
            boltAnimator.SetTrigger("Lock");
        }
    }

    private void ReleaseBolt()
    {
        if (boltAnimator != null && isBoltLocked)
        {
            isBoltLocked = false;
            boltAnimator.SetTrigger("Release");
        }
    }
}