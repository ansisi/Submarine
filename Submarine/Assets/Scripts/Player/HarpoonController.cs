using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HarpoonController : MonoBehaviour
{
   
    public Transform harpoonSpawnPoint;
    public float harpoonSpeed = 25f;
    public float maxHarpoonDistance = 30f;
    public float retractSpeed = 15f;
    public float grabRange = 1f;
    public float springForce = 50f;
    public float springDamper = 5f;
    public LayerMask collisionLayers;
    public float ropeMaxLength = 15f; // 로프 최대 길이
    public float ropeBreakForce = 1000f; // 로프가 끊어지는 힘
    public float ropeTensionMultiplier = 5f; // 로프 장력 승수
    public float ropeBreakDistance = 20f; // 로프가 끊어지는 거리
    public bool isHarpoonActive = false;
    public GameObject harpoonPrefab;

    private float pullForce = 2f; //한 번 당겨져오는 힘
    private LineRenderer lineRenderer;
    private GameObject harpoonObject;
    private Vector3 harpoonPosition;
    private Vector3 harpoonVelocity;
    private bool isRetracting = false;
    private GameObject attachedObject;
    private ConfigurableJoint ropeJoint; // 로프 조인트
    private Rigidbody attachedRigidbody; // 연결된 물체의 Rigidbody
    private float initialRopeLength; // 초기 로프 길이
    private Rigidbody rb;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        foreach (Transform child in transform)
        {
            lineRenderer = child.GetComponent<LineRenderer>();
            if (lineRenderer != null)
            {
                break; // 첫 번째 자식의 LineRenderer를 찾으면 종료
            }
        }
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.03f;
        lineRenderer.endWidth = 0.03f;

        // 충돌 레이어 기본값 설정 (모든 레이어와 충돌)
        if (collisionLayers.value == 0)
            collisionLayers = Physics.AllLayers;

        Logger.Log("작살 컨트롤러 초기화 완료");
    }

    void OnEnable()
    {
        // 혹시 이전 실행에서 남은 후크가 있다면 정리
        if (harpoonObject != null)
            Destroy(harpoonObject);

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isHarpoonActive && attachedRigidbody != null)
            {
                PullHarpoon();
            }
        }
        

        if (isHarpoonActive)
        {
            // 작살 위치 업데이트 (회수 중이거나 물체가 연결되지 않은 경우만)
            if (!isRetracting && attachedObject == null)
            {
                harpoonPosition += harpoonVelocity * Time.deltaTime;

                // 충돌 체크
                CheckHarpoonCollision();

                // 최대 거리 체크
                float distance = Vector3.Distance(transform.position, harpoonPosition);
                if (distance >= maxHarpoonDistance)
                    StartRetraction();
            }
            else if (isRetracting)
            {
                // 작살 되감기
                Vector3 retractDirection = (transform.position - harpoonPosition).normalized;
                harpoonVelocity = retractDirection * retractSpeed;
                harpoonPosition += harpoonVelocity * Time.deltaTime;

                // 후크가 플레이어에 충분히 가까운지 확인
                float distance = Vector3.Distance(transform.position, harpoonPosition);
                if (distance <= grabRange)
                    CleanupHarpoon();
            }

            // 물체가 연결된 경우 로프 관리
            if (attachedObject != null && !isRetracting)
            {
                ManageRopeConnection();
            }

            // 작살 시각적 표현 업데이트
            if (harpoonObject != null)
                harpoonObject.transform.position = harpoonPosition;

            // 라인 렌더러가 활성화되어 있고 위치 업데이트가 필요한 경우에만 업데이트
            if (lineRenderer.enabled && (isHarpoonActive || attachedObject != null))
            {
                UpdateRope();
                if (attachedObject != null)
                {
                    Vector3 offset = harpoonObject.transform.forward * -0.9f + harpoonObject.transform.up * 0.4f;
                    harpoonObject.transform.position = attachedObject.transform.position + offset;
                    UpdateRope();
                }
            }
            else if (lineRenderer.positionCount > 0)
            {
                // 라인 렌더러가 필요하지 않은데 활성화되어 있으면 초기화
                lineRenderer.positionCount = 0;
            }
        }
        else if (lineRenderer.positionCount > 0)
        {
            // 작살가 활성화되지 않았는데 라인 렌더러가 남아있으면 초기화
            lineRenderer.positionCount = 0;
        }

        // UI 위에 있을 때는 입력 무시
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!isHarpoonActive)
                FireHarpoon();
            else if (attachedObject == null)
                StartRetraction();
            else
                ClearRopeConnection(); // 로프 연결 해제
        }

    }

    void FireHarpoon()
    {
        // 작살 오브젝트 생성
        harpoonObject = Instantiate(harpoonPrefab, harpoonSpawnPoint.position, Quaternion.identity);

        // 시각화를 위해 스프라이트 렌더러나 메시 추가 (필요시)
        SphereCollider HarpoonVisual = harpoonObject.AddComponent<SphereCollider>();
        HarpoonVisual.radius = 0.2f;
        HarpoonVisual.isTrigger = true; // 물리적 충돌이 아닌 트리거로 설정

        // 초기 위치와 방향 설정
        harpoonPosition = harpoonSpawnPoint.position;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint = ray.origin + ray.direction * maxHarpoonDistance;
        targetPoint.z = 0f;

        Vector3 direction = (targetPoint - harpoonSpawnPoint.position).normalized;
        direction.z = 0f;
        harpoonVelocity = direction * harpoonSpeed;


        // 방향을 기준으로 회전 적용
        harpoonObject.transform.rotation = Quaternion.LookRotation(direction);
        

        isHarpoonActive = true;
        isRetracting = false;
        lineRenderer.positionCount = 2;

    }

    void PullHarpoon()
    {
        Vector3 direction = (attachedRigidbody.position - transform.position).normalized;
        rb.AddForce(direction * pullForce, ForceMode.Impulse); // 한 번에 강한 힘 적용
    }


    void CheckHarpoonCollision()
    {
        // 레이캐스트로 충돌 감지
        float movementDistance = harpoonVelocity.magnitude * Time.deltaTime;
        RaycastHit hit;

        // 디버그 레이 그리기 (문제 해결 도움)
        Debug.DrawRay(harpoonPosition, harpoonVelocity.normalized * (movementDistance + 0.1f), Color.red, 0.1f);

        // 구체 캐스팅으로 변경 - 더 넓은 충돌 영역 제공
        if (Physics.SphereCast(harpoonPosition, 0.3f, harpoonVelocity.normalized, out hit, movementDistance + 0.1f, collisionLayers))
        {
            // 충돌 감지됨
            harpoonPosition = hit.point;

            // 다양한 오브젝트와의 충돌 처리
            if (hit.collider.CompareTag("Terrain"))
            {
                Logger.Log("지형과 충돌: " + hit.collider.name);
                OnHarpoonCollision(hit.collider.gameObject);
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
        Vector3 retractDirection = (transform.position - harpoonPosition).normalized;
        retractDirection.z = 0f;
        harpoonVelocity = retractDirection * retractSpeed;
    }

    void OnHarpoonCollision(GameObject collidedObject)
    {
        if (collidedObject.CompareTag("Terrain"))
        {
            Logger.Log("지형과 로프 연결 중: " + collidedObject.name);

            // 지형에 부착
            attachedObject = collidedObject;

            // 오브젝트에 Rigidbody가 없으면 추가
            Rigidbody terrainRb = collidedObject.GetComponent<Rigidbody>();
            if (terrainRb == null)
            {
                terrainRb = collidedObject.AddComponent<Rigidbody>();
                terrainRb.useGravity = false;
                terrainRb.isKinematic = true;
                terrainRb.constraints = RigidbodyConstraints.FreezeAll;    //모든 회전,이동 고정
                terrainRb.mass = 100f;

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
            terrainRb.interpolation = RigidbodyInterpolation.Interpolate;
            terrainRb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Rigidbody 참조 저장
            attachedRigidbody = terrainRb;

            // 플레이어와 물체 사이에 로프 조인트 생성
            CreateRopeJoint();

            // 후크가 물체에 위치하도록 설정
            harpoonPosition = collidedObject.transform.position;
            

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
        HarpoonJointBreakListener breakListener = rb.gameObject.AddComponent<HarpoonJointBreakListener>();
        breakListener.harpoonController = this;

        //지형 통과 금지
        ropeJoint.enableCollision = true;
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
            Vector3 tensionDirection = (attachedObject.transform.position - transform.position).normalized;

            // 물체에 장력 적용
            float tensionForce = tensionFactor * ropeTensionMultiplier;
            rb.AddForce(tensionDirection * tensionForce, ForceMode.Force);
        }
    }

    void UpdateRope()
    {
        if (lineRenderer.positionCount != 2)
            lineRenderer.positionCount = 2;

        lineRenderer.SetPosition(0, harpoonSpawnPoint.position);

        // 연결된 물체가 있으면 물체 위치로, 없으면 작살 위치로
        if (attachedObject != null)
            lineRenderer.SetPosition(1, harpoonObject.transform.position);
        else if (harpoonObject != null)
            lineRenderer.SetPosition(1, harpoonObject.transform.position);
        else
            lineRenderer.SetPosition(1, harpoonPosition);
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
        HarpoonJointBreakListener listener = GetComponent<HarpoonJointBreakListener>();
        if (listener != null)
            Destroy(listener);

        // 연결된 물체 참조 제거
        attachedObject = null;
        attachedRigidbody = null;

        // 라인 렌더러 즉시 초기화
        lineRenderer.positionCount = 0;

        // 후크가 활성화 상태면 회수 시작
        if (isHarpoonActive && !isRetracting)
        {
            StartRetraction();
            // 회수 시작 시 라인 렌더러 다시 활성화
            lineRenderer.positionCount = 2;
        }
        else
        {
            // 모든 작살 관련 상태 초기화
            if (harpoonObject != null)
                Destroy(harpoonObject);

            harpoonObject = null;
            isHarpoonActive = false;
            isRetracting = false;
        }

        Logger.Log("로프 연결 해제됨");
    }

    void CleanupHarpoon()
    {
        // 로프 연결 정리 전 라인 렌더러 비활성화 확인
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 0;

        ClearRopeConnection();

        // 작살 오브젝트 정리
        if (harpoonObject != null)
            Destroy(harpoonObject);

        harpoonObject = null;
        isHarpoonActive = false;
        isRetracting = false;

        // 라인 렌더러 다시 활성화 (다음 사용을 위해)
        lineRenderer.enabled = true;

        Logger.Log("작살 정리 완료");

    }


}

// 조인트가 끊어지는 이벤트를 감지하는 보조 클래스
public class HarpoonJointBreakListener : MonoBehaviour
{
    public HarpoonController harpoonController;

    void OnJointBreak(float breakForce)
    {
        if (harpoonController != null)
            harpoonController.OnJointBreak();
    }
}


