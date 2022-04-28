// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;
using System.Collections.Generic;

// <docs>
/// <summary>Interface for classes that can hold user-data.</summary>
public interface IUserDataContainer
{
    /// <summary>Is <see cref="UserData"/> assigned.</summary>
    bool HasUserData { get; }

    /// <summary>Policy whether this implementation constructs <see cref="UserData"/> lazily.</summary>
    bool UserDataInitializedOnGet { get; }

    /// <summary>User-data in concurrent safe map. Lazy constructed on get. If object set to read-only state, so is <see cref="UserData"/> map.</summary>
    /// <remarks>Setter use is discouraged. The implementation is allowed to prevent it by throwing <see cref="InvalidOperationException"/>.</remarks>
    /// <exception cref="InvalidOperationException">If modifying read-only object, or if class prohibits setting data object.</exception>
    IDictionary<string, object?> UserData { get; set; }
}
// </docs>
