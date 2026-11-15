using UnityEngine;
using UnityEngine.UIElements;

public class MeasuringInstrumentComponent:BaseComponent{
    static string Measures{get;set;}
    static int PowerConsumption{get;set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
    }

    // Update is called once per frame
    void Update(){
        
    }
    public static Button Get(string name,string tooltip,int mass,int volume,string measures,int powerConsumption){
        Name=name;
        Tooltip=tooltip;
        Mass=mass;
        Volume=volume;
        Measures=measures;
        PowerConsumption=powerConsumption;
        Button button=new Button();
        button.text=name;
        return button;
    }
}
