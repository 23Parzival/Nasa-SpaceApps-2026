using UnityEngine;
using UnityEngine.UIElements;

public class ProtectionComponent:BaseComponent{
    static int Toughness{get;set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static Button Get(string name,string tooltip,int mass,int volume,int toughness){
        Name=name;
        Tooltip=tooltip;
        Mass=mass;
        Volume=volume;
        Toughness=toughness;
        Button button=new Button();
        button.text=name;
        return button;
    }
}
