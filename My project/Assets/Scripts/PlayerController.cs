using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移動速度")]
    public float moveSpeed = 7f;

    private Rigidbody rb;
    private Vector3 moveInput;

    void Start()
    {
        // 自身のRigidbodyを取得
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // キーボード（WASDキー / 矢印キー）の入力を取得
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        // 移動する方向のベクトルを作成（Y軸は動かさない）
        moveInput = new Vector3(moveX, 0f, moveZ).normalized;
    }

    void FixedUpdate()
    {
        // 物理演算のタイミングでRigidbodyを使ってキャラクターを移動させる
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}