using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FighterController : MonoBehaviour
{
    // =========================================================
    // PLAYER
    // =========================================================

    [Header("Player")]
    public bool isPlayer1 = true;

    // =========================================================
    // REFERENCES
    // =========================================================

    [Header("References")]
    public Transform attackPoint;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public FighterController opponent;
    public GameObject projectilePrefab;

    // =========================================================
    // MOVEMENT
    // =========================================================

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 18f;

    // =========================================================
    // ATTACK
    // =========================================================

    [Header("Attack")]
    public float attackRange = 1.6f;
    public float ultimateRange = 2f;

    public int attackDamage = 4;
    public int specialDamage = 6;
    public int ultimateDamage = 15;

    public float specialEnergyCost = 35f;

    // =========================================================
    // STATS
    // =========================================================

    [Header("Stats")]
    public float maxHP = 100f;
    public float hp = 100f;
    public float energy = 0f;

    // =========================================================
    // STATE
    // =========================================================

    [Header("State")]
    public bool isGrounded = false;
    public bool isBusy = false;
    public bool isBlocking = false;
    public bool isDead = false;

    // =========================================================
    // COMPONENTS
    // =========================================================

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;
    private Collider2D myCollider;

    // =========================================================
    // MOVEMENT STATE
    // =========================================================

    private float moveInput = 0f;
    private int facing = 1;

    // =========================================================
    // ANIMATION
    // =========================================================

    private float specialClipLength = 1.5f;
    private float ultimateClipLength = 1.5f;

    // =========================================================
    // HP UI
    // =========================================================

    private Image hpFill;
    private Text hpText;

    private RectTransform hpFillRect;
    private RectTransform hpBarRect;

    private static Canvas hpCanvas;

    // =========================================================
    // AWAKE
    // =========================================================

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();

        hp = maxHP;

        FindAnimationLengths();
    }

    // =========================================================
    // START
    // =========================================================

    void Start()
    {
        CreateHPBar();

        UpdateHPBar();

        Debug.Log(
            gameObject.name +
            " HP BAR CREATED | HP = " +
            hp
        );
    }

    // =========================================================
    // UPDATE
    // =========================================================

    void Update()
    {
        if (isDead)
            return;

        UpdateGrounded();

        HandleInput();

        FaceOpponent();

        UpdateAnimator();

        // -----------------------------------------------------
        // P1 animation
        // -----------------------------------------------------

        if (anim != null && isPlayer1)
        {
            if (isBlocking)
            {
                anim.Play("justice_swordblock");
            }
            else if (
                Mathf.Abs(moveInput) > 0.01f &&
                isGrounded &&
                !isBusy
            )
            {
                anim.Play("justice_walk");
            }
            else if (
                isGrounded &&
                !isBusy
            )
            {
                anim.Play("justice_idle");
            }
        }
    }

    // =========================================================
    // FIXED UPDATE
    // =========================================================

    void FixedUpdate()
    {
        if (rb == null)
            return;

        if (isDead)
            return;

        rb.velocity =
            new Vector2(
                moveInput * moveSpeed,
                rb.velocity.y
            );
    }

    // =========================================================
    // GROUND CHECK
    // =========================================================

    void UpdateGrounded()
    {
        if (myCollider == null)
        {
            isGrounded = false;
            return;
        }

        Vector2 origin =
            new Vector2(
                myCollider.bounds.center.x,
                myCollider.bounds.min.y + 0.05f
            );

        RaycastHit2D hit =
            Physics2D.Raycast(
                origin,
                Vector2.down,
                0.15f,
                groundLayer
            );

        bool rayGrounded =
            hit.collider != null;

        bool checkGrounded = false;

        if (groundCheck != null)
        {
            Collider2D[] hits =
                Physics2D.OverlapCircleAll(
                    groundCheck.position,
                    0.15f,
                    groundLayer
                );

            checkGrounded =
                hits.Length > 0;
        }

        isGrounded =
            rayGrounded ||
            checkGrounded;
    }

    // =========================================================
    // INPUT
    // =========================================================

    void HandleInput()
    {
        moveInput = 0f;

        // =====================================================
        // PLAYER 1
        // =====================================================

        if (isPlayer1)
        {
            if (!isBusy)
            {
                if (Input.GetKey(KeyCode.A))
                    moveInput = -1f;

                if (Input.GetKey(KeyCode.D))
                    moveInput = 1f;

                isBlocking =
                    Input.GetKey(KeyCode.S);

                if (
                    Input.GetKeyDown(KeyCode.W) &&
                    isGrounded
                )
                {
                    Jump();
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
                TryAttack();

            if (Input.GetKeyDown(KeyCode.G))
                TrySpecial();

            if (Input.GetKeyDown(KeyCode.H))
                TryUltimate();
        }

        // =====================================================
        // PLAYER 2
        // =====================================================

        else
        {
            if (!isBusy)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                    moveInput = -1f;

                if (Input.GetKey(KeyCode.RightArrow))
                    moveInput = 1f;

                isBlocking =
                    Input.GetKey(KeyCode.DownArrow);

                if (
                    Input.GetKeyDown(KeyCode.UpArrow) &&
                    isGrounded
                )
                {
                    Jump();
                }
            }

            if (Input.GetKeyDown(KeyCode.K))
                TryAttack();

            if (Input.GetKeyDown(KeyCode.L))
                TrySpecial();

            if (
                Input.GetKeyDown(
                    KeyCode.Semicolon
                )
            )
            {
                TryUltimate();
            }
        }

        // =====================================================
        // BLOCK / BUSY
        // =====================================================

        if (isBlocking)
            moveInput = 0f;

        if (isBusy)
            moveInput = 0f;
    }

    // =========================================================
    // JUMP
    // =========================================================

    void Jump()
    {
        if (rb == null)
            return;

        rb.velocity =
            new Vector2(
                rb.velocity.x,
                jumpForce
            );
    }

    // =========================================================
    // FACE OPPONENT
    // =========================================================

    void FaceOpponent()
    {
        if (isBusy)
            return;

        if (opponent == null)
            return;

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            facing =
                moveInput > 0
                ? 1
                : -1;
        }
        else
        {
            facing =
                opponent.transform.position.x >=
                transform.position.x
                ? 1
                : -1;
        }

        if (sr != null)
        {
            sr.flipX =
                facing < 0;
        }
    }

    // =========================================================
    // NORMAL ATTACK
    // =========================================================

    void TryAttack()
    {
        if (isBusy)
            return;

        if (isDead)
            return;

        StartCoroutine(
            DoAttack()
        );
    }

    IEnumerator DoAttack()
    {
        isBusy = true;

        if (rb != null)
            rb.velocity = Vector2.zero;

        if (anim != null)
            anim.SetTrigger("Attack");

        yield return new WaitForSeconds(
            0.15f
        );

        CheckMeleeHit(
            attackRange,
            attackDamage,
            false
        );

        yield return new WaitForSeconds(
            0.25f
        );

        isBusy = false;
    }

    // =========================================================
    // SPECIAL
    // =========================================================

    void TrySpecial()
    {
        if (isBusy)
            return;

        if (isDead)
            return;

        if (energy < specialEnergyCost)
            return;

        energy -= specialEnergyCost;

        isBusy = true;

        if (rb != null)
            rb.velocity = Vector2.zero;

        if (anim != null)
            anim.SetTrigger("Special");

        CheckMeleeHit(
            attackRange,
            specialDamage,
            false
        );

        // -----------------------------------------------------
        // PROJECTILE
        // -----------------------------------------------------

        if (
            projectilePrefab != null &&
            attackPoint != null
        )
        {
            GameObject p =
                Instantiate(
                    projectilePrefab,
                    attackPoint.position,
                    Quaternion.identity
                );

            Projectile projectile =
                p.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.Setup(
                    facing,
                    this,
                    specialDamage
                );
            }
        }

        Invoke(
            nameof(EndSpecial),
            specialClipLength
        );
    }

    void EndSpecial()
    {
        if (isDead)
            return;

        isBusy = false;

        if (
            anim != null &&
            isGrounded
        )
        {
            if (isPlayer1)
                anim.Play(
                    "justice_idle"
                );
            else
                anim.Play(
                    "Fool_idle"
                );
        }
    }

    // =========================================================
    // ULTIMATE
    // =========================================================

    void TryUltimate()
    {
        if (isBusy)
            return;

        if (isDead)
            return;

        if (energy < 100f)
            return;

        energy = 0f;

        isBusy = true;

        if (rb != null)
            rb.velocity = Vector2.zero;

        if (anim != null)
            anim.SetTrigger(
                "Ultimate"
            );

        CheckMeleeHit(
            ultimateRange,
            ultimateDamage,
            true
        );

        Invoke(
            nameof(EndUltimate),
            ultimateClipLength
        );
    }

    void EndUltimate()
    {
        if (isDead)
            return;

        isBusy = false;

        if (
            anim != null &&
            isGrounded
        )
        {
            if (isPlayer1)
                anim.Play(
                    "justice_idle"
                );
            else
                anim.Play(
                    "Fool_idle"
                );
        }
    }

    // =========================================================
    // MELEE HIT
    // =========================================================

    void CheckMeleeHit(
        float range,
        int damage,
        bool isUltimate
    )
    {
        if (opponent == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " 沒有設定 Opponent！"
            );

            return;
        }

        if (opponent.isDead)
            return;

        float xDistance =
            Mathf.Abs(
                transform.position.x -
                opponent.transform.position.x
            );

        float yDistance =
            Mathf.Abs(
                transform.position.y -
                opponent.transform.position.y
            );

        Debug.Log(
            gameObject.name +
            " ATTACK | X=" +
            xDistance +
            " | Y=" +
            yDistance +
            " | Range=" +
            range
        );

        if (
            xDistance <= range &&
            yDistance <= 2f
        )
        {
            Debug.Log(
                gameObject.name +
                " HIT " +
                opponent.gameObject.name +
                " | DAMAGE=" +
                damage
            );

            opponent.TakeDamage(
                damage,
                isUltimate
            );

            if (!isUltimate)
            {
                energy =
                    Mathf.Min(
                        100f,
                        energy + 8f
                    );
            }
        }
        else
        {
            Debug.Log(
                gameObject.name +
                " MISS"
            );
        }
    }

    // =========================================================
    // TAKE DAMAGE
    // =========================================================

    public void TakeDamage(
        int rawDamage,
        bool isUltimate
    )
    {
        if (isDead)
            return;

        float finalDamage =
            rawDamage;

        // -----------------------------------------------------
        // BLOCK
        // -----------------------------------------------------

        if (isBlocking)
        {
            finalDamage = 0f;

            Debug.Log(
                gameObject.name +
                " BLOCKED!"
            );
        }
        else
        {
            isBusy = true;

            if (anim != null)
                anim.SetTrigger(
                    "Hit"
                );

            CancelInvoke(
                nameof(EndHitStun)
            );

            Invoke(
                nameof(EndHitStun),
                0.3f
            );
        }

        // =====================================================
        // ACTUAL DAMAGE
        // =====================================================

        hp -= finalDamage;

        hp =
            Mathf.Clamp(
                hp,
                0f,
                maxHP
            );

        energy =
            Mathf.Min(
                100f,
                energy +
                (isUltimate ? 10f : 5f)
            );

        // =====================================================
        // UPDATE HP BAR
        // =====================================================

        UpdateHPBar();

        Debug.Log(
            "======================================"
        );

        Debug.Log(
            gameObject.name +
            " HP: " +
            hp +
            " / " +
            maxHP
        );

        Debug.Log(
            "HP PERCENT: " +
            (hp / maxHP)
        );

        Debug.Log(
            "======================================"
        );

        // =====================================================
        // DEATH
        // =====================================================

        if (hp <= 0f)
        {
            Die();
        }
    }

    // =========================================================
    // HIT STUN END
    // =========================================================

    void EndHitStun()
    {
        if (!isDead)
            isBusy = false;
    }

    // =========================================================
    // DEATH
    // =========================================================

    void Die()
    {
        if (isDead)
            return;

        isDead = true;
        isBusy = true;
        isBlocking = false;

        if (rb != null)
            rb.velocity = Vector2.zero;

        if (anim != null)
            anim.SetTrigger(
                "Down"
            );

        UpdateHPBar();

        if (
            GameManager.instance != null
        )
        {
            GameManager.instance
                .OnFighterDefeated(this);
        }
    }

    // =========================================================
    // ANIMATOR
    // =========================================================

    void UpdateAnimator()
    {
        if (anim == null)
            return;

        anim.SetBool(
            "IsMoving",
            Mathf.Abs(moveInput) > 0.01f &&
            isGrounded &&
            !isBusy
        );

        anim.SetBool(
            "IsGrounded",
            isGrounded
        );

        anim.SetBool(
            "IsBlocking",
            isBlocking
        );
    }

    // =========================================================
    // FIND ANIMATION LENGTHS
    // =========================================================

    void FindAnimationLengths()
    {
        if (anim == null)
            return;

        if (
            anim.runtimeAnimatorController == null
        )
            return;

        RuntimeAnimatorController rac =
            anim.runtimeAnimatorController;

        foreach (
            AnimationClip clip
            in rac.animationClips
        )
        {
            if (
                clip.name ==
                "justice_swordwave" ||
                clip.name ==
                "fool_magicbullet"
            )
            {
                specialClipLength =
                    clip.length;
            }

            if (
                clip.name ==
                "justice_wheeloffate" ||
                clip.name ==
                "fool_towerlighting"
            )
            {
                ultimateClipLength =
                    clip.length;
            }
        }
    }

    // =========================================================
    // CREATE HP CANVAS
    // =========================================================

    void CreateHPBar()
    {
        // -----------------------------------------------------
        // CREATE CANVAS ONCE
        // -----------------------------------------------------

        if (hpCanvas == null)
        {
            GameObject canvasObject =
                new GameObject(
                    "Fighter HP Canvas"
                );

            hpCanvas =
                canvasObject.AddComponent<
                    Canvas
                >();

            hpCanvas.renderMode =
                RenderMode.ScreenSpaceOverlay;

            hpCanvas.sortingOrder = 1000;

            CanvasScaler scaler =
                canvasObject.AddComponent<
                    CanvasScaler
                >();

            scaler.uiScaleMode =
                CanvasScaler.ScaleMode
                    .ScaleWithScreenSize;

            scaler.referenceResolution =
                new Vector2(
                    1920f,
                    1080f
                );

            scaler.matchWidthOrHeight =
                0.5f;

            canvasObject.AddComponent<
                GraphicRaycaster
            >();
        }

        // -----------------------------------------------------
        // BAR OBJECT
        // -----------------------------------------------------

        string barName =
            isPlayer1
            ? "Player 1 HP BAR"
            : "Player 2 HP BAR";

        GameObject barObject =
            new GameObject(
                barName
            );

        barObject.transform.SetParent(
            hpCanvas.transform,
            false
        );

        hpBarRect =
            barObject.AddComponent<
                RectTransform
            >();

        // -----------------------------------------------------
        // P1 LEFT
        // -----------------------------------------------------

        if (isPlayer1)
        {
            hpBarRect.anchorMin =
                new Vector2(
                    0f,
                    1f
                );

            hpBarRect.anchorMax =
                new Vector2(
                    0f,
                    1f
                );

            hpBarRect.pivot =
                new Vector2(
                    0f,
                    1f
                );

            hpBarRect.anchoredPosition =
                new Vector2(
                    70f,
                    -60f
                );
        }

        // -----------------------------------------------------
        // P2 RIGHT
        // -----------------------------------------------------

        else
        {
            hpBarRect.anchorMin =
                new Vector2(
                    1f,
                    1f
                );

            hpBarRect.anchorMax =
                new Vector2(
                    1f,
                    1f
                );

            hpBarRect.pivot =
                new Vector2(
                    1f,
                    1f
                );

            hpBarRect.anchoredPosition =
                new Vector2(
                    -70f,
                    -60f
                );
        }

        hpBarRect.sizeDelta =
            new Vector2(
                500f,
                45f
            );

        // =====================================================
        // BACKGROUND
        // =====================================================

        GameObject backgroundObject =
            new GameObject(
                "HP Background"
            );

        backgroundObject.transform.SetParent(
            barObject.transform,
            false
        );

        RectTransform backgroundRect =
            backgroundObject.AddComponent<
                RectTransform
            >();

        backgroundRect.anchorMin =
            Vector2.zero;

        backgroundRect.anchorMax =
            Vector2.one;

        backgroundRect.offsetMin =
            Vector2.zero;

        backgroundRect.offsetMax =
            Vector2.zero;

        Image backgroundImage =
            backgroundObject.AddComponent<
                Image
            >();

        backgroundImage.color =
            new Color(
                0.03f,
                0.03f,
                0.03f,
                1f
            );

        // =====================================================
        // HP FILL
        // =====================================================

        GameObject fillObject =
            new GameObject(
                "HP Fill"
            );

        fillObject.transform.SetParent(
            barObject.transform,
            false
        );

        hpFillRect =
            fillObject.AddComponent<
                RectTransform
            >();

        // -----------------------------------------------------
        // Fill starts at LEFT
        // -----------------------------------------------------

        hpFillRect.anchorMin =
            new Vector2(
                0f,
                0.5f
            );

        hpFillRect.anchorMax =
            new Vector2(
                0f,
                0.5f
            );

        hpFillRect.pivot =
            new Vector2(
                0f,
                0.5f
            );

        hpFillRect.anchoredPosition =
            new Vector2(
                5f,
                0f
            );

        hpFillRect.sizeDelta =
            new Vector2(
                490f,
                35f
            );

        // -----------------------------------------------------
        // IMAGE
        // -----------------------------------------------------

        hpFill =
            fillObject.AddComponent<
                Image
            >();

        hpFill.color =
            new Color(
                0.9f,
                0.05f,
                0.05f,
                1f
            );

        // -----------------------------------------------------
        // VERY IMPORTANT:
        // DO NOT USE Image.Type.Filled
        //
        // The HP bar is controlled entirely by
        // RectTransform.localScale.x
        // -----------------------------------------------------

        hpFill.type =
            Image.Type.Simple;

        hpFill.preserveAspect = false;

        hpFillRect.localScale =
            new Vector3(
                1f,
                1f,
                1f
            );

        // =====================================================
        // HP TEXT
        // =====================================================

        GameObject textObject =
            new GameObject(
                "HP Text"
            );

        textObject.transform.SetParent(
            barObject.transform,
            false
        );

        RectTransform textRect =
            textObject.AddComponent<
                RectTransform
            >();

        textRect.anchorMin =
            Vector2.zero;

        textRect.anchorMax =
            Vector2.one;

        textRect.offsetMin =
            Vector2.zero;

        textRect.offsetMax =
            Vector2.zero;

        hpText =
            textObject.AddComponent<
                Text
            >();

        hpText.alignment =
            TextAnchor.MiddleCenter;

        hpText.fontSize = 22;

        hpText.fontStyle =
            FontStyle.Bold;

        hpText.color =
            Color.white;

        hpText.text =
            Mathf.RoundToInt(hp) +
            " / " +
            Mathf.RoundToInt(maxHP);

        // -----------------------------------------------------
        // Make text render above fill
        // -----------------------------------------------------

        textObject.transform.SetAsLastSibling();
    }

    // =========================================================
    // UPDATE HP BAR
    // =========================================================

    void UpdateHPBar()
    {
        if (hpFillRect != null)
        {
            float percent =
                hp / maxHP;

            percent =
                Mathf.Clamp01(
                    percent
                );

            // -------------------------------------------------
            // THIS IS THE IMPORTANT PART
            // -------------------------------------------------

            Vector3 scale =
                hpFillRect.localScale;

            scale.x =
                percent;

            hpFillRect.localScale =
                scale;
        }

        // -----------------------------------------------------
        // UPDATE NUMBER
        // -----------------------------------------------------

        if (hpText != null)
        {
            hpText.text =
                Mathf.RoundToInt(hp) +
                " / " +
                Mathf.RoundToInt(maxHP);
        }
    }

    // =========================================================
    // GIZMOS
    // =========================================================

    void OnDrawGizmosSelected()
    {
        Gizmos.color =
            Color.red;

        Vector3 attackPosition =
            transform.position +
            Vector3.right *
            facing *
            attackRange;

        Gizmos.DrawWireSphere(
            attackPosition,
            0.2f
        );

        if (groundCheck != null)
        {
            Gizmos.color =
                Color.green;

            Gizmos.DrawWireSphere(
                groundCheck.position,
                0.15f
            );
        }
    }
}