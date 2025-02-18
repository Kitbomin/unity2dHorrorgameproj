using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;


public class TrackingMechanism : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    public LayerMask layerMask;

    
    public Transform player;
    
    public int walkCount;
    private int currentWalkCount;

    public float speed;

    public float chaseRange;

    public float stopRange;

    private Vector3 vector;

    private bool isSlowed = false;
    public float slowDuration; // 속도 감소 지속 시간
    public float slowedSpeed; // 느려진 속도



    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        StartCoroutine(MoveCoroutine());

    }

    IEnumerator MoveCoroutine()
    {
            while (true) 
            {
                // 플레이어와 적 사이의 거리 계산
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                // 1. 플레이어가 추적 거리 안에 있는지 확인
                if (distanceToPlayer <= chaseRange)
                {
                    // 2. 플레이어가 공격 거리 바깥에 있는지 확인
                    if (distanceToPlayer > stopRange)
                    {

                        // 3. 플레이어 방향으로 적을 이동
                        Vector3 direction = (player.position - transform.position).normalized; // 방향 계산

                        // 방향이 X축 또는 Y축으로만 움직이게 제한
                        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                        {
                            // X축 방향으로만 이동
                            transform.position += new Vector3(direction.x * speed * Time.deltaTime, 0, 0);
                        }
                        else
                        {
                            // Y축 방향으로만 이동
                            transform.position += new Vector3(0, direction.y * speed * Time.deltaTime, 0);
                        }
                    }
                }
              
                yield return null;

            }
    }

    public void SlowDown()
    {
        if (!isSlowed)
        {
            isSlowed = true;
            speed = 0;  // 속도 감소
            StartCoroutine(ResetSpeedAfterDelay());
        }
    }

    // 일정 시간 후 속도를 원래대로 되돌림
    IEnumerator ResetSpeedAfterDelay()
    {
        yield return new WaitForSeconds(slowDuration); // slowDuration 시간 대기
        speed = 50.0f; // 원래 속도로 복구
        isSlowed = false; // 다시 속도 감소 상태 아니게 설정
    }

    IEnumerator WalkCoroutine()
    {
            while (currentWalkCount < walkCount)
            {
                if (vector.x != 0)
                {
                    transform.Translate(vector.x * speed, 0, 0);
                }
                else if (vector.y != 0)
                {
                    transform.Translate(0, vector.y * speed, 0);
                }

                yield return new WaitForSeconds(0.005f);
            }
            currentWalkCount = 0;
       
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= stopRange)
        {
            UnityEngine.Debug.Log("플레이어가 공격 범위에 도달했습니다!");
            Time.timeScale = 0;
            speed = 0;
            stopRange = 0.0f;

        }

        RaycastHit2D hit;
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(vector.x * speed * walkCount, vector.y * speed * walkCount);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, layerMask);
        boxCollider.enabled = true;

        if (hit.transform != null)
            return;

    }

    // 디버깅용: 추적 거리와 공격 거리의 범위를 Scene 뷰에서 시각화
    void OnDrawGizmosSelected()
    {
        // 추적 거리(Chase Range)를 표시하는 파란색 원
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // 공격 거리(Stop Range)를 표시하는 빨간색 원
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);
    }

}
