using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace HighKings
{
    /// <summary>
    /// The MainGame is responsible for all information that is passed between systems in the game
    /// it has all the prototype dictionaries, all action interpreters, ect and should be one of
    /// the main mediators of data overall between systems.
    /// </summary>
    public class MainGame
    {
        public static uint character_num = 0;

        /// <summary>
        /// This is only there so we can start and initiallize game data that can't immediately done
        /// before many other systems are initiallized
        /// </summary>
        public static bool game_started = false;

        //TODO Make Prototype dictionaries more robust

        /// <summary>
        /// The only instance of main game, which mediates all in game interactions
        /// </summary>
        public static MainGame instance; //IDK if this is a good Idea

        public EntityManager entity_manager;
        public Characters characters;
        MathFunctions math;
        PrototypeLoader prototype_loader;

        /// <summary>
        /// Current World that player controlled characters are in. It is the mediator for all area based systems.
        /// TODO: Make the game able to handle chuck based things so characters can be in multiple world tiles (Also possibly change the name)
        /// </summary>
        public World world;

        Action<string> on_rendered_entity_created;

        public MainGame()
        {
            game_started = false;

            if (instance == null) { instance = this; }
            math = new MathFunctions();

            //try to remove unity stuff in the future
            UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

            entity_manager = new EntityManager();
            prototype_loader = PrototypeLoader.instance ?? new PrototypeLoader(JsonParser.instance);
            characters = new Characters();
        }

        //All startup tasks should be done in some form here
        public void StartGame()
        {
            Dictionary<string, Entity> cplayer =  entity_manager.CreateEntities("character", new string[1] { "character_the_player" });
            Dictionary<Entity, Dictionary<string, object[]>> entities = new Dictionary<Entity, Dictionary<string, object[]>>
            { { cplayer.Values.ToArray()[0], new Dictionary<string, object[]>{ {"Position", new object[3] {51,51,0 } } } } };
            prototype_loader.AttachPrototype("character", entities);
            game_started = true;
        }


        public void Update(float dt)
        {

        }

        /// <summary>
        /// Any Action intepreter class should have an import code thing and have the IAction Interface, this is so outside 
        /// code can reference the inside of the data class and the data class will not have to see the outside (so stuff like
        /// if we want to change interpreters or engines would be handled not in the data class)
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="rawCode"></param>
        /// <param name="fileName"></param>
        public void ImportCodeToActions(IActionInterpreter interpreter, string rawCode)
        {
            interpreter.ImportScript(rawCode);
        }

        //////////////////////////////////////////////////////////////////////////
        ///
        /// JSON PROTOTYPE PARSERS
        ///
        //////////////////////////////////////////////////////////////////////////

        public void SetEntityPrototypes(JsonParser parser, string json_file)
        {
            prototype_loader = PrototypeLoader.instance ?? new PrototypeLoader(parser);
        }

        /*
         * Callback Registers
         */

        public void RegisterOnRenderedEntityCreated(Action<MainGame, string> to_register)
        {
        }

        /*
         * Callback external callers
         */

        public void CallOnRenderedCreated(string entity_id)
        {
        }
    }
}



/////<summary>
/////Sets up all prototypes, it is currently just a big switch and there is probably a better way to do this but idk this works fine for now
/////</summary>
/////<param name="strings"></param>
/////<param name="parser"></param>
//public void SetUpAllPrototypes(string jsonString, string type, JsonParser parser)
//{
//    switch (type)
//    {
//        case "TileTypes":
//            SetUpTileTypes(jsonString, parser);
//            //Debug.Log("Set up tile types");
//            //Debug.Log(TileTypes.Keys.ToArray()[0]);
//            //tile_prototypes = new ImmutableEntityTypeSystem<TilePrototype>("tile_type");
//            //foreach (KeyValuePair<string,TileType> ttype in tile_types)
//            //{
//            //    tile_prototypes.AddPrototype(ttype.Key, new TilePrototype(ttype.Value));
//            //}
//            break;
//        case "FurniturePrototypes":
//            SetUpFurniturePrototypes(jsonString, parser);
//            //Debug.Log("Set up furnitures");
//            break;
//        case "BodyPartPrototypes":
//            SetUpBodyPartPrototypes(jsonString, parser);
//            //Debug.Log("Set up Body Parts");
//            break;
//        case "JobPrototypes":
//            //Debug.Log("Set up jobs");
//            SetUpJobPrototypes(jsonString, parser);
//            break;
//    }
//}

//public void SetUpJobPrototypes(string JsonString, JsonParser parser)
//{
//    parser.AppendDictionary(JsonString, job_prototypes);
//}

