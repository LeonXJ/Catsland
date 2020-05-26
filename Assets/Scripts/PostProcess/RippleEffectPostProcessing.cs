using System;
using UnityEngine.Rendering.PostProcessing;
using Catsland.Scripts.Common;

namespace Catsland.Scripts.PostProcess {

  [Serializable]
  [PostProcess(typeof(RippleEffectPostProcessingRenderer), PostProcessEvent.AfterStack, "Custom/RippleEffect")]
  public sealed class RippleEffectPostProcessingEffect : PostProcessEffectSettings {
  }

  public class RippleEffectPostProcessingRenderer : PostProcessEffectRenderer<RippleEffectPostProcessingEffect> {
    public override void Render(PostProcessRenderContext context) {
      context.command.Blit(
        context.source,
        context.destination,
        SceneConfig.getSceneConfig().rippleEffect.GetMaterial());
    }
  }
}
