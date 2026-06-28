using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    private bool isCarried = false; // すでに誰かに持たれているかどうかのフラグ

    // プレイヤーが触れた瞬間に呼ばれる
    void OnTriggerEnter(Collider other)
    {
        // まだ誰にも持たれていなくて、触れた相手が「Player」だったら
        if (!isCarried && other.CompareTag("Player"))
        {
            isCarried = true; // 「持たれました」状態にする

            // お宝をプレイヤーの「子オブジェクト（持ち物）」にする
            transform.SetParent(other.transform);

            // プレイヤーの頭の上にポンッと乗せる（位置の微調整）
            transform.localPosition = new Vector3(0, 1.5f, 0);

            Debug.Log("お宝を拾いました！");
        }
    }
}