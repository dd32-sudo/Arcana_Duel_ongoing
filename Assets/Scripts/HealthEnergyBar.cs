using UnityEngine;
using UnityEngine.UI;

/*
 * HealthEnergyBar.cs
 * 掛在血條/能量條的 UI 物件上，讓它跟著角色的 hp/energy 即時更新。
 *
 * === Unity 設置步驟 ===
 * 1. Hierarchy 右鍵 > UI > Slider，建立血條，改名 "HP_Bar_Justice"
 *    - 拿掉 Slider 上的 Handle Slide Area (不需要拖動，只是顯示用)
 *    - Fill 的顏色改成正義的配色 (金/白)
 * 2. 再建一個 Slider 當能量條，改名 "Energy_Bar_Justice"，Fill改金色
 * 3. 建立一個空 UI GameObject 掛這個腳本 (或直接掛在 Slider 上都行)
 * 4. 把對應的 FighterController 拖進 targetFighter 欄位
 * 5. 把 HP_Bar 和 Energy_Bar 兩個 Slider 拖進對應欄位
 * 6. 愚者那邊複製一份，targetFighter 換成 Player2_Fool
 */

public class HealthEnergyBar : MonoBehaviour
{
    public FighterController targetFighter;
    public Slider hpBar;
    public Slider energyBar;

    void Start()
    {
        if (hpBar != null)
        {
            hpBar.maxValue = 100;
        }
        if (energyBar != null)
        {
            energyBar.maxValue = 100;
        }
    }

    void Update()
    {
        if (targetFighter == null) return;
        if (hpBar != null) hpBar.value = targetFighter.hp;
        if (energyBar != null) energyBar.value = targetFighter.energy;
    }
}
