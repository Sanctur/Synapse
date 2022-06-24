﻿using System;
using MapGeneration;
using PlayerStatsSystem;
using Synapse3.SynapseModule.Map.Rooms;
using UnityEngine;

namespace Synapse3.SynapseModule.Player;

public partial class SynapsePlayer
{
    /// <summary>
    /// The current Position of the Player
    /// </summary>
    public Vector3 Position
    {
        get => PlayerMovementSync.GetRealPosition();
        set => PlayerMovementSync.OverridePosition(value, PlayerRotation);
    }

    /// <summary>
    /// The Rotation of the Player as Quaternion
    /// </summary>
    public Quaternion Rotation => transform.rotation;

    /// <summary>
    /// The Rotation of the Player as vector2
    /// </summary>
    public Vector2 RotationVector2
    {
        get => PlayerMovementSync.RotationSync;
        set => PlayerMovementSync.NetworkRotationSync = value;
    }
    
    /// <summary>
    /// The Rotation of the Player as PlayerRotation
    /// </summary>
    public PlayerMovementSync.PlayerRotation PlayerRotation
    {
        get
        {
            var vec2 = RotationVector2;
            return new PlayerMovementSync.PlayerRotation(vec2.x, vec2.y);
        }
        set => RotationVector2 = new Vector2(value.x.Value, value.y.Value);
    }

    // TODO: Dimenzio nach Room service fragen
    // public SynapseRoom Room
    // {
    //     get => 
    // }
    
    // public MapPoint MapPoint
}