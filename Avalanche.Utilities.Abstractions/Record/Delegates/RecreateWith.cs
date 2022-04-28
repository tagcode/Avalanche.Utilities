// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System;

// <docs>
/// <summary>Create new instance of <paramref name="record"/> with value <paramref name="newValue"/>.</summary>
public delegate void RecreateWith<Record, Field>(ref Record record, Field newValue);
// </docs>
