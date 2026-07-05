using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移動速度")]
    public float moveSpeed = 7f;

    [Header("今いる部屋の番号（0=ロビー）")]
    public int currentRoom = 0;

    
    // ★追加：現在の速度倍率（初期値は1.0、つまり100%のスピード）
    private float currentSpeedMultiplier = 1.0f;

    private Rigidbody rb;
    private Vector3 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        RoomDetector room = other.GetComponent<RoomDetector>();
        if (room != null)
        {
            currentRoom = room.roomNumber;
            Debug.Log($"プレイヤーが 【部屋 {currentRoom}】 に入りました！");
        }
    }

    // ★追加：部屋から出たときの判定処理
    void OnTriggerExit(Collider other)
    {
        RoomDetector room = other.GetComponent<RoomDetector>();
        if (room != null && currentRoom == room.roomNumber)
        {
            currentRoom = 0;
            Debug.Log("プレイヤーが部屋から出てロビー（0）に戻りました");
        }
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(moveX, 0f, moveZ).normalized;
    }

    void FixedUpdate()
    {
        // ★ここを変更！
        // 基本の移動速度(moveSpeed)に、倍率(currentSpeedMultiplier)を掛け算して最終的なスピードを出す
        float actualSpeed = moveSpeed * currentSpeedMultiplier;

        rb.MovePosition(rb.position + moveInput * actualSpeed * Time.fixedDeltaTime);
    }

    // ★追加：宝箱のスクリプトから「この倍率にして！」と命令してもらうための窓口
    public void SetSpeedMultiplier(float multiplier)
    {
        currentSpeedMultiplier = multiplier;
        Debug.Log($"プレイヤーの速度倍率が {multiplier} に変更されました！");
    }


}