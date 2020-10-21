using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighKings;

/// <summary>
/// The master controller for the game functionality
/// </summary>
public class GameController : MonoBehaviour
{
    public JsonParser json_parser;

    /// <summary>
    /// The MainGame class that is being used
    /// </summary>
    public MainGame game;

    /// <summary>
    /// A static reference to this object
    /// </summary>
    public static GameController _instance;

    /// <summary>
    /// A flag that the game needs to be loaded from a file
    /// </summary>
    bool loadGame;

    /// <summary>
    /// Currently undecided on its exact functionality, but the idea is that it is used by Lua code to access particular game functions
    /// </summary>

    public MouseController mouseController;
    public WorldController world_controller;
    public BuildModeController buildModeController;
    public PrototypeLoader prototype_loader;

    public SelectionStatInfoManager selection_info;

    //Sprite Managers
    public SpriteManager sprite_manager;

    public List<LoadPath> dataLoadPaths;
    public List<LoadPath> prototype_load_paths;
    public List<LoadPath> luaLoadPaths;

    // Start is called before the first frame update
    void OnEnable()
    {
        json_parser = JsonParser.instance ?? new JsonParser();

        string initPath = System.IO.Path.Combine(Application.streamingAssetsPath, "Data");
        initPath = System.IO.Path.Combine(initPath, "InitData");
        //dataLoadPaths = jsonParser.ParseString<List<LoadPath>>(System.IO.File.ReadAllText(System.IO.Path.Combine(initPath, "DataLoadPaths.JSON")));

        //luaLoadPaths = jsonParser.ParseString<List<LoadPath>>(System.IO.File.ReadAllText(System.IO.Path.Combine(initPath, "LuaLoadPaths.JSON")));
        prototype_load_paths = json_parser.ParseString<List<LoadPath>>(System.IO.File.ReadAllText(System.IO.Path.Combine(initPath, "PrototypeLoadPaths.JSON")));

        if (_instance != null)
        {
            Debug.LogError("There was a problem, probably. The main game already exists.");
        }
        _instance = this;

        if (MainGame.instance == null )
        {
            if (loadGame)
            {
                loadGame = false;
                game = new MainGame();
            }
            else
            {
                game = new MainGame();
            }
            //LoadAllLua();
            //DoDictionaryLoading();
        } else
        {
            game = MainGame.instance;
        }
    }

    //Calls on first frame
    private void Start()
    {
        string root_path = System.IO.Path.Combine(Application.streamingAssetsPath, "Data");
        foreach (LoadPath l in prototype_load_paths)
        {
            PrototypeLoader.instance.ReadFile(System.IO.File.ReadAllText(l.MakePathFromRoot(root_path)));
        }
        string system_path = System.IO.Path.Combine(root_path, "InitData", "Systems.JSON");
        game.SystemLoading();
        game.SetInitSystems(JsonParser.instance, System.IO.File.ReadAllText(system_path));
        system_path = System.IO.Path.Combine(root_path, "InitData", "InitArgs.JSON");
        Dictionary<string, object[]> system_args = JsonParser.instance.ParseString<Dictionary<string, object[]>>(System.IO.File.ReadAllText(system_path));
        game.CreateSystems(system_args);
        world_controller.OnStart();

        selection_info.SetDisplayQueue();
    }

    /// <summary>
    /// Update is called once per frame 
    /// </summary>
    void Update()
    {
        game.Update(Time.deltaTime);
    }

    public void DebugUnsuspendJob()
    {
    }

    void DoDictionaryLoading()
    {
        foreach (LoadPath l in dataLoadPaths)
        {
            string path = l.MakePathFromRoot(System.IO.Path.Combine(Application.streamingAssetsPath,"Data"));
            //game.SetUpAllPrototypes(System.IO.File.ReadAllText(path), l.Type, jsonParser);
        }
    }

    void DoLUAPaths()
    {
        foreach (LoadPath l in dataLoadPaths)
        {
            string path = l.MakePathFromRoot(System.IO.Path.Combine(Application.streamingAssetsPath, "LUA"));
            //game.SetUpAllPrototypes(System.IO.File.ReadAllText(path), l.Type, jsonParser);
        }
    }
}


/// <summary>
/// Called on the clicking of "Start Game", all game related actions to set up prototypes, ect should be put in here
/// </summary>
//public void StartGame()
//{
//    mouseController = FindObjectOfType<MouseController>();
//    mouseController.OnGameStart();
//    game.StartGame();
//    Debug.Log($"Started Game");
//}

/// <summary>
/// Creates a text file that is parsed into code for the job actions.
/// TODO: Make this loadable from alternative file paths (for mod support)
/// </summary>
//private void ImportJobLua()
//{
//    string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "LUA");
//    filePath = System.IO.Path.Combine(filePath, "Job");

//    List<string[]> dirpaths = new List<string[]>();
//    dirpaths.Add(System.IO.Directory.GetFiles(filePath));
//    string codeText = "";

//    foreach (string s in System.IO.Directory.EnumerateDirectories(filePath))
//    {
//        if (s.Contains(".meta") == false)
//            dirpaths.Add(System.IO.Directory.GetFiles(s));
//    }
//    foreach (string[] paths in dirpaths)
//    {
//        foreach (string s in paths)
//        {
//            if (s.Contains(".meta"))
//                continue;
//            //Add file's text to full lua string, should be able to take all possible mod job luas and put em in here
//            codeText += System.IO.File.ReadAllText(s) + "\n \n"; 
//        }
//    }
//    //Parse the full code into the interpreter
//    game.ImportCodeToActions(game.job_actions, codeText);
//}

//private void LoadAllLua()
//{
//    string jobLua = "";
//    string furnLua = "";
//    string rootPath = System.IO.Path.Combine(Application.streamingAssetsPath, "LUA");
//    foreach (LoadPath path in luaLoadPaths)
//    {
//        string tempPath = path.MakePathFromRoot(rootPath);
//        switch (path.Type)
//        {
//            case "Furniture":
//                furnLua += System.IO.File.ReadAllText(tempPath);
//                break;
//            case "Job":
//                jobLua += System.IO.File.ReadAllText(tempPath)  + "\n \n";
//                break;
//        }
//    }
//    //game.ImportCodeToActions(game.job_actions, jobLua);
//    //game.ImportCodeToActions(game.furn_actions, furnLua);
//}