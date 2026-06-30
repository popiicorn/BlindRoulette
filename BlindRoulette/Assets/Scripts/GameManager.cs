using UnityEngine;
using TMPro; // ★追加：TextMeshPro（UI）をプログラムからいじるための準備

public class GameManager : MonoBehaviour
{
    public CameraShake cameraShake; // カメラシェイクを登録する枠

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

    [Header("爆発エフェクト")]
    public GameObject explosionPrefab; // プレハブを登録する枠

    // 爆発させる部屋のTransform（位置情報）を準備
    public Transform[] roomPositions; // 部屋1～5の位置をインスペクターで指定できるようにする

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
        timerText.text = "審判の刻...！";
        // RoomIDの代わりにRoomDetectorを探す！
        RoomDetector[] allRooms = FindObjectsOfType<RoomDetector>();

        int randomIndex = Random.Range(0, allRooms.Length);
        RoomDetector explodeRoomObj = allRooms[randomIndex];
        int explodeRoomId = explodeRoomObj.roomNumber; // roomNumber を取得！


        Debug.Log($"💥部屋 {explodeRoomId} 💥 が大爆発！！！");

        // 爆発エフェクトをその部屋の位置に生成
        Instantiate(explosionPrefab, explodeRoomObj.transform.position, Quaternion.identity);

        // ★カメラを揺らす
        cameraShake.PlayShake(0.5f, 0.3f);

        TreasureBox[] allTreasures = FindObjectsOfType<TreasureBox>();
        foreach (TreasureBox treasure in allTreasures)
        {
            // ★ここに追加：宝物がどこの部屋にいるかログを出してみる
            Debug.Log($"チェック中の宝物: {treasure.name}, 宝物のいる部屋: {treasure.currentRoom}, 爆発部屋: {explodeRoomId}");

            if (treasure.IsCarried()) treasure.Drop(player.currentRoom);
        }

        // お金の計算処理
        foreach (TreasureBox treasure in allTreasures)
        {
            int currentTreasureMoney = treasure.moneyAmount;
            if (treasure.currentRoom == doubleRoom)
            {
                currentTreasureMoney *= 2;
            }

            // ここで最初に決めた explodeRoomId を使う
            if (treasure.currentRoom == explodeRoomId)
            {
                hostMoney += currentTreasureMoney;
                treasuresToSpawnNextTurn++;
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

        // プレイヤーが爆発した部屋にいたら没収
        if (player.currentRoom == explodeRoomId || player.currentRoom == 0)
        {
            player.totalMoney = 0;
        }

        Invoke("StartNextTurn", 3f);
    }
}