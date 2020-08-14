using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using HighKings;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }
    public int len_x, len_y, len_z;
    public World world { get; protected set; }
    public MainGame game;

    static bool loadWorld = false;

    /// <summary>
    /// 
    /// </summary>
    static bool needsInitialize = true;

    // Start is called before the first frame update
    void OnEnable()
    {      
        if(Instance != null)
        {
            Debug.LogError("There are too many world controllers");
        }
        Instance = this;
        //if (MainGame.instance == null)
        //{
        //    game = new MainGame();
        //}
        world = new World(len_x, len_y,len_z);


        if (loadWorld)
        {
            CreateWorldFromSaveFile();

            loadWorld = false;
        } else
        {
            
        }
        Camera.main.transform.position = new Vector3(world.Width / 2, world.Length / 2, Camera.main.transform.position.z);
        needsInitialize = true;
    }

    public void OnStart()
    {
        MainGame.instance.world = world;
        
        world.SetUpWorld();

    }

    void Update()
    {
        //TODO Add speed controls
        if (needsInitialize)
        {
            //world.Initialize();
            needsInitialize = false;
        }
        world.Update(Time.deltaTime);
    }
/*
    public void CreateAllFurniturePrototypesFromFile(World w, string FileName)
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Data");
        filePath = System.IO.Path.Combine(filePath, "Furniture");
        filePath = System.IO.Path.Combine(filePath, FileName);

        XmlReader reader = XmlReader.Create(filePath);

        w.CreateFurniturePrototypes(reader);
    }
*/
    //public Tile GetTileAtWorldLoc(Vector3 coord) //TODO
    //{
    //    int x = Mathf.FloorToInt(coord.x);
    //    int y = Mathf.FloorToInt(coord.y);

    //    return world.GetTileAt(x, y, 0);
    //}

    public void NewWorld()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void MakeEmptyWorld()
    {
        world = new World(len_x,len_y,len_z);
        world.SetUpWorld();
        Camera.main.transform.position = new Vector3(world.Width / 2, world.Length / 2, Camera.main.transform.position.z);
        //world.TestChar();
    }

    public void SaveWorld()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(World));
        TextWriter writer = new StringWriter();
        serializer.Serialize(writer, world);
        writer.Close();

        PlayerPrefs.SetString("SaveGame00",writer.ToString());
    }

    public void SaveGame(string savename)
    {
        TextWriter writer = new StringWriter();
        XmlSerializer serializer = new XmlSerializer(typeof(MainGame));
        serializer.Serialize(writer, game);

        writer.Close();

        //Assets/StreamingAssets/Saves
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Data");
        filePath = System.IO.Path.Combine(filePath, "Saves");
        filePath = System.IO.Path.Combine(filePath, savename + ".xml");
        System.IO.File.WriteAllText(filePath, writer.ToString());
    }

    public void LoadGame(string savename)
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Data");
        filePath = System.IO.Path.Combine(filePath, "Saves");
        filePath = System.IO.Path.Combine(filePath, savename + ".xml");

        XElement root = XElement.Parse(System.IO.File.ReadAllText(filePath));

        game = new MainGame();
        //game.ReadXml(root);
    }

    public void LoadWorld()
    {
        loadWorld = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void CreateWorldFromSaveFile()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(World));
        TextReader reader = new StringReader(PlayerPrefs.GetString("SaveGame00"));
        world = new World();
        
        world = (World)serializer.Deserialize(reader);
        reader.Close();

    }

    /// <summary>
    /// Takes in the name of save game, without .xml then creates a world from the XML
    /// TODO: Make this whole process more robust (aka add some organization with different files containing information on different aspects of the save, characters
    ///       ect.)
    /// </summary>
    /// <param name="name"></param>
    //void CreateWorldFromSaveFile(string name)
    //{
    //    string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Saves");
    //    filePath = System.IO.Path.Combine(filePath, name + ".xml");

    //    game = new MainGame();

    //    XElement root = XElement.Parse(System.IO.File.ReadAllText(filePath));
    //    foreach (XElement reader in root.Descendants("Game"))
    //        game.ReadXml(reader);

    //}

    //public Tile GetWorldTileFromFloats(float x, float y, float z)
    //{
    //    return world.GetTileAt(Mathf.FloorToInt(x), Mathf.FloorToInt(y), Mathf.FloorToInt(z));
    //}
}

