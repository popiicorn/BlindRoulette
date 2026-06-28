using UnityEngine;

public class RoomDetector : MonoBehaviour
{
    [Header("この部屋の番号")]
    public int roomNumber;

    // プレイヤーがセンサー（部屋）に入った瞬間に呼ばれる
    void OnTriggerEnter(Collider other)
    {
        // ぶつかった相手のTagが「Player」だったら
        if (other.CompareTag("Player"))
        {
            Debug.Log("プレイヤーが 【部屋 " + roomNumber + "】 に入りました！");
        }
    }

    // プレイヤーがセンサー（部屋）から出た瞬間に呼ばれる
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("プレイヤーが 【部屋 " + roomNumber + "】 から出ました！");
        }
    }
}