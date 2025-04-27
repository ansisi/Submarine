using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform spaceship;      // 중심이 될 우주선
    public float spawnRadius = 50f;  // 스폰 반경
    public Transform enemyParent;    // 스폰된 적들을 그룹화할 부모 객체

    public void SpawnSubWave(SubWaveData subWave)
    {
        if (spaceship == null)
        {
            Logger.LogError("Spaceship transform is not assigned!");
            return;
        }

        foreach (var spawnData in subWave.enemySpawnDatas)
        {
            for (int i = 0; i < spawnData.spawnCount; i++)
            {
                // 랜덤 위치 계산
                Vector2 randomPos = Random.insideUnitCircle.normalized * spawnRadius;
                Vector3 spawnPosition = new Vector3(
                    spaceship.position.x + randomPos.x,
                    spaceship.position.y + randomPos.y,
                    spaceship.position.z // Z축 유지
                );

                // 적 생성
                GameObject enemy = Instantiate(spawnData.enemyPrefab, spawnPosition, Quaternion.identity);

                // 적이 있을 경우, 부모 객체로 설정
                if (enemyParent != null)
                {
                    enemy.transform.SetParent(enemyParent);
                }
            }
        }
    }

    // OnDrawGizmosSelected: 씬 뷰에서 해당 객체가 선택되었을 때만 반경을 시각적으로 표시
    private void OnDrawGizmosSelected()
    {
        // Gizmo의 색상 설정
        Gizmos.color = Color.red;

        // 스폰 반경을 원으로 그리기 (2D 평면에서)
        Gizmos.DrawWireSphere(spaceship.position, spawnRadius);
    }
}
