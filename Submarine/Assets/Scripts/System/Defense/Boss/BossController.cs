using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Net.NetworkInformation;

[RequireComponent(typeof(FactionHandler))]
public class BossController : MonoBehaviour, IDamageable
{
    [Header("���� ����")]
    [SerializeField] private float maxHealth = 100f;                               // ���� �ִ� ü���Դϴ�.
    [SerializeField] private float currentHealth;                                  // ���� ���� ü���Դϴ�.
    [SerializeField] private float patternInterval = 5f;          // ���� ���ϱ��� ��� �ð��Դϴ�.

    [Header("���� ��ȯ ����")]  
    [SerializeField] private List<GameObject> monsterPrefabs;     // ��ȯ�� ���� ������ ����Ʈ�Դϴ�.
    [SerializeField] private int spawnCount = 6;                  // �� ���� ��ȯ�� ���� ���Դϴ�.
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(10f, 10f); // ��ȯ ���� ������ (X, Z)

    [Header("��ŷ ���� ����")]
    [SerializeField] private List<Vector3> hackZoneCenters; // ��ŷ ��� �������� �߽� ��ġ ����Ʈ
    [SerializeField] private Vector2 hackZoneSize = new Vector2(15f, 15f); // ��ŷ �ڽ� ũ�� (X, Z)
    [SerializeField] private GameObject hackIndicatorPrefab; // �ε������� ������ (������ ���� �ڽ�)

    [Header("EMP ���� ����")]
    [SerializeField] private float empCooldown = 20f;                 // EMP ���� ��Ÿ���Դϴ�.
    [SerializeField] private float empRadius = 10f;                   // EMP ȿ�� �ݰ��Դϴ�.
    [SerializeField] private float empEffectDuration = 5f;            // �ͷ����� ����� EMP ���� �ð��Դϴ�.
    [SerializeField] private GameObject empWarningPrefab;             // EMP ���� ����� ������Ʈ
    [SerializeField] private float empWarningDuration = 2f;           // ��� �ð�
    [SerializeField] private float empBlinkInterval = 0.2f;           // ����̴� ����
    private float lastEmpTime = -Mathf.Infinity;                      // ������ EMP ���� �ð��Դϴ�.


    [Header("Ǫ�� ���� ����")]
    [SerializeField] private GameObject pushWavePrefab;                 // Ǫ�� ���� �ĵ�(LineRenderer ��) ������
    [SerializeField] private float pushWarningDuration = 1f;            // �ĵ� ��� ���� �ð��Դϴ�.
    [SerializeField] private LayerMask pushableLayerMask;               // Ǫ�� ������ ���̾� ����ũ (�ͷ�, �÷��̾� ��)
    [SerializeField] private LayerMask playerLayerMask;  // �÷��̾� ���̾ ����
    private float nextPushThreshold;                                    // ���� Ǫ�� ���� �ߵ� ü�� �Ӱ�ġ
    private bool pushPending = false;

    [Header("�ڱ�� ���� ����")]
    [SerializeField] private float pullRadius = 20f;            // ������ �ݰ�
    [SerializeField] private float pullDuration = 5f;           // ������� ���� �ð� (���� 5��)
    [SerializeField] private float pullSpeed = 5f;              // ������� �ӵ�
    [SerializeField] private float pullDamage = 200f;           // �߽� ���� �� ���� �����
    [SerializeField] private float groggyDuration = 5f;         // �׷α� ���� �ð�
    [SerializeField] private float reachThreshold = 0.5f;       // ���� �߽� ���� ���� �Ÿ�
    private float nextPullThreshold;     // ���� Pull ���� �ߵ� ü�� ����
    private bool pullPending = false;    // Pull ���� ��� �÷���
    private bool isGroggy = false;       // �׷α� ���� �÷���

    [Header("�ڱ�� ���� ����Ʈ ����")]
    [SerializeField] private int pullEffectSegments = 64; // ������ ��
    [SerializeField] private float pullEffectDuration = 1f; // �� ���� �پ��� �ð�
    [SerializeField] private float pullEffectInterval = 0.3f; // �� ���� ����
    private List<GameObject> pullEffectInstances = new List<GameObject>();
    private Coroutine pullEffectCoroutine;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;



