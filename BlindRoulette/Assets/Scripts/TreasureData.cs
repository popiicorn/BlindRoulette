using UnityEngine;

// これを書くことで、Unity上でこのデータファイルを作成できるようになります
[CreateAssetMenu(fileName = "NewTreasure", menuName = "Treasure/TreasureData")]
public class TreasureData : ScriptableObject
{
    [Header("基本情報")]
    public string itemName;       // アイテムの名前（例：札束、金塊）
    public int moneyAmount;       // 獲得できる金額
    public float moveSpeedRate = 1.0f; // 運ぶ時の速度倍率（1.0で通常）

    [Header("出現設定")]
    public int spawnWeight = 10;  // 出現しやすさ（数字が大きいほど出やすい）
    public GameObject prefab;     // 実際の3Dモデル（プレハブ）
}