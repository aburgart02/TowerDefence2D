using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour
{
    public int levelNumber = 1;
    public void LoadLevel()
    {
        levelNumber += 1;
        foreach (var o in FindObjectsOfType(typeof(GameObject)))
        {
            if (o.name == "Main Camera" || o.name == "LevelManager")
                continue;
            Destroy(o);
        }
        SceneManager.LoadScene($"Level{levelNumber}");
    }
}