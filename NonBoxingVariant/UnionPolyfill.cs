// <copyright file="UnionTypeSupport.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

// Polyfills for the C# union types language feature so the generated LSP protocol types
// (which target net472/netstandard2.0) can compile against an SDK whose runtime declares
// these in System.Private.CoreLib. Mirrors the IsExternalInit polyfill pattern used elsewhere.
namespace System.Runtime.CompilerServices;

/// <summary>
/// Marks a struct emitted by the compiler as a union type.
/// </summary>
[AttributeUsage(AttributeTargets.Struct)]
internal sealed class UnionAttribute : Attribute
{
}

/// <summary>
/// Implemented by union types to expose the currently held value.
/// </summary>
internal interface IUnion
{
    /// <summary>
    /// Gets the value currently held by the union.
    /// </summary>
    object Value { get; }
}
