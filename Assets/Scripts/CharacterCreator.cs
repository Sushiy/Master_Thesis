using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreator : MonoBehaviour
{
    public Transform characterCreationParent;
    public GameObject characterCreationPrefab;

    private void Start()
    {
        AddInitialCharacters();
    }

    public void NewCharacter()
    {
        CharacterCreationPanel p =  Instantiate(characterCreationPrefab, characterCreationParent).GetComponent<CharacterCreationPanel>();
        p.SetContent(CharacterSpawner.instance.NewCharacter());
    }

    void AddInitialCharacters()
    {
        for(int i = 0; i < CharacterSpawner.instance.characterDatas.Count; i++)
        {
            CharacterCreationPanel p = Instantiate(characterCreationPrefab, characterCreationParent).GetComponent<CharacterCreationPanel>();
            p.SetContent(CharacterSpawner.instance.characterDatas[i]);
        }
    }
}
