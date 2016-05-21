using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoToCredits : MonoBehaviour {
    void Start()
    {
        goToScene();
        
    }

    void goToScene()
    {
        StartCoroutine(LoadAfterDelay(03));
    }

    IEnumerator LoadAfterDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(5);
    }


}
