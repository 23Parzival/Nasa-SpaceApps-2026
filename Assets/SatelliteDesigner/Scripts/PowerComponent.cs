using UnityEngine;
using UnityEngine.UI;

public class PowerComponent:BaseComponent{
    static int PowerProduction{get;set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static Button Get(string name,string tooltip,int mass,int volume,int powerProduction){
        Name=name;
        Tooltip=tooltip;
        Mass=mass;
        Volume=volume;
        PowerProduction=powerProduction;
        return new GameObject(name).AddComponent<Button>();
    }
}
