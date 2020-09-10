using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using System;
using HighKings;

namespace HighKings
{
    /// <summary>
    /// Manages a dictionary of all sprites used
    /// </summary>
    public class SpriteManager
    {
        public static SpriteManager current;

        public GameObjectManager go_manager;
        Dictionary<string, Sprite> sprite_map;
        public Dictionary<Entity, GameObject> entity_object_map;
        World world { get { return WorldController.Instance.world; } }

        ComponentSubscriber<RenderComponent> render_values;
        ComponentSubscriber<Position> positions;

        Action<List<Entity>> on_add_action;

        /// <summary>
        /// Default constructor for the sprite manager, TODO: Give it custom sprite paths
        /// </summary>
        public SpriteManager()
        {
            current = this;
            sprite_map = new Dictionary<string, Sprite>();

            string path = System.IO.Path.Combine(Application.streamingAssetsPath, "Data");
            path = System.IO.Path.Combine(path, "InitData");
            path = System.IO.Path.Combine(path, "SpriteDataPaths.JSON");
            List<LoadPath> spriteDataPaths = JsonParser.instance.ParseString<List<LoadPath>>(System.IO.File.ReadAllText(path));

            path = System.IO.Path.Combine(Application.streamingAssetsPath, "Sprites");
            path = System.IO.Path.Combine(path, "JSON");

            List<string> paths = new List<string>();
            foreach (LoadPath l in spriteDataPaths)
            {
                paths.Add(l.MakePathFromRoot(path));
            }

            path = System.IO.Path.Combine(Application.streamingAssetsPath, "Sprites");
            path = System.IO.Path.Combine(path, "Images");

            go_manager = GameObjectManager.instance;
            entity_object_map = go_manager.entity_objects;

            LoadSpritesFromJSON(path, paths, JsonParser.instance);

            on_add_action += (entities) =>
            {
                GameObjectManager.instance.AddObjectsForEntities(entities);
                GameObjectManager.instance.AddComponentToObjects<SpriteRenderer>(entities, SetRenderValues);
                positions.SubscribeAfterAction(entities, SetSpritePosition, "SetSpritePosition");
                render_values.SubscribeAfterAction(entities, SetSpriteValues, "SetSpriteValues");
            };

            MakeSpriteCheckers(MainGame.instance);
        }

        /// <summary>
        /// Makes all of the checkers used for creating and updating sprite data (the hope is that this doesn't get too ridiculous, idk dud)
        /// </summary>
        /// <param name="game"></param>
        public void MakeSpriteCheckers(MainGame game)
        {
            render_values = game.GetSubscriberSystem<RenderComponent>("RenderComponent");
            positions = game.GetSubscriberSystem<Position>("Position");

            render_values.RegisterOnAdded(on_add_action);
        }

        void SetSpriteValues(Entity e)
        {
            SetSpritePosition(e, e.GetComponent<Position>("Position").disp_pos);
            SetSortingLayer(e, e.GetComponent<RenderComponent>("RenderComponent").layer_name);
            SetEntitySprite(e, e.GetComponent<RenderComponent>("RenderComponent").sprite_name);
        }

        void SetSprite(Entity entity_id, string sprite_name)
        {
            SpriteRenderer entity_sr = entity_object_map[entity_id].GetComponent<SpriteRenderer>() == null ?
                entity_object_map[entity_id].AddComponent<SpriteRenderer>() : entity_object_map[entity_id].GetComponent<SpriteRenderer>();
            entity_sr.sprite = GetSprite(sprite_name);
        }

        void SetSortingLayer(Entity entity_id, string layer_name)
        {
            SpriteRenderer entity_sr = entity_object_map[entity_id].GetComponent<SpriteRenderer>() == null ?
                entity_object_map[entity_id].AddComponent<SpriteRenderer>() : entity_object_map[entity_id].GetComponent<SpriteRenderer>();
            entity_sr.sortingLayerName = layer_name;
        }

        void SetSpritePosition(Entity entity_id, Vector3 position)
        {
            if (entity_object_map.ContainsKey(entity_id))
            {
                entity_object_map[entity_id].transform.position = position;
            }
        }

