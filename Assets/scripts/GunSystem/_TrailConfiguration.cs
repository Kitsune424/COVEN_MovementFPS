using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "trail config", menuName = "Weapon/trail config")]
public class _TrailConfiguration : ScriptableObject
{
    public Material Material;
    public AnimationCurve WidthCurve;

    public float Duration = 0.5f;
    public float MinVertexDistance = 0.1f;
    public Gradient Color;

    public float MissDistance = 100f;
    public float SimulationSpeed = 100f;
}
