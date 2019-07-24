using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSpawner : MonoBehaviour
{
    public GOAP_Character.CharacterData example1;
    public GOAP_Character.CharacterData example2;
    public GOAP_Character.CharacterData defaultCharacter;

    [HideInInspector]
    public List<GOAP_Character.CharacterData> characterDatas;

    public GameObject characterPrefab;

    float MaxSpawnDistance = 2f;

    private void Awake()
    {
        characterDatas = new List<GOAP_Character.CharacterData>();

        characterDatas.Add(example1);
        characterDatas.Add(example2);

        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(this.gameObject);
    }

    public GOAP_Character.CharacterData NewCharacter()
    {
        GOAP_Character.CharacterData data = new GOAP_Character.CharacterData(defaultCharacter);
        characterDatas.Add(data);
        return data;
    }

    public void SpawnPreparedCharacters()
    {
        for(int i = 0; i < characterDatas.Count; i++)
        {
            SpawnCharacter(characterDatas[i]);
        }
    }

    public void SpawnCharacter(GOAP_Character.CharacterData characterData)
    {
        Vector3 spawnPosition = InfoBlackBoard.instance.campFireLocation.GetPosition();
        spawnPosition.y = 0.611f;
        Vector2 randomOffset = Random.insideUnitCircle * MaxSpawnDistance;
        spawnPosition += new Vector3(randomOffset.x, 0, randomOffset.y);
        GOAP_Character character = Instantiate(characterPrefab, spawnPosition, Quaternion.identity).GetComponent<GOAP_Character>();
        character.SetCharacterData(characterData);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "Main")
        {
            SpawnPreparedCharacters();
        }
    }
}
