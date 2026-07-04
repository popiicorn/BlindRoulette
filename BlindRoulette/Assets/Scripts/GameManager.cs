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

    [Header("出現するお宝の種類（ScriptableObjectを登録）")]
    public TreasureData[] availableTreasures;

    [Header("お宝の生成")]
    //public GameObject treasurePrefab;
    public Transform spawnPoint;
    public float spawnRadiusX = 10f;
    public float spawnRadiusZ = 2f;

    [Header("お宝の生成設定")]
    public int initialSpawnCount = 5;         // ★追加：ゲーム開始時（1ターン目）に出す数
    public int additionalSpawnPerTurn = 0;    // ★追加：2ターン目以降、無条件で追加する数

    // （元の private int treasuresToSpawnNextTurn = 1; は 0 に書き換えておきます）
    private int treasuresToSpawnNextTurn = 0;

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

    [Header("扉の設定")]
    public GameObject[] doorObjects; // ★配列（[]）にして複数の扉を入れられるようにする

    void Start()
    {
        StartNextTurn();

        // ★追加：最初のターンに出す数をセットしてからゲームスタート！
        treasuresToSpawnNextTurn = initialSpawnCount;
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

        // ▼▼▼ ここから書き換え ▼▼▼
        for (int i = 0; i < treasuresToSpawnNextTurn; i++)
        {
            float randomX = Random.Range(-spawnRadiusX, spawnRadiusX);
            float randomZ = Random.Range(-spawnRadiusZ, spawnRadiusZ);
            float randomY = Random.Range(0f, 2f);

            Vector3 randomOffset = new Vector3(randomX, randomY, randomZ);
            Vector3 spawnPos = spawnPoint.position + randomOffset;

            // ★ここを変更！
            // くじ引きを引いて、今回出すお宝のデータを決める
            TreasureData selectedTreasure = ChooseRandomTreasure();

            // 選ばれたお宝データの中にある「prefab（3Dモデル）」を生成する
            Instantiate(selectedTreasure.prefab, spawnPos, Quaternion.identity);
        }
        // ▲▲▲ ここまで ▲▲▲

        treasuresToSpawnNextTurn = additionalSpawnPerTurn;

        currentTime = turnTime;
        isTimerRunning = true;
    }

    void TimeUp()
    {
        isTimerRunning = false;
        timerText.text = "審判の刻...！";

        // 1. 爆発する部屋を先に決める
        RoomDetector[] allRooms = FindObjectsOfType<RoomDetector>();
        int randomIndex = Random.Range(0, allRooms.Length);
        RoomDetector explodeRoomObj = allRooms[randomIndex];

        // 2. 演出付き爆発コルーチンを開始
        StartCoroutine(ExecuteExplosionSequence(explodeRoomObj));
    }

    // ★演出と計算を順番に行うコルーチン
    private System.Collections.IEnumerator ExecuteExplosionSequence(RoomDetector room)
    {
        // A. 扉を全部閉じる（★ここを修正：子からAnimatorを探す）
        foreach (GameObject door in doorObjects)
        {
            if (door != null)
            {
                // 親の door ではなく、その子供の中から Animator を探す
                Animator anim = door.GetComponentInChildren<Animator>();
                if (anim != null)
                {
                    anim.SetTrigger("Close");
                }
            }
        }

        yield return new WaitForSeconds(1.0f);

        // B. 爆発する部屋と一致する扉だけを点滅させる
        bool foundExplodingDoor = false;
        foreach (GameObject door in doorObjects)
        {
            Door doorScript = door.GetComponent<Door>();
            if (doorScript != null && doorScript.roomNumber == room.roomNumber)
            {
                // FlashDoor にも子からRendererを探す処理が必要なら、FlashDoor自体も修正します
                StartCoroutine(FlashDoor(door));
                foundExplodingDoor = true;
            }
        }

        // 点滅が終わるまで少し待機（FlashDoorが3回点滅するので約1.8秒待つ）
        if (foundExplodingDoor) yield return new WaitForSeconds(1.8f);

        // C. 爆発処理
        int explodeRoomId = room.roomNumber;
        Debug.Log($"💥部屋 {explodeRoomId} 💥 が大爆発！！！");

        Instantiate(explosionPrefab, room.transform.position, Quaternion.identity);
        cameraShake.PlayShake(0.5f, 0.3f);

        TreasureBox[] allTreasures = FindObjectsOfType<TreasureBox>();
        foreach (TreasureBox treasure in allTreasures)
        {
            int currentTreasureMoney = treasure.data.moneyAmount;
            hostMoney += treasure.data.moneyAmount;

            if (treasure.currentRoom == doubleRoom) currentTreasureMoney *= 2;

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

        if (player.currentRoom == explodeRoomId || player.currentRoom == 0)
        {
            player.totalMoney = 0;
        }

        Invoke("StartNextTurn", 3f);
    }

    // ★追加：お宝のくじ引き処理
    private TreasureData ChooseRandomTreasure()
    {
        // 1. 登録されている全てのお宝の「重みの合計」を計算する
        int totalWeight = 0;
        foreach (TreasureData treasure in availableTreasures)
        {
            totalWeight += treasure.spawnWeight;
        }

        // 2. 0 ～ 合計値-1 の間でランダムな数字（当選番号）を引く
        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        // 3. 当選番号がどのお宝の範囲に入っているか調べる
        foreach (TreasureData treasure in availableTreasures)
        {
            currentWeight += treasure.spawnWeight;
            if (randomValue < currentWeight)
            {
                return treasure; // 当たったお宝データを返す！
            }
        }

        // （保険）万が一計算がズレた場合は、リストの一番目を返す
        return availableTreasures[0];
    }

    private System.Collections.IEnumerator FlashDoor(GameObject door)
    {
        Renderer r = door.GetComponentInChildren<Renderer>();
        if (r == null)
        {
            Debug.LogError(door.name + " の子オブジェクトにRendererが見つかりません！");
            yield break;
        }

        Debug.Log(door.name + " を点滅させます！"); // これが出るか確認

        Color originalColor = r.material.color;
        for (int i = 0; i < 3; i++)
        {
            r.material.color = Color.red;
            yield return new WaitForSeconds(0.3f);
            r.material.color = originalColor;
            yield return new WaitForSeconds(0.3f);
        }
    }
}