using ExitGames.Client.Photon;
using GorillaNametags.Tags;
using Photon.Pun;
using Photon.Realtime;

namespace GorillaNametags.Components;

public class PunCallbacks : MonoBehaviourPunCallbacks
{
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        foreach (VRRig rig in GorillaParent.instance.vrrigs)
        {
            if (rig.OwningNetPlayer.UserId != targetPlayer.UserId || rig.isLocal)
                continue;
            
            rig.GetComponent<StatsTag>().UpdateProperties(changedProps);
            break;
        }
    }
}