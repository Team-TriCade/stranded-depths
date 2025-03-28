using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Hunger : MonoBehaviour
{
    public Slider hungerSlider;
    public static int hungerPercent;
    public string sceneName;
    public float baseDecayRate = 300f; // 5 minutes
    public int hungerDecayAmount = 2;
    public float sprintDecayMultiplier = 2f;

    private PlayerController player;

    void Start()
    {
        hungerPercent = 100;
        hungerSlider.maxValue = 100;
        hungerSlider.value = hungerPercent;
        player = FindObjectOfType<PlayerController>();

        InvokeRepeating(nameof(DecreaseHunger), baseDecayRate, baseDecayRate);
    }

    void Update()
    {
        hungerSlider.value = hungerPercent;

        if (hungerPercent > 100)
            hungerPercent = 100;

        if (hungerPercent < 1)
            SceneManager.LoadScene(sceneName);
    }

    void DecreaseHunger()
    {
        float multiplier = player.IsSprinting() ? sprintDecayMultiplier : 1f;
        hungerPercent -= Mathf.RoundToInt(hungerDecayAmount * multiplier);

        if (hungerPercent < 26)
            Health.healthPercent -= 5;
    }
}
