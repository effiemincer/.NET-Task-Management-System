

namespace BlApi;

/// <summary>
/// Class for creating the Business Logic layer
/// </summary>
public static class Factory
{
    public static IBl Get() => new BlImplementation.Bl();

}
