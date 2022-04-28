// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;
using System;

/// <summary>Indicates that field/property is not to be used comparison</summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class IgnoreCompareAttribute : Attribute { }
