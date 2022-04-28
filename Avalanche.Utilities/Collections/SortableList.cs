// Copyright (c) Toni Kalajainen 2022
using System.Runtime.Serialization;
using Avalanche.Utilities.Internal;

namespace Avalanche.Utilities
{
    /// <summary>
    /// List that has elements and associated sort order. 
    /// 
    /// Elements and order values are in separate sub-lists, so that a reference to element and order lists can be taken distictively. 
    /// 
    /// List not maintained in sorted order, but must be put into order with explicit <see cref="Sort"/> call.
    /// 
    /// List has mutability state, and throws <see cref="InvalidOperationException"/> if is modified after put into read-only state.
    /// </summary>
    public interface ISortableList : IReadOnly
    {
        /// <summary>Get element type</summary>
        Type ElementType { get; }
        /// <summary>Elements as <![CDATA[IList<T>]]></summary>
        object Elements { get; set; }
        /// <summary>Orders, each line corresponds to line in <see cref="Elements"/></summary>
        IList<long> Orders { get; set; }
        /// <summary>Get number of lines</summary>
        int Count { get; }
        /// <summary>Sort <see cref="Elements"/> by <see cref="Orders"/> values in ascending order. Must be in mutable-state.</summary>
        void Sort();
        /// <summary>Add <paramref name="element"/> with associated order value <paramref name="order"/>.</summary>
        void Add(long order, object element);
        /// <summary>object that can be used to synchronize the access.</summary>
        object SyncRoot { get; }
    }

    /// <summary>
    /// List that has elements and associated sort order. 
    /// 
    /// Elements and order values are in separate sub-lists, so that a reference to element and order lists can be taken distictively. 
    /// 
    /// List not maintained in sorted order, but must be put into order with explicit <see cref="ISortableList.Sort"/> call.
    /// 
    /// List has mutability state, and throws <see cref="InvalidOperationException"/> if is modified after put into read-only state.
    /// </summary>
    public interface ISortableList<T> : ISortableList
    {
        /// <summary></summary>
        new IList<T> Elements { get; set; }
        /// <summary>Add <paramref name="element"/> with associated order value <paramref name="order"/>.</summary>
        void Add(long order, T element);
    }

    /// <summary>
    /// List that has elements and associated sort order. 
    /// 
    /// Elements and order values are in separate sub-lists, so that a reference to element and order lists can be taken distictively. 
    /// 
    /// List not maintained in sorted order, but must be put into order with explicit <see cref="ISortableList.Sort"/> call.
    /// 
    /// List has mutability state, and throws <see cref="InvalidOperationException"/> if is modified after put into read-only state.
    /// 
    /// This implementation uses internal lock for concurrent use. 
    /// 
    /// Internal lists are allocated lazily.
    /// 
    /// Putting list into immutable state, replaces lists with arrays.
    /// </summary>
    public abstract class SortableList : ISortableList
    {
        /// <summary></summary>
        protected static readonly ConstructorT<SortableList> constructor = new(typeof(SortableList<>));
        /// <summary></summary>
        public static SortableList Create(Type elementType) => constructor.Create(elementType);
        /// <summary>List of order values</summary>
        protected IList<long>? orders;

        /// <summary>Assert writable</summary>
        SortableList AssertWritable { get { if (@readonly) throw new InvalidOperationException("Read-only"); return this; } }

        /// <summary>object that can be used to synchronize the access.</summary>
        public object SyncRoot => this;
        /// <summary>Get element type</summary>
        public abstract Type ElementType { get; }
        /// <summary>Get number of elements</summary>
        public int Count
        {
            get
            {
                // Get reference
                IList<long>? _orders = this.orders;
                // No reference
                if (_orders == null) return 0;
                // Get count
                return _orders.Count;
            }
        }
        /// <summary></summary>
        public object Elements { get => getElements(); set => setElements(value); }
        /// <summary></summary>
        public IList<long> Orders
        {
            get
            {
                IList<long>? value = orders;
                if (value != null) return value;
                lock (SyncRoot) return orders ??= new List<long>();
            }
            set => AssertWritable.orders = value;
        }

