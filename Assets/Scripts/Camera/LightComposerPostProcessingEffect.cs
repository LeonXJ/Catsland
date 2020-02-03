using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Catsland.Scripts.Common;

[Serializable]
[PostProcess(typeof(LightComposerPostProcessingRenderer), PostProcessEvent.AfterStack, "Custom/LightComposer")]
public sealed class LightComposerPostProcessingEffect : PostProcessEffectSettings {
}

public class LightComposerPostProcessingRenderer : PostProcessEffectRenderer<LightComposerPostProcessingEffect> {
    private RenderTexture lightmapRenderTexture;
  public override void Render(PostProcessRenderContext context) {

    if(lightmapRenderTexture == null
      || lightmapRenderTexture.width != context.width
      || lightmapRenderTexture.height != context.height) {
      lightmapRenderTexture = new RenderTexture(context.width, context.height, 16, RenderTextureFormat.ARGB32);
      SceneConfig.getSceneConfig().lightCamera.targetTexture = lightmapRenderTexture;
    }

    var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/LightComposer"));
    sheet.properties.SetTexture("_Lightmap", lightmapRenderTexture);
    context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
  }
}
