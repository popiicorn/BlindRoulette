using UnityEngine;
using TMPro; // ★追加：TextMeshPro（UI）をプログラムからいじるための準備

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
    public float spawnRadiusX = 10f;
    public float spawnRadiusZ = 2f;

    // ★追加：UIパーツを入れる箱
    [Header("UI表示")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI playerMoneyText;
    public TextMeshProUGUI hostMoneyText;

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
        // ★追加：常にお金のUIを最新の数字に書き換える
        playerMoneyText.text = $"プレイヤー: {player.totalMoney}万円";
        hostMoneyText.text = $"仕掛け人: {hostMoney}万円";

        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;

            // ★追加：タイマーのUIを更新する
            timerText.text = $"残り時間: {Mathf.CeilToInt(currentTime)}秒";

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
            timerText.text = "ゲーム終了！";
            return;
        }

        doubleRoom = Random.Range(1, 6);

        for (int i = 0; i < treasuresToSpawnNextTurn; i++)
        {
            float randomX = Random.Range(-spawnRadiusX, spawnRadiusX);
            float randomZ = Random.Range(-spawnRadiusZ, spawnRadiusZ);
            float randomY = Random.Range(0f, 2f);

            Vector3 randomOffset = new Vector3(randomX, randomY, randomZ);
            Vector3 spawnPos = spawnPoint.position + randomOffset;

            Instantiate(treasurePrefab, spawnPos, Quaternion.identity);
        }

        treasuresToSpawnNextTurn = 1;

        currentTime = turnTime;
        isTimerRunning = true;
    }

    void TimeUp()
    {
        isTimerRunning = false;
        timerText.text = "審判の刻...！"; // ★追加：0秒になった時の演出テキスト

        TreasureBox[] allTreasures = FindObjectsOfType<TreasureBox>();
        foreach (TreasureBox treasure in allTreasures)
        {
            if (treasure.IsCarried()) treasure.Drop(player.currentRoom);
        }

        int explodeRoom = Random.Range(1, 6);

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
                Destroy(treasure.gameObject);
            }
            else if (treasure.currentRoom != 0)
            {
                int playerCountInRoom = (player.currentRoom == treasure.currentRoom) ? 1 : 0;
                if (playerCountInRoom > 0)
                {
                    int sharedMoney = currentTreasureMoney / playerCountInRoom;
                    player.totalMoney += sharedMoney;
                    treasuresToSpawnNextTurn++;
                    Destroy(treasure.gameObject);
                }
            }
        }

        if (player.currentRoom == explodeRoom || player.currentRoom == 0)
        {
            player.totalMoney = 0;
        }

        Invoke("StartNextTurn", 3f);
    }
}