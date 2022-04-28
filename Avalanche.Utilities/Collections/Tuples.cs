// -----------------------------------------------------------------
// Copyright:      Toni Kalajainen
// Date:           20.8.2018
// -----------------------------------------------------------------
using System;
using System.Text;
using System.Collections.Generic;

namespace Avalanche.Utilities
{
    /// <summary>Container (1-tuple). Hashcode is cached. Elements are immutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    public struct Container<A> : IEquatable<Container<A>>, IComparable<Container<A>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Container (1-tuple).</summary>
        /// <param name="a"></param>
        public Container(A a) { this.a = a; hashcode = (a==null?0:11*a.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is Container<A> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(Container<A> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(Container<A> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<Container<A>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(Container<A> x, Container<A> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(Container<A> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<Container<A>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(Container<A> x, Container<A> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);

            sb.Append(")");
        }
    }
    /// <summary>Container (1-tuple). Hashcode is cached. Elements are immutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    public class ContainerObject<A> : IEquatable<ContainerObject<A>>, IComparable<ContainerObject<A>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Container (1-tuple).</summary>
        public ContainerObject() { a = default!; hashcode = (a==null?0:11*a.GetHashCode()); }
        /// <summary>Create Container (1-tuple).</summary>
        /// <param name="a"></param>
        public ContainerObject(A a) { this.a = a; hashcode = (a==null?0:11*a.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is ContainerObject<A> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(ContainerObject<A>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(ContainerObject<A>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<ContainerObject<A>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(ContainerObject<A>? x, ContainerObject<A>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(ContainerObject<A> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<ContainerObject<A>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(ContainerObject<A>? x, ContainerObject<A>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);

            sb.Append(")");
        }
    }
    /// <summary>Container (1-tuple). Hashcode is not cached. Elements are mutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    public struct ContainerMutable<A> : IEquatable<ContainerMutable<A>>, IComparable<ContainerMutable<A>>
    {
        /// <summary>A</summary>
        public A a;
        
        
        /// <summary>Create Container (1-tuple).</summary>
        /// <param name="a"></param>
        public ContainerMutable(A a) { this.a = a; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is ContainerMutable<A> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(ContainerMutable<A> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(ContainerMutable<A> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<ContainerMutable<A>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(ContainerMutable<A> x, ContainerMutable<A> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(ContainerMutable<A> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<ContainerMutable<A>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(ContainerMutable<A> x, ContainerMutable<A> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);

            sb.Append(")");
        }
    }
    /// <summary>Container (1-tuple). Hashcode is not cached. Elements are mutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    public class ContainerMutableObject<A> : IEquatable<ContainerMutableObject<A>>, IComparable<ContainerMutableObject<A>>
    {
        /// <summary>A</summary>
        public A a;
        
        
        /// <summary>Create Container (1-tuple).</summary>
        public ContainerMutableObject() { a = default!; }
        /// <summary>Create Container (1-tuple).</summary>
        /// <param name="a"></param>
        public ContainerMutableObject(A a) { this.a = a; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is ContainerMutableObject<A> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(ContainerMutableObject<A>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(ContainerMutableObject<A>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<ContainerMutableObject<A>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(ContainerMutableObject<A>? x, ContainerMutableObject<A>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(ContainerMutableObject<A> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<ContainerMutableObject<A>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(ContainerMutableObject<A>? x, ContainerMutableObject<A>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);

            sb.Append(")");
        }
    }
    /// <summary>Pair (2-tuple). Hashcode is cached. Elements are immutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    public struct Pair<A, B> : IEquatable<Pair<A, B>>, IComparable<Pair<A, B>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>B</summary>
        public readonly B b;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Pair (2-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public Pair(A a, B b) { this.a = a; this.b = b; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is Pair<A, B> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(Pair<A, B> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(Pair<A, B> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<Pair<A, B>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(Pair<A, B> x, Pair<A, B> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(Pair<A, B> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<Pair<A, B>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(Pair<A, B> x, Pair<A, B> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);

            sb.Append(")");
        }
    }
    /// <summary>Pair (2-tuple). Hashcode is cached. Elements are immutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    public class PairObject<A, B> : IEquatable<PairObject<A, B>>, IComparable<PairObject<A, B>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>B</summary>
        public readonly B b;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Pair (2-tuple).</summary>
        public PairObject() { a = default!; b = default!; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()); }
        /// <summary>Create Pair (2-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public PairObject(A a, B b) { this.a = a; this.b = b; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is PairObject<A, B> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(PairObject<A, B>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(PairObject<A, B>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<PairObject<A, B>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(PairObject<A, B>? x, PairObject<A, B>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(PairObject<A, B> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<PairObject<A, B>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(PairObject<A, B>? x, PairObject<A, B>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);

            sb.Append(")");
        }
    }
    /// <summary>Pair (2-tuple). Hashcode is not cached. Elements are mutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    public struct PairMutable<A, B> : IEquatable<PairMutable<A, B>>, IComparable<PairMutable<A, B>>
    {
        /// <summary>A</summary>
        public A a;
        /// <summary>B</summary>
        public B b;
        
        
        /// <summary>Create Pair (2-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public PairMutable(A a, B b) { this.a = a; this.b = b; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is PairMutable<A, B> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(PairMutable<A, B> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(PairMutable<A, B> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<PairMutable<A, B>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(PairMutable<A, B> x, PairMutable<A, B> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(PairMutable<A, B> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<PairMutable<A, B>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(PairMutable<A, B> x, PairMutable<A, B> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);

            sb.Append(")");
        }
    }
    /// <summary>Pair (2-tuple). Hashcode is not cached. Elements are mutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    public class PairMutableObject<A, B> : IEquatable<PairMutableObject<A, B>>, IComparable<PairMutableObject<A, B>>
    {
        /// <summary>A</summary>
        public A a;
        /// <summary>B</summary>
        public B b;
        
        
        /// <summary>Create Pair (2-tuple).</summary>
        public PairMutableObject() { a = default!; b = default!; }
        /// <summary>Create Pair (2-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public PairMutableObject(A a, B b) { this.a = a; this.b = b; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is PairMutableObject<A, B> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(PairMutableObject<A, B>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(PairMutableObject<A, B>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<PairMutableObject<A, B>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(PairMutableObject<A, B>? x, PairMutableObject<A, B>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(PairMutableObject<A, B> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<PairMutableObject<A, B>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(PairMutableObject<A, B>? x, PairMutableObject<A, B>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);

            sb.Append(")");
        }
    }

    /// <summary>Unordered Pair (2-tuple) where elements are interchangeable. Hashcode is cached. Elements are immutable. Type is stack allocated. </summary>
    /// <typeparam name="T"></typeparam>
    public struct PairSet<T> : IEquatable<PairSet<T>>
    {
        /// <summary>A</summary>
        public readonly T a;
        /// <summary>B</summary>
        public readonly T b;

        /// <summary>Hash-code</summary>
        readonly int hashcode;

        /// <summary>Create Pair (2-tuple).</summary>
        public PairSet(T a, T b) { this.a = a; this.b = b; hashcode = (a==null?0:a.GetHashCode()) ^ (b==null?0:b.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is PairSet<T> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(PairSet<T> other) => EqualityComparer.Instance.Equals(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<PairSet<T>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer(EqualityComparer<T>.Default));

            /// <summary>Element comparer.</summary>
            public readonly IEqualityComparer<T> Comparer;

            /// <summary>Create equality comparer.</summary>
            /// <param name="comparer">(optional) elemen comparer</param>
            public EqualityComparer(IEqualityComparer<T> comparer)
            {
                this.Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            /// <summary>Test equality of <paramref name="x"/> to <paramref name="y"/></summary>
            public bool Equals(PairSet<T> x, PairSet<T> y)
            {

                // 0 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b)) return true;
                // 1 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a)) return true;

                return false;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(PairSet<T> obj) => (obj.a==null?0:Comparer.GetHashCode(obj.a)) ^ (obj.b==null?0:Comparer.GetHashCode(obj.b));
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");

            sb.Append(b);

            sb.Append(")");
        }
    }
    

    /// <summary>Unordered Pair (2-tuple) where elements are interchangeable. Hashcode is cached. Elements are immutable. Type is heap allocated. </summary>
    /// <typeparam name="T"></typeparam>
    public class PairSetObject<T> : IEquatable<PairSetObject<T>>
    {
        /// <summary>A</summary>
        public readonly T a;
        /// <summary>B</summary>
        public readonly T b;

        /// <summary>Hash-code</summary>
        readonly int hashcode;

        /// <summary>Create Pair (2-tuple).</summary>
        public PairSetObject() { a = default!; b = default!; hashcode = (a==null?0:a.GetHashCode()) ^ (b==null?0:b.GetHashCode()); }

        /// <summary>Create Pair (2-tuple).</summary>
        public PairSetObject(T a, T b) { this.a = a; this.b = b; hashcode = (a==null?0:a.GetHashCode()) ^ (b==null?0:b.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is PairSetObject<T> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(PairSetObject<T>? other) => EqualityComparer.Instance.Equals(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<PairSetObject<T>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer(EqualityComparer<T>.Default));

            /// <summary>Element comparer.</summary>
            public readonly IEqualityComparer<T> Comparer;

            /// <summary>Create equality comparer.</summary>
            /// <param name="comparer">(optional) elemen comparer</param>
            public EqualityComparer(IEqualityComparer<T> comparer)
            {
                this.Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            /// <summary>Test equality of <paramref name="x"/> to <paramref name="y"/></summary>
            public bool Equals(PairSetObject<T>? x, PairSetObject<T>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                // 0 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b)) return true;
                // 1 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a)) return true;

                return false;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(PairSetObject<T> obj) => obj == null ? 0 : (obj.a==null?0:Comparer.GetHashCode(obj.a)) ^ (obj.b==null?0:Comparer.GetHashCode(obj.b));
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");

            sb.Append(b);

            sb.Append(")");
        }
    }
    

    /// <summary>Unordered Pair (2-tuple) where elements are interchangeable. Hashcode is not cached. Elements are mutable. Type is stack allocated. </summary>
    /// <typeparam name="T"></typeparam>
    public struct PairSetMutable<T> : IEquatable<PairSetMutable<T>>
    {
        /// <summary>A</summary>
        public  T a;
        /// <summary>B</summary>
        public  T b;


        /// <summary>Create Pair (2-tuple).</summary>
        public PairSetMutable(T a, T b) { this.a = a; this.b = b; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is PairSetMutable<T> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(PairSetMutable<T> other) => EqualityComparer.Instance.Equals(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<PairSetMutable<T>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer(EqualityComparer<T>.Default));

            /// <summary>Element comparer.</summary>
            public readonly IEqualityComparer<T> Comparer;

            /// <summary>Create equality comparer.</summary>
            /// <param name="comparer">(optional) elemen comparer</param>
            public EqualityComparer(IEqualityComparer<T> comparer)
            {
                this.Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            /// <summary>Test equality of <paramref name="x"/> to <paramref name="y"/></summary>
            public bool Equals(PairSetMutable<T> x, PairSetMutable<T> y)
            {

                // 0 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b)) return true;
                // 1 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a)) return true;

                return false;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(PairSetMutable<T> obj) => (obj.a==null?0:Comparer.GetHashCode(obj.a)) ^ (obj.b==null?0:Comparer.GetHashCode(obj.b));
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");

            sb.Append(b);

            sb.Append(")");
        }
    }
    

    /// <summary>Unordered Pair (2-tuple) where elements are interchangeable. Hashcode is not cached. Elements are mutable. Type is heap allocated. </summary>
    /// <typeparam name="T"></typeparam>
    public class PairSetMutableObject<T> : IEquatable<PairSetMutableObject<T>>
    {
        /// <summary>A</summary>
        public  T a;
        /// <summary>B</summary>
        public  T b;


        /// <summary>Create Pair (2-tuple).</summary>
        public PairSetMutableObject() { a = default!; b = default!; }

        /// <summary>Create Pair (2-tuple).</summary>
        public PairSetMutableObject(T a, T b) { this.a = a; this.b = b; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is PairSetMutableObject<T> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(PairSetMutableObject<T>? other) => EqualityComparer.Instance.Equals(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<PairSetMutableObject<T>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer(EqualityComparer<T>.Default));

            /// <summary>Element comparer.</summary>
            public readonly IEqualityComparer<T> Comparer;

            /// <summary>Create equality comparer.</summary>
            /// <param name="comparer">(optional) elemen comparer</param>
            public EqualityComparer(IEqualityComparer<T> comparer)
            {
                this.Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            /// <summary>Test equality of <paramref name="x"/> to <paramref name="y"/></summary>
            public bool Equals(PairSetMutableObject<T>? x, PairSetMutableObject<T>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                // 0 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b)) return true;
                // 1 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a)) return true;

                return false;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(PairSetMutableObject<T> obj) => obj == null ? 0 : (obj.a==null?0:Comparer.GetHashCode(obj.a)) ^ (obj.b==null?0:Comparer.GetHashCode(obj.b));
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");

            sb.Append(b);

            sb.Append(")");
        }
    }
    
    /// <summary>Triple (3-tuple). Hashcode is cached. Elements are immutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    public struct Triple<A, B, C> : IEquatable<Triple<A, B, C>>, IComparable<Triple<A, B, C>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>B</summary>
        public readonly B b;
        /// <summary>C</summary>
        public readonly C c;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Triple (3-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public Triple(A a, B b, C c) { this.a = a; this.b = b; this.c = c; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is Triple<A, B, C> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(Triple<A, B, C> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(Triple<A, B, C> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<Triple<A, B, C>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(Triple<A, B, C> x, Triple<A, B, C> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(Triple<A, B, C> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<Triple<A, B, C>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(Triple<A, B, C> x, Triple<A, B, C> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);

            sb.Append(")");
        }
    }
    /// <summary>Triple (3-tuple). Hashcode is cached. Elements are immutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    public class TripleObject<A, B, C> : IEquatable<TripleObject<A, B, C>>, IComparable<TripleObject<A, B, C>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>B</summary>
        public readonly B b;
        /// <summary>C</summary>
        public readonly C c;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Triple (3-tuple).</summary>
        public TripleObject() { a = default!; b = default!; c = default!; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()); }
        /// <summary>Create Triple (3-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public TripleObject(A a, B b, C c) { this.a = a; this.b = b; this.c = c; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is TripleObject<A, B, C> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(TripleObject<A, B, C>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(TripleObject<A, B, C>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<TripleObject<A, B, C>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(TripleObject<A, B, C>? x, TripleObject<A, B, C>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(TripleObject<A, B, C> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<TripleObject<A, B, C>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(TripleObject<A, B, C>? x, TripleObject<A, B, C>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);

            sb.Append(")");
        }
    }
    /// <summary>Triple (3-tuple). Hashcode is not cached. Elements are mutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    public struct TripleMutable<A, B, C> : IEquatable<TripleMutable<A, B, C>>, IComparable<TripleMutable<A, B, C>>
    {
        /// <summary>A</summary>
        public A a;
        /// <summary>B</summary>
        public B b;
        /// <summary>C</summary>
        public C c;
        
        
        /// <summary>Create Triple (3-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public TripleMutable(A a, B b, C c) { this.a = a; this.b = b; this.c = c; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is TripleMutable<A, B, C> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(TripleMutable<A, B, C> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(TripleMutable<A, B, C> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<TripleMutable<A, B, C>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(TripleMutable<A, B, C> x, TripleMutable<A, B, C> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(TripleMutable<A, B, C> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<TripleMutable<A, B, C>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(TripleMutable<A, B, C> x, TripleMutable<A, B, C> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);

            sb.Append(")");
        }
    }
    /// <summary>Triple (3-tuple). Hashcode is not cached. Elements are mutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    public class TripleMutableObject<A, B, C> : IEquatable<TripleMutableObject<A, B, C>>, IComparable<TripleMutableObject<A, B, C>>
    {
        /// <summary>A</summary>
        public A a;
        /// <summary>B</summary>
        public B b;
        /// <summary>C</summary>
        public C c;
        
        
        /// <summary>Create Triple (3-tuple).</summary>
        public TripleMutableObject() { a = default!; b = default!; c = default!; }
        /// <summary>Create Triple (3-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public TripleMutableObject(A a, B b, C c) { this.a = a; this.b = b; this.c = c; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is TripleMutableObject<A, B, C> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(TripleMutableObject<A, B, C>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(TripleMutableObject<A, B, C>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<TripleMutableObject<A, B, C>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(TripleMutableObject<A, B, C>? x, TripleMutableObject<A, B, C>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(TripleMutableObject<A, B, C> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<TripleMutableObject<A, B, C>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(TripleMutableObject<A, B, C>? x, TripleMutableObject<A, B, C>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);

            sb.Append(")");
        }
    }

    /// <summary>Unordered Triple (3-tuple) where elements are interchangeable. Hashcode is cached. Elements are immutable. Type is stack allocated. </summary>
    /// <typeparam name="T"></typeparam>
    public struct TripleSet<T> : IEquatable<TripleSet<T>>
    {
        /// <summary>A</summary>
        public readonly T a;
        /// <summary>B</summary>
        public readonly T b;
        /// <summary>C</summary>
        public readonly T c;

        /// <summary>Hash-code</summary>
        readonly int hashcode;

        /// <summary>Create Triple (3-tuple).</summary>
        public TripleSet(T a, T b, T c) { this.a = a; this.b = b; this.c = c; hashcode = (a==null?0:a.GetHashCode()) ^ (b==null?0:b.GetHashCode()) ^ (c==null?0:c.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is TripleSet<T> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(TripleSet<T> other) => EqualityComparer.Instance.Equals(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<TripleSet<T>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer(EqualityComparer<T>.Default));

            /// <summary>Element comparer.</summary>
            public readonly IEqualityComparer<T> Comparer;

            /// <summary>Create equality comparer.</summary>
            /// <param name="comparer">(optional) elemen comparer</param>
            public EqualityComparer(IEqualityComparer<T> comparer)
            {
                this.Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            /// <summary>Test equality of <paramref name="x"/> to <paramref name="y"/></summary>
            public bool Equals(TripleSet<T> x, TripleSet<T> y)
            {

                // 0 && 1 && 2
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.c)) return true;
                // 0 && 2 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.b)) return true;
                // 1 && 0 && 2
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.c)) return true;
                // 1 && 2 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.a)) return true;
                // 2 && 1 && 0
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.a)) return true;
                // 2 && 0 && 1
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.b)) return true;

                return false;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(TripleSet<T> obj) => (obj.a==null?0:Comparer.GetHashCode(obj.a)) ^ (obj.b==null?0:Comparer.GetHashCode(obj.b)) ^ (obj.c==null?0:Comparer.GetHashCode(obj.c));
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");

            sb.Append(b);
            sb.Append(", ");

            sb.Append(c);

            sb.Append(")");
        }
    }
    

    /// <summary>Unordered Triple (3-tuple) where elements are interchangeable. Hashcode is cached. Elements are immutable. Type is heap allocated. </summary>
    /// <typeparam name="T"></typeparam>
    public class TripleSetObject<T> : IEquatable<TripleSetObject<T>>
    {
        /// <summary>A</summary>
        public readonly T a;
        /// <summary>B</summary>
        public readonly T b;
        /// <summary>C</summary>
        public readonly T c;

        /// <summary>Hash-code</summary>
        readonly int hashcode;

        /// <summary>Create Triple (3-tuple).</summary>
        public TripleSetObject() { a = default!; b = default!; c = default!; hashcode = (a==null?0:a.GetHashCode()) ^ (b==null?0:b.GetHashCode()) ^ (c==null?0:c.GetHashCode()); }

        /// <summary>Create Triple (3-tuple).</summary>
        public TripleSetObject(T a, T b, T c) { this.a = a; this.b = b; this.c = c; hashcode = (a==null?0:a.GetHashCode()) ^ (b==null?0:b.GetHashCode()) ^ (c==null?0:c.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is TripleSetObject<T> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(TripleSetObject<T>? other) => EqualityComparer.Instance.Equals(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<TripleSetObject<T>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer(EqualityComparer<T>.Default));

            /// <summary>Element comparer.</summary>
            public readonly IEqualityComparer<T> Comparer;

            /// <summary>Create equality comparer.</summary>
            /// <param name="comparer">(optional) elemen comparer</param>
            public EqualityComparer(IEqualityComparer<T> comparer)
            {
                this.Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            /// <summary>Test equality of <paramref name="x"/> to <paramref name="y"/></summary>
            public bool Equals(TripleSetObject<T>? x, TripleSetObject<T>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                // 0 && 1 && 2
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.c)) return true;
                // 0 && 2 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.b)) return true;
                // 1 && 0 && 2
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.c)) return true;
                // 1 && 2 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.a)) return true;
                // 2 && 1 && 0
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.a)) return true;
                // 2 && 0 && 1
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.b)) return true;

                return false;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(TripleSetObject<T> obj) => obj == null ? 0 : (obj.a==null?0:Comparer.GetHashCode(obj.a)) ^ (obj.b==null?0:Comparer.GetHashCode(obj.b)) ^ (obj.c==null?0:Comparer.GetHashCode(obj.c));
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");

            sb.Append(b);
            sb.Append(", ");

            sb.Append(c);

            sb.Append(")");
        }
    }
    

    /// <summary>Unordered Triple (3-tuple) where elements are interchangeable. Hashcode is not cached. Elements are mutable. Type is stack allocated. </summary>
    /// <typeparam name="T"></typeparam>
    public struct TripleSetMutable<T> : IEquatable<TripleSetMutable<T>>
    {
        /// <summary>A</summary>
        public  T a;
        /// <summary>B</summary>
        public  T b;
        /// <summary>C</summary>
        public  T c;


        /// <summary>Create Triple (3-tuple).</summary>
        public TripleSetMutable(T a, T b, T c) { this.a = a; this.b = b; this.c = c; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is TripleSetMutable<T> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(TripleSetMutable<T> other) => EqualityComparer.Instance.Equals(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<TripleSetMutable<T>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer(EqualityComparer<T>.Default));

            /// <summary>Element comparer.</summary>
            public readonly IEqualityComparer<T> Comparer;

            /// <summary>Create equality comparer.</summary>
            /// <param name="comparer">(optional) elemen comparer</param>
            public EqualityComparer(IEqualityComparer<T> comparer)
            {
                this.Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            /// <summary>Test equality of <paramref name="x"/> to <paramref name="y"/></summary>
            public bool Equals(TripleSetMutable<T> x, TripleSetMutable<T> y)
            {

                // 0 && 1 && 2
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.c)) return true;
                // 0 && 2 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.b)) return true;
                // 1 && 0 && 2
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.c)) return true;
                // 1 && 2 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.a)) return true;
                // 2 && 1 && 0
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.a)) return true;
                // 2 && 0 && 1
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.b)) return true;

                return false;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(TripleSetMutable<T> obj) => (obj.a==null?0:Comparer.GetHashCode(obj.a)) ^ (obj.b==null?0:Comparer.GetHashCode(obj.b)) ^ (obj.c==null?0:Comparer.GetHashCode(obj.c));
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");

            sb.Append(b);
            sb.Append(", ");

            sb.Append(c);

            sb.Append(")");
        }
    }
    

    /// <summary>Unordered Triple (3-tuple) where elements are interchangeable. Hashcode is not cached. Elements are mutable. Type is heap allocated. </summary>
    /// <typeparam name="T"></typeparam>
    public class TripleSetMutableObject<T> : IEquatable<TripleSetMutableObject<T>>
    {
        /// <summary>A</summary>
        public  T a;
        /// <summary>B</summary>
        public  T b;
        /// <summary>C</summary>
        public  T c;


        /// <summary>Create Triple (3-tuple).</summary>
        public TripleSetMutableObject() { a = default!; b = default!; c = default!; }

        /// <summary>Create Triple (3-tuple).</summary>
        public TripleSetMutableObject(T a, T b, T c) { this.a = a; this.b = b; this.c = c; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is TripleSetMutableObject<T> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(TripleSetMutableObject<T>? other) => EqualityComparer.Instance.Equals(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<TripleSetMutableObject<T>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer(EqualityComparer<T>.Default));

            /// <summary>Element comparer.</summary>
            public readonly IEqualityComparer<T> Comparer;

            /// <summary>Create equality comparer.</summary>
            /// <param name="comparer">(optional) elemen comparer</param>
            public EqualityComparer(IEqualityComparer<T> comparer)
            {
                this.Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            /// <summary>Test equality of <paramref name="x"/> to <paramref name="y"/></summary>
            public bool Equals(TripleSetMutableObject<T>? x, TripleSetMutableObject<T>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                // 0 && 1 && 2
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.c)) return true;
                // 0 && 2 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.b)) return true;
                // 1 && 0 && 2
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.c)) return true;
                // 1 && 2 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.a)) return true;
                // 2 && 1 && 0
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.a)) return true;
                // 2 && 0 && 1
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.b)) return true;

                return false;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(TripleSetMutableObject<T> obj) => obj == null ? 0 : (obj.a==null?0:Comparer.GetHashCode(obj.a)) ^ (obj.b==null?0:Comparer.GetHashCode(obj.b)) ^ (obj.c==null?0:Comparer.GetHashCode(obj.c));
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");

            sb.Append(b);
            sb.Append(", ");

            sb.Append(c);

            sb.Append(")");
        }
    }
    
    /// <summary>Quad (4-tuple). Hashcode is cached. Elements are immutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    public struct Quad<A, B, C, D> : IEquatable<Quad<A, B, C, D>>, IComparable<Quad<A, B, C, D>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>B</summary>
        public readonly B b;
        /// <summary>C</summary>
        public readonly C c;
        /// <summary>D</summary>
        public readonly D d;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Quad (4-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public Quad(A a, B b, C c, D d) { this.a = a; this.b = b; this.c = c; this.d = d; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is Quad<A, B, C, D> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(Quad<A, B, C, D> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(Quad<A, B, C, D> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<Quad<A, B, C, D>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(Quad<A, B, C, D> x, Quad<A, B, C, D> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(Quad<A, B, C, D> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<Quad<A, B, C, D>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(Quad<A, B, C, D> x, Quad<A, B, C, D> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);

            sb.Append(")");
        }
    }
    /// <summary>Quad (4-tuple). Hashcode is cached. Elements are immutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    public class QuadObject<A, B, C, D> : IEquatable<QuadObject<A, B, C, D>>, IComparable<QuadObject<A, B, C, D>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>B</summary>
        public readonly B b;
        /// <summary>C</summary>
        public readonly C c;
        /// <summary>D</summary>
        public readonly D d;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Quad (4-tuple).</summary>
        public QuadObject() { a = default!; b = default!; c = default!; d = default!; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()); }
        /// <summary>Create Quad (4-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public QuadObject(A a, B b, C c, D d) { this.a = a; this.b = b; this.c = c; this.d = d; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is QuadObject<A, B, C, D> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(QuadObject<A, B, C, D>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(QuadObject<A, B, C, D>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<QuadObject<A, B, C, D>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(QuadObject<A, B, C, D>? x, QuadObject<A, B, C, D>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(QuadObject<A, B, C, D> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<QuadObject<A, B, C, D>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(QuadObject<A, B, C, D>? x, QuadObject<A, B, C, D>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);

            sb.Append(")");
        }
    }
    /// <summary>Quad (4-tuple). Hashcode is not cached. Elements are mutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    public struct QuadMutable<A, B, C, D> : IEquatable<QuadMutable<A, B, C, D>>, IComparable<QuadMutable<A, B, C, D>>
    {
        /// <summary>A</summary>
        public A a;
        /// <summary>B</summary>
        public B b;
        /// <summary>C</summary>
        public C c;
        /// <summary>D</summary>
        public D d;
        
        
        /// <summary>Create Quad (4-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public QuadMutable(A a, B b, C c, D d) { this.a = a; this.b = b; this.c = c; this.d = d; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is QuadMutable<A, B, C, D> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(QuadMutable<A, B, C, D> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(QuadMutable<A, B, C, D> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<QuadMutable<A, B, C, D>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(QuadMutable<A, B, C, D> x, QuadMutable<A, B, C, D> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(QuadMutable<A, B, C, D> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<QuadMutable<A, B, C, D>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(QuadMutable<A, B, C, D> x, QuadMutable<A, B, C, D> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);

            sb.Append(")");
        }
    }
    /// <summary>Quad (4-tuple). Hashcode is not cached. Elements are mutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    public class QuadMutableObject<A, B, C, D> : IEquatable<QuadMutableObject<A, B, C, D>>, IComparable<QuadMutableObject<A, B, C, D>>
    {
        /// <summary>A</summary>
        public A a;
        /// <summary>B</summary>
        public B b;
        /// <summary>C</summary>
        public C c;
        /// <summary>D</summary>
        public D d;
        
        
        /// <summary>Create Quad (4-tuple).</summary>
        public QuadMutableObject() { a = default!; b = default!; c = default!; d = default!; }
        /// <summary>Create Quad (4-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public QuadMutableObject(A a, B b, C c, D d) { this.a = a; this.b = b; this.c = c; this.d = d; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is QuadMutableObject<A, B, C, D> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(QuadMutableObject<A, B, C, D>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(QuadMutableObject<A, B, C, D>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<QuadMutableObject<A, B, C, D>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(QuadMutableObject<A, B, C, D>? x, QuadMutableObject<A, B, C, D>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(QuadMutableObject<A, B, C, D> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<QuadMutableObject<A, B, C, D>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(QuadMutableObject<A, B, C, D>? x, QuadMutableObject<A, B, C, D>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);

            sb.Append(")");
        }
    }

    /// <summary>Unordered Quad (4-tuple) where elements are interchangeable. Hashcode is cached. Elements are immutable. Type is stack allocated. </summary>
    /// <typeparam name="T"></typeparam>
    public struct QuadSet<T> : IEquatable<QuadSet<T>>
    {
        /// <summary>A</summary>
        public readonly T a;
        /// <summary>B</summary>
        public readonly T b;
        /// <summary>C</summary>
        public readonly T c;
        /// <summary>D</summary>
        public readonly T d;

        /// <summary>Hash-code</summary>
        readonly int hashcode;

        /// <summary>Create Quad (4-tuple).</summary>
        public QuadSet(T a, T b, T c, T d) { this.a = a; this.b = b; this.c = c; this.d = d; hashcode = (a==null?0:a.GetHashCode()) ^ (b==null?0:b.GetHashCode()) ^ (c==null?0:c.GetHashCode()) ^ (d==null?0:d.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is QuadSet<T> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(QuadSet<T> other) => EqualityComparer.Instance.Equals(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<QuadSet<T>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer(EqualityComparer<T>.Default));

            /// <summary>Element comparer.</summary>
            public readonly IEqualityComparer<T> Comparer;

            /// <summary>Create equality comparer.</summary>
            /// <param name="comparer">(optional) elemen comparer</param>
            public EqualityComparer(IEqualityComparer<T> comparer)
            {
                this.Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            /// <summary>Test equality of <paramref name="x"/> to <paramref name="y"/></summary>
            public bool Equals(QuadSet<T> x, QuadSet<T> y)
            {

                // 0 && 1 && 2 && 3
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.d)) return true;
                // 0 && 1 && 3 && 2
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.c)) return true;
                // 0 && 2 && 1 && 3
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.d)) return true;
                // 0 && 2 && 3 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.b)) return true;
                // 0 && 3 && 2 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.b)) return true;
                // 0 && 3 && 1 && 2
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.c)) return true;
                // 1 && 0 && 2 && 3
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.d)) return true;
                // 1 && 0 && 3 && 2
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.c)) return true;
                // 1 && 2 && 0 && 3
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.d)) return true;
                // 1 && 2 && 3 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.a)) return true;
                // 1 && 3 && 2 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.a)) return true;
                // 1 && 3 && 0 && 2
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.c)) return true;
                // 2 && 1 && 0 && 3
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.d)) return true;
                // 2 && 1 && 3 && 0
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.a)) return true;
                // 2 && 0 && 1 && 3
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.d)) return true;
                // 2 && 0 && 3 && 1
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.b)) return true;
                // 2 && 3 && 0 && 1
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.b)) return true;
                // 2 && 3 && 1 && 0
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.a)) return true;
                // 3 && 1 && 2 && 0
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.a)) return true;
                // 3 && 1 && 0 && 2
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.c)) return true;
                // 3 && 2 && 1 && 0
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.a)) return true;
                // 3 && 2 && 0 && 1
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.b)) return true;
                // 3 && 0 && 2 && 1
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.b)) return true;
                // 3 && 0 && 1 && 2
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.c)) return true;

                return false;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(QuadSet<T> obj) => (obj.a==null?0:Comparer.GetHashCode(obj.a)) ^ (obj.b==null?0:Comparer.GetHashCode(obj.b)) ^ (obj.c==null?0:Comparer.GetHashCode(obj.c)) ^ (obj.d==null?0:Comparer.GetHashCode(obj.d));
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");

            sb.Append(b);
            sb.Append(", ");

            sb.Append(c);
            sb.Append(", ");

            sb.Append(d);

            sb.Append(")");
        }
    }
    

    /// <summary>Unordered Quad (4-tuple) where elements are interchangeable. Hashcode is cached. Elements are immutable. Type is heap allocated. </summary>
    /// <typeparam name="T"></typeparam>
    public class QuadSetObject<T> : IEquatable<QuadSetObject<T>>
    {
        /// <summary>A</summary>
        public readonly T a;
        /// <summary>B</summary>
        public readonly T b;
        /// <summary>C</summary>
        public readonly T c;
        /// <summary>D</summary>
        public readonly T d;

        /// <summary>Hash-code</summary>
        readonly int hashcode;

        /// <summary>Create Quad (4-tuple).</summary>
        public QuadSetObject() { a = default!; b = default!; c = default!; d = default!; hashcode = (a==null?0:a.GetHashCode()) ^ (b==null?0:b.GetHashCode()) ^ (c==null?0:c.GetHashCode()) ^ (d==null?0:d.GetHashCode()); }

        /// <summary>Create Quad (4-tuple).</summary>
        public QuadSetObject(T a, T b, T c, T d) { this.a = a; this.b = b; this.c = c; this.d = d; hashcode = (a==null?0:a.GetHashCode()) ^ (b==null?0:b.GetHashCode()) ^ (c==null?0:c.GetHashCode()) ^ (d==null?0:d.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is QuadSetObject<T> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(QuadSetObject<T>? other) => EqualityComparer.Instance.Equals(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<QuadSetObject<T>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer(EqualityComparer<T>.Default));

            /// <summary>Element comparer.</summary>
            public readonly IEqualityComparer<T> Comparer;

            /// <summary>Create equality comparer.</summary>
            /// <param name="comparer">(optional) elemen comparer</param>
            public EqualityComparer(IEqualityComparer<T> comparer)
            {
                this.Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            /// <summary>Test equality of <paramref name="x"/> to <paramref name="y"/></summary>
            public bool Equals(QuadSetObject<T>? x, QuadSetObject<T>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                // 0 && 1 && 2 && 3
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.d)) return true;
                // 0 && 1 && 3 && 2
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.c)) return true;
                // 0 && 2 && 1 && 3
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.d)) return true;
                // 0 && 2 && 3 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.b)) return true;
                // 0 && 3 && 2 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.b)) return true;
                // 0 && 3 && 1 && 2
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.c)) return true;
                // 1 && 0 && 2 && 3
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.d)) return true;
                // 1 && 0 && 3 && 2
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.c)) return true;
                // 1 && 2 && 0 && 3
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.d)) return true;
                // 1 && 2 && 3 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.a)) return true;
                // 1 && 3 && 2 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.a)) return true;
                // 1 && 3 && 0 && 2
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.c)) return true;
                // 2 && 1 && 0 && 3
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.d)) return true;
                // 2 && 1 && 3 && 0
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.a)) return true;
                // 2 && 0 && 1 && 3
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.d)) return true;
                // 2 && 0 && 3 && 1
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.b)) return true;
                // 2 && 3 && 0 && 1
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.b)) return true;
                // 2 && 3 && 1 && 0
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.a)) return true;
                // 3 && 1 && 2 && 0
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.a)) return true;
                // 3 && 1 && 0 && 2
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.c)) return true;
                // 3 && 2 && 1 && 0
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.a)) return true;
                // 3 && 2 && 0 && 1
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.b)) return true;
                // 3 && 0 && 2 && 1
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.b)) return true;
                // 3 && 0 && 1 && 2
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.c)) return true;

                return false;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(QuadSetObject<T> obj) => obj == null ? 0 : (obj.a==null?0:Comparer.GetHashCode(obj.a)) ^ (obj.b==null?0:Comparer.GetHashCode(obj.b)) ^ (obj.c==null?0:Comparer.GetHashCode(obj.c)) ^ (obj.d==null?0:Comparer.GetHashCode(obj.d));
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");

            sb.Append(b);
            sb.Append(", ");

            sb.Append(c);
            sb.Append(", ");

            sb.Append(d);

            sb.Append(")");
        }
    }
    

    /// <summary>Unordered Quad (4-tuple) where elements are interchangeable. Hashcode is not cached. Elements are mutable. Type is stack allocated. </summary>
    /// <typeparam name="T"></typeparam>
    public struct QuadSetMutable<T> : IEquatable<QuadSetMutable<T>>
    {
        /// <summary>A</summary>
        public  T a;
        /// <summary>B</summary>
        public  T b;
        /// <summary>C</summary>
        public  T c;
        /// <summary>D</summary>
        public  T d;


        /// <summary>Create Quad (4-tuple).</summary>
        public QuadSetMutable(T a, T b, T c, T d) { this.a = a; this.b = b; this.c = c; this.d = d; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is QuadSetMutable<T> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(QuadSetMutable<T> other) => EqualityComparer.Instance.Equals(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<QuadSetMutable<T>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer(EqualityComparer<T>.Default));

            /// <summary>Element comparer.</summary>
            public readonly IEqualityComparer<T> Comparer;

            /// <summary>Create equality comparer.</summary>
            /// <param name="comparer">(optional) elemen comparer</param>
            public EqualityComparer(IEqualityComparer<T> comparer)
            {
                this.Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            /// <summary>Test equality of <paramref name="x"/> to <paramref name="y"/></summary>
            public bool Equals(QuadSetMutable<T> x, QuadSetMutable<T> y)
            {

                // 0 && 1 && 2 && 3
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.d)) return true;
                // 0 && 1 && 3 && 2
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.c)) return true;
                // 0 && 2 && 1 && 3
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.d)) return true;
                // 0 && 2 && 3 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.b)) return true;
                // 0 && 3 && 2 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.b)) return true;
                // 0 && 3 && 1 && 2
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.c)) return true;
                // 1 && 0 && 2 && 3
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.d)) return true;
                // 1 && 0 && 3 && 2
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.c)) return true;
                // 1 && 2 && 0 && 3
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.d)) return true;
                // 1 && 2 && 3 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.a)) return true;
                // 1 && 3 && 2 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.a)) return true;
                // 1 && 3 && 0 && 2
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.c)) return true;
                // 2 && 1 && 0 && 3
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.d)) return true;
                // 2 && 1 && 3 && 0
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.a)) return true;
                // 2 && 0 && 1 && 3
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.d)) return true;
                // 2 && 0 && 3 && 1
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.b)) return true;
                // 2 && 3 && 0 && 1
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.b)) return true;
                // 2 && 3 && 1 && 0
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.a)) return true;
                // 3 && 1 && 2 && 0
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.a)) return true;
                // 3 && 1 && 0 && 2
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.c)) return true;
                // 3 && 2 && 1 && 0
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.a)) return true;
                // 3 && 2 && 0 && 1
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.b)) return true;
                // 3 && 0 && 2 && 1
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.b)) return true;
                // 3 && 0 && 1 && 2
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.c)) return true;

                return false;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(QuadSetMutable<T> obj) => (obj.a==null?0:Comparer.GetHashCode(obj.a)) ^ (obj.b==null?0:Comparer.GetHashCode(obj.b)) ^ (obj.c==null?0:Comparer.GetHashCode(obj.c)) ^ (obj.d==null?0:Comparer.GetHashCode(obj.d));
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");

            sb.Append(b);
            sb.Append(", ");

            sb.Append(c);
            sb.Append(", ");

            sb.Append(d);

            sb.Append(")");
        }
    }
    

    /// <summary>Unordered Quad (4-tuple) where elements are interchangeable. Hashcode is not cached. Elements are mutable. Type is heap allocated. </summary>
    /// <typeparam name="T"></typeparam>
    public class QuadSetMutableObject<T> : IEquatable<QuadSetMutableObject<T>>
    {
        /// <summary>A</summary>
        public  T a;
        /// <summary>B</summary>
        public  T b;
        /// <summary>C</summary>
        public  T c;
        /// <summary>D</summary>
        public  T d;


        /// <summary>Create Quad (4-tuple).</summary>
        public QuadSetMutableObject() { a = default!; b = default!; c = default!; d = default!; }

        /// <summary>Create Quad (4-tuple).</summary>
        public QuadSetMutableObject(T a, T b, T c, T d) { this.a = a; this.b = b; this.c = c; this.d = d; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is QuadSetMutableObject<T> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(QuadSetMutableObject<T>? other) => EqualityComparer.Instance.Equals(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<QuadSetMutableObject<T>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer(EqualityComparer<T>.Default));

            /// <summary>Element comparer.</summary>
            public readonly IEqualityComparer<T> Comparer;

            /// <summary>Create equality comparer.</summary>
            /// <param name="comparer">(optional) elemen comparer</param>
            public EqualityComparer(IEqualityComparer<T> comparer)
            {
                this.Comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            }

            /// <summary>Test equality of <paramref name="x"/> to <paramref name="y"/></summary>
            public bool Equals(QuadSetMutableObject<T>? x, QuadSetMutableObject<T>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                // 0 && 1 && 2 && 3
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.d)) return true;
                // 0 && 1 && 3 && 2
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.c)) return true;
                // 0 && 2 && 1 && 3
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.d)) return true;
                // 0 && 2 && 3 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.b)) return true;
                // 0 && 3 && 2 && 1
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.b)) return true;
                // 0 && 3 && 1 && 2
                if (Comparer.Equals(x.a, y.a) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.c)) return true;
                // 1 && 0 && 2 && 3
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.d)) return true;
                // 1 && 0 && 3 && 2
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.c)) return true;
                // 1 && 2 && 0 && 3
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.d)) return true;
                // 1 && 2 && 3 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.a)) return true;
                // 1 && 3 && 2 && 0
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.a)) return true;
                // 1 && 3 && 0 && 2
                if (Comparer.Equals(x.a, y.b) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.c)) return true;
                // 2 && 1 && 0 && 3
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.d)) return true;
                // 2 && 1 && 3 && 0
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.a)) return true;
                // 2 && 0 && 1 && 3
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.d)) return true;
                // 2 && 0 && 3 && 1
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.d) && Comparer.Equals(x.d, y.b)) return true;
                // 2 && 3 && 0 && 1
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.b)) return true;
                // 2 && 3 && 1 && 0
                if (Comparer.Equals(x.a, y.c) && Comparer.Equals(x.b, y.d) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.a)) return true;
                // 3 && 1 && 2 && 0
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.a)) return true;
                // 3 && 1 && 0 && 2
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.b) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.c)) return true;
                // 3 && 2 && 1 && 0
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.a)) return true;
                // 3 && 2 && 0 && 1
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.c) && Comparer.Equals(x.c, y.a) && Comparer.Equals(x.d, y.b)) return true;
                // 3 && 0 && 2 && 1
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.c) && Comparer.Equals(x.d, y.b)) return true;
                // 3 && 0 && 1 && 2
                if (Comparer.Equals(x.a, y.d) && Comparer.Equals(x.b, y.a) && Comparer.Equals(x.c, y.b) && Comparer.Equals(x.d, y.c)) return true;

                return false;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(QuadSetMutableObject<T> obj) => obj == null ? 0 : (obj.a==null?0:Comparer.GetHashCode(obj.a)) ^ (obj.b==null?0:Comparer.GetHashCode(obj.b)) ^ (obj.c==null?0:Comparer.GetHashCode(obj.c)) ^ (obj.d==null?0:Comparer.GetHashCode(obj.d));
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");

            sb.Append(b);
            sb.Append(", ");

            sb.Append(c);
            sb.Append(", ");

            sb.Append(d);

            sb.Append(")");
        }
    }
    
    /// <summary>Pento (5-tuple). Hashcode is cached. Elements are immutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    public struct Pento<A, B, C, D, E> : IEquatable<Pento<A, B, C, D, E>>, IComparable<Pento<A, B, C, D, E>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>B</summary>
        public readonly B b;
        /// <summary>C</summary>
        public readonly C c;
        /// <summary>D</summary>
        public readonly D d;
        /// <summary>E</summary>
        public readonly E e;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Pento (5-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public Pento(A a, B b, C c, D d, E e) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()) + (e==null?0:23*e.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is Pento<A, B, C, D, E> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(Pento<A, B, C, D, E> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(Pento<A, B, C, D, E> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<Pento<A, B, C, D, E>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(Pento<A, B, C, D, E> x, Pento<A, B, C, D, E> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(Pento<A, B, C, D, E> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<Pento<A, B, C, D, E>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(Pento<A, B, C, D, E> x, Pento<A, B, C, D, E> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);

            sb.Append(")");
        }
    }
    /// <summary>Pento (5-tuple). Hashcode is cached. Elements are immutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class PentoObject<A, B, C, D, E> : IEquatable<PentoObject<A, B, C, D, E>>, IComparable<PentoObject<A, B, C, D, E>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>B</summary>
        public readonly B b;
        /// <summary>C</summary>
        public readonly C c;
        /// <summary>D</summary>
        public readonly D d;
        /// <summary>E</summary>
        public readonly E e;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Pento (5-tuple).</summary>
        public PentoObject() { a = default!; b = default!; c = default!; d = default!; e = default!; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()) + (e==null?0:23*e.GetHashCode()); }
        /// <summary>Create Pento (5-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public PentoObject(A a, B b, C c, D d, E e) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()) + (e==null?0:23*e.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is PentoObject<A, B, C, D, E> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(PentoObject<A, B, C, D, E>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(PentoObject<A, B, C, D, E>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<PentoObject<A, B, C, D, E>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(PentoObject<A, B, C, D, E>? x, PentoObject<A, B, C, D, E>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(PentoObject<A, B, C, D, E> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<PentoObject<A, B, C, D, E>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(PentoObject<A, B, C, D, E>? x, PentoObject<A, B, C, D, E>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);

            sb.Append(")");
        }
    }
    /// <summary>Pento (5-tuple). Hashcode is not cached. Elements are mutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    public struct PentoMutable<A, B, C, D, E> : IEquatable<PentoMutable<A, B, C, D, E>>, IComparable<PentoMutable<A, B, C, D, E>>
    {
        /// <summary>A</summary>
        public A a;
        /// <summary>B</summary>
        public B b;
        /// <summary>C</summary>
        public C c;
        /// <summary>D</summary>
        public D d;
        /// <summary>E</summary>
        public E e;
        
        
        /// <summary>Create Pento (5-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public PentoMutable(A a, B b, C c, D d, E e) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is PentoMutable<A, B, C, D, E> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(PentoMutable<A, B, C, D, E> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(PentoMutable<A, B, C, D, E> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<PentoMutable<A, B, C, D, E>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(PentoMutable<A, B, C, D, E> x, PentoMutable<A, B, C, D, E> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(PentoMutable<A, B, C, D, E> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<PentoMutable<A, B, C, D, E>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(PentoMutable<A, B, C, D, E> x, PentoMutable<A, B, C, D, E> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);

            sb.Append(")");
        }
    }
    /// <summary>Pento (5-tuple). Hashcode is not cached. Elements are mutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    public class PentoMutableObject<A, B, C, D, E> : IEquatable<PentoMutableObject<A, B, C, D, E>>, IComparable<PentoMutableObject<A, B, C, D, E>>
    {
        /// <summary>A</summary>
        public A a;
        /// <summary>B</summary>
        public B b;
        /// <summary>C</summary>
        public C c;
        /// <summary>D</summary>
        public D d;
        /// <summary>E</summary>
        public E e;
        
        
        /// <summary>Create Pento (5-tuple).</summary>
        public PentoMutableObject() { a = default!; b = default!; c = default!; d = default!; e = default!; }
        /// <summary>Create Pento (5-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public PentoMutableObject(A a, B b, C c, D d, E e) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is PentoMutableObject<A, B, C, D, E> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(PentoMutableObject<A, B, C, D, E>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(PentoMutableObject<A, B, C, D, E>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<PentoMutableObject<A, B, C, D, E>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(PentoMutableObject<A, B, C, D, E>? x, PentoMutableObject<A, B, C, D, E>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(PentoMutableObject<A, B, C, D, E> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<PentoMutableObject<A, B, C, D, E>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(PentoMutableObject<A, B, C, D, E>? x, PentoMutableObject<A, B, C, D, E>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);

            sb.Append(")");
        }
    }
    /// <summary>Sextim (6-tuple). Hashcode is cached. Elements are immutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    public struct Sextim<A, B, C, D, E, F> : IEquatable<Sextim<A, B, C, D, E, F>>, IComparable<Sextim<A, B, C, D, E, F>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>B</summary>
        public readonly B b;
        /// <summary>C</summary>
        public readonly C c;
        /// <summary>D</summary>
        public readonly D d;
        /// <summary>E</summary>
        public readonly E e;
        /// <summary>F</summary>
        public readonly F f;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Sextim (6-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        public Sextim(A a, B b, C c, D d, E e, F f) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = f; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()) + (e==null?0:23*e.GetHashCode()) + (f==null?0:29*f.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is Sextim<A, B, C, D, E, F> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(Sextim<A, B, C, D, E, F> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(Sextim<A, B, C, D, E, F> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<Sextim<A, B, C, D, E, F>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;
            /// <summary>Element F comparer.</summary>
            public readonly IEqualityComparer<F> fComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default, IEqualityComparer<F>? fComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
                this.fComparer = fComparer ?? EqualityComparer<F>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(Sextim<A, B, C, D, E, F> x, Sextim<A, B, C, D, E, F> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                if (!fComparer.Equals(x.f, y.f)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(Sextim<A, B, C, D, E, F> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e)) + (obj.f==null?0:29*fComparer.GetHashCode(obj.f));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<Sextim<A, B, C, D, E, F>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;
            /// <summary>Element F comparer</summary>
            public readonly IComparer<F> fComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default, IComparer<F>? fComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;
                this.fComparer = fComparer ?? System.Collections.Generic.Comparer<F>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(Sextim<A, B, C, D, E, F> x, Sextim<A, B, C, D, E, F> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;
                d = fComparer.Compare(x.f, y.f);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);
            sb.Append(", ");
            sb.Append(f);

            sb.Append(")");
        }
    }
    /// <summary>Sextim (6-tuple). Hashcode is cached. Elements are immutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    public class SextimObject<A, B, C, D, E, F> : IEquatable<SextimObject<A, B, C, D, E, F>>, IComparable<SextimObject<A, B, C, D, E, F>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>B</summary>
        public readonly B b;
        /// <summary>C</summary>
        public readonly C c;
        /// <summary>D</summary>
        public readonly D d;
        /// <summary>E</summary>
        public readonly E e;
        /// <summary>F</summary>
        public readonly F f;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Sextim (6-tuple).</summary>
        public SextimObject() { a = default!; b = default!; c = default!; d = default!; e = default!; f = default!; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()) + (e==null?0:23*e.GetHashCode()) + (f==null?0:29*f.GetHashCode()); }
        /// <summary>Create Sextim (6-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        public SextimObject(A a, B b, C c, D d, E e, F f) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = f; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()) + (e==null?0:23*e.GetHashCode()) + (f==null?0:29*f.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is SextimObject<A, B, C, D, E, F> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(SextimObject<A, B, C, D, E, F>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(SextimObject<A, B, C, D, E, F>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<SextimObject<A, B, C, D, E, F>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;
            /// <summary>Element F comparer.</summary>
            public readonly IEqualityComparer<F> fComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default, IEqualityComparer<F>? fComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
                this.fComparer = fComparer ?? EqualityComparer<F>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(SextimObject<A, B, C, D, E, F>? x, SextimObject<A, B, C, D, E, F>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                if (!fComparer.Equals(x.f, y.f)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(SextimObject<A, B, C, D, E, F> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e)) + (obj.f==null?0:29*fComparer.GetHashCode(obj.f));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<SextimObject<A, B, C, D, E, F>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;
            /// <summary>Element F comparer</summary>
            public readonly IComparer<F> fComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default, IComparer<F>? fComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;
                this.fComparer = fComparer ?? System.Collections.Generic.Comparer<F>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(SextimObject<A, B, C, D, E, F>? x, SextimObject<A, B, C, D, E, F>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;
                d = fComparer.Compare(x.f, y.f);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);
            sb.Append(", ");
            sb.Append(f);

            sb.Append(")");
        }
    }
    /// <summary>Sextim (6-tuple). Hashcode is not cached. Elements are mutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    public struct SextimMutable<A, B, C, D, E, F> : IEquatable<SextimMutable<A, B, C, D, E, F>>, IComparable<SextimMutable<A, B, C, D, E, F>>
    {
        /// <summary>A</summary>
        public A a;
        /// <summary>B</summary>
        public B b;
        /// <summary>C</summary>
        public C c;
        /// <summary>D</summary>
        public D d;
        /// <summary>E</summary>
        public E e;
        /// <summary>F</summary>
        public F f;
        
        
        /// <summary>Create Sextim (6-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        public SextimMutable(A a, B b, C c, D d, E e, F f) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = f; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is SextimMutable<A, B, C, D, E, F> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(SextimMutable<A, B, C, D, E, F> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(SextimMutable<A, B, C, D, E, F> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<SextimMutable<A, B, C, D, E, F>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;
            /// <summary>Element F comparer.</summary>
            public readonly IEqualityComparer<F> fComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default, IEqualityComparer<F>? fComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
                this.fComparer = fComparer ?? EqualityComparer<F>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(SextimMutable<A, B, C, D, E, F> x, SextimMutable<A, B, C, D, E, F> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                if (!fComparer.Equals(x.f, y.f)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(SextimMutable<A, B, C, D, E, F> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e)) + (obj.f==null?0:29*fComparer.GetHashCode(obj.f));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<SextimMutable<A, B, C, D, E, F>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;
            /// <summary>Element F comparer</summary>
            public readonly IComparer<F> fComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default, IComparer<F>? fComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;
                this.fComparer = fComparer ?? System.Collections.Generic.Comparer<F>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(SextimMutable<A, B, C, D, E, F> x, SextimMutable<A, B, C, D, E, F> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;
                d = fComparer.Compare(x.f, y.f);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);
            sb.Append(", ");
            sb.Append(f);

            sb.Append(")");
        }
    }
    /// <summary>Sextim (6-tuple). Hashcode is not cached. Elements are mutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    public class SextimMutableObject<A, B, C, D, E, F> : IEquatable<SextimMutableObject<A, B, C, D, E, F>>, IComparable<SextimMutableObject<A, B, C, D, E, F>>
    {
        /// <summary>A</summary>
        public A a;
        /// <summary>B</summary>
        public B b;
        /// <summary>C</summary>
        public C c;
        /// <summary>D</summary>
        public D d;
        /// <summary>E</summary>
        public E e;
        /// <summary>F</summary>
        public F f;
        
        
        /// <summary>Create Sextim (6-tuple).</summary>
        public SextimMutableObject() { a = default!; b = default!; c = default!; d = default!; e = default!; f = default!; }
        /// <summary>Create Sextim (6-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        public SextimMutableObject(A a, B b, C c, D d, E e, F f) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = f; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is SextimMutableObject<A, B, C, D, E, F> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(SextimMutableObject<A, B, C, D, E, F>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(SextimMutableObject<A, B, C, D, E, F>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<SextimMutableObject<A, B, C, D, E, F>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;
            /// <summary>Element F comparer.</summary>
            public readonly IEqualityComparer<F> fComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default, IEqualityComparer<F>? fComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
                this.fComparer = fComparer ?? EqualityComparer<F>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(SextimMutableObject<A, B, C, D, E, F>? x, SextimMutableObject<A, B, C, D, E, F>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                if (!fComparer.Equals(x.f, y.f)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(SextimMutableObject<A, B, C, D, E, F> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e)) + (obj.f==null?0:29*fComparer.GetHashCode(obj.f));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<SextimMutableObject<A, B, C, D, E, F>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;
            /// <summary>Element F comparer</summary>
            public readonly IComparer<F> fComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default, IComparer<F>? fComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;
                this.fComparer = fComparer ?? System.Collections.Generic.Comparer<F>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(SextimMutableObject<A, B, C, D, E, F>? x, SextimMutableObject<A, B, C, D, E, F>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;
                d = fComparer.Compare(x.f, y.f);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);
            sb.Append(", ");
            sb.Append(f);

            sb.Append(")");
        }
    }
    /// <summary>Septim (7-tuple). Hashcode is cached. Elements are immutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="G"></typeparam>
    public struct Septim<A, B, C, D, E, F, G> : IEquatable<Septim<A, B, C, D, E, F, G>>, IComparable<Septim<A, B, C, D, E, F, G>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>B</summary>
        public readonly B b;
        /// <summary>C</summary>
        public readonly C c;
        /// <summary>D</summary>
        public readonly D d;
        /// <summary>E</summary>
        public readonly E e;
        /// <summary>F</summary>
        public readonly F f;
        /// <summary>G</summary>
        public readonly G g;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Septim (7-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        public Septim(A a, B b, C c, D d, E e, F f, G g) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = f; this.g = g; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()) + (e==null?0:23*e.GetHashCode()) + (f==null?0:29*f.GetHashCode()) + (g==null?0:31*g.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is Septim<A, B, C, D, E, F, G> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(Septim<A, B, C, D, E, F, G> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(Septim<A, B, C, D, E, F, G> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<Septim<A, B, C, D, E, F, G>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;
            /// <summary>Element F comparer.</summary>
            public readonly IEqualityComparer<F> fComparer;
            /// <summary>Element G comparer.</summary>
            public readonly IEqualityComparer<G> gComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default, IEqualityComparer<F>? fComparer = default, IEqualityComparer<G>? gComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
                this.fComparer = fComparer ?? EqualityComparer<F>.Default;
                this.gComparer = gComparer ?? EqualityComparer<G>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(Septim<A, B, C, D, E, F, G> x, Septim<A, B, C, D, E, F, G> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                if (!fComparer.Equals(x.f, y.f)) return false;
                if (!gComparer.Equals(x.g, y.g)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(Septim<A, B, C, D, E, F, G> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e)) + (obj.f==null?0:29*fComparer.GetHashCode(obj.f)) + (obj.g==null?0:31*gComparer.GetHashCode(obj.g));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<Septim<A, B, C, D, E, F, G>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;
            /// <summary>Element F comparer</summary>
            public readonly IComparer<F> fComparer;
            /// <summary>Element G comparer</summary>
            public readonly IComparer<G> gComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default, IComparer<F>? fComparer = default, IComparer<G>? gComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;
                this.fComparer = fComparer ?? System.Collections.Generic.Comparer<F>.Default;
                this.gComparer = gComparer ?? System.Collections.Generic.Comparer<G>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(Septim<A, B, C, D, E, F, G> x, Septim<A, B, C, D, E, F, G> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;
                d = fComparer.Compare(x.f, y.f);
                if (d != 0) return d;
                d = gComparer.Compare(x.g, y.g);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);
            sb.Append(", ");
            sb.Append(f);
            sb.Append(", ");
            sb.Append(g);

            sb.Append(")");
        }
    }
    /// <summary>Septim (7-tuple). Hashcode is cached. Elements are immutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="G"></typeparam>
    public class SeptimObject<A, B, C, D, E, F, G> : IEquatable<SeptimObject<A, B, C, D, E, F, G>>, IComparable<SeptimObject<A, B, C, D, E, F, G>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>B</summary>
        public readonly B b;
        /// <summary>C</summary>
        public readonly C c;
        /// <summary>D</summary>
        public readonly D d;
        /// <summary>E</summary>
        public readonly E e;
        /// <summary>F</summary>
        public readonly F f;
        /// <summary>G</summary>
        public readonly G g;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Septim (7-tuple).</summary>
        public SeptimObject() { a = default!; b = default!; c = default!; d = default!; e = default!; f = default!; g = default!; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()) + (e==null?0:23*e.GetHashCode()) + (f==null?0:29*f.GetHashCode()) + (g==null?0:31*g.GetHashCode()); }
        /// <summary>Create Septim (7-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        public SeptimObject(A a, B b, C c, D d, E e, F f, G g) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = f; this.g = g; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()) + (e==null?0:23*e.GetHashCode()) + (f==null?0:29*f.GetHashCode()) + (g==null?0:31*g.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is SeptimObject<A, B, C, D, E, F, G> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(SeptimObject<A, B, C, D, E, F, G>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(SeptimObject<A, B, C, D, E, F, G>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<SeptimObject<A, B, C, D, E, F, G>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;
            /// <summary>Element F comparer.</summary>
            public readonly IEqualityComparer<F> fComparer;
            /// <summary>Element G comparer.</summary>
            public readonly IEqualityComparer<G> gComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default, IEqualityComparer<F>? fComparer = default, IEqualityComparer<G>? gComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
                this.fComparer = fComparer ?? EqualityComparer<F>.Default;
                this.gComparer = gComparer ?? EqualityComparer<G>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(SeptimObject<A, B, C, D, E, F, G>? x, SeptimObject<A, B, C, D, E, F, G>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                if (!fComparer.Equals(x.f, y.f)) return false;
                if (!gComparer.Equals(x.g, y.g)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(SeptimObject<A, B, C, D, E, F, G> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e)) + (obj.f==null?0:29*fComparer.GetHashCode(obj.f)) + (obj.g==null?0:31*gComparer.GetHashCode(obj.g));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<SeptimObject<A, B, C, D, E, F, G>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;
            /// <summary>Element F comparer</summary>
            public readonly IComparer<F> fComparer;
            /// <summary>Element G comparer</summary>
            public readonly IComparer<G> gComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default, IComparer<F>? fComparer = default, IComparer<G>? gComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;
                this.fComparer = fComparer ?? System.Collections.Generic.Comparer<F>.Default;
                this.gComparer = gComparer ?? System.Collections.Generic.Comparer<G>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(SeptimObject<A, B, C, D, E, F, G>? x, SeptimObject<A, B, C, D, E, F, G>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;
                d = fComparer.Compare(x.f, y.f);
                if (d != 0) return d;
                d = gComparer.Compare(x.g, y.g);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);
            sb.Append(", ");
            sb.Append(f);
            sb.Append(", ");
            sb.Append(g);

            sb.Append(")");
        }
    }
    /// <summary>Septim (7-tuple). Hashcode is not cached. Elements are mutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="G"></typeparam>
    public struct SeptimMutable<A, B, C, D, E, F, G> : IEquatable<SeptimMutable<A, B, C, D, E, F, G>>, IComparable<SeptimMutable<A, B, C, D, E, F, G>>
    {
        /// <summary>A</summary>
        public A a;
        /// <summary>B</summary>
        public B b;
        /// <summary>C</summary>
        public C c;
        /// <summary>D</summary>
        public D d;
        /// <summary>E</summary>
        public E e;
        /// <summary>F</summary>
        public F f;
        /// <summary>G</summary>
        public G g;
        
        
        /// <summary>Create Septim (7-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        public SeptimMutable(A a, B b, C c, D d, E e, F f, G g) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = f; this.g = g; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is SeptimMutable<A, B, C, D, E, F, G> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(SeptimMutable<A, B, C, D, E, F, G> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(SeptimMutable<A, B, C, D, E, F, G> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<SeptimMutable<A, B, C, D, E, F, G>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;
            /// <summary>Element F comparer.</summary>
            public readonly IEqualityComparer<F> fComparer;
            /// <summary>Element G comparer.</summary>
            public readonly IEqualityComparer<G> gComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default, IEqualityComparer<F>? fComparer = default, IEqualityComparer<G>? gComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
                this.fComparer = fComparer ?? EqualityComparer<F>.Default;
                this.gComparer = gComparer ?? EqualityComparer<G>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(SeptimMutable<A, B, C, D, E, F, G> x, SeptimMutable<A, B, C, D, E, F, G> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                if (!fComparer.Equals(x.f, y.f)) return false;
                if (!gComparer.Equals(x.g, y.g)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(SeptimMutable<A, B, C, D, E, F, G> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e)) + (obj.f==null?0:29*fComparer.GetHashCode(obj.f)) + (obj.g==null?0:31*gComparer.GetHashCode(obj.g));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<SeptimMutable<A, B, C, D, E, F, G>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;
            /// <summary>Element F comparer</summary>
            public readonly IComparer<F> fComparer;
            /// <summary>Element G comparer</summary>
            public readonly IComparer<G> gComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default, IComparer<F>? fComparer = default, IComparer<G>? gComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;
                this.fComparer = fComparer ?? System.Collections.Generic.Comparer<F>.Default;
                this.gComparer = gComparer ?? System.Collections.Generic.Comparer<G>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(SeptimMutable<A, B, C, D, E, F, G> x, SeptimMutable<A, B, C, D, E, F, G> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;
                d = fComparer.Compare(x.f, y.f);
                if (d != 0) return d;
                d = gComparer.Compare(x.g, y.g);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);
            sb.Append(", ");
            sb.Append(f);
            sb.Append(", ");
            sb.Append(g);

            sb.Append(")");
        }
    }
    /// <summary>Septim (7-tuple). Hashcode is not cached. Elements are mutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="G"></typeparam>
    public class SeptimMutableObject<A, B, C, D, E, F, G> : IEquatable<SeptimMutableObject<A, B, C, D, E, F, G>>, IComparable<SeptimMutableObject<A, B, C, D, E, F, G>>
    {
        /// <summary>A</summary>
        public A a;
        /// <summary>B</summary>
        public B b;
        /// <summary>C</summary>
        public C c;
        /// <summary>D</summary>
        public D d;
        /// <summary>E</summary>
        public E e;
        /// <summary>F</summary>
        public F f;
        /// <summary>G</summary>
        public G g;
        
        
        /// <summary>Create Septim (7-tuple).</summary>
        public SeptimMutableObject() { a = default!; b = default!; c = default!; d = default!; e = default!; f = default!; g = default!; }
        /// <summary>Create Septim (7-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        public SeptimMutableObject(A a, B b, C c, D d, E e, F f, G g) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = f; this.g = g; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is SeptimMutableObject<A, B, C, D, E, F, G> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(SeptimMutableObject<A, B, C, D, E, F, G>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(SeptimMutableObject<A, B, C, D, E, F, G>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<SeptimMutableObject<A, B, C, D, E, F, G>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;
            /// <summary>Element F comparer.</summary>
            public readonly IEqualityComparer<F> fComparer;
            /// <summary>Element G comparer.</summary>
            public readonly IEqualityComparer<G> gComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default, IEqualityComparer<F>? fComparer = default, IEqualityComparer<G>? gComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
                this.fComparer = fComparer ?? EqualityComparer<F>.Default;
                this.gComparer = gComparer ?? EqualityComparer<G>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(SeptimMutableObject<A, B, C, D, E, F, G>? x, SeptimMutableObject<A, B, C, D, E, F, G>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                if (!fComparer.Equals(x.f, y.f)) return false;
                if (!gComparer.Equals(x.g, y.g)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(SeptimMutableObject<A, B, C, D, E, F, G> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e)) + (obj.f==null?0:29*fComparer.GetHashCode(obj.f)) + (obj.g==null?0:31*gComparer.GetHashCode(obj.g));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<SeptimMutableObject<A, B, C, D, E, F, G>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;
            /// <summary>Element F comparer</summary>
            public readonly IComparer<F> fComparer;
            /// <summary>Element G comparer</summary>
            public readonly IComparer<G> gComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default, IComparer<F>? fComparer = default, IComparer<G>? gComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;
                this.fComparer = fComparer ?? System.Collections.Generic.Comparer<F>.Default;
                this.gComparer = gComparer ?? System.Collections.Generic.Comparer<G>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(SeptimMutableObject<A, B, C, D, E, F, G>? x, SeptimMutableObject<A, B, C, D, E, F, G>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;
                d = fComparer.Compare(x.f, y.f);
                if (d != 0) return d;
                d = gComparer.Compare(x.g, y.g);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);
            sb.Append(", ");
            sb.Append(f);
            sb.Append(", ");
            sb.Append(g);

            sb.Append(")");
        }
    }
    /// <summary>Octet (8-tuple). Hashcode is cached. Elements are immutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="G"></typeparam>
    /// <typeparam name="H"></typeparam>
    public struct Octet<A, B, C, D, E, F, G, H> : IEquatable<Octet<A, B, C, D, E, F, G, H>>, IComparable<Octet<A, B, C, D, E, F, G, H>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>B</summary>
        public readonly B b;
        /// <summary>C</summary>
        public readonly C c;
        /// <summary>D</summary>
        public readonly D d;
        /// <summary>E</summary>
        public readonly E e;
        /// <summary>F</summary>
        public readonly F f;
        /// <summary>G</summary>
        public readonly G g;
        /// <summary>H</summary>
        public readonly H h;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Octet (8-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        public Octet(A a, B b, C c, D d, E e, F f, G g, H h) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = f; this.g = g; this.h = h; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()) + (e==null?0:23*e.GetHashCode()) + (f==null?0:29*f.GetHashCode()) + (g==null?0:31*g.GetHashCode()) + (h==null?0:37*h.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is Octet<A, B, C, D, E, F, G, H> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(Octet<A, B, C, D, E, F, G, H> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(Octet<A, B, C, D, E, F, G, H> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<Octet<A, B, C, D, E, F, G, H>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;
            /// <summary>Element F comparer.</summary>
            public readonly IEqualityComparer<F> fComparer;
            /// <summary>Element G comparer.</summary>
            public readonly IEqualityComparer<G> gComparer;
            /// <summary>Element H comparer.</summary>
            public readonly IEqualityComparer<H> hComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            /// <param name="hComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default, IEqualityComparer<F>? fComparer = default, IEqualityComparer<G>? gComparer = default, IEqualityComparer<H>? hComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
                this.fComparer = fComparer ?? EqualityComparer<F>.Default;
                this.gComparer = gComparer ?? EqualityComparer<G>.Default;
                this.hComparer = hComparer ?? EqualityComparer<H>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(Octet<A, B, C, D, E, F, G, H> x, Octet<A, B, C, D, E, F, G, H> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                if (!fComparer.Equals(x.f, y.f)) return false;
                if (!gComparer.Equals(x.g, y.g)) return false;
                if (!hComparer.Equals(x.h, y.h)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(Octet<A, B, C, D, E, F, G, H> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e)) + (obj.f==null?0:29*fComparer.GetHashCode(obj.f)) + (obj.g==null?0:31*gComparer.GetHashCode(obj.g)) + (obj.h==null?0:37*hComparer.GetHashCode(obj.h));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<Octet<A, B, C, D, E, F, G, H>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;
            /// <summary>Element F comparer</summary>
            public readonly IComparer<F> fComparer;
            /// <summary>Element G comparer</summary>
            public readonly IComparer<G> gComparer;
            /// <summary>Element H comparer</summary>
            public readonly IComparer<H> hComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            /// <param name="hComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default, IComparer<F>? fComparer = default, IComparer<G>? gComparer = default, IComparer<H>? hComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;
                this.fComparer = fComparer ?? System.Collections.Generic.Comparer<F>.Default;
                this.gComparer = gComparer ?? System.Collections.Generic.Comparer<G>.Default;
                this.hComparer = hComparer ?? System.Collections.Generic.Comparer<H>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(Octet<A, B, C, D, E, F, G, H> x, Octet<A, B, C, D, E, F, G, H> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;
                d = fComparer.Compare(x.f, y.f);
                if (d != 0) return d;
                d = gComparer.Compare(x.g, y.g);
                if (d != 0) return d;
                d = hComparer.Compare(x.h, y.h);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);
            sb.Append(", ");
            sb.Append(f);
            sb.Append(", ");
            sb.Append(g);
            sb.Append(", ");
            sb.Append(h);

            sb.Append(")");
        }
    }
    /// <summary>Octet (8-tuple). Hashcode is cached. Elements are immutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="G"></typeparam>
    /// <typeparam name="H"></typeparam>
    public class OctetObject<A, B, C, D, E, F, G, H> : IEquatable<OctetObject<A, B, C, D, E, F, G, H>>, IComparable<OctetObject<A, B, C, D, E, F, G, H>>
    {
        /// <summary>A</summary>
        public readonly A a;
        /// <summary>B</summary>
        public readonly B b;
        /// <summary>C</summary>
        public readonly C c;
        /// <summary>D</summary>
        public readonly D d;
        /// <summary>E</summary>
        public readonly E e;
        /// <summary>F</summary>
        public readonly F f;
        /// <summary>G</summary>
        public readonly G g;
        /// <summary>H</summary>
        public readonly H h;
        /// <summary>Precalculated hashcode</summary>
        readonly int hashcode;

        /// <summary>Create Octet (8-tuple).</summary>
        public OctetObject() { a = default!; b = default!; c = default!; d = default!; e = default!; f = default!; g = default!; h = default!; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()) + (e==null?0:23*e.GetHashCode()) + (f==null?0:29*f.GetHashCode()) + (g==null?0:31*g.GetHashCode()) + (h==null?0:37*h.GetHashCode()); }
        /// <summary>Create Octet (8-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        public OctetObject(A a, B b, C c, D d, E e, F f, G g, H h) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = f; this.g = g; this.h = h; hashcode = (a==null?0:11*a.GetHashCode()) + (b==null?0:13*b.GetHashCode()) + (c==null?0:17*c.GetHashCode()) + (d==null?0:19*d.GetHashCode()) + (e==null?0:23*e.GetHashCode()) + (f==null?0:29*f.GetHashCode()) + (g==null?0:31*g.GetHashCode()) + (h==null?0:37*h.GetHashCode()); }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => hashcode;
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is OctetObject<A, B, C, D, E, F, G, H> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(OctetObject<A, B, C, D, E, F, G, H>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(OctetObject<A, B, C, D, E, F, G, H>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<OctetObject<A, B, C, D, E, F, G, H>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;
            /// <summary>Element F comparer.</summary>
            public readonly IEqualityComparer<F> fComparer;
            /// <summary>Element G comparer.</summary>
            public readonly IEqualityComparer<G> gComparer;
            /// <summary>Element H comparer.</summary>
            public readonly IEqualityComparer<H> hComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            /// <param name="hComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default, IEqualityComparer<F>? fComparer = default, IEqualityComparer<G>? gComparer = default, IEqualityComparer<H>? hComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
                this.fComparer = fComparer ?? EqualityComparer<F>.Default;
                this.gComparer = gComparer ?? EqualityComparer<G>.Default;
                this.hComparer = hComparer ?? EqualityComparer<H>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(OctetObject<A, B, C, D, E, F, G, H>? x, OctetObject<A, B, C, D, E, F, G, H>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                if (!fComparer.Equals(x.f, y.f)) return false;
                if (!gComparer.Equals(x.g, y.g)) return false;
                if (!hComparer.Equals(x.h, y.h)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(OctetObject<A, B, C, D, E, F, G, H> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e)) + (obj.f==null?0:29*fComparer.GetHashCode(obj.f)) + (obj.g==null?0:31*gComparer.GetHashCode(obj.g)) + (obj.h==null?0:37*hComparer.GetHashCode(obj.h));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<OctetObject<A, B, C, D, E, F, G, H>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;
            /// <summary>Element F comparer</summary>
            public readonly IComparer<F> fComparer;
            /// <summary>Element G comparer</summary>
            public readonly IComparer<G> gComparer;
            /// <summary>Element H comparer</summary>
            public readonly IComparer<H> hComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            /// <param name="hComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default, IComparer<F>? fComparer = default, IComparer<G>? gComparer = default, IComparer<H>? hComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;
                this.fComparer = fComparer ?? System.Collections.Generic.Comparer<F>.Default;
                this.gComparer = gComparer ?? System.Collections.Generic.Comparer<G>.Default;
                this.hComparer = hComparer ?? System.Collections.Generic.Comparer<H>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(OctetObject<A, B, C, D, E, F, G, H>? x, OctetObject<A, B, C, D, E, F, G, H>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;
                d = fComparer.Compare(x.f, y.f);
                if (d != 0) return d;
                d = gComparer.Compare(x.g, y.g);
                if (d != 0) return d;
                d = hComparer.Compare(x.h, y.h);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);
            sb.Append(", ");
            sb.Append(f);
            sb.Append(", ");
            sb.Append(g);
            sb.Append(", ");
            sb.Append(h);

            sb.Append(")");
        }
    }
    /// <summary>Octet (8-tuple). Hashcode is not cached. Elements are mutable. Type is stack allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="G"></typeparam>
    /// <typeparam name="H"></typeparam>
    public struct OctetMutable<A, B, C, D, E, F, G, H> : IEquatable<OctetMutable<A, B, C, D, E, F, G, H>>, IComparable<OctetMutable<A, B, C, D, E, F, G, H>>
    {
        /// <summary>A</summary>
        public A a;
        /// <summary>B</summary>
        public B b;
        /// <summary>C</summary>
        public C c;
        /// <summary>D</summary>
        public D d;
        /// <summary>E</summary>
        public E e;
        /// <summary>F</summary>
        public F f;
        /// <summary>G</summary>
        public G g;
        /// <summary>H</summary>
        public H h;
        
        
        /// <summary>Create Octet (8-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        public OctetMutable(A a, B b, C c, D d, E e, F f, G g, H h) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = f; this.g = g; this.h = h; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is OctetMutable<A, B, C, D, E, F, G, H> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(OctetMutable<A, B, C, D, E, F, G, H> other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(OctetMutable<A, B, C, D, E, F, G, H> other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<OctetMutable<A, B, C, D, E, F, G, H>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;
            /// <summary>Element F comparer.</summary>
            public readonly IEqualityComparer<F> fComparer;
            /// <summary>Element G comparer.</summary>
            public readonly IEqualityComparer<G> gComparer;
            /// <summary>Element H comparer.</summary>
            public readonly IEqualityComparer<H> hComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            /// <param name="hComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default, IEqualityComparer<F>? fComparer = default, IEqualityComparer<G>? gComparer = default, IEqualityComparer<H>? hComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
                this.fComparer = fComparer ?? EqualityComparer<F>.Default;
                this.gComparer = gComparer ?? EqualityComparer<G>.Default;
                this.hComparer = hComparer ?? EqualityComparer<H>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(OctetMutable<A, B, C, D, E, F, G, H> x, OctetMutable<A, B, C, D, E, F, G, H> y)
            {

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                if (!fComparer.Equals(x.f, y.f)) return false;
                if (!gComparer.Equals(x.g, y.g)) return false;
                if (!hComparer.Equals(x.h, y.h)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(OctetMutable<A, B, C, D, E, F, G, H> obj)  => (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e)) + (obj.f==null?0:29*fComparer.GetHashCode(obj.f)) + (obj.g==null?0:31*gComparer.GetHashCode(obj.g)) + (obj.h==null?0:37*hComparer.GetHashCode(obj.h));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<OctetMutable<A, B, C, D, E, F, G, H>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;
            /// <summary>Element F comparer</summary>
            public readonly IComparer<F> fComparer;
            /// <summary>Element G comparer</summary>
            public readonly IComparer<G> gComparer;
            /// <summary>Element H comparer</summary>
            public readonly IComparer<H> hComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            /// <param name="hComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default, IComparer<F>? fComparer = default, IComparer<G>? gComparer = default, IComparer<H>? hComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;
                this.fComparer = fComparer ?? System.Collections.Generic.Comparer<F>.Default;
                this.gComparer = gComparer ?? System.Collections.Generic.Comparer<G>.Default;
                this.hComparer = hComparer ?? System.Collections.Generic.Comparer<H>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(OctetMutable<A, B, C, D, E, F, G, H> x, OctetMutable<A, B, C, D, E, F, G, H> y)
            {

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;
                d = fComparer.Compare(x.f, y.f);
                if (d != 0) return d;
                d = gComparer.Compare(x.g, y.g);
                if (d != 0) return d;
                d = hComparer.Compare(x.h, y.h);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);
            sb.Append(", ");
            sb.Append(f);
            sb.Append(", ");
            sb.Append(g);
            sb.Append(", ");
            sb.Append(h);

            sb.Append(")");
        }
    }
    /// <summary>Octet (8-tuple). Hashcode is not cached. Elements are mutable. Type is heap allocated. </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <typeparam name="C"></typeparam>
    /// <typeparam name="D"></typeparam>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="G"></typeparam>
    /// <typeparam name="H"></typeparam>
    public class OctetMutableObject<A, B, C, D, E, F, G, H> : IEquatable<OctetMutableObject<A, B, C, D, E, F, G, H>>, IComparable<OctetMutableObject<A, B, C, D, E, F, G, H>>
    {
        /// <summary>A</summary>
        public A a;
        /// <summary>B</summary>
        public B b;
        /// <summary>C</summary>
        public C c;
        /// <summary>D</summary>
        public D d;
        /// <summary>E</summary>
        public E e;
        /// <summary>F</summary>
        public F f;
        /// <summary>G</summary>
        public G g;
        /// <summary>H</summary>
        public H h;
        
        
        /// <summary>Create Octet (8-tuple).</summary>
        public OctetMutableObject() { a = default!; b = default!; c = default!; d = default!; e = default!; f = default!; g = default!; h = default!; }
        /// <summary>Create Octet (8-tuple).</summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="e"></param>
        /// <param name="f"></param>
        /// <param name="g"></param>
        /// <param name="h"></param>
        public OctetMutableObject(A a, B b, C c, D d, E e, F f, G g, H h) { this.a = a; this.b = b; this.c = c; this.d = d; this.e = e; this.f = f; this.g = g; this.h = h; }

        /// <summary>Get Hash-Code</summary>
        public override int GetHashCode() => EqualityComparer.Instance.GetHashCode(this);
        /// <summary>Test equality to <paramref name="obj"/>.</summary>
        public override bool Equals(object? obj) => obj is OctetMutableObject<A, B, C, D, E, F, G, H> other ? EqualityComparer.Instance.Equals(this, other) : false;
        /// <summary>Test equality to <paramref name="other"/>.</summary>
        public bool Equals(OctetMutableObject<A, B, C, D, E, F, G, H>? other) => EqualityComparer.Instance.Equals(this, other);
        /// <summary>Compare order to <paramref name="other"/>.</summary>
        public int CompareTo(OctetMutableObject<A, B, C, D, E, F, G, H>? other) => Comparer.Instance.Compare(this, other);

        /// <summary>Equality comparer</summary>
        public class EqualityComparer : IEqualityComparer<OctetMutableObject<A, B, C, D, E, F, G, H>>
        {
            /// <summary>Singleton</summary>
            static EqualityComparer? instance;
            /// <summary>Singleton</summary>
            public static EqualityComparer Instance => instance ?? (instance = new EqualityComparer());

            /// <summary>Element A comparer.</summary>
            public readonly IEqualityComparer<A> aComparer;
            /// <summary>Element B comparer.</summary>
            public readonly IEqualityComparer<B> bComparer;
            /// <summary>Element C comparer.</summary>
            public readonly IEqualityComparer<C> cComparer;
            /// <summary>Element D comparer.</summary>
            public readonly IEqualityComparer<D> dComparer;
            /// <summary>Element E comparer.</summary>
            public readonly IEqualityComparer<E> eComparer;
            /// <summary>Element F comparer.</summary>
            public readonly IEqualityComparer<F> fComparer;
            /// <summary>Element G comparer.</summary>
            public readonly IEqualityComparer<G> gComparer;
            /// <summary>Element H comparer.</summary>
            public readonly IEqualityComparer<H> hComparer;


            /// <summary>Create equality comparer.</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            /// <param name="hComparer">(optional) element comparer</param>
            public EqualityComparer(IEqualityComparer<A>? aComparer = default, IEqualityComparer<B>? bComparer = default, IEqualityComparer<C>? cComparer = default, IEqualityComparer<D>? dComparer = default, IEqualityComparer<E>? eComparer = default, IEqualityComparer<F>? fComparer = default, IEqualityComparer<G>? gComparer = default, IEqualityComparer<H>? hComparer = default)
            {
                this.aComparer = aComparer ?? EqualityComparer<A>.Default;
                this.bComparer = bComparer ?? EqualityComparer<B>.Default;
                this.cComparer = cComparer ?? EqualityComparer<C>.Default;
                this.dComparer = dComparer ?? EqualityComparer<D>.Default;
                this.eComparer = eComparer ?? EqualityComparer<E>.Default;
                this.fComparer = fComparer ?? EqualityComparer<F>.Default;
                this.gComparer = gComparer ?? EqualityComparer<G>.Default;
                this.hComparer = hComparer ?? EqualityComparer<H>.Default;
            }

            /// <summary>Test equality of <paramref name="x"/> and <paramref name="y"/></summary>
            public bool Equals(OctetMutableObject<A, B, C, D, E, F, G, H>? x, OctetMutableObject<A, B, C, D, E, F, G, H>? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                if (!aComparer.Equals(x.a, y.a)) return false;
                if (!bComparer.Equals(x.b, y.b)) return false;
                if (!cComparer.Equals(x.c, y.c)) return false;
                if (!dComparer.Equals(x.d, y.d)) return false;
                if (!eComparer.Equals(x.e, y.e)) return false;
                if (!fComparer.Equals(x.f, y.f)) return false;
                if (!gComparer.Equals(x.g, y.g)) return false;
                if (!hComparer.Equals(x.h, y.h)) return false;
                return true;
            }

            /// <summary>Calculate <paramref name="obj"/> hash-code.</summary>
            public int GetHashCode(OctetMutableObject<A, B, C, D, E, F, G, H> obj)  => obj == null ? 0 : (obj.a==null?0:11*aComparer.GetHashCode(obj.a)) + (obj.b==null?0:13*bComparer.GetHashCode(obj.b)) + (obj.c==null?0:17*cComparer.GetHashCode(obj.c)) + (obj.d==null?0:19*dComparer.GetHashCode(obj.d)) + (obj.e==null?0:23*eComparer.GetHashCode(obj.e)) + (obj.f==null?0:29*fComparer.GetHashCode(obj.f)) + (obj.g==null?0:31*gComparer.GetHashCode(obj.g)) + (obj.h==null?0:37*hComparer.GetHashCode(obj.h));
        }

        /// <summary>Order comparer</summary>
        public class Comparer : IComparer<OctetMutableObject<A, B, C, D, E, F, G, H>>
        {
            /// <summary>Singleton</summary>
            static Comparer? instance;
            /// <summary>Singleton</summary>
            public static Comparer Instance => instance ?? (instance = new Comparer());

            /// <summary>Element A comparer</summary>
            public readonly IComparer<A> aComparer;
            /// <summary>Element B comparer</summary>
            public readonly IComparer<B> bComparer;
            /// <summary>Element C comparer</summary>
            public readonly IComparer<C> cComparer;
            /// <summary>Element D comparer</summary>
            public readonly IComparer<D> dComparer;
            /// <summary>Element E comparer</summary>
            public readonly IComparer<E> eComparer;
            /// <summary>Element F comparer</summary>
            public readonly IComparer<F> fComparer;
            /// <summary>Element G comparer</summary>
            public readonly IComparer<G> gComparer;
            /// <summary>Element H comparer</summary>
            public readonly IComparer<H> hComparer;


            /// <summary>Create comparer</summary>
            /// <param name="aComparer">(optional) element comparer</param>
            /// <param name="bComparer">(optional) element comparer</param>
            /// <param name="cComparer">(optional) element comparer</param>
            /// <param name="dComparer">(optional) element comparer</param>
            /// <param name="eComparer">(optional) element comparer</param>
            /// <param name="fComparer">(optional) element comparer</param>
            /// <param name="gComparer">(optional) element comparer</param>
            /// <param name="hComparer">(optional) element comparer</param>
            public Comparer(IComparer<A>? aComparer = default, IComparer<B>? bComparer = default, IComparer<C>? cComparer = default, IComparer<D>? dComparer = default, IComparer<E>? eComparer = default, IComparer<F>? fComparer = default, IComparer<G>? gComparer = default, IComparer<H>? hComparer = default)
            {
                this.aComparer = aComparer ?? System.Collections.Generic.Comparer<A>.Default;
                this.bComparer = bComparer ?? System.Collections.Generic.Comparer<B>.Default;
                this.cComparer = cComparer ?? System.Collections.Generic.Comparer<C>.Default;
                this.dComparer = dComparer ?? System.Collections.Generic.Comparer<D>.Default;
                this.eComparer = eComparer ?? System.Collections.Generic.Comparer<E>.Default;
                this.fComparer = fComparer ?? System.Collections.Generic.Comparer<F>.Default;
                this.gComparer = gComparer ?? System.Collections.Generic.Comparer<G>.Default;
                this.hComparer = hComparer ?? System.Collections.Generic.Comparer<H>.Default;

            }

            /// <summary>Compare <paramref name="x"/> to <paramref name="y"/>.</summary>
            public int Compare(OctetMutableObject<A, B, C, D, E, F, G, H>? x, OctetMutableObject<A, B, C, D, E, F, G, H>? y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;

                int d;
                d = aComparer.Compare(x.a, y.a);
                if (d != 0) return d;
                d = bComparer.Compare(x.b, y.b);
                if (d != 0) return d;
                d = cComparer.Compare(x.c, y.c);
                if (d != 0) return d;
                d = dComparer.Compare(x.d, y.d);
                if (d != 0) return d;
                d = eComparer.Compare(x.e, y.e);
                if (d != 0) return d;
                d = fComparer.Compare(x.f, y.f);
                if (d != 0) return d;
                d = gComparer.Compare(x.g, y.g);
                if (d != 0) return d;
                d = hComparer.Compare(x.h, y.h);
                if (d != 0) return d;

                return 0;
            }
        }

        /// <summary>Print info</summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendTo(sb);
            return sb.ToString();
        }

        /// <summary>Append info to <paramref name="sb"/></summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append(GetType().Name);
            sb.Append("(");
            sb.Append(a);
            sb.Append(", ");
            sb.Append(b);
            sb.Append(", ");
            sb.Append(c);
            sb.Append(", ");
            sb.Append(d);
            sb.Append(", ");
            sb.Append(e);
            sb.Append(", ");
            sb.Append(f);
            sb.Append(", ");
            sb.Append(g);
            sb.Append(", ");
            sb.Append(h);

            sb.Append(")");
        }
    }

}
