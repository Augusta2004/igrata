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
        AsyncOperation operation = SceneManager.LoadSceneAsync(NetworkManager.sceneName, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;        

        /*if (NetworkManager.isLogging == true)
        {
            NetworkManager.isLogging = false;
        }*/

        loadingScreen.SetActive(true);


        if(NetworkManager.sceneName.Contains("Room"))
        {
            NetworkManager.instance.GetComponent<NetworkManager>().ConnectToServer();
            Debug.Log("Loading...");
            Debug.Log(NetworkManager.playerLoaded);

            Debug.Log("Loading 1...");
            //yield return new WaitForSeconds(5f);
            int count = 0;
            //while (!NetworkManager.playerLoaded)
            while (!NetworkManager.playerLoaded || !NetworkManager.otherPlayersLoaded)
            {
                yield return new WaitForSeconds(0.1f);
                progressText.text = "Loading... bears";
                
                //Debug.Log(NetworkManager.playerLoaded);
                count++;
            }
        }
        
        //Debug.Log(NetworkManager.playerLoaded + " | " + NetworkManager.otherPlayersLoaded);
        while (!operation.isDone)
        {
            yield return new WaitForSeconds(0.1f);
            Debug.Log(operation.progress);

            if (operation.progress >= 0.9f)
            {
                GameObject.Find("Canvas").SetActive(false);

                NetworkManager.playerLoaded = false;
                NetworkManager.otherPlayersLoaded = false;

                break;
            }

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            progressText.text = "Loading... " + (progress * 100) + "%  scene";
            Debug.Log("Inside Loading...");


            yield return null;
        }

        Debug.Log("Loading 2...");
        //yield return new WaitForSeconds(5f);
        operation.allowSceneActivation = true;


        yield return operation;
    }
}
