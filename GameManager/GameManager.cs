using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode
{
    TITLE_SCREEN = 0,
    OVERWORLD = 1,
    SCENE = 2,
    BATTLE = 3,
    REWARDS = 4,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public SaveGame activeSave = new SaveGame();

    public List<SaveGame> saveGames = new List<SaveGame>() { null, null, null, null, null, null, };

    public Dictionary<string, GameObject> gameObjectDictionary = new Dictionary<string, GameObject>();
    public Dictionary<string, Character> enemyDictionary = new Dictionary<string, Character>();

    [SerializeField] private GameObject playerInteriorPrefab;
    [SerializeField] private GameObject playerOverworldPrefab;
    [SerializeField] private GameObject loadingScreen;
    public string lastInteriorSubSceneName = "";

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
        GetSaveFilesFromDataPath();
        GetEnemyCharacterInfoFromDataPath();

        var gameObjects = Resources.LoadAll<GameObject>("GameObjects");
        foreach (GameObject m in gameObjects)
        {
            //print(m.name);
            gameObjectDictionary.Add(m.name, m);
        }

        //load player if scene is not battle or title
        if (SceneManager.GetActiveScene().name == "OverworldScene")
        {
            activeSave = saveGames[0];
            LoadOverworldScene(new Vector3(0, 3, 0));
        }
        if (SceneManager.GetActiveScene().name == "InteriorScene")
        {
            activeSave = saveGames[0];
            LoadInteriorScene("HouseSubScene");
        }
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            activeSave = saveGames[0];
            LoadBattleScene();
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
            Character[] convertedCharacters = JsonConvert.DeserializeObject<Character[]>(eJ);
            foreach (Character conChar in convertedCharacters)
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
        activeSave = NewGameInformation.STARTING_SAVE_FILE;
        LoadInteriorScene(activeSave.subSceneName);
    }
    public void LoadSavedGame(int slot)
    {
        print("Load Game!");
        if (slot < 0 || slot > 5)
        {
            Debug.LogError("Load Button Error");
        }
        activeSave = saveGames[slot];
        LoadInteriorScene(activeSave.subSceneName);
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
    public void LoadInteriorScene(string subSceneName)
    {
        lastInteriorSubSceneName = subSceneName;
        StartCoroutine(LoadInteriorSceneRoutine(subSceneName));
    }
    public void LoadLastInteriorScene()
    {
        if (lastInteriorSubSceneName == "")
        {
            Debug.LogWarning("Last Interior Subscene empty string!");
            return;
        }
        StartCoroutine(LoadInteriorSceneRoutine(lastInteriorSubSceneName));
    }
    public void LoadBattleScene()
    {
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
    private IEnumerator LoadOverworldSceneRoutine(Vector3 overworldPosition)
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
        Instantiate(playerOverworldPrefab, overworldPosition, Quaternion.identity, null);
        activeSave.onOverworldMap = true;
        activeSave.subSceneName = "";
        loadingScreen.SetActive(false);
    }
    private IEnumerator LoadInteriorSceneRoutine(string subSceneName)
    {
        loadingScreen.SetActive(true);

        yield return new WaitForSeconds(0.5f);

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
        frameSceneLoad.allowSceneActivation = true;
        subSceneLoad.allowSceneActivation = true;
        yield return new WaitForSeconds(0.5f);

        //interior stuff
        Instantiate(playerInteriorPrefab, null);

        //mark the save correctly
        activeSave.onOverworldMap = false;
        activeSave.subSceneName = subSceneName;

        loadingScreen.SetActive(false);
    }
    private IEnumerator LoadBattleSceneRoutine()
    {
        loadingScreen.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        AsyncOperation operation = SceneManager.LoadSceneAsync("BattleScene");
        AsyncOperation subOperation = SceneManager.LoadSceneAsync("DesertBBS", LoadSceneMode.Additive); //BBS tagged
        operation.allowSceneActivation = false;
        subOperation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            //print(progressValue);
            yield return null;
        }
        operation.allowSceneActivation = true;
        subOperation.allowSceneActivation = true;
        yield return new WaitForSeconds(0.5f);

        loadingScreen.SetActive(false); 
    }
    private IEnumerator LoadRewardsSceneRoutine(BattleResults battleResults)
    {
        print("Loading Rewards!");
        print(battleResults.ToString());
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
    }

    //RANDOM RESOURCES LOADED FROM FILES
    public Character GetRandomEnemy()
    {
        //print("Number of enemies in dictionary: " + enemyDictionary.Count.ToString());
        int randInt = UnityEngine.Random.Range(0, enemyDictionary.Count);
        return enemyDictionary.ElementAt(randInt).Value;
    }
    public Item GetRandomItem()
    {
        return new Item("Molotov", 5, true);
    }
    public Weapon GetRandomWeapon()
    {
        return new Weapon("9mm Handgun", 5);
    }
}