using UnityEngine;

public class BaseComponent : MonoBehaviour{
    public static string Name{get;set;}
    public static string Tooltip{get;set;}
    public static int Mass{get;set;}
    public static int Volume{get;set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        Debug.Log("component intialized");
    }

    // Update is called once per frame
    void Update(){
        
    }
}
