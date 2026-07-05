using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    [Header("お宝のデータ")]
    public TreasureData data; // ★これを追加！先ほど作ったデータをセットする枠です

    [Header("今置いてある部屋の番号（0=ロビー）")]
    public int currentRoom = 0;

    private bool isCarried = false;
    private bool isPlayerInRange = false;
    private Transform playerTransform;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ★データがセットされているか確認用のログ
        if (data != null)
        {
            Debug.Log($"この宝箱は {data.itemName} です。金額: {data.moneyAmount}");
        }
    }

    void Update()
    {
        if (isCarried && Input.GetKeyDown(KeyCode.Space))
        {
            Drop(transform.parent.GetComponent<PlayerController>().currentRoom);
        }
        else if (!isCarried && isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            isCarried = true;

            if (rb != null) rb.isKinematic = true;

            // ★ここを修正！：コライダーを消すのではなく、プレイヤーとの衝突を無視させる！
            Collider playerCol = playerTransform.GetComponent<Collider>();
            foreach (Collider col in GetComponents<Collider>())
            {
                if (!col.isTrigger) Physics.IgnoreCollision(col, playerCol, true);
            }

            transform.SetParent(playerTransform);
            transform.localPosition = new Vector3(0, 1.5f, 0);

            if (data != null)
            {
                playerTransform.GetComponent<PlayerController>().SetSpeedMultiplier(data.moveSpeedRate);
            }
        }
    }

    public void Drop(int roomNumber)
    {
        isCarried = false;
        currentRoom = roomNumber;
        Transform player = transform.parent;

        // ★ここを修正！：投げる時は、プレイヤーとの衝突無視を解除する
        if (player != null)
        {
            Collider playerCol = player.GetComponent<Collider>();
            foreach (Collider col in GetComponents<Collider>())
            {
                if (!col.isTrigger) Physics.IgnoreCollision(col, playerCol, false);
            }
            player.GetComponent<PlayerController>().SetSpeedMultiplier(1.0f);
        }

        transform.SetParent(null);

        if (rb != null) rb.isKinematic = false;

        // ★ここも削除！：もうColliderのオンオフは不要です

        if (player != null)
        {
            Vector3 throwDirection = player.forward * 5f + Vector3.up * 2f;
            rb.AddForce(throwDirection, ForceMode.Impulse);
        }
    }

    public bool IsCarried()
    {
        return isCarried;
    }

    void OnTriggerEnter(Collider other)
    {
        // 【既存の処理】プレイヤーが近づいたときの判定
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerTransform = other.transform;
        }

        // ★【ここを追加！】宝箱が部屋（RoomDetector）に触れたら、その部屋番号を自動で記憶する
        RoomDetector room = other.GetComponent<RoomDetector>();
        if (room != null)
        {
            currentRoom = room.roomNumber;
            Debug.Log($"{name} が自動で 【部屋 {currentRoom}】 にあると認識しました！");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 【既存の処理】プレイヤーが離れたときの判定
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerTransform = null;
        }

        // ★【ここを追加！】宝箱が部屋（RoomDetector）から完全に外に出たら、一旦ロビー（0）に戻す
        RoomDetector room = other.GetComponent<RoomDetector>();
        if (room != null)
        {
            // 部屋のエリアから出たとき、現在覚えている部屋番号と同じなら0（ロビー）に戻す
            if (currentRoom == room.roomNumber)
            {
                currentRoom = 0;
                Debug.Log($"{name} が部屋から出てロビー（0）に戻りました");
            }
        }
    }

    // TreasureBox.cs に追加
    public void UpdateRoom(int roomNumber)
    {
        currentRoom = roomNumber;
        Debug.Log($"{name} がプレイヤーに持たれたまま 【部屋 {currentRoom}】 に入りました！");
    }
}