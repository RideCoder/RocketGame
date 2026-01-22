using UnityEngine;
using UnityEngine.UI; // Required for UI elements, although not strictly needed for Application.OpenURL

public class DiscordLinkManager : MonoBehaviour
{
    // Public method that can be called by the Unity Button's OnClick event
    public void OpenDiscordServer()
    {
        // Replace "YOUR_DISCORD_INVITE_LINK" with your actual Discord server invite link
        Application.OpenURL("https://discord.gg/nSZYVjms");
    }
}
