using UnityEngine;

public class RoomDetector : MonoBehaviour
{
    [Header("この部屋の番号")]
    public int roomNumber;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // プレイヤーのメモ帳を「この部屋の番号」に書き換える
            other.GetComponent<PlayerController>().currentRoom = roomNumber;
            Debug.Log($"プレイヤーが 【部屋 {roomNumber}】 に入りました！");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 部屋から出たら「0（ロビー）」に戻す
            other.GetComponent<PlayerController>().currentRoom = 0;
            Debug.Log($"プレイヤーが 【部屋 {roomNumber}】 から出ました（ロビーへ）");
        }
    }
}