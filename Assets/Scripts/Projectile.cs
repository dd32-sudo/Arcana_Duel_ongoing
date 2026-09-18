using UnityEngine;

/*
 * Projectile.cs
 * 特殊技射出去的飛行物(劍氣/魔法彈)掛這個腳本。
 *
 * === Unity 設置步驟 ===
 * 1. 建一個新的 GameObject，命名 "Projectile_Justice" (之後愚者可以再複製一份改圖)
 *    - 加 Sprite Renderer (放劍氣/魔法彈的圖)
 *    - 加 Rigidbody2D，Gravity Scale 設 0 (飛行物不受重力)
 *    - 加 Circle Collider 2D，勾選 "Is Trigger"
 *    - 把這個腳本掛上去
 * 2. 把這個物件拖到 Project 視窗做成 Prefab (拖到 Assets 資料夾)
 * 3. 場景裡的原始物件可以刪掉，只留 Prefab
 * 4. 把這個 Prefab 分別拖進 Player1 和 Player2 的 FighterController 的
 *    "Projectile Prefab" 欄位
 */

public class Projectile : MonoBehaviour
{
    public float speed = 8f;
    private int direction = 1;
    private FighterController owner;
    private int damage = 10;
    private bool isUltimate = false;

    public void Setup(int dir, FighterController ownerFighter, int dmg, bool ultimate = false)
    {
        direction = dir;
        owner = ownerFighter;
        damage = dmg;
        isUltimate = ultimate;

        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * direction;
        transform.localScale = s;
    }

    void Update()
    {
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        // 超出畫面範圍就自動銷毀，避免累積過多物件
        if (Mathf.Abs(transform.position.x) > 20f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        FighterController target = other.GetComponent<FighterController>();
        if (target != null && target != owner && !target.isDead)
        {
            target.TakeDamage(damage, isUltimate);
            Destroy(gameObject);
        }
    }
}