    private List<Pattern> patterns;                      // ���� ������ ���ϵ��� ��������Ʈ ����Ʈ�Դϴ�.
    private bool isPatternRunning = false;                        // ���� ���� �ߺ� ���� �÷����Դϴ�.
    
    

    public Action<string> OnPatternWarningStarted;  // ��� ���� �� ȣ�� (���� �̸� ����)
    public Action OnPatternWarningEnded;            // ��� ���� �� ȣ��
    // ���� ������ ���� Ŭ����
    private class Pattern
    {
        public Func<IEnumerator> PatternRoutine;   // ���� ���� �ڷ�ƾ �Լ�
        public float WarningTime;                  // ���� ���� �� ���ð�

        public Pattern(Func<IEnumerator> routine, float warningTime)
        {
            PatternRoutine = routine;
            WarningTime = warningTime;
        }
    }
    private void Start()
    {
        currentHealth = maxHealth;                                // ���� ü�� �ʱ�ȭ

        BossUIManager.Instance.ShowFor(this);

        nextPushThreshold = maxHealth * 0.85f;                    // 15% ���� �������� Ǫ��
        nextPullThreshold = maxHealth * 0.7f;
        InitializePatterns();                                     // ���� ����Ʈ �ʱ�ȭ
        //StartCoroutine(PullPattern());
        StartCoroutine(PatternRunner());                   // ���� ���� �ڷ�ƾ ����
    }

    private void InitializePatterns()
    {
        patterns = new List<Pattern>()
        {
            new Pattern(SpawnMonsterPattern, 3f),  // ���� ��ȯ ����, ���� �� 2�� ���
            new Pattern(HackTurretPattern, 3f),  // ��ŷ ����, ���� �� 3�� ���
            new Pattern(EmpPattern, 3f), 
            // , OtherPattern1, OtherPattern2 �� ���� ���� �߰�
        };
    }

    private IEnumerator PatternRunner()
    {
        while (currentHealth > 0)
        {
            if (!isPatternRunning && !isGroggy)
            {
                isPatternRunning = true;

                // PushBackPattern (���� ���� �켱����) 
                if (pushPending)
                {
                    yield return StartCoroutine(PatternWarning(0f, nameof(PushBackPattern)));
                    yield return StartCoroutine(PushBackPattern());
                    pushPending = false;
                }
                // PullPattern
                else if (pullPending)
                {
                    yield return StartCoroutine(PatternWarning(2f, nameof(PullPattern)));
                    yield return StartCoroutine(PullPattern());
                    pullPending = false;
                }
                // EmpPattern (��ٿ� & ��� ���� ��)
                else if (Time.time - lastEmpTime >= empCooldown &&
                         Physics.OverlapSphere(transform.position, empRadius)
                             .Any(c => c.TryGetComponent<Turret>(out _)))
                {
                    yield return StartCoroutine(PatternWarning(3f, nameof(EmpPattern)));
                    yield return StartCoroutine(EmpPattern());
                }
                // SpawnMonsterPattern & HackTurretPattern (���� �켱����)
                else
                {
                    // �������� �ϳ� ����
                    var list = new List<Pattern> 
                    {
                        new Pattern(SpawnMonsterPattern, 3f),
                        new Pattern(HackTurretPattern, 3f)
                    };
                    int idx = UnityEngine.Random.Range(0, list.Count);
                    var p = list[idx];
                    yield return StartCoroutine(PatternWarning(p.WarningTime, p.PatternRoutine.Method.Name));
                    yield return StartCoroutine(p.PatternRoutine());
                }

                yield return new WaitForSeconds(patternInterval);
                isPatternRunning = false;
            }
            yield return null;
        }
    }

    private IEnumerator PatternWarning(float warningTime, string patternName)
    {
        Logger.Log($"[���� ���] {patternName} ������ {warningTime}�� �� ���۵˴ϴ�.");
        OnPatternWarningStarted?.Invoke(patternName);  // ��� ���� �̺�Ʈ ȣ��

        yield return new WaitForSeconds(warningTime);

        OnPatternWarningEnded?.Invoke();               // ��� ���� �̺�Ʈ ȣ��
    }

