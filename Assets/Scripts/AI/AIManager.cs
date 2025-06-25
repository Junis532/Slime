//using UnityEngine;

//public class AIManager : MonoBehaviour
//{
//    public DefaultDatable data;                  // AI 설정 데이터 (속도, ID 등)
//    private int currentAIId;                     // 현재 AI 유형 ID
//    private Transform player;                    // 플레이어의 Transform
//    private EnemyAnimation enemyAnimation;       // 적 애니메이션을 제어하는 컴포넌트

//    void Start()
//    {
//        // 데이터가 할당되어 있으면 AI ID 설정
//        if (data != null)
//        {
//            currentAIId = data.id;
//        }
//        else
//        {
//            Debug.LogError("No DefaultDatable assigned to AIManager!");
//        }

//        // 태그가 "Player"인 오브젝트 찾기
//        player = GameObject.FindWithTag("Player")?.transform;

//        // EnemyAnimation 컴포넌트 가져오기
//        enemyAnimation = GetComponent<EnemyAnimation>();
//    }

//    void Update()
//    {
//        // 현재 AI ID에 따른 행동 실행
//        ExecuteAIBehavior(currentAIId);
//    }

//    // AI ID에 따라 서로 다른 행동 수행
//    private void ExecuteAIBehavior(int aiId)
//    {
//        switch (aiId)
//        {
//            case 1:
//                FollowPlayerAI();    // AI 유형 1: 플레이어 추적
//                break;

//            case 2:
//                AggressiveAI();      // AI 유형 2: 공격적인 행동
//                break;

//            case 3:
//                DefensiveAI();       // AI 유형 3: 방어적인 행동
//                break;

//            default:
//                Debug.LogWarning("Unknown AI id");  // 정의되지 않은 ID 경고
//                break;
//        }
//    }

//    // 플레이어를 따라가는 AI 동작
//    private void FollowPlayerAI()
//    {
//        if (player == null || data == null) return;

//        Vector2 moveDir = Vector2.zero;
//        Vector2 toPlayer = player.position - transform.position;
//        float distanceToPlayer = toPlayer.magnitude;

//        // 플레이어가 일정 거리 이상일 때만 이동
//        if (distanceToPlayer > 0.8f)
//        {
//            moveDir += toPlayer.normalized;
//            enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);  // 이동 애니메이션
//        }
//        else
//        {
//            enemyAnimation.PlayAnimation(EnemyAnimation.State.Idle); // 정지 애니메이션
//        }

//        // 방향에 따라 스프라이트 반전
//        if (toPlayer.x < 0)
//        {
//            enemyAnimation.FlipSprite(false); // 왼쪽 보기
//        }
//        else
//        {
//            enemyAnimation.FlipSprite(true);  // 오른쪽 보기
//        }

//        // 실제 이동 처리
//        transform.Translate(moveDir.normalized * data.spd * Time.deltaTime);
//    }

//    // 플레이어를 빠르게 추격하는 AI
//    private void AggressiveAI()
//    {
//        if (player == null || data == null) return;

//        Vector2 direction = (player.position - transform.position).normalized;
//        transform.Translate(direction * Time.deltaTime * data.spd * 1.2f); // 속도 1.2배
//        enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
//    }

//    // 플레이어로부터 도망치는 AI
//    private void DefensiveAI()
//    {
//        if (player == null) return;

//        Vector2 direction = (transform.position - player.position).normalized;
//        transform.Translate(direction * Time.deltaTime * 1.5f); // 고정 속도 1.5
//        enemyAnimation.PlayAnimation(EnemyAnimation.State.Move);
//    }
//}
