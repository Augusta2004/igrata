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
        
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            yield return new WaitForSeconds(0.1f);
            Debug.Log(operation.progress + "Progress");

            if (operation.progress >= 0.9f)
            {
                NetworkManager.playerLoaded = false;
                NetworkManager.otherPlayersLoaded = false;

                break;
            }

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            slider.value = progress;
            progressText.text = "Loading... " + (progress * 100) + "%  scene";
            
            yield return null;
        }


        if (NetworkManager.sceneName.Contains("Room"))
        {
            NetworkManager.instance.GetComponent<NetworkManager>().ConnectToServer();
           
            int count = 0;
            while (!NetworkManager.playerLoaded || !NetworkManager.otherPlayersLoaded)
            {
                yield return new WaitForSeconds(0.1f);
                progressText.text = "Loading... bears";
             
                count++;
            }
        }
        
        GameObject.Find("Canvas").SetActive(false);
        //yield return new WaitForSeconds(5f);
        operation.allowSceneActivation = true;
        
        yield return operation;
    }
}
