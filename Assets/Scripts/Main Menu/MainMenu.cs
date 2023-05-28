using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class MainMenu : EventTrigger
{
     
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
    
    public static void LoadSave(TMPro.TextMeshProUGUI text)
    {
        WorldSettings.Change(text.text);
        SceneManager.LoadScene("Game");
    }

    public static void CreateWorld(TMPro.TextMeshProUGUI text)
    {
        bool empty = true;

        foreach(char c in text.text)
        {
            if(char.IsLetterOrDigit(c))
            {
                empty = false;
                break;
            }
        }

        if(empty) 
        {
            return;
        }
        
        if(System.IO.Directory.Exists("saves/" + text.text))
        {
            GameObject.Find("Canvas").transform.Find("Create Menu").transform.Find("Error Text").gameObject.SetActive(true);
        }
        else
        {
            System.IO.Directory.CreateDirectory("saves/" + text.text);
            LoadSave(text);
        }

    }

    public static void NavigateToSavesMenu()
    {
        GameObject.Find("Canvas").transform.Find("Create Menu").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Main Menu").gameObject.SetActive(false);

        GameObject.Find("Canvas").transform.Find("Saves Menu").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("Create Menu").transform.Find("Error Text").gameObject.SetActive(false);
    }

    public static void NavigateToMainMenu()
    {
        GameObject.Find("Canvas").transform.Find("Saves Menu").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Create Menu").gameObject.SetActive(false);

        GameObject.Find("Canvas").transform.Find("Main Menu").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("Create Menu").transform.Find("Error Text").gameObject.SetActive(false);
    }

    public static void NavigateToCreateMenu()
    {
        GameObject.Find("Canvas").transform.Find("Saves Menu").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("Main Menu").gameObject.SetActive(false);

        GameObject.Find("Canvas").transform.Find("Create Menu").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("Create Menu").transform.Find("Error Text").gameObject.SetActive(false);
    }

    public static void ShowGameSaves(GameObject canvas)
    {
        string[] saves = System.IO.Directory.GetDirectories("saves/");
        GameObject.Find("Main Menu").SetActive(false);

        GameObject savesMenu = GameObject.Find("Canvas").transform.Find("Saves Menu").gameObject;
        savesMenu.SetActive(true);

        GameObject saveFileUI = (GameObject) Resources.Load("UI/saveFileUI");
        
        int index = 0;

        foreach(string str in saves)
        {
            GameObject obj = Instantiate(saveFileUI, Vector3.zero, Quaternion.identity);
            obj.transform.parent = savesMenu.transform;
            obj.transform.position = new Vector3(0, 0, 0);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(0, 108 - (80 * index++), 0);
            obj.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = System.IO.Path.GetFileName(str);
            obj.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = System.IO.Directory.GetCreationTime(str).ToString();

            // obj.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().margin = new Vector4(0, 0, -330, 0);
            // obj.transform.Find("Date").GetComponent<TMPro.TextMeshProUGUI>().margin = new Vector4(0, 0, -330, 0);

            //Debug.Log(System.IO.Path.GetFileName(str));
        }
    }

}
