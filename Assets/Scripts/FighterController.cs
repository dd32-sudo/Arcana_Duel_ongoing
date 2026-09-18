using UnityEngine;

public class FighterController : MonoBehaviour
{
    public bool isPlayer1 = true;

    public Transform attackPoint;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public FighterController opponent;
    public GameObject projectilePrefab;

    public float moveSpeed = 5f;
    public float jumpForce = 14f;
    public float attackRange = 4f;
    public float ultimateRange = 5f;
    public int attackDamage = 4;
    public int ultimateDamage = 15;
    public float specialEnergyCost = 35f;

    public float hp = 100f;
    public float energy = 0f;
    public bool isGrounded;
    public bool isBusy = false;
    public bool isBlocking = false;
    public bool isDead = false;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private float moveInput;
    private int facing = 1;
    private int groundContactCount = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer.value) != 0) groundContactCount++;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer.value) != 0) groundContactCount = Mathf.Max(0, groundContactCount - 1);
    }

    void Update()
    {
        if (isDead) return;
        isGrounded = groundContactCount > 0;
        HandleInput();
        FaceOpponent();
        UpdateAnimator();

        if (anim != null && isPlayer1)
        {
            if (isBlocking) anim.Play("justice_swordblock");
            else if (Mathf.Abs(moveInput) > 0.01f && isGrounded && !isBusy) anim.Play("justice_walk");
            else if (isGrounded && !isBusy) anim.Play("justice_idle");
        }
    }

    void HandleInput()
    {
        moveInput = 0;
        if (isPlayer1)
        {
            if (!isBusy)
            {
                if (Input.GetKey(KeyCode.A)) moveInput = -1;
                if (Input.GetKey(KeyCode.D)) moveInput = 1;
                isBlocking = Input.GetKey(KeyCode.S);
                if (Input.GetKeyDown(KeyCode.W) && isGrounded) Jump();
            }
            if (Input.GetKeyDown(KeyCode.F)) TryAttack();
            if (Input.GetKeyDown(KeyCode.G)) TrySpecial();
            if (Input.GetKeyDown(KeyCode.H)) TryUltimate();
        }
        else
        {
            if (!isBusy)
            {
                if (Input.GetKey(KeyCode.LeftArrow)) moveInput = -1;
                if (Input.GetKey(KeyCode.RightArrow)) moveInput = 1;
                isBlocking = Input.GetKey(KeyCode.DownArrow);
                if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded) Jump();
            }
            if (Input.GetKeyDown(KeyCode.K)) TryAttack();
            if (Input.GetKeyDown(KeyCode.L)) TrySpecial();
            if (Input.GetKeyDown(KeyCode.Semicolon)) TryUltimate();
        }

        if (isBlocking) moveInput = 0;
        if (isBusy) moveInput = 0;
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    void Jump() { rb.velocity = new Vector2(rb.velocity.x, jumpForce); }

    void FaceOpponent()
    {
        if (isBusy) return;
        int flipFix = isPlayer1 ? -1 : 1;
        if (Mathf.Abs(moveInput) > 0.01f) facing = (moveInput > 0 ? 1 : -1) * flipFix;
        else if (opponent != null) facing = ((opponent.transform.position.x >= transform.position.x) ? 1 : -1) * flipFix;
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * facing;
        transform.localScale = s;
    }

    void TryAttack()
    {
        if (isBusy) return;
        StartCoroutine(DoAttack());
    }

    System.Collections.IEnumerator DoAttack()
    {
        isBusy = true;
        rb.velocity = Vector2.zero;
        if (anim != null) anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.15f);
        CheckMeleeHit(attackRange, attackDamage, false);
        yield return new WaitForSeconds(0.25f);
        isBusy = false;
    }

    void TrySpecial()
    {
        if (isBusy) return;
        isBusy = true;
        rb.velocity = Vector2.zero;
        if (isPlayer1 && sr != null) sr.flipX = true;
        Debug.Log(gameObject.name + " 特殊技開始, anim為null嗎=" + (anim == null));
        if (anim != null) anim.SetTrigger("Special");
        CheckMeleeHit(attackRange, 6, false);
        if (projectilePrefab != null && attackPoint != null)
        {
            GameObject p = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
            Projectile proj = p.GetComponent<Projectile>();
            if (proj != null) proj.Setup(facing, this, 6);
        }
        float dur = isPlayer1 ? 2.3f : 0.7f;
        Invoke(nameof(EndSpecial), dur);
    }

    void EndSpecial()
    {
        Debug.Log(gameObject.name + " 特殊技結束, 時間=" + Time.time);
        isBusy = false;
        if (isPlayer1 && sr != null) sr.flipX = false;
        if (anim != null && isGrounded) anim.Play(isPlayer1 ? "justice_idle" : "Fool_idle");
    }

    void TryUltimate()
    {
        if (isBusy) return;
        isBusy = true;
        rb.velocity = Vector2.zero;
        if (isPlayer1 && sr != null) sr.flipX = true;
        if (anim != null) anim.SetTrigger("Ultimate");
        CheckMeleeHit(ultimateRange, ultimateDamage, true);
        float dur = isPlayer1 ? 2.3f : 1.0f;
        Invoke(nameof(EndUltimate), dur);
    }

    void EndUltimate()
    {
        isBusy = false;
        if (isPlayer1 && sr != null) sr.flipX = false;
        if (anim != null && isGrounded) anim.Play(isPlayer1 ? "justice_idle" : "Fool_idle");
    }

    void CheckMeleeHit(float range, int damage, bool isUltimate)
    {
        if (opponent == null || opponent.isDead) return;
        float dist = Vector2.Distance(transform.position, opponent.transform.position);
        if (dist <= range)
        {
            opponent.TakeDamage(damage, isUltimate);
            energy = Mathf.Min(100, energy + (isUltimate ? 0 : 8));
        }
    }

    public void TakeDamage(int rawDamage, bool isUltimate)
    {
        if (isDead) return;
        float finalDamage = rawDamage;
        if (isBlocking) finalDamage = 0f;
        else
        {
            isBusy = true;
            if (anim != null) anim.SetTrigger("Hit");
            Invoke(nameof(EndHitStun), 0.3f);
        }
        hp -= finalDamage;
        energy = Mathf.Min(100, energy + (isUltimate ? 10 : 5));
        if (hp <= 0) { hp = 0; Die(); }
    }

    void EndHitStun() { if (!isDead) isBusy = false; }

    void Die()
    {
        isDead = true;
        isBusy = true;
        if (anim != null) anim.SetTrigger("Down");
        rb.velocity = Vector2.zero;
        if (GameManager.instance != null) GameManager.instance.OnFighterDefeated(this);
    }

    void UpdateAnimator()
    {
        if (anim == null) return;
        anim.SetBool("IsMoving", Mathf.Abs(moveInput) > 0.01f && isGrounded && !isBusy);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsBlocking", isBlocking);
    }
}