    private IEnumerator SpawnMonsterPattern()
    {
        // ���� 6������ ���� �ֺ��� ���� ��ġ�� ��ȯ
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 offset = new Vector3(
                UnityEngine.Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                UnityEngine.Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f), 0f); 
            Vector3 spawnPos = transform.position + offset;

            Instantiate(monsterPrefabs[UnityEngine.Random.Range(0, monsterPrefabs.Count)],
                        spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(0.3f);  // ��ȯ ���� (���� ����)
        }
    }

    private IEnumerator HackTurretPattern()
    {
        // ���� ���� �ͷ��� �ִ� ���� �ε��� ã��
        int maxTurretCount = -1;
        List<int> candidateZones = new List<int>();

        for (int i = 0; i < hackZoneCenters.Count; i++)
        {
            Vector3 center = hackZoneCenters[i];
            Vector3 halfExtents = new Vector3(hackZoneSize.x / 2f, hackZoneSize.y / 2f, 1f); // Z�� ���
            
            Collider[] cols = Physics.OverlapBox(center, halfExtents);
            
            int count = cols.Count(c => c.TryGetComponent<Turret>(out _));

            if (count > maxTurretCount)
            {
                maxTurretCount = count;
                candidateZones.Clear();
                candidateZones.Add(i);
            }
            else if (count == maxTurretCount)
            {
                candidateZones.Add(i);
            }
        }

        // �ͷ� ���� ���� ������ ���� ���� �� �� ���� ����
        int bestZoneIndex = candidateZones[UnityEngine.Random.Range(0, candidateZones.Count)];

        // �ε������� ������ ���� (��� �ð�ȭ)
        GameObject indicator = Instantiate(hackIndicatorPrefab, hackZoneCenters[bestZoneIndex], Quaternion.identity);
        indicator.transform.localScale = new Vector3(hackZoneSize.x, hackZoneSize.y, 0.5f); // ���� Z

        // 3�ʰ� ����� ȿ�� (PatternWarning �ð���ŭ ����� ����)
        float blinkDuration = 5f;
        float timer = 0f;
        float blinkInterval = 0.5f;
        bool visible = true;

        while (timer < blinkDuration)
        {
            indicator.SetActive(visible);
            visible = !visible;
            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        Destroy(indicator); // �ε������� ����

        // ���� ��ŷ ����
        Vector3 bestCenter = hackZoneCenters[bestZoneIndex];
        Vector3 bestHalfExtents = new Vector3(hackZoneSize.x / 2f, hackZoneSize.y / 2f, 1f);
        Collider[] targets = Physics.OverlapBox(bestCenter, bestHalfExtents);

        foreach (var col in targets)
        {
            if (col.TryGetComponent<Turret>(out var turret))
            {
                turret.SetHacked(true);
            }
        }

        yield break;
    }

    private IEnumerator EmpPattern()
    {
        // ��Ÿ�� Ȯ��
        if (Time.time - lastEmpTime < empCooldown)
        {
            Logger.Log("[EMP] ��Ÿ���� ���� ���� �־� ������ ����մϴ�.");
            yield break;
        }

        // �ݰ� �� �ͷ� Ž��
        Collider[] cols = Physics.OverlapSphere(transform.position, empRadius);
        var targets = cols
            .Where(c => c.TryGetComponent<Turret>(out _))
            .Select(c => c.GetComponent<Turret>())
            .ToList();

        if (targets.Count == 0)
        {
            Logger.Log("[EMP] �ݰ� ���� �ͷ��� ���� ������ ����մϴ�.");
            yield break;
        }

        // EMP ��� ǥ��
        yield return StartCoroutine(ShowEmpWarning());

        // ��ź(Mine) �ı� ó��
        Collider[] mines = Physics.OverlapSphere(transform.position, empRadius);
        foreach (var col in mines)
        {
            if (col.TryGetComponent<Mine>(out var mine))
            {
                Destroy(mine.gameObject);  // ���� �� ��� ���� �ı�
            }
        }

        // ���� ����
        Logger.Log($"[EMP] {targets.Count}���� �ͷ��� EMP ȿ���� �����մϴ�.");
        foreach (var turret in targets)
        {
            turret.ApplyEMPEffect(empEffectDuration);  // EMP ȿ�� ����
        }

        lastEmpTime = Time.time;
        yield return null;
    }

    private IEnumerator PushBackPattern()
    {
        // Ǫ�� ���� ���� �� ��� ǥ��
        yield return StartCoroutine(ShowPushWarning());

        Logger.Log("[PushBack] ���� �� �ͷ��� �÷��̾ �о���ϴ�.");

        int combinedMask = pushableLayerMask | (1 << LayerMask.NameToLayer("Player"));
        Collider[] cols = Physics.OverlapSphere(transform.position, empRadius, combinedMask);

        foreach (var col in cols)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null)
            {
                Vector3 origin = transform.position;
                Vector3 targetPos = col.transform.position;

                // XY ��� ���� ���� ���
                Vector3 direction = targetPos - origin;
                direction.z = 0f;
                float distance = direction.magnitude;

                if (distance < empRadius)
                {
                    direction.Normalize();

                    // ��ǥ ��ġ ��� (EMP ���� ��輱 ��ġ)
                    Vector3 destination = origin + direction * empRadius;

                    // �ڷ�ƾ���� �о�� ����
                    StartCoroutine(PushToPosition(rb, destination, 0.15f));
                }
            }
        }

        yield return null;
    }

    private IEnumerator ShowPushWarning()
    {
        // Ǫ�� �ĵ� �����տ��� LineRenderer ��������
        GameObject wave = Instantiate(pushWavePrefab, transform.position, Quaternion.Euler(90f, 0f, 0f));
        LineRenderer lineRenderer = wave.GetComponent<LineRenderer>();  // LineRenderer ������Ʈ
        if (lineRenderer == null)
        {
            Debug.LogWarning("pushWavePrefab�� LineRenderer�� �����ϴ�.");
            Destroy(wave);
            yield break;
        }

        // �ĵ� ���� ����
        int segments = 64;                                      // ���� ������ ���׸�Ʈ ��
        float elapsed = 0f;
        float startRadius = 0f;
        float endRadius = empRadius;                           
        float duration = pushWarningDuration;                   // ���� �ð�

        // LineRenderer �ʱ� ����
        lineRenderer.positionCount = segments + 1;                        // ������ ���� �������� ����
        lineRenderer.useWorldSpace = false;                               // ���� ��ǥ�� ���

        // ��� �ĵ� �ִϸ��̼�
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentRadius = Mathf.Lerp(startRadius, endRadius, t);

            // ���� ��ǥ ���
            for (int i = 0; i <= segments; i++)
            {
                float angle = 2 * Mathf.PI * i / segments;
                float x = Mathf.Cos(angle) * currentRadius;
                float z = Mathf.Sin(angle) * currentRadius;
                lineRenderer.SetPosition(i, new Vector3(x, 0f, z));      // y=0 ��鿡 �׸���
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // �Ϸ� �� ����
        Destroy(wave);
    }

    private IEnumerator PushToPosition(Rigidbody rb, Vector3 destination, float duration)
    {
        float timer = 0f;
        Vector3 start = rb.position;

        // ���� constraints ����
        RigidbodyConstraints originalConstraints = rb.constraints;

        // X, Y �ุ ���� (Z�� �״�� ����)
        rb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY);

        while (timer < duration)
        {
            // z�� ���� (XY ������θ� �̵�)
            Vector3 nextPos = Vector3.Lerp(start, destination, timer / duration);
            nextPos.z = rb.position.z;

            rb.MovePosition(nextPos);

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // ��ġ ����
        destination.z = rb.position.z;
        rb.MovePosition(destination);

        // �ӵ� ����
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // constraints ������� ����
        rb.constraints = originalConstraints;
    }

    private IEnumerator PullPattern()
    {
        Logger.Log("[Pull] ���� �� ������Ʈ�� ������ϴ�.");

        Collider[] bossCols = GetComponentsInChildren<Collider>();
        foreach (var bc in bossCols)
        {
            bc.isTrigger = true;
        }

        // ������ ����Ʈ ����
        pullEffectCoroutine = StartCoroutine(ShowPullEffect());

        //�ʱ� ��� ����
        List<Rigidbody> targets = new List<Rigidbody>();
        float elapsed = 0f;
        var originalConstraints = new Dictionary<Rigidbody, RigidbodyConstraints>();
        

        // ������� �ڷ�ƾ
        while (elapsed < pullDuration)
        {
            // �� ������ ���� ���� ��� �߰�
            Collider[] cols = Physics.OverlapSphere(transform.position, pullRadius, pushableLayerMask);
            foreach (var col in cols)
            {
                Rigidbody rb = col.attachedRigidbody;
                if (rb != null && !targets.Contains(rb))
                {
                    targets.Add(rb);                                        // targets ����Ʈ�� �߰��մϴ�.
                    originalConstraints[rb] = rb.constraints;               // ���� ���൵ �����մϴ�.
                }
            }

            for (int i = targets.Count - 1; i >= 0; i--)
            {
                var rb = targets[i];
                if (rb == null)
                {
                    targets.RemoveAt(i); 
                    continue;
                }

                // ������� ���� ��, X/Y Freeze ����
                rb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY);

                Vector3 dir = (transform.position - rb.position);
                dir.z = 0f;
                float dist = dir.magnitude;
                dir.Normalize();

                // ��ǥ ��ġ(���� �߽� ��輱) ���
                Vector3 destination = transform.position;

                // ��ź(Bomb) ����
                if (dist <= reachThreshold && rb.TryGetComponent<Mine>(out var mine))
                {
                    yield return StartCoroutine(mine.ExplodeAfterDelay());

                    // ��� ���� ���� �� �׷α� ���� ����
                    StartCoroutine(GroggyState());
                    // Pull ����Ʈ ����
                    if (pullEffectCoroutine != null)
                        StopCoroutine(pullEffectCoroutine);

                    // ���� �ݶ��̴� ���� ���� ���ο� ���� �÷��̾ �о���ϴ�.
                    Collider[] stuckPlayers = Physics.OverlapSphere(transform.position, reachThreshold, playerLayerMask);
                    foreach (var col in stuckPlayers)
                    {
                        Transform t = col.transform;
                        Vector3 dirt = (t.position - transform.position).normalized;    // ���� �߽ɿ��� �ٱ� ����
                        Vector3 safePos = transform.position + dirt * (reachThreshold + 0.1f); // ���� �Ÿ� + ����
                        t.position = safePos;  // �÷��̾� ��ġ ���� �̵�
                    }

                    // �����ִ� ��� Ǯ ����Ʈ �ν��Ͻ� �ı�
                    foreach (var effect in pullEffectInstances)
                        Destroy(effect);
                    pullEffectInstances.Clear();

                    foreach (var bc in bossCols)
                        bc.isTrigger = false;


                    elapsed = pullDuration;    // ��� ����

                    // ��� constraints ����
                    foreach (var kv in originalConstraints)
                        if (kv.Key != null)
                            kv.Key.constraints = kv.Value;

                    yield break;
                }

                // �߽� ���� �� �ı�
                if (dist <= reachThreshold)
                {
                    rb.GetComponent<IDamageable>()?.TakeDamage(pullDamage);
                    targets.RemoveAt(i);
                    continue;
                }

                // �̵�
                Vector3 next = rb.position + dir * pullSpeed * Time.deltaTime;
                next.z = rb.position.z;
                rb.MovePosition(next);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }
        // Pull ����Ʈ ����
        if (pullEffectCoroutine != null)
            StopCoroutine(pullEffectCoroutine);

        foreach (var bc in bossCols)
        {
            bc.isTrigger = false; 
        }

        // ���� Ÿ�ٵ� ���� constraints ����
        foreach (var kv in originalConstraints)
            if (kv.Key != null)
                kv.Key.constraints = kv.Value;
    }

    private IEnumerator ShowPullEffect()
    {
        float totalTime = 0f;
        while (totalTime < pullDuration)
        {
            // �� ���ݸ��� ���ο� �� �ڷ�ƾ�� ����
            StartCoroutine(AnimatePullRing());

            yield return new WaitForSeconds(pullEffectInterval);
            totalTime += pullEffectInterval;
        }
    }

    private IEnumerator AnimatePullRing()
    {
        // �� ������Ʈ ����
        GameObject instantiate = Instantiate(pushWavePrefab, transform.position, Quaternion.identity);
        pullEffectInstances.Add(instantiate);   
        LineRenderer lineRenderer = instantiate.GetComponent<LineRenderer>();

        if (lineRenderer == null) yield break;

        lineRenderer.positionCount = pullEffectSegments + 1;
        lineRenderer.useWorldSpace = true;

        float elapsed = 0f;
        while (elapsed < pullEffectDuration)
        {
            if (lineRenderer == null) yield break;

            float effectDuration = elapsed / pullEffectDuration;
            float currentRadius = Mathf.Lerp(pullRadius, 0f, effectDuration);

            // ���� ��ǥ ���
            for (int i = 0; i <= pullEffectSegments; i++)
            {
                float effectSegments = 2f * Mathf.PI * i / pullEffectSegments;
                Vector3 pos = transform.position + new Vector3(Mathf.Cos(effectSegments), Mathf.Sin(effectSegments), 0f) * currentRadius;
                lineRenderer.SetPosition(i, pos);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ������ �����ӿ��� ���� ���
        if (lineRenderer != null)
        {
            for (int i = 0; i <= pullEffectSegments; i++)
                lineRenderer.SetPosition(i, transform.position);
        }

        pullEffectInstances.Remove(instantiate);
        Destroy(instantiate);
    }

    private IEnumerator GroggyState()
    {
        isGroggy = true;
        // ���Ѵٸ� �ִϸ��̼�/����Ʈ �߰�
        yield return new WaitForSeconds(groggyDuration);
        isGroggy = false;
    }

    public void TakeDamage(float amount)  // IDamageable ����
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }

        // 15% ���Ҹ��� Ǫ�� �ߵ�
        if (currentHealth <= nextPushThreshold)
        {
            pushPending = true;
            nextPushThreshold -= maxHealth * 0.15f;
        }

        // Pull ���� (30% ����)
        if (currentHealth <= nextPullThreshold)
        {
            pullPending = true;
            nextPullThreshold -= maxHealth * 0.3f;
        }
    }

    private IEnumerator ShowEmpWarning()
    {
        GameObject warning = Instantiate(empWarningPrefab, transform.position, Quaternion.Euler(90f, 0f, 0f));
        warning.transform.localScale = new Vector3(empRadius * 2, 0.05f, empRadius * 2); // �߽ɿ��� ������ * 2�� �ǵ���

        Renderer renderer = warning.GetComponent<Renderer>();
        if (renderer == null)
        {
            Logger.LogWarning("��� ������Ʈ�� Renderer�� �����ϴ�.");
            yield break;
        }

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < empWarningDuration)
        {
            renderer.enabled = visible;
            visible = !visible;

            yield return new WaitForSeconds(empBlinkInterval);
            elapsed += empBlinkInterval;
        }

        Destroy(warning);
    }

    private void Die()
    {
        BossUIManager.Instance.Hide();

        AudioManager.Instance.StartRepair();
        //
        if (pullEffectCoroutine != null)
            StopCoroutine(pullEffectCoroutine);

        foreach (var effect in pullEffectInstances)
            Destroy(effect);
        pullEffectInstances.Clear();

        // ���� ��� ó�� (���� ����Ʈ, ���� ��� ��)
        CameraController.Instance.ExitBossCameraMode(); // ���� ī�޶� ��� ����
        BossArenaManager.Instance.DisableArena(); // ������ �Ʒ��� ��Ȱ��ȭ
        GameOverUIManager.Instance.ShowGameClearUI();
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected()
    {
        // ��ȯ ������ ��ŷ ������ Gizmos�� �ð�ȭ
        Gizmos.color = Color.green;
        Vector3 spawnSize = new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0.5f); // ���� Z
        Gizmos.DrawWireCube(transform.position, spawnSize);

        // ��ŷ ������ �ð�ȭ
        Gizmos.color = Color.red;
        foreach (Vector3 center in hackZoneCenters)
        {
            Vector3 hackSize = new Vector3(hackZoneSize.x, hackZoneSize.y, 0.5f);
            Gizmos.DrawWireCube(center, hackSize);
        }

        // EMP �ݰ� �ð�ȭ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, empRadius);  // EMP �ݰ� �ð�ȭ

        //Ǯ ���� �ݰ� (pullRadius)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pullRadius);

        // ���� �߽� ���� ���� �Ÿ� (reachThreshold)
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, reachThreshold);
    }
}
