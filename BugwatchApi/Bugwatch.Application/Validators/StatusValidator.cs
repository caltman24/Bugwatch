using System.Reflection;
using Bugwatch.Application.Constants;

namespace Bugwatch.Application.Validators;

public static class StatusValidator
{
    public static bool ValidateProject(string? value)
    {
        if (value == null) return false;

        var fields =
            typeof(ProjectStatuses).GetFields(BindingFlags.Public | BindingFlags.Static |
                                              BindingFlags.FlattenHierarchy);
        return fields.Any(field =>
            field is { IsLiteral: true, IsInitOnly: false } &&
            field.GetValue(null)?.ToString()?.ToLower() == value.ToLower());
    }

    public static bool ValidateTicket(string? value)
    {
        if (value == null) return false;

        // TODO: Change type to ticket status constants
        var fields =
            typeof(ProjectStatuses).GetFields(BindingFlags.Public | BindingFlags.Static |
                                              BindingFlags.FlattenHierarchy);
        return fields.Any(field =>
            field is { IsLiteral: true, IsInitOnly: false } && field.GetValue(null)?.ToString() == value);
    }
}