using System.Collections.Generic;
using System.ComponentModel;

namespace Synapse.Config
{
    public class SynapseConfiguration : AbstractConfigSection
    {
        [Description("Enables or disables the embedded Database. Warning: Disabling this option can break plugins and is not recommended")]
        public bool DatabaseEnabled = true;

        [Description("Enables or disables if the Player needs to equip his keycard to open a door")]
        public bool RemoteKeyCard = false;

        [Description("The Broadcast Message a Player gets when he joins the Server")]
        public string JoinBroadcast = string.Empty;

        [Description("The TextHint a Player gets when he joins the Server")]
        public string JoinTextHint = string.Empty;

        [Description("The Duration which the TextHint and Broadcast will be displayed")]
        public ushort JoinMessagesDuration = 5;

        [Description("The ID�s of the Scps which can speak")]
        public List<int> SpeakingScps = new List<int> { 16, 17 };

        [Description("If Enabled your Server is marked as Synapse Server")]
        public bool NameTracking = true;
    }
}