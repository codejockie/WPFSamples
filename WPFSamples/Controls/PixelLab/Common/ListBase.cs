using System.Collections;
using System.Diagnostics;
using WPFSamples.Controls.PixelLab.Contracts;

namespace WPFSamples.Controls.PixelLab.Common;

/// <summary>
///     Serves as a base implementation of <see cref="IList{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the item in the list.</typeparam>
[ContractClass(typeof(ListBaseContract<>))]
public abstract class ListBase<T> : IList<T>, IList
{
    protected virtual void RemoveItem(int index)
    {
        throw new NotSupportedException();
    }

    protected virtual void InsertItem(int index, T item)
    {
        throw new NotSupportedException();
    }

    protected virtual void ClearItems()
    {
        throw new NotSupportedException();
    }

    protected virtual void SetItem(int index, T value)
    {
        throw new NotSupportedException();
    }

    protected virtual bool IsReadOnly => true;

    protected virtual bool IsFixedSize => true;

    protected virtual object SyncRoot
    {
        get
        {
            if (_mSyncRoot == null)
            {
                Interlocked.CompareExchange(ref _mSyncRoot, new object(), null);
            }

            return _mSyncRoot;
        }
    }

    #region IList<T> Members

    public virtual int IndexOf(T item)
    {
        for (var i = 0; i < Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(this[i], item))
            {
                return i;
            }
        }

        return -1;
    }

    public T this[int index] => GetItem(index);

    protected abstract T GetItem(int index);

    void IList<T>.Insert(int index, T item)
    {
        InsertItem(index, item);
    }

    void IList<T>.RemoveAt(int index)
    {
        RemoveItem(index);
    }

    T IList<T>.this[int index]
    {
        get => this[index];
        set => SetItem(index, value);
    }

    #endregion

    #region ICollection<T> Members

    [Pure]
    public virtual bool Contains(T item)
    {
        if (item == null)
        {
            for (var num1 = 0; num1 < Count; num1++)
            {
                if (this[num1] == null)
                {
                    return true;
                }
            }

            return false;
        }

        var comparer1 = EqualityComparer<T>.Default;
        for (var num2 = 0; num2 < Count; num2++)
        {
            if (comparer1.Equals(this[num2], item))
            {
                return true;
            }
        }

        return false;
    }

    public virtual void CopyTo(T[] array, int arrayIndex)
    {
        CopyTo((Array)array, arrayIndex);
    }

    public abstract int Count { get; }

    bool ICollection<T>.IsReadOnly => IsReadOnly;

    void ICollection<T>.Add(T item)
    {
        InsertItem(Count, item);
    }

    void ICollection<T>.Clear()
    {
        ClearItems();
    }

    bool ICollection<T>.Remove(T item)
    {
        var index = IndexOf(item);
        if (index < 0)
        {
            return false;
        }

        RemoveItem(index);
        return true;
    }

    #endregion

    #region IEnumerable<T> Members

    public virtual IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    #endregion

    #region IList Members

    int IList.Add(object value)
    {
        VerifyValueType(value);
        InsertItem(Count, (T)value);
        return Count - 1;
    }

    bool IList.Contains(object value)
    {
        return IsCompatibleObject(value) && Contains((T)value);
    }

    int IList.IndexOf(object value)
    {
        if (IsCompatibleObject(value))
        {
            return IndexOf((T)value);
        }

        return -1;
    }

    void IList.Insert(int index, object value)
    {
        VerifyValueType(value);
        InsertItem(index, (T)value);
    }

    void IList.Remove(object value)
    {
        if (IsCompatibleObject(value))
        {
            RemoveItem(IndexOf((T)value));
        }
    }

    object IList.this[int index]
    {
        get => this[index];
        set
        {
            VerifyValueType(value);
            SetItem(index, (T)value);
        }
    }

    void IList.RemoveAt(int index)
    {
        RemoveItem(index);
    }

    bool IList.IsReadOnly => IsReadOnly;

    bool IList.IsFixedSize => IsFixedSize;

    void IList.Clear()
    {
        ClearItems();
    }

    #endregion

    #region ICollection Members

    public virtual void CopyTo(Array array, int index)
    {
        for (var i = 0; i < Count; i++)
        {
            array.SetValue(this[i], index + i);
        }
    }

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => SyncRoot;

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    private object _mSyncRoot;

    #region Private Static Helpers

    private static bool IsCompatibleObject(object value)
    {
        if (!(value is T) && (value != null || typeof(T).IsValueType))
        {
            return false;
        }

        return true;
    }

    [DebuggerStepThrough]
    private static void VerifyValueType(object value)
    {
        if (!IsCompatibleObject(value))
        {
            throw new ArgumentException("value");
        }
    }

    #endregion
}

[ContractClassFor(typeof(ListBase<>))]
internal abstract class ListBaseContract<T> : ListBase<T>
{
    protected override T GetItem(int index)
    {
        Contract.Requires(index >= 0);
        Contract.Requires(index < Count);
        return default;
    }

    public override int Count => default;
}