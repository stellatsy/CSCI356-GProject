using UnityEngine;
using UnityEngine.Rendering;

public class StageAtmosphere : MonoBehaviour
{
    public Light sun;                                     // drag your Directional Light here
    public float sunIntensity = 0.25f;
    public Color ambient = new Color(0.06f, 0.09f, 0.16f);
    public Color fogColor = new Color(0.04f, 0.06f, 0.10f);
    public float fogDensity = 0.015f;

    bool oldFog; FogMode oldMode; Color oldFogColor; float oldDensity;
    AmbientMode oldAmbientMode; Color oldAmbient; float oldSun;

    void OnEnable()
    {
        oldFog = RenderSettings.fog; oldMode = RenderSettings.fogMode;
        oldFogColor = RenderSettings.fogColor; oldDensity = RenderSettings.fogDensity;
        oldAmbientMode = RenderSettings.ambientMode; oldAmbient = RenderSettings.ambientLight;
        if (sun != null) oldSun = sun.intensity;

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = ambient;
        if (sun != null) sun.intensity = sunIntensity;
    }

    void OnDisable()
    {
        RenderSettings.fog = oldFog; RenderSettings.fogMode = oldMode;
        RenderSettings.fogColor = oldFogColor; RenderSettings.fogDensity = oldDensity;
        RenderSettings.ambientMode = oldAmbientMode; RenderSettings.ambientLight = oldAmbient;
        if (sun != null) sun.intensity = oldSun;
    }
}