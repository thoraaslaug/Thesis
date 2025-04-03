using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

[Serializable]
[VolumeComponentMenu("Custom/SphereVolumeComponent")]
public class SphereVolumeComponent : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter intensity = new ClampedFloatParameter(1.0f, 0f, 1f, true);
    public ClampedFloatParameter ditherStrength = new ClampedFloatParameter(1.0f, 0f, 1f);
    public TextureParameter ditherRamp = new TextureParameter(null);
    public BoolParameter useScrolling = new BoolParameter(false);
    public ColorParameter tintColor = new ColorParameter(Color.white);
    public Vector2Parameter scrollSpeed = new Vector2Parameter(new Vector2(0.1f, 0.05f));


    public bool IsActive() => intensity.value > 0f;
}
    