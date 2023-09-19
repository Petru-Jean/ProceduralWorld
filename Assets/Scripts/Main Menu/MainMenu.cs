using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    [SerializeField] 
    GameObject[] menus;

    [SerializeField]
    GameObject saveFileUI;

    List<GameObject> saves;
    int              selectedMenu;

    enum Menu : int
    {
        Main        = 0,
        Saves       = 1,
        CreateWorld = 2,
        Statistics  = 3
    };

    void Start()
    {
        saves = new List<GameObject>();
    }

    public void NavigateToMenu(int menu)
    {
        if(menu >= menus.Length)
        {
            Debug.Log("Attempted to navigate to invalid menu id: " + menu);
            return;
        }

        selectedMenu = menu;

        for (int i = 0; i < menus.Length;   i++)
        {
            menus[i].SetActive(i == menu ? true : false);
        }

    }

    public void LoadSaves()
    {
        foreach(GameObject save in saves)
        {
            Destroy(save);
        }

        saves.Clear();

        if(!System.IO.Directory.Exists("saves"))
        {   
            System.IO.Directory.CreateDirectory("saves");
        }

        string[] savedWorlds = System.IO.Directory.GetDirectories("saves/");

        int index = 0;

        foreach(string str in savedWorlds)
        {
            GameObject obj = Instantiate(saveFileUI, Vector3.zero, Quaternion.identity);
            obj.transform.parent     = menus[(int)Menu.Saves].transform;
            obj.transform.position   = new Vector3(0, 0, 0);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(0, 108 - (80 * index++), 0);
            obj.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.IO.Path.GetFileName(str);
            obj.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = System.IO.Directory.GetCreationTime(str).ToString();

            saves.Add(obj);
        }

    }

    public void CreateWorld(TMPro.TMP_InputField inputField)
    {
        string worldName = inputField.text.ToString();//text.ToString();

        if(string.IsNullOrEmpty(worldName) || string.IsNullOrWhiteSpace(worldName))
        {
            Debug.Log("Attempted to create a save with no name.");
            return;
        }

        if(System.IO.Directory.Exists("saves/" + worldName))
        {
            menus[(int)Menu.CreateWorld].transform.Find("Error Text").gameObject.SetActive(true);
        }
        else
        {
            System.IO.Directory.CreateDirectory("saves/" + worldName);
         
            ChunkUtil.LoadSave(worldName);
        }

    }

    public void LoadStatistics(TMPro.TextMeshProUGUI text)
    {
        ChunkUtil.LoadSave(text.text);

        //Statistics stats = new Statistics("")
    }

    public void LoadWorld(TMPro.TextMeshProUGUI text)
    {
        ChunkUtil.LoadSave(text.text);
    }

    public static void OnPointerEnter(TMPro.TextMeshProUGUI text)
    {
        text.fontStyle = TMPro.FontStyles.Bold;
    }

    public static void OnPointerExit(TMPro.TextMeshProUGUI text)
    {
        text.fontStyle = TMPro.FontStyles.Normal;
    }

    public static void OnExit()
    {
        Application.Quit();
    }   
    

}   
