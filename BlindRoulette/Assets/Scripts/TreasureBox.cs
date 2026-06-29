using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    [Header("お宝の金額")]
    public int moneyAmount = 1000;

    [Header("今置いてある部屋の番号（0=ロビー）")]
    public int currentRoom = 0;

    private bool isCarried = false;
    private bool isPlayerInRange = false;
    private Transform playerTransform;

    // ★追加：物理演算のコンポーネントを操作するための準備
    private Rigidbody rb;

    void Start()
    {
        // ゲーム開始時に自分についているRigidbodyを取得しておく
        rb = GetComponent<Rigidbody>();
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

            // ★修正1：持っている間は「重力」と「物理的な衝突」をオフ（無効化）にする！
            if (rb != null) rb.isKinematic = true;

            // プレイヤーとぶつかってガタガタしないよう、固い方の当たり判定だけを消す
            foreach (Collider col in GetComponents<Collider>())
            {
                if (!col.isTrigger) col.enabled = false;
            }

            transform.SetParent(playerTransform);
            transform.localPosition = new Vector3(0, 1.5f, 0);
            Debug.Log("お宝を拾いました！");
        }
    }

    public void Drop(int roomNumber)
    {
        isCarried = false;
        currentRoom = roomNumber;
        transform.SetParent(null);

        // ★修正2：手放したら「重力」をオンに戻し、固い当たり判定も復活させる！
        if (rb != null) rb.isKinematic = false;
        foreach (Collider col in GetComponents<Collider>())
        {
            if (!col.isTrigger) col.enabled = true;
        }

        // 以前の「強制的にY:0.5に置く」処理は削除。頭の高さからポトッと自然に床へ落ちます！
        Debug.Log($"お宝を部屋 {roomNumber} に置きました！");
    }

    public bool IsCarried()
    {
        return isCarried;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerTransform = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerTransform = null;
        }
    }
}