using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SpaceshipEnvironmentGenerator : EditorWindow
{
    private GameObject[] prefabs;
    private Material[] materials;
    private GameObject environmentParent;

    // Ship generation settings
    private enum ShipSection { Bridge, Corridor, Engineering, Quarters, Airlock }
    private Vector2Int shipSize = new Vector2Int(3, 5); // width, length in sections
    private float sectionSize = 8f;
    private float corridorWidth = 3f;
    private float wallHeight = 4f;

    [MenuItem("Tools/Generate Spaceship Environment")]
    public static void ShowWindow()
    {
        GetWindow<SpaceshipEnvironmentGenerator>("Spaceship Generator");
    }

    private void OnEnable()
    {
        LoadAssets();
    }

    private void LoadAssets()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        string[] materialGuids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });

        List<GameObject> relevantPrefabs = new List<GameObject>();
        List<Material> relevantMaterials = new List<Material>();

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null && IsRelevantAsset(path))
            {
                relevantPrefabs.Add(prefab);
            }
        }

        foreach (string guid in materialGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null && IsRelevantAsset(path))
            {
                relevantMaterials.Add(material);
            }
        }

        prefabs = relevantPrefabs.ToArray();
        materials = relevantMaterials.ToArray();
    }

    private void OnGUI()
    {
        GUILayout.Label("Spaceship Environment Generator", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        GUILayout.Label("Ship Configuration", EditorStyles.boldLabel);
        shipSize.x = EditorGUILayout.IntSlider("Ship Width (sections)", shipSize.x, 2, 5);
        shipSize.y = EditorGUILayout.IntSlider("Ship Length (sections)", shipSize.y, 3, 8);
        sectionSize = EditorGUILayout.Slider("Section Size", sectionSize, 6f, 12f);
        corridorWidth = EditorGUILayout.Slider("Corridor Width", corridorWidth, 2f, 4f);
        wallHeight = EditorGUILayout.Slider("Wall Height", wallHeight, 3f, 6f);

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Spaceship"))
        {
            GenerateSpaceship();
        }

        if (GUILayout.Button("Clear Spaceship"))
        {
            ClearEnvironment();
        }
    }

    private void GenerateSpaceship()
    {
        ClearEnvironment();
        environmentParent = new GameObject("Spaceship");
        Undo.RegisterCreatedObjectUndo(environmentParent, "Generate Spaceship");

        // Create main sections
        CreateMainStructure();
        CreateCorridors();
        AddSectionDetails();
        AddLighting();
    }

    private void CreateMainStructure()
    {
        // Create the bridge at the front
        CreateSection(new Vector3(0, 0, shipSize.y * sectionSize / 2f), ShipSection.Bridge);

        // Create engineering at the back
        CreateSection(new Vector3(0, 0, -shipSize.y * sectionSize / 2f), ShipSection.Engineering);

        // Create quarters and other rooms along the sides
        for (int z = -shipSize.y/2 + 1; z < shipSize.y/2; z++)
        {
            for (int x = -shipSize.x/2; x <= shipSize.x/2; x++)
            {
                if (Mathf.Abs(x) == shipSize.x/2) // Side rooms
                {
                    Vector3 position = new Vector3(x * sectionSize, 0, z * sectionSize);
                    CreateSection(position, ShipSection.Quarters);
                }
            }
        }

        // Create the main deck floor
        GameObject mainDeck = GameObject.CreatePrimitive(PrimitiveType.Plane);
        mainDeck.name = "MainDeck";
        mainDeck.transform.localScale = new Vector3(
            (shipSize.x * sectionSize) / 10f,
            1,
            (shipSize.y * sectionSize) / 10f
        );
        mainDeck.transform.SetParent(environmentParent.transform);
        
        Material floorMaterial = FindMaterialByKeyword("metal");
        if (floorMaterial != null)
        {
            mainDeck.GetComponent<Renderer>().material = floorMaterial;
        }
    }

    private void CreateSection(Vector3 position, ShipSection sectionType)
    {
        GameObject section = new GameObject(sectionType.ToString());
        section.transform.position = position;
        section.transform.SetParent(environmentParent.transform);

        CreateSectionWalls(section.transform, sectionType);
        AddSectionSpecificDetails(section.transform, sectionType);
    }

    private void CreateSectionWalls(Transform parent, ShipSection sectionType)
    {
        GameObject wallPrefab = FindPrefabByKeyword("wall");
        float halfSize = sectionSize / 2f;

        if (wallPrefab != null)
        {
            // Create walls using prefab
            for (int i = 0; i < 4; i++)
            {
                Vector3 position = Vector3.zero;
                Quaternion rotation = Quaternion.identity;

                switch (i)
                {
                    case 0: // Left
                        position = new Vector3(-halfSize, wallHeight/2f, 0);
                        rotation = Quaternion.Euler(0, 90, 0);
                        break;
                    case 1: // Right
                        position = new Vector3(halfSize, wallHeight/2f, 0);
                        rotation = Quaternion.Euler(0, -90, 0);
                        break;
                    case 2: // Front
                        position = new Vector3(0, wallHeight/2f, halfSize);
                        rotation = Quaternion.Euler(0, 180, 0);
                        break;
                    case 3: // Back
                        position = new Vector3(0, wallHeight/2f, -halfSize);
                        break;
                }

                GameObject wall = Instantiate(wallPrefab, parent.position + position, rotation, parent);
                wall.name = $"Wall_{i}";
                
                // Adjust scale based on wall prefab's original size
                float wallWidth = sectionSize;
                Vector3 originalSize = wallPrefab.GetComponent<MeshRenderer>()?.bounds.size ?? Vector3.one;
                wall.transform.localScale = new Vector3(
                    wallWidth / originalSize.x,
                    wallHeight / originalSize.y,
                    1
                );
            }
        }
        else
        {
            CreatePrimitiveWalls(parent, sectionSize, sectionSize, wallHeight);
        }
    }

    private void CreatePrimitiveWalls(Transform parent, float width, float length, float height)
    {
        // Create basic walls using cubes
        GameObject[] walls = new GameObject[4];
        string[] wallNames = { "LeftWall", "RightWall", "FrontWall", "BackWall" };
        Vector3[] positions = new Vector3[]
        {
            new Vector3(-width/2f, height/2f, 0f),
            new Vector3(width/2f, height/2f, 0f),
            new Vector3(0f, height/2f, length/2f),
            new Vector3(0f, height/2f, -length/2f)
        };
        Vector3[] scales = new Vector3[]
        {
            new Vector3(0.1f, height, length),
            new Vector3(0.1f, height, length),
            new Vector3(width, height, 0.1f),
            new Vector3(width, height, 0.1f)
        };

        for (int i = 0; i < 4; i++)
        {
            walls[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            walls[i].name = wallNames[i];
            walls[i].transform.position = positions[i];
            walls[i].transform.localScale = scales[i];
            walls[i].transform.SetParent(parent);
        }
    }

    private void CreateCorridorSegment(Vector3 start, Vector3 end, Transform parent)
    {
        GameObject corridor = new GameObject("Corridor Segment");
        corridor.transform.SetParent(parent);

        Vector3 direction = (end - start).normalized;
        float length = Vector3.Distance(start, end);
        Vector3 center = (start + end) / 2f;

        // Create floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "CorridorFloor";
        floor.transform.position = center + Vector3.up * 0.1f;
        floor.transform.localScale = new Vector3(corridorWidth, 0.2f, length);
        floor.transform.rotation = Quaternion.LookRotation(direction);
        floor.transform.SetParent(corridor.transform);

        // Create walls
        GameObject wallPrefab = FindPrefabByKeyword("corridor");
        if (wallPrefab != null)
        {
            Vector3 rightOffset = Vector3.Cross(Vector3.up, direction) * (corridorWidth / 2f);

            GameObject leftWall = Instantiate(wallPrefab, 
                center + Vector3.up * (wallHeight/2f) - rightOffset, 
                Quaternion.LookRotation(direction));
            leftWall.name = "LeftWall";

            GameObject rightWall = Instantiate(wallPrefab, 
                center + Vector3.up * (wallHeight/2f) + rightOffset, 
                Quaternion.LookRotation(direction));
            rightWall.name = "RightWall";

            leftWall.transform.SetParent(corridor.transform);
            rightWall.transform.SetParent(corridor.transform);

            // Adjust wall scale based on corridor length
            Vector3 originalSize = wallPrefab.GetComponent<MeshRenderer>()?.bounds.size ?? Vector3.one;
            Vector3 newScale = new Vector3(
                length / originalSize.z,
                wallHeight / originalSize.y,
                1
            );
            leftWall.transform.localScale = newScale;
            rightWall.transform.localScale = newScale;
        }
    }

    private void CreateCorridors()
    {
        GameObject corridorParent = new GameObject("Corridors");
        corridorParent.transform.SetParent(environmentParent.transform);

        // Main corridor
        float corridorLength = shipSize.y * sectionSize;
        CreateCorridorSegment(
            new Vector3(0, 0, -corridorLength/2f),
            new Vector3(0, 0, corridorLength/2f),
            corridorParent.transform
        );

        // Side corridors
        for (int z = -shipSize.y/2 + 1; z < shipSize.y/2; z++)
        {
            float zPos = z * sectionSize;
            CreateCorridorSegment(
                new Vector3(-shipSize.x * sectionSize/2f, 0, zPos),
                new Vector3(shipSize.x * sectionSize/2f, 0, zPos),
                corridorParent.transform
            );
        }
    }

    private void AddSectionDetails()
    {
        // Implement section details based on section type
        foreach (Transform section in environmentParent.transform)
        {
            if (section.name == "Bridge")
                AddBridgeDetails(section);
            else if (section.name == "Engineering")
                AddEngineeringDetails(section);
            else if (section.name == "Quarters")
                AddQuartersDetails(section);
        }
    }

    // ... [Previous helper methods remain unchanged]

    private void AddLighting()
    {
        GameObject lightingParent = new GameObject("Lighting");
        lightingParent.transform.SetParent(environmentParent.transform);

        // Main lights
        for (int x = -shipSize.x; x <= shipSize.x; x++)
        {
            for (int z = -shipSize.y; z <= shipSize.y; z++)
            {
                GameObject lightObj = new GameObject($"Light_{x}_{z}");
                Light light = lightObj.AddComponent<Light>();
                light.type = LightType.Point;
                light.intensity = 1f;
                light.range = sectionSize;
                light.color = new Color(0.9f, 0.95f, 1f);

                lightObj.transform.position = new Vector3(
                    x * sectionSize/2f,
                    wallHeight - 0.5f,
                    z * sectionSize/2f
                );
                lightObj.transform.SetParent(lightingParent.transform);
            }
        }

        // Ambient light
        GameObject ambient = new GameObject("Ambient");
        Light ambientLight = ambient.AddComponent<Light>();
        ambientLight.type = LightType.Directional;
        ambientLight.intensity = 0.5f;
        ambient.transform.SetParent(lightingParent.transform);
        ambient.transform.rotation = Quaternion.Euler(50, -30, 0);
    }

    private void ClearEnvironment()
    {
        if (environmentParent != null)
        {
            Undo.RecordObject(environmentParent, "Clear Spaceship");
            DestroyImmediate(environmentParent);
        }
    }

    // ... [Rest of the helper methods remain unchanged]
    
// Add these methods to your SpaceshipEnvironmentGenerator class

private void AddBridgeDetails(Transform bridge)
{
    // Find and place console prefabs
    GameObject[] consoles = FindPrefabsByKeywords(new[] { "console", "control", "panel" });
    
    if (consoles.Length > 0)
    {
        // Main console at the front
        Vector3 mainConsolePosition = bridge.position + Vector3.forward * (sectionSize * 0.3f);
        GameObject mainConsole = Instantiate(consoles[0], mainConsolePosition, Quaternion.identity, bridge);
        mainConsole.name = "MainConsole";

        // Side consoles
        if (consoles.Length > 1)
        {
            float consoleSpacing = sectionSize * 0.25f;
            for (int i = 0; i < 2; i++)
            {
                Vector3 sidePosition = bridge.position + 
                    (i == 0 ? Vector3.left : Vector3.right) * consoleSpacing +
                    Vector3.forward * (sectionSize * 0.2f);
                
                GameObject sideConsole = Instantiate(consoles[Random.Range(0, consoles.Length)], 
                    sidePosition, 
                    Quaternion.Euler(0, i == 0 ? -45 : 45, 0), 
                    bridge);
                sideConsole.name = $"SideConsole_{i}";
            }
        }
    }
}

private void AddEngineeringDetails(Transform engineering)
{
    // Find and place engineering-related prefabs
    GameObject[] engineProps = FindPrefabsByKeywords(new[] { "engine", "generator", "power", "reactor" });
    
    if (engineProps.Length > 0)
    {
        // Central reactor/engine
        Vector3 centerPos = engineering.position;
        GameObject mainEngine = Instantiate(engineProps[0], centerPos, Quaternion.identity, engineering);
        mainEngine.name = "MainReactor";

        // Surrounding equipment
        int numSupportEquipment = Mathf.Min(engineProps.Length - 1, 4);
        for (int i = 0; i < numSupportEquipment; i++)
        {
            float angle = i * (360f / numSupportEquipment);
            Vector3 position = centerPos + (Quaternion.Euler(0, angle, 0) * Vector3.forward * (sectionSize * 0.3f));
            GameObject equipment = Instantiate(engineProps[(i % engineProps.Length) + 1], 
                position, 
                Quaternion.Euler(0, angle + 180, 0), 
                engineering);
            equipment.name = $"SupportEquipment_{i}";
        }
    }
}

private void AddQuartersDetails(Transform quarters)
{
    // Find and place quarters-related prefabs
    GameObject[] quarterProps = FindPrefabsByKeywords(new[] { "bed", "locker", "chair", "table", "furniture" });
    
    if (quarterProps.Length > 0)
    {
        // Calculate a grid for furniture placement
        int gridSize = 2;
        float cellSize = sectionSize / gridSize;
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                if (Random.value > 0.3f) // 70% chance to place furniture in each cell
                {
                    Vector3 position = quarters.position + 
                        new Vector3(
                            (x - gridSize/2f + 0.5f) * cellSize,
                            0,
                            (z - gridSize/2f + 0.5f) * cellSize
                        );
                    
                    GameObject prop = Instantiate(
                        quarterProps[Random.Range(0, quarterProps.Length)],
                        position,
                        Quaternion.Euler(0, Random.Range(0, 360), 0),
                        quarters
                    );
                    prop.name = $"QuartersProp_{x}_{z}";
                }
            }
        }
    }
}

