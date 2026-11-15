using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>One CubeSat component, as the builder sees it.</summary>
[Serializable]
public class CubesatPart
{
    public string name;
    public float massKg;
    public float volumeU;          // volume in CubeSat units (1U = 10 cm cube)
    public float powerProducedW;   // e.g. solar panels
    public float powerConsumedW;   // e.g. payload, radio, computer
}

/// <summary>
/// Keeps the running totals for the design and writes them to four labels.
/// Call AddPart / RemovePart when the player adds or removes a component,
/// and hook the reset button's OnClick to ResetStats.
/// </summary>
public class MissionStats : MonoBehaviour
{
    [Header("Labels")]
    public TMP_Text massLabel;
    public TMP_Text volumeLabel;
    public TMP_Text energyProducedLabel;
    public TMP_Text energyConsumedLabel;

    [Header("Design limits")]
    public float totalVolumeU = 3f;     // size of the CubeSat being built (3U by default)

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color warningColor = new Color(1f, 0.4f, 0.4f);

    readonly List<CubesatPart> parts = new List<CubesatPart>();

    public IReadOnlyList<CubesatPart> Parts => parts;
    public float TotalMassKg { get; private set; }
    public float UsedVolumeU { get; private set; }
    public float RemainingVolumeU => totalVolumeU - UsedVolumeU;
    public float PowerProducedW { get; private set; }
    public float PowerConsumedW { get; private set; }

    void Start()
    {
        Refresh();
    }

    /// <summary>True if the part still fits in the remaining volume.</summary>
    public bool CanFit(CubesatPart part)
    {
        return part.volumeU <= RemainingVolumeU + 0.0001f;
    }

    public void AddPart(CubesatPart part)
    {
        if (part == null) return;
        parts.Add(part);
        Refresh();
    }

    /// <summary>Pass the same CubesatPart instance that was added.</summary>
    public void RemovePart(CubesatPart part)
    {
        if (part == null) return;
        parts.Remove(part);
        Refresh();
    }

    /// <summary>Hook this to the Reset button's OnClick.</summary>
    public void ResetStats()
    {
        parts.Clear();
        Refresh();
    }

    void Refresh()
    {
        // Recompute from the list each time so repeated add/remove never drifts.
        TotalMassKg = 0f;
        UsedVolumeU = 0f;
        PowerProducedW = 0f;
        PowerConsumedW = 0f;

        foreach (var p in parts)
        {
            TotalMassKg += p.massKg;
            UsedVolumeU += p.volumeU;
            PowerProducedW += p.powerProducedW;
            PowerConsumedW += p.powerConsumedW;
        }

        SetText(massLabel, $"Mass: {TotalMassKg:0.00} kg", normalColor);
        SetText(volumeLabel, $"Volume left: {RemainingVolumeU:0.0} U",
            RemainingVolumeU < 0f ? warningColor : normalColor);
        SetText(energyProducedLabel, $"Energy produced: {PowerProducedW:0.0} W", normalColor);
        SetText(energyConsumedLabel, $"Energy consumed: {PowerConsumedW:0.0} W",
            PowerConsumedW > PowerProducedW ? warningColor : normalColor);
    }

    static void SetText(TMP_Text label, string text, Color color)
    {
        if (label == null) return;
        label.text = text;
        label.color = color;
    }
}
