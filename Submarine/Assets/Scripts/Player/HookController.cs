using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookController : MonoBehaviour
{
    public Transform hookSpawnPoint;
    public float hookSpeed = 25f;
    public float maxHookDistance = 30f;
    public float retractSpeed = 15f;
    public float grabRange = 1f;
    public float springForce = 50f;
    public float springDamper = 5f;
    public LayerMask collisionLayers;
    public float ropeMaxLength = 15f; // 로프 최대 길이
    public float ropeBreakForce = 1000f; // 로프가 끊어지는 힘
    public float ropeTensionMultiplier = 5f; // 로프 장력 승수
    public float ropeBreakDistance = 20f; // 로프가 끊어지는 거리
    public Mesh mesh; //후크 모델링
    public Material material;   // 후크 모델링 머테리얼
    public PlayerPickup playerPickup;
    public bool isHookActive = false;

    private float pullForce = 1.3f; //한 번 당겨져오는 거리
    private LineRenderer lineRenderer;
    private GameObject hookObject;
    private Vector3 hookPosition;
    private Vector3 hookVelocity;
    private bool isRetracting = false;
    private GameObject attachedObject;
    private ConfigurableJoint ropeJoint; // 로프 조인트
    private Rigidbody attachedRigidbody; // 연결된 물체의 Rigidbody
    private float initialRopeLength; // 초기 로프 길이

    public HookUIManager uiManager; // UI 매니저 참조

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // 충돌 레이어 기본값 설정 (모든 레이어와 충돌)
        if (collisionLayers.value == 0)
            collisionLayers = Physics.AllLayers;

        Logger.Log("후크 컨트롤러 초기화 완료");
    }

    void OnEnable()
    {
        // 혹시 이전 실행에서 남은 후크가 있다면 정리
        if (hookObject != null)
            Destroy(hookObject);

        // 라인 렌더러 초기화
        lineRenderer.positionCount = 0;

        ClearRopeConnection();
    }

    void OnDisable()
    {
        // 스크립트가 비활성화될 때 모든 라인 렌더러 초기화
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 0;
        }
    }

    void Update()
    {
        if (playerPickup.IsGrabbing == false)
        {
            if (Input.GetMouseButtonDown(1))
            {
            if (!isHookActive)
                FireHook();
            else if (attachedObject == null)
                StartRetraction();
            else
                ClearRopeConnection(); // 로프 연결 해제
            }

        
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isHookActive && attachedRigidbody != null)
                {
                    PullHook();
                }
            }
        }

        if (isHookActive)
        {
            // 후크 위치 업데이트 (회수 중이거나 물체가 연결되지 않은 경우만)
            if (!isRetracting && attachedObject == null)
            {
                hookPosition += hookVelocity * Time.deltaTime;

                // 충돌 체크
                CheckHookCollision();

                // 최대 거리 체크
                float distance = Vector3.Distance(transform.position, hookPosition);
                if (distance >= maxHookDistance)
                    StartRetraction();
            }
            else if (isRetracting)
            {
                // 후크 되감기
                Vector3 retractDirection = (transform.position - hookPosition).normalized;
                hookVelocity = retractDirection * retractSpeed;
                hookPosition += hookVelocity * Time.deltaTime;

                // 후크가 플레이어에 충분히 가까운지 확인
                float distance = Vector3.Distance(transform.position, hookPosition);
                if (distance <= grabRange)
                    CleanupHook();
            }

            // 물체가 연결된 경우 로프 관리
            if (attachedObject != null && !isRetracting)
            {
                ManageRopeConnection();

                // 후크가 플레이어에 충분히 가까운지 확인
                float distance = Vector3.Distance(transform.position, attachedObject.transform.position);
                if (distance <= grabRange + 0.5f)
                    CleanupHook();
            }

            // 후크 시각적 표현 업데이트
            if (hookObject != null)
                hookObject.transform.position = hookPosition;

            // 라인 렌더러가 활성화되어 있고 위치 업데이트가 필요한 경우에만 업데이트
            if (lineRenderer.enabled && (isHookActive || attachedObject != null))
            {
                UpdateRope(); 
                if(attachedObject != null)
                hookObject.transform.position = attachedObject.transform.position;

            }
            else if (lineRenderer.positionCount > 0)
            {
                // 라인 렌더러가 필요하지 않은데 활성화되어 있으면 초기화
                lineRenderer.positionCount = 0;
            }
        }
        else if (lineRenderer.positionCount > 0)
        {
            // 후크가 활성화되지 않았는데 라인 렌더러가 남아있으면 초기화
            lineRenderer.positionCount = 0;
        }

    }

    void FireHook()
    {
        // 후크 오브젝트 생성
        hookObject = new GameObject("Hook");
        hookObject.transform.position = hookSpawnPoint.position;

        // 크기 축소
        hookObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); // 크기 절반으로 줄임

        // 시각화를 위해 스프라이트 렌더러나 메시 추가 (필요시)
        // MeshFilter 추가 및 할당
        MeshFilter meshFilter = hookObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        // MeshRenderer 추가 및 머테리얼 적용
        MeshRenderer meshRenderer = hookObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        SphereCollider hookVisual = hookObject.AddComponent<SphereCollider>();
        hookVisual.radius = 0.2f;
        hookVisual.isTrigger = true; // 물리적 충돌이 아닌 트리거로 설정

        // 초기 위치와 방향 설정
        hookPosition = hookSpawnPoint.position;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint = ray.origin + ray.direction * maxHookDistance;
        targetPoint.z = 0f;

        Vector3 direction = (targetPoint - hookSpawnPoint.position).normalized;
        direction.z = 0f;
        hookVelocity = direction * hookSpeed;


        // 방향을 기준으로 회전 적용
        Quaternion baseRotation = Quaternion.Euler(180f, 0f, 90f);
        Quaternion directionRotation = Quaternion.LookRotation(direction);
        hookObject.transform.rotation = directionRotation * baseRotation;

        isHookActive = true;
        isRetracting = false;
        lineRenderer.positionCount = 2;

        uiManager?.UpdateHookUI(isHookActive);
    }

    void PullHook()
    {
        Vector3 direction = (transform.position - attachedRigidbody.position).normalized;
        //attachedRigidbody.AddForce(direction * pullForce, ForceMode.Acceleration);
        attachedRigidbody.AddForce(direction * pullForce, ForceMode.Impulse); // 한 번에 강한 힘 적용
    }

    void CheckHookCollision()
    {
        // 레이캐스트로 충돌 감지
        float movementDistance = hookVelocity.magnitude * Time.deltaTime;
        RaycastHit hit;

        // 디버그 레이 그리기 (문제 해결 도움)
        Debug.DrawRay(hookPosition, hookVelocity.normalized * (movementDistance + 0.1f), Color.red, 0.1f);

        // 구체 캐스팅으로 변경 - 더 넓은 충돌 영역 제공
        if (Physics.SphereCast(hookPosition, 0.3f, hookVelocity.normalized, out hit, movementDistance + 0.1f, collisionLayers))
        {
            // 충돌 감지됨
            hookPosition = hit.point;

            // 다양한 오브젝트와의 충돌 처리
            if (hit.collider.CompareTag("Resource"))
            {
                Logger.Log("리소스와 충돌: " + hit.collider.name);
                OnHookCollision(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("Player"))
            {
                // 플레이어와 충돌 처리 (필요한 경우)
                Logger.Log("후크가 플레이어와 충돌했습니다!");
                StartRetraction();
            }
            else
            {
                // 다른 물체와 충돌, 되감기 시작
                Logger.Log("다른 물체와 충돌: " + hit.collider.name);
                StartRetraction();
            }
        }
    }

    void StartRetraction()
    {
        isRetracting = true;
        Vector3 retractDirection = (transform.position - hookPosition).normalized;
        retractDirection.z = 0f;
        hookVelocity = retractDirection * retractSpeed;
    }

    void OnHookCollision(GameObject collidedObject)
    {
        if (collidedObject.CompareTag("Resource"))
        {
            Logger.Log("리소스와 로프 연결 중: " + collidedObject.name);

            // 리소스에 부착
            attachedObject = collidedObject;

            // 오브젝트에 Rigidbody가 없으면 추가
            Rigidbody resourceRb = collidedObject.GetComponent<Rigidbody>();
            if (resourceRb == null)
            {
                resourceRb = collidedObject.AddComponent<Rigidbody>();
                resourceRb.useGravity = true;
                resourceRb.isKinematic = false;
                resourceRb.mass = 1f;
                resourceRb.drag = 0.5f;

                // 콜라이더 확인
                if (collidedObject.GetComponent<Collider>() == null)
                {
                    BoxCollider boxCollider = collidedObject.AddComponent<BoxCollider>();
                    Renderer renderer = collidedObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        boxCollider.center = renderer.bounds.center - collidedObject.transform.position;
                        boxCollider.size = renderer.bounds.size;
                    }
                }
            }

            // 물리 안정화를 위한 설정
            resourceRb.interpolation = RigidbodyInterpolation.Interpolate;
            resourceRb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Rigidbody 참조 저장
            attachedRigidbody = resourceRb;

            // 플레이어와 물체 사이에 로프 조인트 생성
            CreateRopeJoint();

            // 후크가 물체에 위치하도록 설정
            hookPosition = collidedObject.transform.position;

            // 초기 로프 길이 저장
            initialRopeLength = Vector3.Distance(transform.position, attachedObject.transform.position);

            Logger.Log("로프 연결 완료: 초기 길이 = " + initialRopeLength);

            // 회수 모드를 종료하여 로프 모드로 전환
            isRetracting = false;
        }
        else
        {
            StartRetraction();
        }
    }

    void CreateRopeJoint()
    {
        if (attachedObject == null || attachedRigidbody == null)
            return;

        // 플레이어 Rigidbody 확인 또는 추가
        Rigidbody playerRb = GetComponent<Rigidbody>();
        if (playerRb == null)
        {
            playerRb = gameObject.AddComponent<Rigidbody>();
            playerRb.isKinematic = true; // 플레이어는 물리 영향을 받지 않음
            playerRb.useGravity = false;
        }

        // ConfigurableJoint 생성
        ropeJoint = gameObject.AddComponent<ConfigurableJoint>();
        ropeJoint.connectedBody = attachedRigidbody;

        // 각 축 별 모션 제한 설정
        ropeJoint.xMotion = ConfigurableJointMotion.Limited;
        ropeJoint.yMotion = ConfigurableJointMotion.Limited;
        ropeJoint.zMotion = ConfigurableJointMotion.Limited;

        // 회전 자유도 설정
        ropeJoint.angularXMotion = ConfigurableJointMotion.Free;
        ropeJoint.angularYMotion = ConfigurableJointMotion.Free;
        ropeJoint.angularZMotion = ConfigurableJointMotion.Free;

        // 선형 제한 설정 (로프 최대 길이)
        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = ropeMaxLength;
        ropeJoint.linearLimit = limit;

        // 끊어지는 힘 설정
        ropeJoint.breakForce = ropeBreakForce;
        ropeJoint.breakTorque = ropeBreakForce;

        // 스프링 설정 (약간의 탄성)
        SoftJointLimitSpring spring = new SoftJointLimitSpring();
        spring.spring = 10f;
        spring.damper = 5f;
        ropeJoint.linearLimitSpring = spring;

        // 조인트 이벤트 리스너 추가 (조인트가 끊어질 때 감지)
        Rigidbody rb = GetComponent<Rigidbody>();
        HookJointBreakListener breakListener = rb.gameObject.AddComponent<HookJointBreakListener>();
        breakListener.hookController = this;
    }

    void ManageRopeConnection()
    {
        if (attachedObject == null || attachedRigidbody == null)
            return;

        // 현재 거리 계산
        float currentDistance = Vector3.Distance(transform.position, attachedObject.transform.position);

        // 거리가 최대치를 초과하면 연결 해제
        if (currentDistance > ropeBreakDistance)
        {
            Logger.Log("로프가 너무 멀어져서 끊어짐: " + currentDistance);
            ClearRopeConnection();
            return;
        }

        // 로프 장력 계산 (최대 길이에 가까울수록 더 큰 힘)
        if (currentDistance > initialRopeLength)
        {
            float tensionFactor = (currentDistance - initialRopeLength) / (ropeMaxLength - initialRopeLength);
            tensionFactor = Mathf.Clamp01(tensionFactor);

            // 장력 방향 (플레이어 쪽으로)
            Vector3 tensionDirection = (transform.position - attachedObject.transform.position).normalized;

            // 물체에 장력 적용
            float tensionForce = tensionFactor * ropeTensionMultiplier;
            attachedRigidbody.AddForce(tensionDirection * tensionForce, ForceMode.Force);
        }
    }

    void UpdateRope()
    {
        if (lineRenderer.positionCount != 2)
            lineRenderer.positionCount = 2;

        lineRenderer.SetPosition(0, hookSpawnPoint.position);

        // 연결된 물체가 있으면 물체 위치로, 없으면 후크 위치로
        if (attachedObject != null)
            lineRenderer.SetPosition(1, attachedObject.transform.position);
        else if (hookObject != null)
            lineRenderer.SetPosition(1, hookObject.transform.position);
        else
            lineRenderer.SetPosition(1, hookPosition);
    }

    public void OnJointBreak()
    {
        Logger.Log("로프 조인트가 물리적으로 끊어짐");
        ClearRopeConnection();
    }

    void ClearRopeConnection()
    {
        // 로프 조인트 제거
        if (ropeJoint != null)
        {
            Destroy(ropeJoint);
            ropeJoint = null;
        }

        // JointBreakListener 제거
        HookJointBreakListener listener = GetComponent<HookJointBreakListener>();
        if (listener != null)
            Destroy(listener);

        // 연결된 물체 참조 제거
        attachedObject = null;
        attachedRigidbody = null;

        // 라인 렌더러 즉시 초기화
        lineRenderer.positionCount = 0;

        // 후크가 활성화 상태면 회수 시작
        if (isHookActive && !isRetracting)
        {
            StartRetraction();
            // 회수 시작 시 라인 렌더러 다시 활성화
            lineRenderer.positionCount = 2;
        }
        else
        {
            // 모든 후크 관련 상태 초기화
            if (hookObject != null)
                Destroy(hookObject);

            hookObject = null;
            isHookActive = false;
            isRetracting = false;
        }

        Logger.Log("로프 연결 해제됨");
    }

    void CleanupHook()
    {
        // 로프 연결 정리 전 라인 렌더러 비활성화 확인
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;

        ClearRopeConnection();

        // 후크 오브젝트 정리
        if (hookObject != null)
            Destroy(hookObject);

        hookObject = null;
        isHookActive = false;
        isRetracting = false;

        // 라인 렌더러 다시 활성화 (다음 사용을 위해)
        lineRenderer.enabled = true;

        Logger.Log("후크 정리 완료");

        uiManager?.UpdateHookUI(isHookActive);
    }


}

// 조인트가 끊어지는 이벤트를 감지하는 보조 클래스
public class HookJointBreakListener : MonoBehaviour
{
    public HookController hookController;

    void OnJointBreak(float breakForce)
    {
        if (hookController != null)
            hookController.OnJointBreak();
    }
}
