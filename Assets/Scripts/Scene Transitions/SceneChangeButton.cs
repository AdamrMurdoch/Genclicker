using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    [SerializeField] private SceneTransitionManager scenetransitionmanager;

    private void Start()
    {        
        if (scenetransitionmanager == null)
        {
            Scene currentScene = gameObject.scene;
            GameObject[] rootObjects = currentScene.GetRootGameObjects();
            
            foreach (GameObject obj in rootObjects)
            {
                scenetransitionmanager = obj.GetComponentInChildren<SceneTransitionManager>();
                if (scenetransitionmanager != null)
                {
                    break;
                }
            }
            
            if (scenetransitionmanager == null)
            {
                Debug.LogError($"SceneTransitionManager not found in scene: {currentScene.name}");
            }
        }
    }

    public void ButtonPressed(string ChosenScene)
    {
        if (ChosenScene == "Singles")
        {
            ChosenScene = "Released Single Details";
        }

        scenetransitionmanager.OnSceneChanged(ChosenScene);
    }

    public void ButtonPressedFade(string ChosenScene)
    {
        if (ChosenScene == "Singles")
        {
            ChosenScene = "Released Single Details";
        }

        scenetransitionmanager.OnSceneChangedFade(ChosenScene, 0.2f);
    }
}
