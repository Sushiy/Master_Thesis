using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreator : MonoBehaviour
{
    public Transform characterCreationParent;
    public GameObject characterCreationPrefab;

    public CharacterSpawner characterSpawner;

    private void Start()
    {
        AddInitialCharacters();
    }

    public void NewCharacter()
    {
        CharacterCreationPanel p =  Instantiate(characterCreationPrefab, characterCreationParent).GetComponent<CharacterCreationPanel>();
        p.SetContent(characterSpawner.NewCharacter(), characterSpawner);
    }

    void AddInitialCharacters()
    {
        for(int i = 0; i < characterSpawner.characterDatas.Count; i++)
        {
            CharacterCreationPanel p = Instantiate(characterCreationPrefab, characterCreationParent).GetComponent<CharacterCreationPanel>();
            p.SetContent(characterSpawner.characterDatas[i], characterSpawner);
        }
    }
}
