using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour {


    public GameObject loadingScreen;
    public Slider slider;
    public Text progressText;

     void Start()
    {
        StartCoroutine(LoadAsynchronously());
    }


    IEnumerator LoadAsynchronously()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(NetworkManager.sceneName);
        operation.allowSceneActivation = false;        

        if (NetworkManager.isLogging == true)
        {
            //NetworkManager.instance.GetComponent<NetworkManager>().ConnectToServer();

            NetworkManager.isLogging = false;
        }

        Debug.Log("Loading...");

        loadingScreen.SetActive(true);
        Debug.Log("Loading 1...");
        //yield return new WaitForSeconds(5f);
        while (!operation.isDone)
        {
            yield return new WaitForSeconds(2f);
            Debug.Log(operation.progress);

            if (operation.progress >= 0.9f)
            {

                break;
            }

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            progressText.text = "Loading... " + (progress * 100) + "%";
            Debug.Log("Inside Loading...");


            yield return null;
        }

        Debug.Log("Loading 2...");
        operation.allowSceneActivation = true;
        yield return new WaitForSeconds(5f);
        
        yield return operation;
    }
}
