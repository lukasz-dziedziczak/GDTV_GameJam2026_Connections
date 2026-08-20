using UnityEngine;

public class UI_Sound : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip buildButtonPress;
    [SerializeField] AudioClip cancelMode;
    [SerializeField] AudioClip startBuildingNode;
    [SerializeField] AudioClip nodePlace;
    [SerializeField] AudioClip roadPlace;
    [SerializeField] AudioClip connectionStart;
    [SerializeField] AudioClip connectionAdd;
    [SerializeField] AudioClip connectionComplete;
    [SerializeField] AudioClip mouseOverBuilding;
    [SerializeField] AudioClip deleteModePress;
    [SerializeField] AudioClip buttonPress;
    [SerializeField] AudioClip deleteNode;
    [SerializeField] AudioClip levelComplete;
    [SerializeField] AudioClip pauseGame;
    [SerializeField] AudioClip gateResourceDelivered;

    public void PlayBuildButtonPress()
    {
        if (buildButtonPress != null) audioSource.PlayOneShot(buildButtonPress);
    }

    public void PlayCancelMode()
    {
        if (cancelMode != null) audioSource.PlayOneShot(cancelMode);
    }

    public void PlayStartBuildingNode()
    {
        if (startBuildingNode != null) audioSource.PlayOneShot(startBuildingNode);
    }

    public void PlayNodePlace()
    {
        if (nodePlace == null) return;

        audioSource.PlayOneShot(nodePlace);
    }

    public void PlayConnectionStart()
    {
        if (connectionStart != null) audioSource.PlayOneShot(connectionStart);
    }

    public void PlayConnectionAdd()
    {
        if (connectionAdd != null) audioSource.PlayOneShot(connectionAdd);
    }

    public void PlayConnectionComplete()
    {
        if (connectionComplete != null) audioSource.PlayOneShot(connectionComplete);
    }

    public void PlayRoadPlace()
    {
        if (roadPlace == null) return;
        audioSource.PlayOneShot(roadPlace);
    }

    public void PlayMouseOverBuilding()
    {
        if (mouseOverBuilding == null) return;
        audioSource.PlayOneShot(mouseOverBuilding);
    }

    public void PlayDeleteModePress()
    {
        if (deleteModePress == null) return;
        audioSource.PlayOneShot(deleteModePress);
    }

    public void PlayButtonPress()
    {
        if (buttonPress == null) return;
        audioSource.PlayOneShot(buttonPress);
    }

    public void PlayDeleteNode()
    {
        if (deleteNode == null) return;
        audioSource.PlayOneShot(deleteNode);
    }

    public void PlayLevelComplete()
    {
        if (levelComplete == null) return;
        audioSource.PlayOneShot(levelComplete);
    }

    public void PlayPauseGame()
    {
        if (pauseGame == null) return;
        audioSource.PlayOneShot(pauseGame);
    }

    public void PlayGateResourceDelivered()
    {
        if (gateResourceDelivered == null) return;
        audioSource.PlayOneShot(gateResourceDelivered);
    }
}
