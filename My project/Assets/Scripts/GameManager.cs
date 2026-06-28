using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("ゲーム設定")]
    public int maxTurns = 5;
    public float turnTime = 10f;

    [Header("プレイヤーの情報")]
    public PlayerController player;
    public int hostMoney = 0;

    [Header("お宝の生成")]
    public GameObject treasurePrefab;
    public Transform spawnPoint;

    // ★追加：落下させる範囲の広さをInspectorで調整できるようにする
    [Header("お宝の落下範囲")]
    public float spawnRadiusX = 10f; // 横幅のバラつき（ロビーの幅に合わせる）
    public float spawnRadiusZ = 2f;  // 奥行きのバラつき

    private int currentTurn = 0;
    private float currentTime;
    private bool isTimerRunning = false;
    private int doubleRoom = 0;
    private int treasuresToSpawnNextTurn = 1;

    void Start()
    {
        StartNextTurn();
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

    void StartNextTurn()
    {
        currentTurn++;
        if (currentTurn > maxTurns)
        {
            Debug.Log($"🏁 【ゲーム終了】 最終結果発表！ プレイヤー: {player.totalMoney}万円 / 仕掛け人: {hostMoney}万円");
            return;
        }

        doubleRoom = Random.Range(1, 6);

        for (int i = 0; i < treasuresToSpawnNextTurn; i++)
        {
            // ★修正：さっき追加した範囲（spawnRadius）を使って、ランダムな座標を広く計算する
            float randomX = Random.Range(-spawnRadiusX, spawnRadiusX);
            float randomZ = Random.Range(-spawnRadiusZ, spawnRadiusZ);
            float randomY = Random.Range(0f, 2f); // 降ってくるタイミングを少しズラすための高さのバラつき

            Vector3 randomOffset = new Vector3(randomX, randomY, randomZ);
            Vector3 spawnPos = spawnPoint.position + randomOffset;

            Instantiate(treasurePrefab, spawnPos, Quaternion.identity);
        }

        treasuresToSpawnNextTurn = 1;

        currentTime = turnTime;
        isTimerRunning = true;
        Debug.Log($"\n=== 第 {currentTurn} ターン開始！ === (✨2倍部屋は【部屋 {doubleRoom}】だ！)");
    }

    void TimeUp()
    {
        isTimerRunning = false;
        Debug.Log("【タイムアップ！】扉が閉まった！");

        TreasureBox[] allTreasures = FindObjectsOfType<TreasureBox>();
        foreach (TreasureBox treasure in allTreasures)
        {
            if (treasure.IsCarried()) treasure.Drop(player.currentRoom);
        }

        int explodeRoom = Random.Range(1, 6);
        Debug.Log($"【審判の瞬間】 💥部屋 {explodeRoom} 💥 が大爆発！！！");

        foreach (TreasureBox treasure in allTreasures)
        {
            int currentTreasureMoney = treasure.moneyAmount;
            if (treasure.currentRoom == doubleRoom)
            {
                currentTreasureMoney *= 2;
            }

            if (treasure.currentRoom == explodeRoom)
            {
                hostMoney += currentTreasureMoney;
                Debug.Log($"💀 仕掛け人が【部屋 {explodeRoom}】のお宝を総取り！ ＋{currentTreasureMoney}万円");
                Destroy(treasure.gameObject);
            }
            else if (treasure.currentRoom != 0)
            {
                int playerCountInRoom = (player.currentRoom == treasure.currentRoom) ? 1 : 0;
                if (playerCountInRoom > 0)
                {
                    int sharedMoney = currentTreasureMoney / playerCountInRoom;
                    player.totalMoney += sharedMoney;
                    Debug.Log($"🎉 【山分け】 ＋{sharedMoney}万円獲得！");
                    treasuresToSpawnNextTurn++;
                    Destroy(treasure.gameObject);
                }
            }
        }

        if (player.currentRoom == explodeRoom || player.currentRoom == 0)
        {
            player.totalMoney = 0;
            Debug.Log($"💀 【死亡】 所持金が 0 円になりました...");
        }
        else
        {
            Debug.Log($"🎉 【生存】 プレイヤー現在の総所持金: {player.totalMoney}万円！");
        }

        Invoke("StartNextTurn", 3f);
    }
}