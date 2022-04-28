// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities.Record;
using System;

// <docs>
/// <summary>Assign new value to <paramref name="newValue"/>.</summary>
public delegate void FieldWrite<Record, Field>(ref Record record, Field newValue);
// </docs>
