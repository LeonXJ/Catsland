using UnityEngine;

[ExecuteInEditMode]
public class ComposeLightmap: MonoBehaviour {

  public Camera lightmapCamera;
  public Material lightmapMaterial;
  private RenderTexture lightmapRenderTexture;

  void OnRenderImage(RenderTexture source, RenderTexture destination) {
    if(lightmapRenderTexture == null
      || lightmapRenderTexture.width != source.width
      || lightmapRenderTexture.height != source.height) {
      lightmapRenderTexture = new RenderTexture(source.width, source.height, 16, RenderTextureFormat.ARGB32);
      lightmapCamera.targetTexture = lightmapRenderTexture;
    }
    lightmapMaterial.SetTexture("_MainTex", source);
    lightmapMaterial.SetTexture("_Lightmap", lightmapRenderTexture);

    Graphics.Blit(source, destination, lightmapMaterial);



  }
}
