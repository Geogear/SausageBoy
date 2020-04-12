using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void EndGame()
    {
        Invoke("Restart",2f);
    }

    private void Restart()
    {
        SceneManager.LoadScene("Menu");
    }
}
