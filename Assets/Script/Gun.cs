using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class Gun : MonoBehaviour
{
    [Header("레이캐스트 사격 설정")]
    public float maxDistance = 100f; // 사격 가능한 최대 거리
    public LayerMask hitLayers = -1; // 맞출 수 있는 레이어 (기본값: 모든 레이어)

    [Header("총알 구멍 설정")]
    public GameObject bulletHolePrefab; // 총알 구멍 프리팹
    public float holeOffset = 0.01f;    // 구멍이 벽에 파묻히지 않게 띄울 거리

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

    // 내부 변수
    private Animator boltAnimator;
    private bool isBoltLocked = false;

    public GameObject BoomPos; // 이펙트 위치

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
        // 1. 노리쇠가 잠겨있으면 전진만 시키고 발사 안 함
        if (isBoltLocked)
        {
            ReleaseBolt();
            if (BoomPos != null) EffectManager.instance.PlayEffect("Boom", BoomPos.transform.position, Quaternion.identity);
            return;
        }

        // 2. 탄창과 총알 확인
        if (hasMagazine)
        {
            if (currentMagazine.ammoCount > 0)
            {
                // ==========================================
                // 레이캐스트 발사 로직
                // ==========================================
                RaycastHit hit;

                // 총구 위치에서, 총구 앞방향으로 레이저를 발사
                if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, maxDistance, hitLayers))
                {
                    // A. 맞은 물체가 표적지인지 확인 (TargetScore 스크립트 찾기)
                    TargetScore target = hit.collider.GetComponent<TargetScore>();
                    if (target != null)
                    {
                        // 표적지라면 점수 계산 함수 호출
                        target.OnHit(hit.point);
                    }

                    // B. 총알 구멍 생성
                    if (bulletHolePrefab != null)
                    {
                        Vector3 spawnPos = hit.point + (hit.normal * holeOffset);
                        Quaternion spawnRot = Quaternion.LookRotation(hit.normal) * Quaternion.Euler(90, 0, 0);

                        GameObject hole = Instantiate(bulletHolePrefab, spawnPos, spawnRot);
                        hole.transform.SetParent(hit.collider.transform); // 맞은 물체를 부모로 설정

                        Destroy(hole, 10f); // 10초 뒤 삭제
                    }

                    // C. (선택) 맞은 물체가 물리효과(Rigidbody)가 있다면 밀어버리기
                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(-hit.normal * 50f);
                    }
                }
                // ==========================================

                // 3. 탄약 감소
                currentMagazine.ammoCount--;

                // 4. 탄피 배출
                if (shellPrefab != null && ejectionPoint != null)
                {
                    GameObject spawnedShell = Instantiate(shellPrefab, ejectionPoint.position, ejectionPoint.rotation);
                    Rigidbody shellRb = spawnedShell.GetComponent<Rigidbody>();
                    shellRb.AddForce(ejectionPoint.right * ejectionForce, ForceMode.Impulse);
                    Destroy(spawnedShell, 3f);
                }

                // 5. 소리 및 이펙트
                audioSource.PlayOneShot(fireSound);
                if (BoomPos != null) EffectManager.instance.PlayEffect("Boom", BoomPos.transform.position, Quaternion.identity);

                // 6. 햅틱 진동 (손맛은 남겨둠)
                if (arg.interactorObject is XRBaseControllerInteractor controllerInteractor)
                {
                    controllerInteractor.xrController.SendHapticImpulse(hapticAmplitude, hapticDuration);
                }

                // 7. 반동 로직 (삭제됨)

                // 8. 마지막 탄환 체크 (노리쇠 후퇴 고정)
                if (currentMagazine.ammoCount <= 0)
                {
                    LockBolt();
                }
            }
            else // 총알 없음
            {
                audioSource.PlayOneShot(emptyClickSound);
                LockBolt();
            }
        }
        else // 탄창 없음
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