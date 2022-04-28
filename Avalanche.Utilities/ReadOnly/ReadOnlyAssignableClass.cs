// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System.Collections.Concurrent;
using System.Runtime.Serialization;

/// <summary>Base class for class that can be assigned as read-only.</summary>
public abstract class ReadOnlyAssignableClass : IReadOnly
{
    /// <summary>Is read-only state</summary>
    [IgnoreDataMember] protected bool @readonly;
    /// <summary>Is read-only state</summary>
    [IgnoreDataMember] bool IReadOnly.ReadOnly { get => @readonly; set { if (@readonly == value) return; if (!value) throw new InvalidOperationException("Read-only"); if (value) setReadOnly(); } }
    /// <summary>Override this to modify assignment action.</summary>
    protected virtual void setReadOnly() => @readonly = true;

    /// <summary>Read-only assignable class with user-data container.</summary>
    public abstract class UserDataContainer : ReadOnlyAssignableClass, IUserDataContainer
    {
        /// <summary>User-data</summary>
        protected IDictionary<string, object?>? userdata;
        /// <summary>User-data</summary>
        public virtual IDictionary<string, object?> UserData { get => userdata!; set => this.AssertWritable().userdata = value; }
        /// <summary>Is <see cref="UserData"/> assigned.</summary>
        bool IUserDataContainer.HasUserData => userdata != null;
        /// <summary>Policy whether this implementation constructs <see cref="UserData"/> lazily.</summary>
        bool IUserDataContainer.UserDataInitializedOnGet => userDataInitializedOnGet;
        /// <summary>Policy whether this implementation constructs <see cref="UserData"/> lazily.</summary>
        protected virtual bool userDataInitializedOnGet => false;
    }

    /// <summary>Read-only assignable class with user-data container.</summary>
    public abstract class UserDataContainerLazyConstructed : UserDataContainer
    {
        /// <summary>Lock object for concurrency</summary>
        protected object mLock = new object();
        /// <summary>User-data</summary>
        public override IDictionary<string, object?> UserData { get => userdata ?? getOrCreateUserData(); set => this.AssertWritable().userdata = value; }
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
                // Copy read-only state
                ((IReadOnly)_userdata).ReadOnly = this.@readonly;
                // Return
                return _userdata;
            }
        }

        /// <summary>Assign into read-only state</summary>
        protected override void setReadOnly()
        {
            // Recurse
            base.setReadOnly();
            // Lock dictionary
            lock (mLock)
            {
                // Get reference again
                var _userdata = this.userdata;
                // Assign map
                if (_userdata != null) ((LockableDictionary<string, object?>)_userdata).SetReadOnly();
            }
        }
    }

}
