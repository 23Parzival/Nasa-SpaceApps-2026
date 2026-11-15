using UnityEngine;

public class BaseComponent : MonoBehaviour{
    public string Name{get;set;}
    public string Tooltip{get;set;}
    public int Mass{get;set;}
    public int Volume{get;set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("HELPPPPPP");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