//public void SetUpTileTypes(string json_string, JsonParser parser)
//{
//    //tile_types_map = parser.ParseString<PrototypeEntitySystem<TileType>>(json_string);
//    //parser.AppendDictionary(json_string, tile_types);
//}

//public void SetUpFurniturePrototypes(string JsonString, JsonParser parser)
//{
//    parser.AppendDictionary(JsonString, furniture_dictionary);
//}

//public void SetUpBodyPartPrototypes(string JsonString, JsonParser parser)
//{
//    parser.AppendDictionary(JsonString, body_part_prototypes);
//}

//public void SetUpInventoryItemPrototypes(string JsonString, JsonParser parser)
//{
//    parser.AppendDictionary(JsonString, inventory_item_prototypes);
//}


//private void CreateInventoryItemPrototypesFromXML()
//{
//    InventoryItemPrototypes = new Dictionary<string, InventoryItem>();
//    string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Data");
//    filePath = System.IO.Path.Combine(filePath, "InventoryItems");

//    List<string[]> dirpaths = new List<string[]>();
//    dirpaths.Add(System.IO.Directory.GetFiles(filePath));
//    XElement root;

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

//            root = XElement.Parse(System.IO.File.ReadAllText(s));
//            CreateInventoryItemPrototypesFromXML(root);
//        }
//    }
//}


//public TileType GetTileType(string s)
//{
//    if (tile_types_map.CheckTypeExists(s) == false)
//    {
//        Debug.LogError("Tile type not found: " + s);
//        return tile_types_map.GetDefaultVal();
//    }
//    return  tile_types_map.
//}

//public InventoryItem GetInventoryItemPrototype(string s)
//{
//    if (inventory_item_prototypes.ContainsKey(s) == false)
//    {
//        Debug.LogError("InventoryPrototype Not Found: " + s);
//        return null; //TODO: Maybe have an "ERROR" Type?
//    }
//    return inventory_item_prototypes[s];
//}

//public void SetUpDescriptions()
//{
//    string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Data");
//    filePath = System.IO.Path.Combine(filePath, "DescriptionText");

//    List<string[]> dirpaths = new List<string[]>();
//    dirpaths.Add(System.IO.Directory.GetFiles(filePath));
//    XElement root;
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

//            root = XElement.Parse(System.IO.File.ReadAllText(s));
//            if (descriptions == null)
//                SetUpDescriptions(root);
//            else
//            {
//                AppendDescriptions(root);
//            }
//        }
//    }
//}

//public void SetUpDescriptions(XElement root)
//{
//    descriptions = new DescriptionSystem(root);
//}

//void AppendDescriptions(XElement root)
//{
//    descriptions.AppendToDescsDict(root);
//}

//public void WriteXml(XmlWriter writer)
//{
//    writer.WriteStartElement("Game");
//    writer.WriteAttributeString("PlayerName", player_char.entity_string_id);
//    writer.WriteStartElement("CurrentWorld");
//    world.WriteXml(writer);
//    writer.WriteEndElement();
//    writer.WriteEndElement();
//}

//public void ReadXml(XElement reader)
//{
//    world = new World();
//    world.ReadXml(reader);
//    player_char = world.character_list.char_list[reader.Attribute("PlayerName").Value];
//}

//public BodyPart InstantiateBodyPart(string partName, Dictionary<string,string> descVals)
//{
//    return body_part_prototypes[partName].InstantiateType(descVals);
//}

///// <summary>
///// Depreciated, Use the XML
///// </summary>
///// <param name="nameid"></param>
///// <param name="movementCost"></param>
///// <param name="objheight"></param>
///// <param name="objwidth"></param>
///// <param name="links"></param>
///// <param name="roomDiv"></param>
//void GenFurniturePrototype(string nameid, float movementCost, int objheight, int objwidth, bool links, bool roomDiv)
//{
//    if (furniture_dictionary == null)
//    {
//        Debug.LogError("GenInstalledObjectPrototype tried to instantiate prototype before map");
//        return;
//    }

//    Furniture obj = Furniture.CreatePrototype(
//        nameid,
//        movementCost, //Impassable is 0
//        objheight,  //TODO, set to 1
//        objwidth,  //TODO, set to 1
//        links,
//        roomDiv);

//    furniture_dictionary.Add(nameid, obj);
//}

//public List<string> FurniturePrototypes
//{
//    get
//    {
//        return furniture_dictionary.Keys.ToList();
//    }
//}

//public string GetFurnitureInGameName(string key)
//{
//    return furniture_dictionary[key].display_name;
//}

//public JobPrototype GetJobPrototype(string id)
//{
//    if (job_prototypes.ContainsKey(id))
//        return job_prototypes[id];
//    else
//    {
//        Debug.LogError("Could not find job prototype: " + id);
//        return null;
//    }
//}