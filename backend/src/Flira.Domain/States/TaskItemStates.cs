using System;
using System.Collections.Generic;

namespace Flira.Domain.States;

public static class TaskItemStates
{
    public static readonly TaskItemState Backlog = new CustomTaskItemState("Backlog", new[] { "Todo" });
    public static readonly TaskItemState Todo = new CustomTaskItemState("Todo", new[] { "In Progress", "Backlog" });
    public static readonly TaskItemState InProgress = new CustomTaskItemState("In Progress", new[] { "Review", "Todo" });
    public static readonly TaskItemState Review = new CustomTaskItemState("Review", new[] { "Done", "In Progress" });
    public static readonly TaskItemState Done = new CustomTaskItemState("Done", new[] { "Review" });

    public static TaskItemState CreateCustom(string name, IEnumerable<string> allowedTransitions)
    {
        return new CustomTaskItemState(name, allowedTransitions);
    }
}
