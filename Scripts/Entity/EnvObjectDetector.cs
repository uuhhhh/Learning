using System.Collections.Generic;
using System.Linq;
using Godot;
using Learning.Scripts.Environment;

namespace Learning.Scripts.Entity;

public abstract partial class EnvObjectDetector : Area2D
{
    [Signal]
    public delegate void EnvObjectEnteredAreaEventHandler(EnvObject entered);

    [Signal]
    public delegate void EnvObjectExitedAreaEventHandler(EnvObject exited);

    [Signal]
    public delegate void NewHighestPriorityEnvObjectEventHandler(
        EnvObject oldHighestPriority, EnvObject highestPriority);

    [Signal]
    public delegate void ZeroEnvObjectsEventHandler();

    [Signal]
    public delegate void ZeroToOneEnvObjectsEventHandler();

    private PriorityComparer _comparer;
    private ISet<EnvObject> _currentInArea;

    public override void _Ready()
    {
        _comparer = new PriorityComparer(this);
        _currentInArea = new SortedSet<EnvObject>(_comparer);

        BodyEntered += DetectEnvObjectToAdd;
        BodyExited += DetectEnvObjectToRemove;
    }

    private void DetectEnvObjectToAdd(Node2D mayBeEnvObject)
    {
        if (mayBeEnvObject is EnvObject envObject) AddEnvObject(envObject);
    }

    private void AddEnvObject(EnvObject toAdd)
    {
        EnvObject oldHighestPriority = GetCurrentEnvObject();

        _currentInArea.Add(toAdd);
        EmitSignal(SignalName.EnvObjectEnteredArea, toAdd);

        if (_currentInArea.Count == 1) EmitSignal(SignalName.ZeroToOneEnvObjects);

        CheckForNewHighestPriority(oldHighestPriority);
    }

    private void DetectEnvObjectToRemove(Node2D mayBeEnvObject)
    {
        if (mayBeEnvObject is EnvObject envObject) RemoveEnvObject(envObject);
    }

    private void RemoveEnvObject(EnvObject toRemove)
    {
        if (!_currentInArea.Contains(toRemove))
        {
            GD.PrintErr($"DetectorComp {this} trying to remove EnvObject it doesn't have");
            return;
        }

        EnvObject oldHighestPriority = GetCurrentEnvObject();

        _currentInArea.Remove(toRemove);
        EmitSignal(SignalName.EnvObjectExitedArea, toRemove);

        if (!HasEnvObject()) EmitSignal(SignalName.ZeroEnvObjects);

        CheckForNewHighestPriority(oldHighestPriority);
    }

    private void CheckForNewHighestPriority(EnvObject oldHighestPriority)
    {
        EnvObject newHighestPriority = GetCurrentEnvObject();
        if (oldHighestPriority != newHighestPriority)
            EmitSignal(SignalName.NewHighestPriorityEnvObject, oldHighestPriority,
                newHighestPriority);
    }

    public EnvObject GetCurrentEnvObject()
    {
        return HasEnvObject() ? _currentInArea.Last() : null;
    }

    public bool HasEnvObject()
    {
        return _currentInArea.Count != 0;
    }

    protected abstract int GetPriorityOf(EnvObject envObject);

    public class PriorityComparer : IComparer<EnvObject>
    {
        private readonly EnvObjectDetector _priorityDeterminer;

        public PriorityComparer(EnvObjectDetector priorityDeterminer)
        {
            _priorityDeterminer = priorityDeterminer;
        }

        public int Compare(EnvObject x, EnvObject y)
        {
            return (x, y) switch
            {
                (null, null) => 0,
                (null, _) => -1,
                (_, null) => 1,
                _ => _priorityDeterminer.GetPriorityOf(x)
                    .CompareTo(_priorityDeterminer.GetPriorityOf(y))
            };
        }
    }
}