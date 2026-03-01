namespace TC.Agro.Contracts.Realtime;

public static class OwnerScopeErrors
{
    public const string AdminOwnerScopeRequiredCode = "OWNER_SCOPE_REQUIRED";

    public const string AdminOwnerScopeRequiredMessage =
        "Admin must provide a valid non-empty ownerId before joining owner groups.";

    public static string ToHubError(string code, string message) => $"{code}: {message}";
}
