using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    private bool isCarried = false;
    private bool isPlayerInRange = false; // プレイヤーがお宝の近くにいるかどうかのメモ
    private Transform playerTransform;    // 近くにいるプレイヤー自身の情報

    void Update()
    {
        // ①【置く処理】持っている状態でスペースキーを押したら
        if (isCarried && Input.GetKeyDown(KeyCode.Space))
        {
            isCarried = false;
            transform.SetParent(null);

            // 床の高さ（Y: 0.5）にピタッと置く
            Vector3 currentPos = transform.position;
            transform.position = new Vector3(currentPos.x, 0.5f, currentPos.z);

            Debug.Log("お宝を床に置きました！");
        }
        // ②【拾う処理】持っていなくて ＆ プレイヤーが近くにいて ＆ スペースキーを押したら
        else if (!isCarried && isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            isCarried = true;
            transform.SetParent(playerTransform);
            transform.localPosition = new Vector3(0, 1.5f, 0); // 頭の上に乗せる
            Debug.Log("お宝を拾いました！");
        }
    }

    // プレイヤーがお宝のセンサー（範囲）に入った瞬間
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerTransform = other.transform; // 拾う時のために「誰が近づいてきたか」を記憶しておく
        }
    }

    // プレイヤーがお宝のセンサー（範囲）から出た瞬間
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerTransform = null; // 離れたら記憶を消す
        }
    }
}