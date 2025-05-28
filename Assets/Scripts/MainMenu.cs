using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Метод для переключения сцены, вызываемый кнопкой
    public void LoadGameScene(string SampleScene)
    {
        SceneManager.LoadScene(SampleScene);
    }
}