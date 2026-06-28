using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("1ターンの制限時間")]
    public float turnTime = 10f; // テスト用に短めの10秒に設定しています

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
            // 残り時間を減らしていく
            currentTime -= Time.deltaTime;

            // 画面の下（Console）に残り時間を表示（カウントダウン）
            if (currentTime > 0)
            {
                Debug.Log($"残り時間: {Mathf.CeilToInt(currentTime)}秒");
            }
            else
            {
                // 0秒になったらタイムアップ
                TimeUp();
            }
        }
    }

    // ターン開始の合図
    void StartTurn()
    {
        currentTime = turnTime;
        isTimerRunning = true;
        Debug.Log("【強欲タイム開始！】お宝を部屋に運べ！");
    }

    // タイムアップ時の処理
    void TimeUp()
    {
        isTimerRunning = false;
        Debug.Log("【タイムアップ！】扉が閉まります...");

        // プロトタイプ用に、1〜5番の部屋からランダムに1つ爆発させる
        int explodeRoom = Random.Range(1, 6);
        Debug.Log($"【審判の瞬間】 部屋 {explodeRoom} が大爆発しました！！！");

        // ※ここにあとで「プレイヤーが生き残ったか判定」「吹っ飛ばす処理」を追加します
    }
}