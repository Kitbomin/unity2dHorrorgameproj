using System.Collections;
using UnityEngine;

public class EyeMonster : MonoBehaviour
{
    public enum EyeState {Open, Closed}
    public EyeState currentState = EyeState.Closed;


    public Transform scanCenter; // 스캔 중심 위치 (몬스터 기준)
    public float scanRadius = 5f; // 스캔 범위 (반지름)
    public LayerMask playerLayer; // 플레이어 감지용 레이어
    public float stillTimeThreshold = 3f; // 가만히 있는 시간 체크
    
    private bool isScanning = false;
    private float stillTimer = 0f;
    private PlayerController player;

    public float openDuration = 2f;
    public float closedDuration = 3f;

    private SpriteRenderer spriteRenderer;
    public Sprite openEyeSprite;
    public Sprite colosedEyeSprite;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(ChangeEyeState());
    }

    void Update()
    {
        ScanForPlayer();
    }

    void ScanForPlayer()
    {
        if (currentState == EyeState.Closed) return; // 눈을 감고 있으면 감지 X

        Collider2D detectedPlayer = Physics2D.OverlapCircle(scanCenter.position, scanRadius, playerLayer);

        if (detectedPlayer)
        {
            player = detectedPlayer.GetComponent<PlayerController>();

            if (player != null)
            {
                if (player.IsMoving)
                {
                    KillPlayer(); // 즉사 처리
                }
                else
                {
                    stillTimer += Time.deltaTime;
                    if (stillTimer >= stillTimeThreshold)
                    {
                        Debug.Log("통과");
                        stillTimer = 0; // 타이머 초기화
                    }
                }
            }
        }
        else
        {
            stillTimer = 0; // 감지 안되면 초기화
        }
    }


    void KillPlayer()
    {
        Debug.Log("으앙 쥬금");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(scanCenter.position, scanRadius);
    }

    IEnumerator ChangeEyeState()
    {
        while (true)
        {
            // 눈 뜨기
            currentState = EyeState.Open;
            spriteRenderer.sprite = openEyeSprite;
            Debug.Log("눈깔 괴물이 눈을 떴다!");
            yield return new WaitForSeconds(Random.Range(2f, 5f));

            // 눈 감기 + 이동
            currentState = EyeState.Closed;
            spriteRenderer.sprite = closedEyeSprite;
            Debug.Log("눈깔 괴물이 눈을 감았다... 이동 중...");
        
            // 이동 로직 (랜덤 위치 이동)
            transform.position = GetRandomPosition();
        
            yield return new WaitForSeconds(Random.Range(3f, 6f));
        }
    }

    Vector2 GetRandomPosition()
    {
        return new Vector2(Random.Range(-5f, 5f), Random.Range(-3f, 3f)); // 예제 이동 범위
    }



    void TryKillMonster()
    {
        if (currentState == EyeState.Closed && canBeKilled)
        {
            Debug.Log("눈깔 괴물 처치 성공!");
            Destroy(gameObject); // 괴물 삭제
        }
        else
        {
            Debug.Log("눈깔 괴물: 눈을 뜨고 있어서 처치 불가능!");
        }
    }

}
