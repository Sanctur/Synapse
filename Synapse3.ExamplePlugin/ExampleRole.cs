﻿using System.Collections.Generic;
using Neuron.Core.Meta;
using Synapse3.SynapseModule.Role;

namespace Synapse3.ExamplePlugin;

[Automatic]
[RoleInformation(
    Name = "ExampleRole",
    ID = 60
)]
public class ExampleRole : SynapseRole
{
    public override int GetTeamID() => 15;

    public override void SpawnPlayer(bool spawnLite)
    {
        Player.RoleType = RoleType.ClassD;
    }

    public override List<int> GetEnemiesID() => new (){ (int)Team.CDP };
}