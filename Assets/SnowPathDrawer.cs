using StarterAssets;
using UnityEngine;

public class SnowPathDrawer : MonoBehaviour
{
    public ComputeShader snowComputeShader;
    public RenderTexture snowRT;

    private string snowImageProperty = "snowImage";
    private string colorValueProperty = "colorValueToAdd";
    private string resolutionProperty = "resolution";
    private string positionXProperty = "positionX";
    private string positionYProperty = "positionY";
    private string spotSizeProperty = "spotSize";

    private string drawSpotKernel = "DrawSpot";

    private Vector2Int position = new Vector2Int(256, 256);
    public float spotSize = 1.5f;

    private SnowController snowController;
    private GameObject[] snowControllerObjs;

    private ThirdPersonController _controller;

    void Start()
    {
        snowControllerObjs = GameObject.FindGameObjectsWithTag("SnowTagGround");
        _controller = GetComponent<ThirdPersonController>();
    }

    private void Update()
    {
        for(int i = 0; i < snowControllerObjs.Length; i++)
        {
            if (Vector3.Distance(snowControllerObjs[i].transform.position, transform.position) > spotSize * 5f) continue;

            snowController = snowControllerObjs[i].GetComponent<SnowController>();
            snowRT = snowController.snowRT;
            GetPosition();
            DrawSpot();
        }
    }

    public void GetPosition()
    {
        if (snowController == null || snowRT == null)
        {
            Debug.LogError(" GetPosition(): SnowController or snowRT is NULL!");
            return;
        }

        float scaleX = snowController.transform.localScale.x;
        float scaleY = snowController.transform.localScale.z; 

        float snowPosX = snowController.transform.position.x;
        float snowPosY = snowController.transform.position.z;

        float playerPosX = _controller.transform.position.x;
        float playerPosY = _controller.transform.position.z;
        
        float offsetX = snowRT.width / 2f;
        float offsetY = snowRT.height / 2f;

        // Convert world position to texture space
        int posX = (int)(((playerPosX - snowPosX) / scaleX + 0.5f) * snowRT.width + offsetX);
        int posY = (int)(((playerPosY - snowPosY) / scaleY + 0.5f) * snowRT.height + offsetY);

        //  Flip Y-axis
        posY = snowRT.height - posY;

        // If X is also inverted, uncomment the next line
        posX = snowRT.width - posX;

        posX = Mathf.Clamp(posX, 0, snowRT.width - 1);
        posY = Mathf.Clamp(posY, 0, snowRT.height - 1);

        position = new Vector2Int(posX, posY);

        Debug.Log($"✅ Player Pos: {playerPosX}, {playerPosY} → Texture Pos: {position}");
    }
    
    public void DrawSpot()
    {
        if (snowRT == null) return;
        if (snowComputeShader == null) return;

        int kernel_handle = snowComputeShader.FindKernel(drawSpotKernel);
        snowComputeShader.SetTexture(kernel_handle, snowImageProperty, snowRT);
        snowComputeShader.SetFloat(colorValueProperty, 0);
        snowComputeShader.SetFloat(resolutionProperty, snowRT.width);
        snowComputeShader.SetFloat(positionXProperty, position.x);
        snowComputeShader.SetFloat(positionYProperty, position.y);
        snowComputeShader.SetFloat(spotSizeProperty, spotSize);
        snowComputeShader.Dispatch(kernel_handle, snowRT.width / 8, snowRT.height / 8, 1);
    }
}