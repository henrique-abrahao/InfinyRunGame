using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public void abrirJogo()
    {
        SceneManager.LoadScene("Jogo", LoadSceneMode.Single);
    }

    public void abrirMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
