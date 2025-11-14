using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform muzzlePoint;
    public float bulletSpeed = 100f;

    public GameObject shellPrefab;
    public Transform ejectionPoint;
    public float ejectionForce = 2f;

    private AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip emptyClickSound;
    public AudioClip magInsertSound;

    public XRSocketInteractor magazineSocket;
    private Magazine currentMagazine = null;
    private bool hasMagazine = false;

    public float hapticAmplitude = 0.8f;
    public float hapticDuration = 0.1f;

    public float recoilForce = 10f;
    public float recoilUpwardForce = 5f;

    private Animator boltAnimator;
    private bool isBoltLocked = false;

    private Rigidbody gunRigidbody;

    void Start()
    {
        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(Fire);

        audioSource = GetComponent<AudioSource>();

        gunRigidbody = GetComponent<Rigidbody>();
        if (gunRigidbody == null)
        {
            Debug.LogError("Gun 오브젝트에 Rigidbody 컴포넌트가 없습니다! 반동 효과를 위해 추가해주세요.");
        }

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
            return;
        }

        if (hasMagazine)
        {
            if (currentMagazine.ammoCount > 0)
            {
                GameObject spawnedBullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
                spawnedBullet.GetComponent<Rigidbody>().velocity = muzzlePoint.forward * bulletSpeed;
                Destroy(spawnedBullet, 5f);

                currentMagazine.ammoCount--;
                if (shellPrefab != null && ejectionPoint != null)
                {
                    GameObject spawnedShell = Instantiate(shellPrefab, ejectionPoint.position, ejectionPoint.rotation);

                    Rigidbody shellRb = spawnedShell.GetComponent<Rigidbody>();

                    shellRb.AddForce(ejectionPoint.right * ejectionForce, ForceMode.Impulse);

                    Destroy(spawnedShell, 3f);
                }

                audioSource.PlayOneShot(fireSound);

                if (arg.interactorObject is XRBaseControllerInteractor controllerInteractor)
                {
                    controllerInteractor.xrController.SendHapticImpulse(hapticAmplitude, hapticDuration);
                }

                if (gunRigidbody != null)
                {
                    gunRigidbody.AddForce(-muzzlePoint.forward * recoilForce, ForceMode.Impulse);
                    gunRigidbody.AddForce(muzzlePoint.up * recoilUpwardForce, ForceMode.Impulse);
                }

                if (currentMagazine.ammoCount <= 0)
                {
                    LockBolt();
                }
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

            if (currentMagazine.ammoCount > 0 && isBoltLocked)
            {
                ReleaseBolt();
            }
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