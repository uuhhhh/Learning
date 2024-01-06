using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Learning.Scripts.Environment;

/// <summary>
///     An Area2D that focuses mainly on detecting EnvObjects that enter its vicinity,
///     and determining which, of multiple EnvObjects in its vicinity, to make its "current" one.
/// </summary>
public abstract partial class EnvObjectDetector : Area2D
{
    /// <summary>
    ///     A signal for when an EnvObject enters the vicinity of this EnvObjectDetector.
    /// </summary>
    [Signal]
    public delegate void EnvObjectEnteredAreaEventHandler(EnvObject entered);

    /// <summary>
    ///     A signal for when an EnvObject exits the vicinity of this EnvObjectDetector.
    /// </summary>
    [Signal]
    public delegate void EnvObjectExitedAreaEventHandler(EnvObject exited);

    /// <summary>
    ///     A signal for when a different EnvObject takes the place for
    ///     the highest-priority EnvObject in the vicinity of this EnvObjectDetector.
    /// </summary>
    [Signal]
    public delegate void NewHighestPriorityEnvObjectEventHandler(
        EnvObject oldHighestPriority, EnvObject highestPriority);

    /// <summary>
    ///     A signal for when the number of EnvObjects in the vicinity of this EnvObjectDetector
    ///     goes from nonzero to zero.
    /// </summary>
    [Signal]
    public delegate void ZeroEnvObjectsEventHandler();

    /// <summary>
    ///     A signal for when the number of EnvObjects in the vicinity of this EnvObjectDetector
    ///     goes from zero to nonzero.
    /// </summary>
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

    /// <returns>
    ///     The highest-priority EnvObject in the vicinity of this EnvObjectDetector,
    ///     or null if there aren't any EnvObjects
    /// </returns>
    public EnvObject GetCurrentEnvObject()
    {
        return HasEnvObject() ? _currentInArea.Last() : null;
    }

    /// <returns>
    ///     Whether there are any EnvObjects
    ///     in the vicinity of this EnvObjectDetector
    /// </returns>
    public bool HasEnvObject()
    {
        return _currentInArea.Count != 0;
    }

    /// <summary>
    ///     Finds a priority value for the given EnvObject. Used for comparing with other
    ///     priority values to determine which EnvObject in the vicinity should be the "current" one.
    ///     Higher priority values take greater precedence.
    /// </summary>
    /// <param name="envObject">The EnvObject to get the priority of</param>
    /// <returns>The priority value</returns>
    protected abstract int GetPriorityOf(EnvObject envObject);

    /// <summary>
    ///     A class for comparing the priority values of EnvObjects.
    /// </summary>
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