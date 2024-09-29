using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public SaveGame activeSave = new SaveGame();

    public List<SaveGame> saveGames = new List<SaveGame>() { null, null, null, null, null, null, };

    //pulls from Resources
    public Dictionary<string, GameObject> gameObjectDictionary = new Dictionary<string, GameObject>();

    //pull from persistent data path
    public Dictionary<string, AICharacter> enemyDictionary = new Dictionary<string, AICharacter>();
    public Dictionary<string, Weapon> weaponDictionary = new Dictionary<string, Weapon>();
    public Dictionary<string, Spell> spellDictionary = new Dictionary<string, Spell>();
    public Dictionary<string, Item> itemDictionary = new Dictionary<string, Item>();

    [SerializeField] private GameObject playerInteriorPrefab;
    [SerializeField] private GameObject playerOverworldPrefab;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Image loadingScreenImage;

    [Space(20)]
    [Header("Game Music")]
    private AudioSource aS;
    [SerializeField] private AudioClip track1;

    private void Awake()
    {
        //SINGLETON
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        aS = GetComponent<AudioSource>();
        GetSaveFilesFromDataPath();
        GetEnemyCharacterInfoFromDataPath();

        var gameObjects = Resources.LoadAll<GameObject>("GameObjects");
        foreach (GameObject m in gameObjects)
        {
            //print(m.name);
            gameObjectDictionary.Add(m.name, m);
        }

        activeSave = saveGames[0];

        //load player if scene is not battle or title
        if (SceneManager.GetActiveScene().name == "OverworldScene")
        {
            LoadOverworldScene(new Vector3(169, 2, -56));
        }
        if (SceneManager.GetActiveScene().name == "InteriorScene")
        {
            LoadInteriorScene("CrossHallSubscene", Vector3.zero); //OrthoPrerenderedSubscene
        }

        loadingScreen.SetActive(false);
    }

    //INTERACTING WITH EXTERNAL FILES
    private void GetSaveFilesFromDataPath()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/saves");
        FileInfo[] saveSlots = dir.GetFiles("*.json");

        //If there aren't six, make six.
        if (saveGames.Count != 6)
        {
            Debug.LogError("Should be 6 save slots!");
        }

        for (int i = 0; i < saveSlots.Length; i++)
        {
            string sJ = File.ReadAllText(saveSlots[i].ToString());
            SaveGame convertedSave = JsonConvert.DeserializeObject<SaveGame>(sJ);
            saveGames[i] = convertedSave;
        }
    }
    private void GetEnemyCharacterInfoFromDataPath()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath + "/enemies");
        FileInfo[] enemyJSONs = dir.GetFiles("enemies.json");

        for (int i = 0; i < enemyJSONs.Length; i++)
        {
            string eJ = File.ReadAllText(enemyJSONs[i].ToString());
            Character[] convertedCharacters = JsonConvert.DeserializeObject<AICharacter[]>(eJ);
            foreach (AICharacter conChar in convertedCharacters)
            {
                //print(conChar.Name);
                enemyDictionary.Add(conChar.Name, conChar);
            }
        }
    }
    public string SaveGame(int slot)
    {
        activeSave.dateCreated = DateTime.Now.ToString();
        activeSave.x = 0;
        activeSave.y = 0;
        activeSave.z = 0;

        string saveJson = JsonConvert.SerializeObject(activeSave);

        //TODO, allow user to choose slot
        File.WriteAllText(Application.persistentDataPath + "/saves/save" + slot.ToString() + ".json", saveJson);
        return activeSave.dateCreated;
    }


    //MAIN MENU ACTIONS
    public void StartNewGame()
    {
        print("Start Game!");
        activeSave = GameConstants.STARTING_SAVE_FILE;
        LoadInteriorScene(activeSave.subSceneName, new Vector3(activeSave.x, activeSave.y, activeSave.z));
    }
    public void LoadSavedGame(int slot)
    {
        print("Load Game!");
        if (slot < 0 || slot > 5)
        {
            Debug.LogError("Load Button Error");
        }
        activeSave = saveGames[slot];
        LoadInteriorScene(activeSave.subSceneName, new Vector3(activeSave.x, activeSave.y, activeSave.z));
    }
    public void QuitToDesktop()
    {
        print("Quit Game!");
        Application.Quit();
    }


    //WORLD TRAVERSAL
    public void LoadOverworldScene(Vector3 overworldPosition)
    {
        StartCoroutine(LoadOverworldSceneRoutine(overworldPosition));
    }
    public void LoadInteriorScene(string subSceneName, Vector3 newScenePosition)
    {
        activeSave.subSceneName = subSceneName;
        StartCoroutine(LoadInteriorSceneRoutine(subSceneName, newScenePosition));
    }
    public void LoadScenePriorToBattle()
    {
        if (activeSave.onOverworldMap)
        {
            StartCoroutine(LoadOverworldSceneRoutine(new Vector3(activeSave.x, activeSave.y, activeSave.z)));
        }
        else if (activeSave.subSceneName != "")
        {
            StartCoroutine(LoadInteriorSceneRoutine(activeSave.subSceneName, new Vector3(activeSave.x, activeSave.y, activeSave.z)));
        }
        else
        {
            Debug.LogError("Player was not on Overworld, but also no subSceneName!");
        }
    }
    public void LoadBattleScene(Vector3 playerIntOrExtLocation)
    {
        //Save the position of the player before battle (INTERIOR OR EXTERIOR [could cause bugs?])
        activeSave.x = playerIntOrExtLocation.x;
        activeSave.y = playerIntOrExtLocation.y;
        activeSave.z = playerIntOrExtLocation.z;

        StartCoroutine(LoadBattleSceneRoutine());
    }
    public void LoadRewardsScene(BattleResults battleResults)
    {
        StartCoroutine(LoadRewardsSceneRoutine(battleResults));
    }
    public void LoadTitleScreenScene()
    {
        SceneManager.LoadScene("TitleScreenScene");
    }
    public void LoadGameOverScene()
    {
        SceneManager.LoadScene("GameOverScreenScene");
    }


    //ROUTINES
    private IEnumerator LoadOverworldSceneRoutine(Vector3 position)
    {
        loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync("OverworldScene");

        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            //print(progressValue);
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);
        operation.allowSceneActivation = true;
        yield return null;

        //overworld stuff
        Instantiate(playerOverworldPrefab, position, Quaternion.identity, null);
        activeSave.onOverworldMap = true;
        activeSave.subSceneName = "";
        loadingScreen.SetActive(false);
    }
    private IEnumerator LoadInteriorSceneRoutine(string subSceneName, Vector3 position)
    {
        StartCoroutine(FadeInOutLoadingScreen(true));

        //load frame scene
        AsyncOperation frameSceneLoad = SceneManager.LoadSceneAsync("InteriorScene");
        AsyncOperation subSceneLoad = SceneManager.LoadSceneAsync(subSceneName, LoadSceneMode.Additive);
        frameSceneLoad.allowSceneActivation = false;
        subSceneLoad.allowSceneActivation = false;

        while (frameSceneLoad.progress < 0.9f && subSceneLoad.progress < 0.9f)
        {
            float progressValue = Mathf.Clamp01(frameSceneLoad.progress / 0.9f);
            //print(progressValue);
            yield return null;
        }
        yield return new WaitForSeconds(GameConstants.LOAD_SCREEN_PAD_S);
        frameSceneLoad.allowSceneActivation = true;
        subSceneLoad.allowSceneActivation = true;
        yield return null;

        //interior stuff
        activeSave.subSceneName = subSceneName;
        activeSave.onOverworldMap = false;
        Instantiate(playerInteriorPrefab, position, Quaternion.identity, null);

        StartCoroutine(FadeInOutLoadingScreen(false));
    }
    private IEnumerator LoadBattleSceneRoutine()
    {
        StartCoroutine(FadeInOutLoadingScreen(true));        

        AsyncOperation operation = SceneManager.LoadSceneAsync("BattleScene");
        AsyncOperation subOperation = SceneManager.LoadSceneAsync("CavePitBSS", LoadSceneMode.Additive); //CavePitBSS //DesertBSS
        operation.allowSceneActivation = false;
        subOperation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            //print(progressValue);
            yield return null;
        }
        yield return new WaitForSeconds(GameConstants.LOAD_SCREEN_PAD_S);
        operation.allowSceneActivation = true;
        subOperation.allowSceneActivation = true;
        yield return null;

        StartCoroutine(FadeInOutLoadingScreen(false));
    }
    private IEnumerator LoadRewardsSceneRoutine(BattleResults battleResults)
    {
        loadingScreen.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync("RewardsScene");
        operation.allowSceneActivation = false;
        while (operation.progress < 0.9f)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            //print(progressValue);
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);
        operation.allowSceneActivation = true;
        yield return null; //one frame

        //battle is done by the battle manager
        loadingScreen.SetActive(false);

        //test test test MKL
        StopMusic();
        GameObject.Find("RewardsUI").GetComponent<sRewardsUI>().DisplayBattleResults(battleResults);
    }
    private IEnumerator FadeInOutLoadingScreen(bool fadeIn)
    {
        if (fadeIn)
        {
            loadingScreenImage.color = Color.clear;
            loadingScreen.SetActive(true);
            yield return null;
            float time = 0f;
            while (time < 1)
            {
                time += Time.deltaTime / GameConstants.LOAD_SCREEN_PAD_S;
                float clampedOpacity = Mathf.Clamp01(time);
                loadingScreenImage.color = new Color(1, 1, 1, clampedOpacity); //losing opacity
                yield return null;
            }
        }
        else
        {
            loadingScreenImage.color = Color.white;
            yield return null;
            float time = 1f;
            while (time > 0)
            {
                time -= Time.deltaTime / GameConstants.LOAD_SCREEN_PAD_S;
                float clampedOpacity = Mathf.Clamp01(time);
                loadingScreenImage.color = new Color(1, 1, 1, clampedOpacity); //losing opacity
                yield return null;
            }
            loadingScreen.SetActive(false);
        }
    }


    //RANDOM RESOURCES
    public Character GetRandomEnemy()
    {
        //print("Number of enemies in dictionary: " + enemyDictionary.Count.ToString());
        int randInt = UnityEngine.Random.Range(0, enemyDictionary.Count);

        //return a CLONE!
        return enemyDictionary.ElementAt(randInt).Value.ShallowCopy();
    }
    public int GetRandomGoldAmount()
    {
        return UnityEngine.Random.Range(10, 100);
    }
    public Item GetRandomItem()
    {
        return new Item("Molotov", 5, true);
    }
    public Weapon GetRandomWeapon()
    {
        return new Weapon() 
        { 
            Name = "9mm Handgun",
            Damage = 5,
        };
    }
    

    //MUSIC
    public void PlayMusicTrack(int trackNumber)
    {
        print("playing track: " + trackNumber.ToString());
        aS.volume = 0.25f;
        aS.PlayOneShot(track1);
    }
    public void StopMusic()
    {
        aS.Stop();
    }
}