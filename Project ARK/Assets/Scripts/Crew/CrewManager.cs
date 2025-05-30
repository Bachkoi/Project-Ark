using System.Collections;
using System.Collections.Generic;
using Backend;
using Sirenix.OdinInspector;
using UnityEngine;

public class CrewManager : MonoBehaviour
{
    public static CrewManager Instance { get; private set; }

    // Establish Necessary fields
    [BoxGroup("Crew Members")] public List<CrewMember> crewMembers = new List<CrewMember>();
    [BoxGroup("Crew Members")] public List<Enforcer> enforcers = new List<Enforcer>();
    [BoxGroup("Crew Members")] public List<Engineer> engineers = new List<Engineer>();
    [BoxGroup("Crew Members")] public List<Farmer> farmers = new List<Farmer>();
    [BoxGroup("Crew Members")] public List<Medic> medics = new List<Medic>();
    [BoxGroup("Crew Members")] public List<Navigator> navigators = new List<Navigator>();
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            // Pass the references across
            
            
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
