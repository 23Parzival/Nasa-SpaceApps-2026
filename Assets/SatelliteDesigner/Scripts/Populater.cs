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

            JsonElement jsonRoot=JsonDocument.Parse(componentsJson).RootElement;
            JsonElement measuringInstrumentsElement=jsonRoot.GetProperty("measuringInstruments"),
                        powersElement=jsonRoot.GetProperty("power"),
                        protectionsElement=jsonRoot.GetProperty("protection");

            ScrollView componentList=new ScrollView(ScrollViewMode.Vertical);
            componentList.style.width=425;
            componentList.style.height=650;
            foreach (JsonElement measuringInstrument in measuringInstrumentsElement.EnumerateArray()){
                JsonElement name=measuringInstrument.GetProperty("name"),
                            measures=measuringInstrument.GetProperty("measures"),
                            mass=measuringInstrument.GetProperty("mass"),
                            tooltip=measuringInstrument.GetProperty("tooltip"),
                            volume=measuringInstrument.GetProperty("volume"),
                            powerConsumption=measuringInstrument.GetProperty("powerConsumption");
                componentList.Add(MeasuringInstrumentComponent.Get(name.GetString(),tooltip.GetString(),mass.GetInt16(),volume.GetInt16(),measures.GetString(),powerConsumption.GetInt16()));
            }

            foreach (JsonElement powerElement in powersElement.EnumerateArray()){
                JsonElement name=powerElement.GetProperty("name"),
                            tooltip=powerElement.GetProperty("tooltip"),
                            mass=powerElement.GetProperty("mass"),
                            volume=powerElement.GetProperty("volume"),
                            powerProduction=powerElement.GetProperty("powerProduction");
                componentList.Add(PowerComponent.Get(name.GetString(),tooltip.GetString(),mass.GetInt16(),volume.GetInt16(),powerProduction.GetInt16()));
            }
            foreach (JsonElement protectionElement in protectionsElement.EnumerateArray()){
                JsonElement name=protectionElement.GetProperty("name"),
                            tooltip=protectionElement.GetProperty("tooltip"),
                            mass=protectionElement.GetProperty("mass"),
                            volume=protectionElement.GetProperty("volume"),
                            toughness=protectionElement.GetProperty("toughness");
                componentList.Add(ProtectionComponent.Get(name.GetString(),tooltip.GetString(),mass.GetInt16(),volume.GetInt16(),toughness.GetInt16()));
            }

            componentList.transform.parent=GameObject.FindWithTag("ComponentList").transform;
            firstTime=false;
        }
    }
}
