﻿using System;
using System.Collections.Generic;
using InventorySystem.Items;
using Mirror;
using Neuron.Core.Logging;
using RemoteAdmin;
using Synapse3.SynapseModule.Map;
using Synapse3.SynapseModule.Map.Objects;
using Synapse3.SynapseModule.Map.Schematic;
using Synapse3.SynapseModule.Player;
using UnityEngine;

namespace Synapse3.SynapseModule.Dummy;

public class SynapseDummy : DefaultSynapseObject, IRefreshable
{
    private readonly MapService _map;
    private readonly DummyService _dummy;

    public MovementDirection Direction { get; set; }

    public float SneakSpeed { get; set; } = 1.8f;

    public float WalkSpeed { get; set; }

    public float RunSpeed { get; set; }

    public override GameObject GameObject { get; }
    public override ObjectType Type => ObjectType.Dummy;

    public override Vector3 Position
    {
        get => Player.Position;
        set => Player.Position = value;
    }

    public override Quaternion Rotation
    {
        get => Quaternion.Euler(RotationVector2.x, RotationVector2.y, 90f);
        set => RotationVector2 = new Vector2(value.eulerAngles.x, value.eulerAngles.y);
    }
    
    public Vector2 RotationVector2
    {
        get => Player.RotationVector2;
        set
        {
            Player.RotationVector2 = value;
            Player.CameraReference.rotation = Quaternion.Euler(value.x, value.y, 90f);
        }
    }

    public override Vector3 Scale
    {
        get => Player.Scale;
        set => Player.Scale = value;
    }

    public string Name
    {
        get => Player.NickName;
        set => Player.NicknameSync.Network_myNickSync = value;
    }

    public ItemType HeldItem
    {
        get => Player.VanillaInventory.NetworkCurItem.TypeId;
        set => Player.VanillaInventory.NetworkCurItem = new ItemIdentifier(value, 0);
    }
    
    public PlayerMovementState Movement
    {
        get => Player.AnimationController.MoveState;
        set
        {
            Player.AnimationController.MoveState = value;
            Player.AnimationController.RpcReceiveState((byte)value);
        }
    }

    public DummyPlayer Player { get; }

    public SynapseDummy(Vector3 position, Vector2 rotation, RoleType role, string name, string badge,
        string badgeColor)
    {
        _map = Synapse.Get<MapService>();
        _dummy = Synapse.Get<DummyService>();
        
        Player = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab, _dummy._dummyParent)
            .GetComponent<DummyPlayer>();
        GameObject = Player.gameObject;
        var comp = GameObject.AddComponent<SynapseObjectScript>();
        comp.Object = this;
        
        Player.SynapseDummy = this;
        var transform = Player.transform;
        transform.localScale = Vector3.one;
        transform.position = position;
        Player.PlayerMovementSync.RealModelPosition = position;
        RotationVector2 = rotation;
        Player.QueryProcessor.NetworkPlayerId = QueryProcessor._idIterator;
        Player.QueryProcessor._ipAddress = Synapse.Get<PlayerService>().Host.IpAddress;
        Player.ClassManager.CurClass = role;
        Player.Health = Player.MaxHealth;
        Player.NicknameSync.Network_myNickSync = name;
        Player.RankName = badge;
        Player.RankColor = badgeColor;
        Player.GodMode = true;
        Player.PlayerMovementSync.NetworkGrounded = true;
        RunSpeed = CharacterClassManager._staticClasses[(int)role].runSpeed;
        WalkSpeed = CharacterClassManager._staticClasses[(int)role].walkSpeed;
        _ = MEC.Timing.RunCoroutine(Update());

        NetworkServer.Spawn(GameObject);

        MoveInElevator = true;
    }

    internal SynapseDummy(SchematicConfiguration.DummyConfiguration configuration, SynapseSchematic schematic) :
        this(configuration.Position, new Vector2(configuration.Rotation.X, configuration.Rotation.Y),
            configuration.Role, configuration.Name,
            configuration.Badge, configuration.BadgeColor)
    {
        Parent = schematic;
        schematic._dummies.Add(this);
        OriginalScale = configuration.Scale;
        CustomAttributes = configuration.CustomAttributes;
    }

    public void Refresh()
    {
        Position = base.Position;
        Rotation = base.Rotation;
        Scale = base.Scale;
    }

    public bool UpdateEveryFrame { get; }
    
    public void RotateToPosition(Vector3 pos)
    {
        var rot = Quaternion.LookRotation((pos - GameObject.transform.position).normalized);
        RotationVector2 = new Vector2(rot.eulerAngles.x, rot.eulerAngles.y);
    }

    public void Despawn()
        => NetworkServer.UnSpawn(GameObject);

    public void Spawn()
        => NetworkServer.Spawn(GameObject);
    
    
    //Thanks to GameHunt.I used some of his code for the Dummy API https://github.com/gamehunt/CustomNPCs
    private IEnumerator<float> Update()
    {
        for (;;)
        {
            yield return MEC.Timing.WaitForSeconds(0.1f);
            try
            {
                if (GameObject == null) yield break;
                if (Direction == MovementDirection.None)
                {
                    continue;
                }

                var wall = false;
                var speed = 0f;

                switch (Movement)
                {
                    case PlayerMovementState.Sneaking:
                        speed = SneakSpeed;
                        break;

                    case PlayerMovementState.Sprinting:
                        speed = RunSpeed * _map.HumanSprintSpeed;
                        break;

                    case PlayerMovementState.Walking:
                        speed = WalkSpeed * _map.HumanWalkSpeed;
                        break;
                }

                switch (Direction)
                {
                    case MovementDirection.Forward:
                        var pos = Position + Player.CameraReference.forward / 10 * speed;

                        if (!Physics.Linecast(Position, pos, Player.PlayerMovementSync.CollidableSurfaces))
                            Player.PlayerMovementSync.OverridePosition(pos, null, true);
                        else wall = true;
                        break;

                    case MovementDirection.BackWards:
                        pos = Position - Player.CameraReference.forward / 10 * speed;

                        if (!Physics.Linecast(Position, pos, Player.PlayerMovementSync.CollidableSurfaces))
                            Player.PlayerMovementSync.OverridePosition(pos, null, true);
                        else wall = true;
                        break;

                    case MovementDirection.Right:
                        pos = Position + Quaternion.AngleAxis(90, Vector3.up) * Player.CameraReference.forward / 10 *
                            speed;

                        if (!Physics.Linecast(Position, pos, Player.PlayerMovementSync.CollidableSurfaces))
                            Player.PlayerMovementSync.OverridePosition(pos, null, true);
                        else wall = true;
                        break;

                    case MovementDirection.Left:
                        pos = Position - Quaternion.AngleAxis(90, Vector3.up) * Player.CameraReference.forward / 10 *
                            speed;

                        if (!Physics.Linecast(Position, pos, Player.PlayerMovementSync.CollidableSurfaces))
                            Player.PlayerMovementSync.OverridePosition(pos, null, true);
                        else wall = true;
                        break;
                }

                if (wall)
                {
                    Direction = MovementDirection.None;
                }
            }
            catch (Exception e)
            {
                NeuronLogger.For<Synapse>().Error($"Sy3 Dummy: Update Failed:\n{e}");
            }
        }
    }
}