        void SetSpritePosition(Entity e, Position p)
        {
            entity_object_map[e].transform.position = p.disp_pos;
        }

        void SetSpriteValues(Entity e, RenderComponent r)
        {
            SpriteRenderer entity_sr = entity_object_map[e].GetComponent<SpriteRenderer>();
            entity_sr.sprite = GetSprite(r.sprite_name);
            entity_sr.sortingLayerName = r.layer_name;
        }

        void SetRenderValues(SpriteRenderer renderer, Entity e)
        {
            RenderComponent r = e.GetComponent<RenderComponent>("RenderComponent");
            renderer.sprite = GetSprite(r.sprite_name);
            renderer.sortingLayerName = r.layer_name;
            renderer.gameObject.transform.position = e.GetComponent<Position>("Position").disp_pos;
        }

        void RemoveRenderedObject(Entity entity_id)
        {
            UnityEngine.Object.Destroy(entity_object_map[entity_id]);
        }

        void SetEntitySprite(Entity id, string sprite_id)
        {
            entity_object_map[id].GetComponent<SpriteRenderer>().sprite = GetSprite(sprite_id);
        }

        /// <summary>
        /// Creates the sprite dictionary for all sprites in the game. imagePath is the file path to the image folder
        /// dataPaths are the file paths to the json data for creating the sprite, parser is the json parser that will be parsing all json files 
        /// </summary>
        /// <param name="imagePaths"></param>
        /// <param name="dataPaths"></param>
        /// <param name="parser"></param>
        void LoadSpritesFromJSON(string imagePath, List<string> dataPaths, JsonParser parser)
        {
            //Make all of the structures for individual sprite data loading
            List<SpriteData> allSpriteData = new List<SpriteData>();
            foreach (string path in dataPaths)
            {
                //Debug.Log(path);
                allSpriteData.AddRange(parser.ParseString<List<SpriteData>>(System.IO.File.ReadAllText(path)));
            }

            //Get all of the sprite images
            Dictionary<string, Texture2D> images = new Dictionary<string, Texture2D>();
            string[] imagePaths = System.IO.Directory.GetFiles(imagePath);

            //Create images for each file in the image folder
            foreach (string s in imagePaths)
            {
                if (s.Contains(".meta")) { continue; }

                //Debug.Log(System.IO.Path.GetFileName(s));

                Texture2D tempTexture = new Texture2D(2, 2);
                if (tempTexture.LoadImage(System.IO.File.ReadAllBytes(s)) == false)
                {
                    Debug.LogError("Could not read image on: " + s);
                    continue;
                }

                images.Add(System.IO.Path.GetFileName(s), tempTexture);
            }

            //Make the sprite for each data struct and add it to the map
            foreach (SpriteData data in allSpriteData)
            {
                //Debug.Log(data.objName);
                sprite_map.Add(data.objName, CreateSpriteFromDataStruct(images[data.fileName], data));
            }

        }

        Sprite CreateSpriteFromDataStruct(Texture2D imageTexture, SpriteData data)
        {
            // All are in pixles
            // In percentage (Should be set to always have the bottom left corner be the pivot)
            //By default its 32 pixels per unit but if that ain't the case it should be declared in the JSON in pixels
            return Sprite.Create(imageTexture, new Rect(data.x1, data.y1, data.width, data.height)
                                             , new Vector2(0f, 0f), data.pixels > 0 ? data.pixels : 32);
        }

