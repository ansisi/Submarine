using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureGimmick : MonoBehaviour
{
    public GameObject temperatureUIPrefab; // 프리팹 연결 (Inspector에서)
    private RectTransform temperatureNeedle;
    private GameObject instantiatedUI;

    public float maxColdTime = 300f;
    private float elapsedColdTime = 0f;

    private float warmAngle = -195f;
    private float coldAngle = 90f;




    private void OnEnable()
    {
        // Gauge라는 이름의 부모 UI 찾기
        GameObject gauge = GameObject.Find("Gauge");
        if (gauge == null)
        {
            Logger.LogWarning("Gauge 오브젝트를 찾을 수 없습니다!");
            return;
        }

        // 체온 UI 프리팹 생성 + Gauge 하위로 설정
        instantiatedUI = Instantiate(temperatureUIPrefab, gauge.transform);

        // Needle 참조 설정
        Transform needle = instantiatedUI.transform.Find("TemperatureNeedle");
        if (needle != null)
        {
            temperatureNeedle = needle.GetComponent<RectTransform>();
        }
        else
        {
            Logger.LogWarning("Needle 오브젝트를 찾을 수 없습니다!");
        }

        instantiatedUI.SetActive(true);
    }

    private void OnDisable()
    {
        if (instantiatedUI != null)
            Destroy(instantiatedUI); // 비활성화 시 UI 제거
    }

    void Update()
    {
        elapsedColdTime += Time.deltaTime;
        float coldRatio = Mathf.Clamp01(elapsedColdTime / maxColdTime);
        float temperatureAngle = Mathf.Lerp(warmAngle, coldAngle, coldRatio);

        if (temperatureNeedle != null)
        {
            temperatureNeedle.localEulerAngles = new Vector3(0, 0, temperatureAngle);
        }

        if (coldRatio >= 1.0f)
        {
            GameManager.Instance.GameOver();
            Logger.Log("체온 저하! 게임 오버");
        }
    }

    public void WarmUp(float amount)
    {
        StopCoroutine("RefillHeat");
        StartCoroutine(RefillHeat(amount));
    }

    private IEnumerator RefillHeat(float amount)
    {
        float startTime = elapsedColdTime;
        float targetTime = Mathf.Max(elapsedColdTime - amount, 0f);
        float duration = 1.0f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            elapsedColdTime = Mathf.Lerp(startTime, targetTime, t / duration);
            yield return null;
        }
        elapsedColdTime = targetTime;
    }

    public bool IsTooCold()
    {
        float coldRatio = Mathf.Clamp01(elapsedColdTime / maxColdTime);
        float temperatureAngle = Mathf.Lerp(warmAngle, coldAngle, coldRatio);
        return temperatureAngle >= -50f;
    }

    public float GetColdRatio()
    {
        return Mathf.Clamp01(elapsedColdTime / maxColdTime);
    }
}