        /// <summary></summary>
        protected abstract object getElements();
        /// <summary></summary>
        protected abstract void setElements(object services);

        /// <summary>Is read-only state</summary>
        protected bool @readonly;
        /// <summary>Is read-only state</summary>
        [IgnoreDataMember] bool IReadOnly.ReadOnly { get => @readonly; set { if (@readonly == value) return; if (!value) throw new InvalidOperationException("Read-only"); setReadOnly(); } }

        /// <summary>Assign read-only state</summary>
        protected virtual void setReadOnly() { @readonly = true; }

        /// <summary>Sort <see cref="Elements"/> by <see cref="Orders"/> values in ascending order. Must be in mutable-state.</summary>
        public abstract void Sort();

        /// <summary>Add <paramref name="element"/> with associated order value <paramref name="order"/>.</summary>
        public abstract void Add(long order, object element);
    }

    /// <summary>
    /// List that has elements and associated sort order. 
    /// 
    /// Elements and order values are in separate sub-lists, so that a reference to element and order lists can be taken distictively. 
    /// 
    /// List not maintained in sorted order, but must be put into order with explicit <see cref="Sort"/> call.
    /// 
    /// List has mutability state, and throws <see cref="InvalidOperationException"/> if is modified after put into read-only state.
    /// 
    /// This implementation uses internal lock for concurrent use. 
    /// 
    /// Internal lists are allocated lazily.
    /// 
    /// Putting list into immutable state, replaces lists with arrays.
    /// </summary>
    public class SortableList<T> : SortableList, ISortableList<T>
    {
        /// <summary></summary>
        protected IList<T>? elements;

        /// <summary></summary>
        public override Type ElementType => typeof(T);
        /// <summary>Assert writable</summary>
        SortableList<T> AssertWritable => @readonly ? throw new InvalidOperationException("Read-only") : this;

        /// <summary></summary>
        public new IList<T> Elements
        {
            get
            {
                IList<T>? value = elements;
                if (value != null) return value;
                lock (SyncRoot) return elements ??= new List<T>();
            }
            set => AssertWritable.elements = value;
        }

        /// <summary></summary>
        public SortableList()
        {
            this.elements = new List<T>(1);
            this.orders = new List<long>(1);
        }

        /// <summary></summary>
        protected override object getElements() => Elements;
        /// <summary></summary>
        protected override void setElements(object value) => Elements = (IList<T>)value;

        /// <summary>Assign read-only state</summary>
        protected override void setReadOnly()
        {
            // Already read-only
            if (@readonly) return;
            // 
            lock (SyncRoot)
            {
                // Already read-only
                if (@readonly) return;
                // Take array snapshots
                this.orders = this.orders == null ? Array.Empty<long>() : this.orders.ToArray();
                this.elements = this.elements == null ? Array.Empty<T>() : this.elements.ToArray();
                // Assign read-only
                @readonly = true;
            }
        }

        /// <summary>Add <paramref name="element"/> with associated order value <paramref name="order"/>.</summary>
        public void Add(long order, T element)
        {
            lock (SyncRoot)
            {
                // Read-only
                if (@readonly) throw new InvalidOperationException("Read-only");
                // Add
                this.Orders.Add(order);
                this.Elements.Add(element);
            }
        }

        /// <summary>Add <paramref name="element"/> with associated order value <paramref name="order"/>.</summary>
        public override void Add(long order, object element)
        {
            // Cast
            T elementT = (T)element;
            lock (SyncRoot)
            {
                // Read-only
                if (@readonly) throw new InvalidOperationException("Read-only");
                // Add
                this.Orders.Add(order);
                this.Elements.Add(elementT);
            }
        }

