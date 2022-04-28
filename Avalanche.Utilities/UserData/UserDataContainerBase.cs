// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Collections.Concurrent;

/// <summary>Read-only assignable class with user-data container.</summary>
public abstract class UserDataContainerBase : IUserDataContainer
{
    /// <summary>User-data</summary>
    protected IDictionary<string, object?>? userdata;
    /// <summary>User-data</summary>
    public virtual IDictionary<string, object?> UserData { get => userdata!; set => userdata = value; }
    /// <summary>Is <see cref="UserData"/> assigned.</summary>
    bool IUserDataContainer.HasUserData => userdata != null;
    /// <summary>Policy whether this implementation constructs <see cref="UserData"/> lazily.</summary>
    bool IUserDataContainer.UserDataInitializedOnGet => userDataInitializedOnGet;
    /// <summary>Policy whether this implementation constructs <see cref="UserData"/> lazily.</summary>
    protected virtual bool userDataInitializedOnGet => false;

    /// <summary>Read-only assignable class with user-data container.</summary>
    public abstract class LazyConstructed : UserDataContainerBase
    {
        /// <summary>Lock object for concurrency</summary>
        protected object mLock = new object();
        /// <summary>User-data</summary>
        public override IDictionary<string, object?> UserData { get => userdata ?? getOrCreateUserData(); set => this.userdata = value; }
        /// <summary>Policy whether this implementation constructs <see cref="UserData"/> lazily.</summary>
        protected override bool userDataInitializedOnGet => true;

        /// <summary>Get-or-create data</summary>
        protected virtual IDictionary<string, object?> getOrCreateUserData()
        {
            // Get reference
            var _userdata = this.userdata;
            // Got reference
            if (_userdata != null) return _userdata;
            lock (mLock)
            {
                // Get reference again
                _userdata = this.userdata;
                // Got reference
                if (_userdata != null) return _userdata;
                // Create 
                _userdata = this.userdata = new LockableDictionary<string, object?>(new ConcurrentDictionary<string, object?>());
                // Return
                return _userdata;
            }
        }
    }

}

