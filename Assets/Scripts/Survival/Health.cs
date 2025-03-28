using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public Slider healthSlider;
    public static int healthPercent;
    public string sceneName;

    void Start()
    {
        healthPercent = 100;
        healthSlider.maxValue = 100;
        healthSlider.value = healthPercent;
    }

    void Update()
    {
        
        healthSlider.value = healthPercent;

        if (healthPercent > 100)
        {
            healthPercent = 100;
        }

        if (healthPercent < 1)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
