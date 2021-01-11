using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class InstantiateButtons : MonoBehaviour
{
    public GameObject prefab;

    private void Awake()
    {
        DoInstantiateButtons();
    }

    private void Start()
    {
        GetComponentInParent<ScrollRect>().gameObject.SetActive(false);
    }

    private static void LoadMap(string path)
    {
        SaveEdit.pathToLoad = path;
        SceneManager.LoadScene("Main");
    }

    private void DoInstantiateButtons()
    {
        var maps = SaveSystem.GetAllMapNames();
        if (maps.Length == 0)
        {
            var instance = Instantiate(prefab, transform);
            instance.GetComponentInChildren<Text>().text = "You Have No Saved Maps In The Map Folder";
            instance.GetComponent<Button>().onClick.AddListener(delegate { GetComponentInParent<ScrollRect>().gameObject.SetActive(false); });
        }
        foreach (var map in maps)
        {
            var instance= Instantiate(prefab, transform);
            instance.gameObject.name = map;
            instance.GetComponentInChildren<Text>().text = map;
            instance.GetComponent<Button>().onClick.AddListener(delegate { LoadMap(map); });
        }
    }

    public void OpenSaveFolder()
    {
        if (SaveSystem.NoMapDirectory)
            SaveSystem.CreateMapFolder();
        SaveEdit.OpenSaveFolder();
    }

    public void Refresh()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        DoInstantiateButtons();
    }
}
