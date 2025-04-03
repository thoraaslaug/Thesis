using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class DitherEffectRenderFeature : ScriptableRendererFeature
{
    class DitherEffectPass : ScriptableRenderPass
    {
        const string m_PassName = "DitherEffectPass";
        Material m_BlitMaterial;
        public LayerMask excludeLayers = 0;


        public void Setup(Material mat)
        {
            m_BlitMaterial = mat;
            requiresIntermediateTexture = true;
        }
        // This class stores the data needed by the RenderGraph pass.
        // It is passed as a parameter to the delegate function that executes the RenderGraph pass.

        // This static method is passed as the RenderFunc delegate to the RenderGraph render pass.
        // It is used to execute draw commands.
        
        
   
        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
           var stack = VolumeManager.instance.stack;
           var customEffect = stack.GetComponent<SphereVolumeComponent>();
           if (!customEffect.IsActive())
               return;

           var resourceData = frameData.Get<UniversalResourceData>();

           if (resourceData.isActiveTargetBackBuffer)
           {
               Debug.LogError($"SKIPPING RENDER PASS");
               return;
           }

           var source = resourceData.activeColorTexture;
           
           var destionationDesc = renderGraph.GetTextureDesc(source);
           destionationDesc.name = $"CameraColor-{m_PassName}";
           destionationDesc.clearBuffer = false;

           TextureHandle destiation = renderGraph.CreateTexture(destionationDesc);

           RenderGraphUtils.BlitMaterialParameters para = new(source, destiation, m_BlitMaterial, 0);
           renderGraph.AddBlitPass(para, passName: m_PassName);

           resourceData.cameraColor = destiation;
            }
        }

    public RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;
    public Material material;

    DitherEffectPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        
        m_ScriptablePass = new DitherEffectPass();
        //m_ScriptablePass.excludeLayers = excludeLayers;

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = injectionPoint;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (material == null)
        {
            Debug.LogWarning("no material assigned");
            return;
        }
        
        m_ScriptablePass.Setup(material);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}
