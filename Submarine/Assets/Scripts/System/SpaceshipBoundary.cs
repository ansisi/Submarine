using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 기능을 위해 추가
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class SpaceshipBoundary : MonoBehaviour
{
    public static SpaceshipBoundary Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }
        Instance = this;
    }

    public Transform player;
    public ItemSpawner itemSpawner;
    [SerializeField] private float baseRadius = 3f;
    public float currentRadius = 3f;
    public int circleSegments = 100;

    private LineRenderer line;
    public CanvasGroup canvasGroup;  // 페이드 아웃을 위한 CanvasGroup
    public TextMeshProUGUI countdownText;  // 카운트다운을 표시할 텍스트 UI
    public Image fadeImage;

    private float countdownTime = 10f;  // 카운트다운 시간 (10초)
    private bool isOutOfBounds = false;  // 범위 벗어났는지 확인하는 변수

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = circleSegments + 1;
        line.loop = true;
        line.useWorldSpace = true;
        line.widthMultiplier = 0.5f;

        countdownText.text = "";  // 텍스트 초기화

        Color color = fadeImage.color;
        color.a = 0f; // 시작 시 완전 투명
        fadeImage.color = color;

        DrawCircle();
    }

    void Update()
    {
        float distance = Vector2.Distance(new Vector2(player.position.x, player.position.y), new Vector2(transform.position.x, transform.position.y));

        if (distance > currentRadius)
        {
            if (!isOutOfBounds)  // 처음 벗어난 경우에만 카운트다운 시작
            {
                isOutOfBounds = true;
                StartCoroutine(StartCountdown());
            }
        }
        else
        {
            // 범위 내로 돌아오면 카운트다운 및 페이드 아웃 초기화
            if (isOutOfBounds)
            {
                isOutOfBounds = false;
                StopAllCoroutines();  // 카운트다운 정지
                countdownText.text = "";  // 텍스트 초기화
                StartCoroutine(FadeIn());  // 페이드 인
            }
        }

        DrawCircle(); // 실시간으로 반경 반영
    }

    public void SetAntennaUpgradeLevel(int level)
    {
        currentRadius = baseRadius * Mathf.Pow(1.4f, level);
        itemSpawner.ExpandSecondSpawnAreaByTwo();
        DrawCircle();
    }
    
    void DrawCircle()
    {
        if (line == null)  // line이 초기화되지 않았을 경우
            line = GetComponent<LineRenderer>();

        float angle = 0f;
        for (int i = 0; i <= circleSegments; i++)
        {
            float x = Mathf.Cos(angle) * currentRadius;
            float y = Mathf.Sin(angle) * currentRadius;
            line.SetPosition(i, new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z)); // Z 고정
            angle += 2 * Mathf.PI / circleSegments;
        }
    }

    // 카운트다운 시작
    private IEnumerator StartCountdown()
    {
        float timer = countdownTime;
        StartCoroutine(FadeOut());

        // 초기 메시지 출력
        countdownText.text = $"[{Mathf.Ceil(timer)}]\n경고: 우주 유영 제한 거리를 초과했습니다.\n복귀하지 않으면 생존이 불가합니다.";

        while (timer > 0)
        {
            countdownText.text = $"[{Mathf.Ceil(timer)}]\n경고: 우주 유영 제한 거리를 초과했습니다.\n복귀하지 않으면 생존이 불가합니다.";
            timer -= Time.deltaTime;
            yield return null;
        }

        // 카운트다운 종료 후
        countdownText.text = "";
        // 카운트다운이 끝나면 게임 오버 로직
        GameManager.Instance.GameOver();
    }

    // 페이드 아웃 효과
    private IEnumerator FadeOut()
    {
        float fadeTime = 10f;  // 10초 동안 페이드 아웃
        float startAlpha = fadeImage.color.a;

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeTime;
            float newAlpha = Mathf.Lerp(startAlpha, 1f, normalizedTime); // 점점 불투명해짐

            Color currentColor = fadeImage.color;
            currentColor.a = newAlpha;
            fadeImage.color = currentColor;

            yield return null;
        }

        // 완전 검게 고정
        Color finalColor = fadeImage.color;
        finalColor.a = 1f;
        fadeImage.color = finalColor;
    }

    // 페이드 인 효과
    private IEnumerator FadeIn()
    {
        float fadeTime = 2f;  // 페이드 인 시간
        float startAlpha = fadeImage.color.a;

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, normalizedTime); // 점점 투명해짐

            Color currentColor = fadeImage.color;
            currentColor.a = newAlpha;
            fadeImage.color = currentColor;

            yield return null;
        }

        // 완전히 투명하게 고정
        Color finalColor = fadeImage.color;
        finalColor.a = 0f;
        fadeImage.color = finalColor;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 center = transform.position;

        float baseR = baseRadius; // base → baseR 또는 다른 이름

        Color[] levelColors = new Color[]
        {
            new Color(1f, 1f, 1f, 0.3f),        // Level 0 - 하얀색
            new Color(0.2f, 0.8f, 1f, 0.3f),    // Level 1 - 하늘색
            new Color(0.4f, 1f, 0.4f, 0.3f),    // Level 2 - 연두색
            new Color(1f, 0.4f, 0.4f, 0.3f)     // Level 3 - 연한 빨강
        };

        for (int level = 0; level <= 3; level++)
        {
            float radius = baseR * Mathf.Pow(1.4f, level);
            Gizmos.color = levelColors[level];
            DrawCircleGizmo(center, radius, 100);
        }
    }

    private void DrawCircleGizmo(Vector3 center, float radius, int segments)
    {
        float angle = 0f;
        Vector3 prev = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;

        for (int i = 1; i <= segments; i++)
        {
            angle += 2 * Mathf.PI / segments;
            Vector3 next = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
#endif
}