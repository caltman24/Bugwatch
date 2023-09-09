using System.Reflection;
using Bugwatch.Application.Constants;

namespace Bugwatch.Application.Validators;

public static class TicketTypeValidator
{
    public static bool Validate(string? value)
    {
        if (value == null) return false;

        var fields =
            typeof(TicketTypes).GetFields(BindingFlags.Public | BindingFlags.Static |
                                         BindingFlags.FlattenHierarchy);
        return fields.Any(field =>
            field is { IsLiteral: true, IsInitOnly: false } &&
            field.GetValue(null)?.ToString()?.ToLower() == value.ToLower());
    }
}