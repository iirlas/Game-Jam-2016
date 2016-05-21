using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour{

    public void GotoScene (int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void Quit ()
    {
        Application.Quit();
    }

}