        /// <summary>Sort lines</summary>
        public override void Sort()
        {
            // Read-only
            if (@readonly) throw new InvalidOperationException("Read-only");
            //
            lock (SyncRoot)
            {
                // No orders and services
                if (this.orders == null && this.elements == null) return;
                // Sort list
                SortableListSorter<T>.Instance.Sort(this);
            }
        }

        /// <summary>Forwards dispose to contents.</summary>
        public class DisposeContents : SortableList<T>, IDisposable
        {
            /// <summary></summary>
            public void Dispose()
            {
                // Get elements
                IList<T>? _elements = elements;
                // No elements to dispose
                if (_elements == null || _elements.Count == 0) return;
                // Captured errors here
                StructList4<Exception> errors = new StructList4<Exception>();
                // Dispose each element
                foreach (T element in _elements)
                {
                    // Cast
                    if (element is not IDisposable disposable) continue;
                    //
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (AggregateException ae)
                    {
                        foreach (Exception e in ae.InnerExceptions)
                            errors.Add(e);
                    }
                    catch (Exception e)
                    {
                        // Capture error
                        errors.Add(e);
                    }

                }
                // Suppress further finalize
                GC.SuppressFinalize(this);
                // Throw captured errors
                if (errors.Count > 0) throw ExceptionUtilities.Wrap(errors);
            }
        }
    }
}

namespace Avalanche.Utilities.Internal
{
    /// <summary>Sorts <see cref="ISortableList{T}"/> by order value.</summary>
    public class SortableListSorter<T>
    {
        /// <summary>Singleton</summary>
        static readonly SortableListSorter<T> instance = new SortableListSorter<T>();
        /// <summary>Singleton</summary>
        public static SortableListSorter<T> Instance => instance;

        /// <summary>Sort elements of <paramref name="list"/>.</summary>
        public void Sort(ISortableList<T> list)
        {
            // Get references
            IList<T> elements = list.Elements;
            IList<long> orders = list.Orders;
            // Assert valid
            if (elements == null) throw new ArgumentNullException(nameof(list), nameof(ISortableList.Elements) + " is null");
            if (orders == null) throw new ArgumentNullException(nameof(list), nameof(ISortableList.Orders) + " is null");
            int ec = elements.Count, oc = orders.Count;
            if (ec != oc) throw new InvalidOperationException($"Invalid state, Elements.Count={ec}, orders.Count={oc}");
            // Nothing to do
            if (ec <= 1 && oc <= 1) return;
            //
            QuickSort(elements, orders, 0, ec - 1);
        }

        /// <summary>Internal sort</summary>
        private void QuickSort(IList<T> elements, IList<long> orders, int left, int right)
        {
            if (left < right)
            {
                int pivot = Partition(elements, orders, left, right);

                if (pivot > 1)
                {
                    QuickSort(elements, orders, left, pivot - 1);
                }
                if (pivot + 1 < right)
                {
                    QuickSort(elements, orders, pivot + 1, right);
                }
            }
        }

        /// <summary>Partition</summary>
        private int Partition(IList<T> elements, IList<long> orders, int left, int right)
        {
            if (left > right) return -1;
            int end = left;
            // Order of pivot
            long pivot = orders[right];

            for (int i = left; i < right; i++)
            {
                long oi = orders[i];

                if (oi < pivot)
                {
                    // Swap elements[i] and elements[end]
                    T tmpElement = elements[i];
                    elements[i] = elements[end];
                    elements[end] = tmpElement;

                    // Swap orders[i] and orders[end]
                    long tmpOrder = orders[i];
                    orders[i] = orders[end];
                    orders[end] = tmpOrder;

                    end++;
                }
            }
            {
                // Swap elements[end] and elements[right]
                T tmpElement = elements[end];
                elements[end] = elements[right];
                elements[right] = tmpElement;

                // Swap orders[end] and orders[right]
                long tmpOrder = orders[end];
                orders[end] = orders[right];
                orders[right] = tmpOrder;
            }
            return end;
        }
    }
}
