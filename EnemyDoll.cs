using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class EnemyDoll : MonoBehaviour
{
    private EnemyManager enemyManager;
    public int Health;

    public float speed; 
    public float range; 
    public float detectionRange; // 플레이어 감지 범위
    public float returnDelay; // 대기 시간

    private Vector3 startPosition;
    private GameObject player;
    private bool isChasing = false; // 추적 중인지 여부
    private float stopChasingTime; // 추적 종료 시간

    private Coroutine moveCoroutine;

  
    void Start()
    {
        enemyManager = FindFirstObjectByType<EnemyManager>();
        startPosition = transform.position; // 시작 위치 저장
        player = GameObject.FindGameObjectWithTag("Player"); // 플레이어 찾기
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < detectionRange)
        {
            

            if (enemyManager != null)
            {
                if (moveCoroutine == null) // 중복 실행 방지
                {
                    moveCoroutine = StartCoroutine(enemyManager.MoveCoroutine());
                    moveCoroutine = StartCoroutine(enemyManager.WalkCoroutine());
                }

                stopChasingTime = Time.time + returnDelay;
                isChasing = true;
            }
            else
            {
                Debug.LogError("EnemyManager를 찾을 수 없습니다!");
            }
            
        }

        if (isChasing)
        {
            if (Time.time > stopChasingTime)
            {
                isChasing = false;
                startPosition = transform.position; // 현재 위치를 새로운 시작 위치로 저장
            }
        }
        else
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }

            Wander();
        }
      
    }


    void Wander()
    {
        float offsetX = Mathf.Sin(Time.time * speed) * range; // X축으로 왕복 이동
        transform.position = new Vector3(startPosition.x + offsetX, startPosition.y, startPosition.z);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (enemyManager != null)
        {
            enemyManager.SlowDown(); // 적의 속도 감소
        }
        else
        {
            Debug.LogError("EnemyManager를 찾을 수 없습니다!");
        }

        if (Health <= 0)
        {
            Debug.Log($" {gameObject.name} 처치됨!");
            Destroy(gameObject); // 적이 죽으면 삭제
        }
    }

    // 디버깅용: 추적 거리와 공격 거리의 범위를 Scene 뷰에서 시각화
    void OnDrawGizmosSelected()
    {
        // 기본 이동 범위를 표시하는 주황색 원
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,range);

        
    }

}
