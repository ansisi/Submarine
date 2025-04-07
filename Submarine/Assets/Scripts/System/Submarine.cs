using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    [SerializeField] private OxygenTank oxygenTank;

    // 연료 게이지 관련 변수 (시간 기반)
    public RectTransform fuelNeedle;     // UI 연료 게이지 바늘 (Inspector에서 할당)
    public float maxFuelTime = 600f;       // 연료가 다 떨어질 때까지의 최대 시간(초)
    private float elapsedFuelTime = 0f;    // 경과된 연료 소비 시간

    // 게이지 바늘의 회전 각도 (UI에 맞게 조정)
    public float fullFuelAngle = -195f;    // 연료가 가득 찼을 때(소비 시간이 0일 때)의 바늘 각도
    public float emptyFuelAngle = 90f;     // 연료가 다 떨어졌을 때(소비 시간이 maxFuelTime일 때)의 바늘 각도


    // 부품 수집 등 기존 기능 관련 변수
    private Dictionary<PartType, int> collectedParts = new Dictionary<PartType, int>();
    private List<PartType> missionParts;    // 
    private Animator animator;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        missionParts = GameManager.Instance.GetMissionParts();

        foreach (PartType part in missionParts)
        {
            collectedParts[part] = 0;
        }

    }

    void Update()
    {
        // 매 프레임마다 연료 소비 시간을 누적 (시간 기반 소비)
        elapsedFuelTime += Time.deltaTime;

        // 연료 잔량 비율 계산 (0 ~ 1)
        float fuelRatio = Mathf.Clamp01(elapsedFuelTime / maxFuelTime);

        // 연료 게이지 바늘 회전 각도 계산
        float fuelAngle = Mathf.Lerp(fullFuelAngle, emptyFuelAngle, fuelRatio);
        if (fuelNeedle != null)
        {
            fuelNeedle.localEulerAngles = new Vector3(0, 0, fuelAngle);
        }

        bool isFuelLow = fuelAngle >= 30f;

        bool isOxygenLow = oxygenTank != null && oxygenTank.IsLow();

        VignetteController.Instance.UpdateVignetteState(isOxygenLow, isFuelLow);

        // 연료가 다 소비되었을 때 게임 오버 처리 (또는 원하는 임계값 사용)
        if (fuelRatio >= 1.0f)
        {
            GameManager.Instance.GameOver();
            Logger.Log("연료 부족! 게임 오버");
        }
    }


    // 연료 보충 함수 (연료 소비 시간 감소)
    public void AddFuel(float amount)
    {
        // 코루틴을 이용하여 부드럽게 연료 소비 시간을 감소시킵니다.
        StopCoroutine("RefillFuel");
        StartCoroutine(RefillFuel(amount));
    }

    private IEnumerator RefillFuel(float amount)
    {
        float startTime = elapsedFuelTime;
        float targetTime = Mathf.Max(elapsedFuelTime - amount, 0f); // 음수가 되지 않도록
        float duration = 1.0f; // 보충 효과 지속 시간
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            elapsedFuelTime = Mathf.Lerp(startTime, targetTime, t / duration);
            yield return null;
        }
        elapsedFuelTime = targetTime;
    }

    // 부품 추가 함수 등 기존 기능 유지
    public void AddPart(PartType partType)
    {
        // 미션 부품에 포함된 경우만 처리
        if (missionParts.Contains(partType))
        {
            collectedParts[partType]++;
            Logger.Log($"부품 추가: {partType} - {collectedParts[partType]}개");
            GameManager.Instance.UpdateCollectedParts(partType, collectedParts[partType]);
        }
        else
        {
            Logger.Log($"부품 {partType}는 미션과 관련 없음 연료 패널티 적용");
            AddFuel(-30f); // 패널티 적용
            animator.SetTrigger("WrongTrigger");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        DeliverableItem item = other.GetComponent<DeliverableItem>();
        if (item != null && item.isGrabbed)
        {
            item.OnDelivered(this);
        }
    }
}
