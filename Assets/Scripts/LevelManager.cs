using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class LevelManager : MonoBehaviour
{
    public TMP_Text levelMessage;

    private int golemsKilled = 0;
    private const int requiredGolems = 2;

    void Start()
    {
        levelMessage.text = "";
    }

    public void GolemKilled()
    {
        golemsKilled++;

        if (golemsKilled >= requiredGolems)
        {
            levelMessage.text = "¡Avanzas de nivel!";

            StartCoroutine(LoadNextLevelAfterDelay(2f));
        }
    }

    IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Level2");
    }
}