using System;
using System.Collections.Generic;

namespace Flira.Domain.States;

public abstract class TaskItemState
{
    public abstract string Name { get; }

    public virtual bool CanTransitionTo(TaskItemState nextState) => false;

    public override bool Equals(object? obj) => obj is TaskItemState other && Name == other.Name;

    public override int GetHashCode() => Name.GetHashCode();

    public override string ToString() => Name;
}

public class CustomTaskItemState : TaskItemState
{
    private readonly string _name;
    private readonly HashSet<string> _allowedTransitions;

    public CustomTaskItemState(string name, IEnumerable<string> allowedTransitions)
    {
        _name = name;
        _allowedTransitions = new HashSet<string>(allowedTransitions, StringComparer.OrdinalIgnoreCase);
    }

    public override string Name => _name;

    public override bool CanTransitionTo(TaskItemState nextState)
    {
        return _allowedTransitions.Contains(nextState.Name);
    }
}
