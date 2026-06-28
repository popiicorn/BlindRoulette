using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    [Header("お宝の金額")]
    public int moneyAmount = 1000; // この箱は1000万円の価値！

    [Header("今置いてある部屋の番号（0=ロビー）")]
    public int currentRoom = 0;

    private bool isCarried = false;
    private bool isPlayerInRange = false;
    private Transform playerTransform;

    void Update()
    {
        if (isCarried && Input.GetKeyDown(KeyCode.Space))
        {
            Drop(transform.parent.GetComponent<PlayerController>().currentRoom);
        }
        else if (!isCarried && isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            isCarried = true;
            transform.SetParent(playerTransform);
            transform.localPosition = new Vector3(0, 1.5f, 0);
            Debug.Log("お宝を拾いました！");
        }
    }

    // お宝を床に置く共通の処理
    public void Drop(int roomNumber)
    {
        isCarried = false;
        currentRoom = roomNumber; // 置かれた部屋の番号を記憶
        transform.SetParent(null);

        Vector3 currentPos = transform.position;
        transform.position = new Vector3(currentPos.x, 0.5f, currentPos.z);
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