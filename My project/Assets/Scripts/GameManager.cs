using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("1ターンの制限時間")]
    public float turnTime = 10f;

    [Header("プレイヤーの情報")]
    public PlayerController player; // 追加：プレイヤーを監視するための変数

    private float currentTime;
    private bool isTimerRunning = false;

    void Start()
    {
        StartTurn();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                TimeUp();
            }
        }
    }

    void StartTurn()
    {
        currentTime = turnTime;
        isTimerRunning = true;
        Debug.Log("【強欲タイム開始！】お宝を部屋に運べ！");
    }

    void TimeUp()
    {
        isTimerRunning = false;
        Debug.Log("【タイムアップ！】扉がガシャーンと閉まった！");

        int explodeRoom = Random.Range(1, 6);
        Debug.Log($"【審判の瞬間】 💥部屋 {explodeRoom} 💥 が大爆発！！！");

        // 生死の判定
        if (player.currentRoom == explodeRoom)
        {
            Debug.Log("💀 【死亡】 爆破された部屋にいたため、木っ端微塵になりました...");
        }
        else if (player.currentRoom == 0)
        {
            Debug.Log("💀 【死亡】 タイムアップ時にロビーにいたため、爆風の巻き添えで吹き飛びました...");
        }
        else
        {
            Debug.Log($"🎉 【生存】 プレイヤーは部屋 {player.currentRoom} で無事に生き残りました！お宝ゲット！");
        }
    }
}