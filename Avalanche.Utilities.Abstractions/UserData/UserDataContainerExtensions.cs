// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using Avalanche.Utilities;

/// <summary>Extension methods for <see cref="IUserDataContainer"/></summary>
public static class UserDataContainerExtensions
{
    /// <summary>Assign <paramref name="key"/> to <paramref name="value"/>.</summary>
    public static void SetUserData(this IUserDataContainer userDataContainer, string key, object? value)
    {
        userDataContainer.UserData[key] = value;
    }
    /// <summary>Assign <paramref name="key"/> to <paramref name="value"/>.</summary>
    public static S SetUserData<S>(this S userDataContainer, string key, object? value) where S : IUserDataContainer
    {
        userDataContainer.UserData[key] = value;
        return userDataContainer;
    }

    /* Use discouraged
    /// <summary>Assign user-data map. It is not recommended to assign explicit record, but to let the implementation lazy-create.</summary>
    /// <exception cref="InvalidOperationException">The implementation is allowed to throw in set</exception>
    public static S SetData<S>(this S userDatacontainer, IDictionary<string, object?> data) where S : IUserDataContainer
    {
        userDatacontainer.Data = data;
        return userDatacontainer;
    }
    */
}

