﻿using System.Linq;
using Synapse3.SynapseModule.Map.Objects;
using Synapse3.SynapseModule.Map.Schematic;
using UnityEngine;

namespace Synapse3.SynapseModule.Map.Elevators;

public class SchematicDestination : DefaultSynapseObject, IElevatorDestination
{
    public SchematicDestination(SynapseSchematic destination, int id, string name, IElevator elevator)
    {
        Elevator = elevator;
        DestinationName = name;
        ElevatorId = id;
        Transform = destination._custom.FirstOrDefault(x => x.ID == 1)?.GameObject.transform;
        Schematic = destination;
        destination.Parent = this;
    }
    
    public SynapseSchematic Schematic { get; }
    public IElevator Elevator { get; }
    public Transform Transform { get; }
    public string DestinationName { get; }

    public virtual bool Open
    {
        get => Schematic._doors.First().Open;
        set
        {
            foreach (var door in Schematic._doors)
            {
                door.Open = value;
            }
        }
    }

    public virtual bool Locked
    {
        get => Schematic._doors.First().Locked;
        set
        {
            foreach (var door in Schematic._doors)
            {
                door.Locked = value;
            }
        }
    }
    public int ElevatorId { get; }
    
    public Vector3 RangeScale { get; set; }

    public Vector3 DestinationPosition
    {
        get => Transform.position;
        set => Transform.position = value;
    }

    public override GameObject GameObject => Schematic.GameObject;
    public override ObjectType Type => ObjectType.ElevatorDestination;

    public override Vector3 Position
    {
        get => Schematic.Position;
        set => Schematic.Position = value;
    }

    public override Quaternion Rotation
    {
        get => Schematic.Rotation;
        set => Schematic.Rotation = value;
    }

    public override Vector3 Scale
    {
        get => Schematic.Scale;
        set => Schematic.Scale = value;
    }


    public Vector3 GetWorldPosition(Vector3 localPosition) => Transform.TransformPoint(localPosition);
    public Vector3 GetLocalPosition(Vector3 worldPosition) => Transform.InverseTransformPoint(worldPosition);
}