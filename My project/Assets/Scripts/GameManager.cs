using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("1ターンの制限時間")]
    public float turnTime = 10f;

    [Header("プレイヤーの情報")]
    public PlayerController player;

    [Header("仕掛け人の総獲得金額")]
    public int hostMoney = 0; // 追加：仕掛け人（ホスト）の財布

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

        // 1. もしプレイヤーがお宝を持ったままなら強制ドロップさせる
        TreasureBox[] allTreasures = FindObjectsOfType<TreasureBox>();
        foreach (TreasureBox treasure in allTreasures)
        {
            if (treasure.IsCarried())
            {
                treasure.Drop(player.currentRoom);
            }
        }

        // 2. 仕掛け人が爆発させる部屋をランダム決定
        int explodeRoom = Random.Range(1, 6);
        Debug.Log($"【審判の瞬間】 💥部屋 {explodeRoom} 💥 が大爆発！！！");

        // 3. お宝の清算処理（山分け ＆ 総取り）
        foreach (TreasureBox treasure in allTreasures)
        {
            // --- パターンA：仕掛け人が部屋を爆破した場合 ---
            if (treasure.currentRoom == explodeRoom)
            {
                hostMoney += treasure.moneyAmount; // 仕掛け人が全額獲得！
                Debug.Log($"💀 仕掛け人が【部屋 {explodeRoom}】のお宝を総取り！ ＋{treasure.moneyAmount}万円（仕掛け人合計: {hostMoney}万円）");
                Destroy(treasure.gameObject); // お宝は消滅
            }
            // --- パターンB：爆発を免れた安全な部屋の場合 ---
            else if (treasure.currentRoom != 0) // ロビー以外
            {
                // その部屋にいるプレイヤーの人数を数える
                // (※現在はプロトタイプで1人しかいないため、自分がその部屋にいたら1人、いなければ0人として計算します)
                int playerCountInRoom = 0;
                if (player.currentRoom == treasure.currentRoom)
                {
                    playerCountInRoom = 1; // 将来マルチプレイ化するときは、ここに部屋内の全員を数える処理が入ります
                }

                // もし部屋にプレイヤーがいたら山分けして回収
                if (playerCountInRoom > 0)
                {
                    // ★C#の性質上、整数(int)同士の割り算は自動的に小数点以下が「切り捨て」になります！
                    int sharedMoney = treasure.moneyAmount / playerCountInRoom;

                    if (player.currentRoom == treasure.currentRoom)
                    {
                        player.totalMoney += sharedMoney;
                        Debug.Log($"🎉 【山分け】部屋 {treasure.currentRoom} のお宝（{treasure.moneyAmount}万円）を {playerCountInRoom} 人で山分け！ 1人あたり ＋{sharedMoney}万円獲得！");
                    }
                    Destroy(treasure.gameObject); // 獲得されたので画面から消す
                }
                else
                {
                    // 誰もいない安全な部屋のお宝は【そのまま床に残る】
                    Debug.Log($"💰 部屋 {treasure.currentRoom} にはお宝だけがあり誰もいないため、次ターンの遺産になりました。");
                }
            }
        }

        // 4. プレイヤーの生死・所持金ペナルティ判定
        if (player.currentRoom == explodeRoom || player.currentRoom == 0)
        {
            player.totalMoney = 0; // 死亡ペナルティ：所持金0円リセット
            Debug.Log($"💀 【死亡】 プレイヤーの総所持金は 0 円になりました...");
        }
        else
        {
            Debug.Log($"🎉 【生存】 プレイヤー現在の総所持金: {player.totalMoney}万円！");
        }
    }
}