        public Sprite GetSprite(string spriteName)
        {
            if (sprite_map.ContainsKey(spriteName) == false)
            {
                Debug.LogError("No sprite with the name: " + spriteName);
                return null;
            }

            return sprite_map[spriteName];
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public struct SpriteData
    {
        [JsonProperty]
        public int x1;

        [JsonProperty]
        public int y1;

        [JsonProperty]
        public int width;

        [JsonProperty]
        public int height;

        [JsonProperty]
        public int pixels;

        [JsonProperty]
        public string objName;

        [JsonProperty]
        public string fileName;
    }

}



//Legacy Code

//void LoadSprites()
//{
//    string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Sprites");
//    filePath = System.IO.Path.Combine(filePath, "Images");

//    string xmlFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, "Sprites");
//    xmlFilePath = System.IO.Path.Combine(xmlFilePath, "Xml");

//    XElement root;
//    List<string[]> dirpaths = new List<string[]>();
//    dirpaths.Add(System.IO.Directory.GetFiles(xmlFilePath));
//    foreach (string s in System.IO.Directory.EnumerateDirectories(xmlFilePath))
//    {
//        if(s.Contains(".meta") == false)
//            dirpaths.Add(System.IO.Directory.GetFiles(s));
//    } 
//    foreach (string[] paths in dirpaths)
//    {
//        foreach (string s in paths)
//        {
//            if (s.Contains(".meta")) //FIXME this seems like it could lead to bugs
//                continue;
//            //Debug.Log(s);
//            root = XElement.Parse(System.IO.File.ReadAllText(s));
//            foreach (XElement r in root.Descendants("Sprites"))
//            {
//                string prefix = r.Attribute("TypeName").Value + "_";
//                string imagePath = filePath;

//                if (r.Attribute("ParentName") != null)
//                    imagePath = System.IO.Path.Combine(filePath, r.Attribute("ParentName").Value);

//                imagePath = System.IO.Path.Combine(imagePath, r.Attribute("FileName").Value);

//                byte[] imageBytes = System.IO.File.ReadAllBytes(imagePath);

//                Texture2D imagetexture = new Texture2D(2, 2);
//                if(imagetexture.LoadImage(imageBytes) == false)
//                {
//                    Debug.LogError("Image was not able to be loaded" + filePath);
//                }

//                foreach (XElement e in r.Descendants("Sprite"))
//                {
//                    string objName = prefix + e.Attribute("ObjectName").Value;
//                    spriteMap.Add(objName, CreateSpriteFromFile(imagetexture, e));
//                }
//            }
//        }
//    }
//}

//Sprite CreateSpriteFromFile(Texture2D imageTexture, XElement reader)
//{
//    // All are in pixles
//    int x1 = int.Parse((string)reader.Attribute("X1"));
//    int width = int.Parse((string)reader.Attribute("Width"));
//    int y1 = int.Parse((string)reader.Attribute("Y1"));
//    int height = int.Parse((string)reader.Attribute("Height")); // Unity weirdly has the Y axis pointed down (Just to remember what's up with that)
//    Rect spriteCoordinates = new Rect(x1, y1, width, height); 

//    //By default its 32 pixels per unit but if that ain't the case it should be declared in the xml in PixelsPerUnit
//    int pixels = 32;
//    if (reader.Attribute("PixelsPerUnit") != null)
//    {
//        pixels = int.Parse(reader.Attribute("PixelsPerUnit").Value);
//    }

//    Vector2 pivot = new Vector2(0f, 0f);        // In percentage (Should be set to always have the bottom left corner be the pivot)

//    Sprite newSprite = Sprite.Create(imageTexture, spriteCoordinates, pivot, pixels);
//    return newSprite;
//}

//game.RegisterOnRenderedEntityCreated((g, entity_id) =>
//{
//    CreateEntitySprite(entity_id, g.entity_string_types["sprite_type"].GetEntityTypeVal(entity_id),
//        g.entity_string_types["sprite_layer"].GetEntityTypeVal(entity_id), world.entity_positions.DisplacedVector(entity_id));
//});
//world.entity_positions.RegisterEntityChangeCB((all_pos, entity_id) =>
//{

//    SetSpritePosition(entity_id, all_pos.DisplacedVector(entity_id));
//});

////Currently assuming that everything with a position will be rendered
//world.entity_positions.RegisterOnPosCreated((all_pos, entity_id) =>
//{
//    CreateSpriteContainer(entity_id, all_pos.DisplacedVector(entity_id));
//});

////Make sure if the tile type changes then the sprite type changes too (There should be a better way to do this)
//game.tile_types_map.RegisterOnAdded((tp, entity_id) =>
//{
//    game.entity_string_types["sprite_type"].SetEntityType(entity_id, "tile_" + tp.GetEntityTypeVal(entity_id).string_id);
//    game.entity_string_types["sprite_layer"].SetEntityType(entity_id, "Ground");
//});

////Make sure if the tile type changes then the sprite type changes too
//game.tile_types_map.RegisterOnChanged((tp, entity_id) =>
//{
//    game.entity_string_types["sprite_type"].SetEntityType(entity_id, "tile_" + tp.GetEntityTypeVal(entity_id).string_id);
//});

//renders.RegisterOnRenderAdd((rend, entity_id) =>
//{
//    CreateEntitySprite(rend.GetComponent(entity_id));
//});

//renders.RegisterOnRenderChange((rend, entity_id) =>
//{
//    SetSpriteValues(rend.GetComponent(entity_id));
//});

//renders.RegisterOnRenderRemove((rend, entity_id) =>
//{
//    RemoveRenderedObject(entity_id);
//});

//renders.RegisterOnRenderAdd((rend, entity_id) =>
//{
//    CreateSpriteContainer(entity_id, rend.GetComponent(entity_id).position);
//});
//renders.RegisterOnRenderChange((rend, entity_id) =>
//{
//    SetEntitySprite(rend.GetComponent(entity_id));
//});

//PrototypeEntitySystem<string> sprite_types = new PrototypeEntitySystem<string>("sprite_type");
//foreach (string type_name in sprite_map.Keys)
//{
//    sprite_types.AddPrototype(type_name, type_name); //This is currently semi dumb but idk it should work
//}

////Anything in this set should for sure have a sprite that is rendered
//sprite_types.RegisterOnChanged((e, entity_id) => 
//{
//    SetSprite(entity_id, e.GetEntityTypeVal(entity_id));
//});

//game.entity_string_types.Add("sprite_type", sprite_types);

//PrototypeEntitySystem<string> sprite_layers = new PrototypeEntitySystem<string>("sprite_layer");
//foreach (string layer_name in layer_list)
//{
//    sprite_layers.AddPrototype(layer_name, layer_name);
//}

//sprite_layers.RegisterOnChanged((e, entity_id) =>
//{
//    SetSortingLayer(entity_id, e.GetEntityTypeVal(entity_id));
//});


//void CreateEntitySprite(Entity entity_id, RenderComponent comp)
//{
//    if (entity_object_map.ContainsKey(entity_id))
//    {
//        Debug.LogError("Tried to create an entity's sprite twice " + entity_id);
//    }
//    GameObject entity_go = new GameObject();

//    entity_go.name = entity_id.entity_string_id;

//    //Set position
//    entity_go.transform.SetParent(GameController._instance.transform, true);
//    entity_go.transform.position = comp.position;

//    //Set sprite
//    SpriteRenderer entity_sr = entity_go.AddComponent<SpriteRenderer>();
//    entity_sr.sprite = GetSprite(comp.sprite_name);
//    entity_sr.sortingLayerName = comp.layer_name;

//    entity_object_map.Add(entity_id, entity_go);
//}

//void CreateEntitySprite(Entity e)
//{
//    CreateEntitySprite(e, e.GetComponent<RenderComponent>("RenderComponent"));
//}

//// Start is called before the first frame update
//void OnEnable()
//{
//    JsonParser parser = JsonParser.instance ?? new JsonParser();
//    current = this;
//    sprite_map = new Dictionary<string, Sprite>();

//    string path = System.IO.Path.Combine(Application.streamingAssetsPath, "Data");
//    path = System.IO.Path.Combine(path, "InitData");
//    path = System.IO.Path.Combine(path, "SpriteDataPaths.JSON");
//    List<LoadPath> spriteDataPaths = parser.ParseString<List<LoadPath>>(System.IO.File.ReadAllText(path));

//    path = System.IO.Path.Combine(Application.streamingAssetsPath, "Sprites");
//    path = System.IO.Path.Combine(path, "JSON");

//    List<string> paths = new List<string>();
//    foreach (LoadPath l in spriteDataPaths)
//    {
//        paths.Add(l.MakePathFromRoot(path));
//    }

//    path = System.IO.Path.Combine(Application.streamingAssetsPath, "Sprites");
//    path = System.IO.Path.Combine(path, "Images");

//    entity_object_map = go_manager.entity_objects;

//    LoadSpritesFromJSON(path, paths, parser);
//}