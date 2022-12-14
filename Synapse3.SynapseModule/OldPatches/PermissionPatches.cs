﻿using System;
using HarmonyLib;
using Neuron.Core.Logging;

namespace Synapse3.SynapseModule.Patches;

[Patches]
[HarmonyPatch]
public static class PermissionPatches
{
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.RefreshPermissions)), HarmonyPrefix]
    public static bool RefreshPermission(ServerRoles __instance, bool disp = false)
    {
        try
        {
            var player = __instance.GetSynapsePlayer();
            player.RefreshPermission(disp);
        }
        catch(Exception e)
        {
            NeuronLogger.For<Synapse>().Error($"Sy3 Permission: RefreshPermissionPatch failed!!\n{e}");
        }
        return false;
    }

    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.SetGroup)), HarmonyPrefix]
    public static bool SetGroup() => false;
}