private GameObject[] FindPrefabsByKeywords(string[] keywords)
{
    List<GameObject> found = new List<GameObject>();
    foreach (GameObject prefab in prefabs)
    {
        if (prefab == null) continue;
        
        string prefabName = prefab.name.ToLower();
        foreach (string keyword in keywords)
        {
            if (prefabName.Contains(keyword.ToLower()))
            {
                found.Add(prefab);
                break;
            }
        }
    }
    return found.ToArray();
}

private bool IsRelevantAsset(string path)
{
    string[] relevantKeywords = new string[] 
    { 
        "space", "ship", "sci", "fi", "metal", "door", "wall", "floor", 
        "ceiling", "panel", "corridor", "room", "tech", "console", "engine",
        "bed", "chair", "table", "power", "generator", "locker", "screen",
        "reactor", "furniture"
    };

    path = path.ToLower();
    foreach (string keyword in relevantKeywords)
    {
        if (path.Contains(keyword))
            return true;
    }
    return false;
}

private GameObject FindPrefabByKeyword(string keyword)
{
    if (prefabs == null) return null;
    
    foreach (GameObject prefab in prefabs)
    {
        if (prefab != null && prefab.name.ToLower().Contains(keyword.ToLower()))
        {
            return prefab;
        }
    }
    return null;
}

