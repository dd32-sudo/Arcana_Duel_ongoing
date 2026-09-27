using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DivinationManager : MonoBehaviour
{
    [Header("Before drawing")]
    public GameObject     questionPanel;
    public TMP_InputField questionInput;
    public Button         drawButton;

    [Header("After drawing")]
    public GameObject     revealPanel;
    public Image          cardImage;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI orientationText;
    public TextMeshProUGUI meaningText;

    [Header("Navigation")]
    public Button backButton;

    // ── Card data ──────────────────────────────────────────────────────────

    static string[] cardNames = {
        "The Fool", "The Magician", "The High Priestess", "The Empress", "The Emperor",
        "The Hierophant", "The Lovers", "The Chariot", "Strength", "The Hermit",
        "Wheel of Fortune", "Justice", "The Hanged Man", "Death", "Temperance",
        "The Devil", "The Tower", "The Star", "The Moon", "The Sun",
        "The Last Judgment", "The World"
    };

    static string[] imageFiles = {
        "tarot_00","tarot_01","tarot_02","tarot_03","tarot_04",
        "tarot_05","tarot_06","tarot_07","tarot_08","tarot_09",
        "tarot_10","tarot_11","tarot_12","tarot_13","tarot_14",
        "tarot_15","tarot_16","tarot_17","tarot_18","tarot_19",
        "tarot_20","tarot_21"
    };

    static string[] uprightMeanings = {
        "Folly, mania, extravagance, intoxication, delirium, frenzy, involuntary waste, and the state of one who is about to embark on a journey without forethought or reason.",
        "Skill, diplomacy, address, subtlety; sickness, pain, loss, disaster, snares laid by enemies; self-confidence, will, and the man himself as the adept.",
        "Secrets, mystery, the future as yet unrevealed; silence, tenacity; wisdom, science, and the woman who knows.",
        "Fruitfulness, action, initiative, length of days; the unknown, the clandestine; also difficulty, doubt, ignorance, and the gestation of what is yet unseen.",
        "Stability, power, protection, realisation; reason, conviction; a great personage; the rule of order and authority.",
        "Marriage, alliance, captivity, servitude; mercy, goodness, inspiration; the man to whom the orthodox path points.",
        "Attraction, love, beauty, trials overcome, and the harmony of choice made in the presence of the higher self.",
        "Succour, providence; war, triumph, presumption, vengeance, trouble, and the conquest of opposing forces through will.",
        "Power, energy, action, courage, magnanimity; also complete success and honours, the taming of the lion within.",
        "Prudence, circumspection; treason, dissimulation, corruption; the search for divine truth by one who walks alone.",
        "Destiny, fortune, success, felicity, exaltation, the turn of the wheel that brings culmination and good hap.",
        "Equity, righteousness, probity, virtue, honour, and the triumph of the deserving side in legal or moral judgment.",
        "Wisdom, prudence, discernment, trials, sacrifice, intuition, divination, and the suspension of the self for higher understanding.",
        "End, mortality, destruction, corruption, the pain of loss, and the necessary cessation of a current state or hope.",
        "Economy, moderation, frugality, management, accommodation, and the harmonious blending of opposing elements.",
        "Ravage, violence, vehemence, extraordinary efforts, force, fatality, and that which predestines one to a state of bondage without escape.",
        "Misery, distress, adversity, calamity, disgrace, deception, ruin, and the sudden overthrow of established structures.",
        "Loss, theft, privation, abandonment; yet simultaneously hope, bright prospects, and the spiritual light that follows the storm.",
        "Hidden enemies, danger, calumny, darkness, terror, deception, occult forces, and the errors that arise from walking in twilight.",
        "Material happiness, fortunate marriage, contentment, glory, gain, and the radiance of unveiled truth.",
        "Final decision, sentence, determination of a matter without appeal, renewal, rebirth, and the call to a higher state.",
        "Assured success, recompense, voyage, route, emigration, flight, change of place, completion, and the attainment of the perfect whole."
    };

    static string[] reversedMeanings = {
        "Negligence, absence, distribution, carelessness, apathy, nullity, vanity, and the void left by inattention.",
        "Physician, Magus, mental illness, disgrace, disquiet, and the misuse of one's faculties.",
        "Passion, moral or physical ardour, conceit, surface knowledge, and the stirring of hidden fires.",
        "Light, truth, the unravelling of involved matters, public rejoicings; according to another reading, hesitation, vacillation.",
        "Benevolence, compassion, confidence; also immaturity, ineffectiveness, and confusion before adversaries.",
        "Society, good understanding, concord, over-kindness, weakness, and the surrender of one's own authority.",
        "Failure, foolish designs, frustrated love, and the disruption of union by unwise freedom.",
        "Riot, quarrel, dispute, litigation, defeat, and the loss of control over conflicting impulses.",
        "Despotism, abuse of power, weakness, discord, disgrace, and the fall that follows arrogance.",
        "Concealment, disguise, policy, fear, unreasoned caution, and the paralysis that comes from excessive secrecy.",
        "Increase, abundance, superfluity, and the continuance of favourable momentum.",
        "Law in delay, bias, severity, and the miscarriage of impartial decision.",
        "Selfishness, the crowd, body politic, vested interests, and the refusal to surrender personal will.",
        "Inertia, sleep, lethargy, petrifaction, and the stagnation that resists the natural cycle of change.",
        "Things connected with churches, religions, sects, divisions, unfortunate combinations, and competing interests that refuse reconciliation.",
        "Evil fatality, weakness, pettiness, blindness, and the inability to perceive the chains that bind.",
        "Oppression, imprisonment, tyranny, and according to one account, the same calamity but in a lesser degree.",
        "Arrogance, haughtiness, impotence, and the failure to grasp the gifts offered by providence.",
        "Instability, inconstancy, silence, lesser degrees of deception and error, and the gradual dispelling of illusion.",
        "The same in a lesser sense; also clouded joy, partial success, and warmth that does not fully penetrate.",
        "Weakness, pusillanimity, simplicity, delay, and the inability to answer the summons when it comes.",
        "Inertia, fixity, stagnation, permanence, and the refusal to move beyond the achieved circle."
    };

    // ─────────────────────────────────────────────────────────────────────

    void Start()
    {
        questionPanel.SetActive(true);
        revealPanel.SetActive(false);

        drawButton.onClick.AddListener(OnDraw);
        backButton.onClick.AddListener(OnBack);
    }

    void OnDraw()
    {
        int index    = Random.Range(0, 22);
        bool reversed = Random.value > 0.5f;

        questionPanel.SetActive(false);
        revealPanel.SetActive(true);

        cardNameText.text    = cardNames[index];
        
        // 【已修改】將原本的 ⟳ 替換成標準的 ↓，與 ↑ 形成完美對齊，徹底解決字型缺字報錯
        orientationText.text = reversed ? "↓  Reversed" : "↑  Upright";
        
        meaningText.text     = reversed ? reversedMeanings[index] : uprightMeanings[index];

        Sprite spr = Resources.Load<Sprite>("Tarot/" + imageFiles[index]);
        if (spr != null)
        {
            cardImage.sprite = spr;
            cardImage.color  = Color.white;

            // Rotate image if reversed
            cardImage.transform.localEulerAngles = reversed
                ? new Vector3(0f, 0f, 180f)
                : Vector3.zero;
        }
    }

    void OnBack()
    {
        if (GameManager.Instance != null) GameManager.Instance.GoToMainMenu();
        else UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}