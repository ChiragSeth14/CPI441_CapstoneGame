using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TalkBombBlobby : MonoBehaviour, IInteractable
{
    public string DeskID { get; private set; }
    [SerializeField] private string sceneName;

    void Start()
    {
        DeskID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }

    public void Interact()
    {
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(sceneName);
    }
    public bool CanInteract()
    {
        return !false;
    }
}