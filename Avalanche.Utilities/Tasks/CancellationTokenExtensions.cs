// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Utilities;

/// <summary>Extension methods for <see cref="CancellationToken"/></summary>
public static class CancellationTokenExtensions
{
    /// <summary>Unify <paramref name="t1"/> and <paramref name="t2"/>.</summary>
    /// <returns>Disposable source</returns>
    public static IDisposable? UnifyToken(this CancellationToken t1, CancellationToken t2, out CancellationToken unifiedToken)
    {
        // No tokens
        if (!t1.CanBeCanceled && !t2.CanBeCanceled) { unifiedToken = default; return null; }
        // Return t1
        if (t1.CanBeCanceled && !t2.CanBeCanceled) { unifiedToken = t1; return null; }
        // Return t2
        if (!t1.CanBeCanceled && t2.CanBeCanceled) { unifiedToken = t2; return null; }
        // Unify
        CancellationTokenSource src = CancellationTokenSource.CreateLinkedTokenSource(t1, t2);
        unifiedToken = src.Token;
        return src;
    }

    /// <summary>Unify 0 to n tokens</summary>
    public static CancellationToken UnifyTokens(IEnumerable<CancellationToken>? cancelTokens = null, IEnumerable<CancellationTokenSource>? cancelTokenSources = null, params CancellationToken[]? moreTokens)
    {
        //
        StructList4<CancellationToken> list = new();
        //
        if (cancelTokens != null)
        {
            //
            foreach (CancellationToken t in cancelTokens)
            {
                // Invalid token
                if (!t.CanBeCanceled) continue;
                //
                list.Add(t);
            }
        }
        //
        if (cancelTokenSources != null)
        {
            //
            foreach (CancellationTokenSource ts in cancelTokenSources)
            {
                //
                CancellationToken t = ts.Token;
                // Invalid token
                if (!t.CanBeCanceled) continue;
                //
                list.Add(t);
            }
        }
        //
        if (moreTokens != null)
        {
            //
            foreach (CancellationToken t in moreTokens)
            {
                // Invalid token
                if (!t.CanBeCanceled) continue;
                //
                list.Add(t);
            }
        }
        //
        if (list.Count == 0) return default;
        // 
        if (list.Count == 1) return list[0];
        //
        return CancellationTokenSource.CreateLinkedTokenSource(list.ToArray()).Token;
    }

}
