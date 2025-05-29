using UnityEngine;

public class ViewSwitcher : MonoBehaviour
{
    [Header("Camera references")]
    public Camera gameCam;
    public Camera altCam;

    [Header("Display routing")]
    public int gameCamDisplay = 0;
    public int altCamDisplay = 1;

    [Header("Key bindings")]
    public KeyCode toggleViewKey = KeyCode.V;
    public KeyCode swapMonitorsKey = KeyCode.F1;

    void Awake()
    {
        for (int i = 1; i < Display.displays.Length; i++)
            Display.displays[i].Activate();

        EnableView(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleViewKey))
            EnableView(altCam.enabled ? 0 : 1);

        if (Input.GetKeyDown(swapMonitorsKey))
            SwapDisplays();
    }

    void EnableView(int viewIndex)
    {
        bool useGameCam = viewIndex == 0;

        gameCam.enabled = useGameCam;
        altCam.enabled = !useGameCam;

        gameCam.targetDisplay = useGameCam ? gameCamDisplay : altCamDisplay;
        altCam.targetDisplay = useGameCam ? altCamDisplay : gameCamDisplay;
    }

    void SwapDisplays()
    {
        int temp = gameCam.targetDisplay;
        gameCam.targetDisplay = altCam.targetDisplay;
        altCam.targetDisplay = temp;
    }
}
