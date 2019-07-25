using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public TextMeshProUGUI characterCount;
    public CharacterSpawner characterSpawner;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        characterCount.text = "Characters ("+ characterSpawner.characterDatas.Count + ")";
	}
    
    public void LoadMainScene()
    {
        SceneManager.LoadScene(1);
    }
}
