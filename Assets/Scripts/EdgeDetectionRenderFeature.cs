using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EdgeDetectionRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class EdgeDetectionSettings
    {
        public Material edgeDetectionMaterial;
    }

    public EdgeDetectionSettings settings = new EdgeDetectionSettings();
    private EdgeDetectionRenderPass pass;

    public override void Create()
    {
        pass = new EdgeDetectionRenderPass(settings.edgeDetectionMaterial)
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.edgeDetectionMaterial != null)
        {
            renderer.EnqueuePass(pass);
        }
    }

    class EdgeDetectionRenderPass : ScriptableRenderPass
    {
        private Material material;
        private RTHandle tempTexture;

        public EdgeDetectionRenderPass(Material material)
        {
            this.material = material;
            ConfigureInput(ScriptableRenderPassInput.Depth);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            RenderingUtils.ReAllocateIfNeeded(ref tempTexture, desc, name: "_EdgeTempTexture");
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null) return;

            var cmd = CommandBufferPool.Get("EdgeDetection");
            var cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

            Blitter.BlitCameraTexture(cmd, cameraColorTarget, tempTexture);
            Blitter.BlitCameraTexture(cmd, tempTexture, cameraColorTarget, material, 0);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            tempTexture?.Release();
        }
    }
}