private Material FindMaterialByKeyword(string keyword)
{
    if (materials == null) return null;
    
    foreach (Material material in materials)
    {
        if (material != null && material.name.ToLower().Contains(keyword.ToLower()))
        {
            return material;
        }
    }
    return null;
}

private void AddSectionSpecificDetails(Transform section, ShipSection sectionType)
{
    switch (sectionType)
    {
        case ShipSection.Bridge:
            AddBridgeDetails(section);
            break;
        case ShipSection.Engineering:
            AddEngineeringDetails(section);
            break;
        case ShipSection.Quarters:
            AddQuartersDetails(section);
            break;
        case ShipSection.Corridor:
            // Corridors are handled separately
            break;
        case ShipSection.Airlock:
            AddAirlockDetails(section);
            break;
    }
}

private void AddAirlockDetails(Transform airlock)
{
    // Find and place airlock-specific prefabs
    GameObject airlockDoor = FindPrefabByKeyword("airlock");
    if (airlockDoor != null)
    {
        GameObject door = Instantiate(airlockDoor, 
            airlock.position, 
            Quaternion.identity, 
            airlock);
        door.name = "AirlockDoor";
    }
    
    // Add control panel near the airlock
    GameObject controlPanel = FindPrefabByKeyword("panel");
    if (controlPanel != null)
    {
        Vector3 panelPosition = airlock.position + Vector3.right * (sectionSize * 0.3f);
        GameObject panel = Instantiate(controlPanel, 
            panelPosition, 
            Quaternion.Euler(0, -90, 0), 
            airlock);
        panel.name = "AirlockPanel";
    }
}
}