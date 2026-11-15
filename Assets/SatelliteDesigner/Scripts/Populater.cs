using UnityEngine;
using UnityEngine.UIElements;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.IO;
using System;

public class Populater:MonoBehaviour{
    private bool firstTime=true;
    void Update(){
        if(firstTime){
            string componentsJson=File.ReadAllText("Assets/SatelliteDesigner/Json/satelliteComponents.json");

            JsonDocument componentsDOM;
            var componentList=(ScrollView)GameObject.FindWithTag("ComponentList");
            componentList.Add(new Label("test"));
            firstTime=false;
        }
    }
}
