// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary>Indicates that class bind a <see cref="IServiceProvider"/> instance.</summary>
public interface IServiceProviderBinding
{
    /// <summary>Attached <![CDATA[IService]]>. </summary>
    IServiceProvider ServiceProvider { get; set; }
}

