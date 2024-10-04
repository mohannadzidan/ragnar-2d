using System.Collections.Generic;
// Priority Queue (Min-Heap) implementation for Node objects
public class MinHeap<T> where T : IHeapItem<T>
{
    private List<T> items;
    public int Count { get; private set; }

    public MinHeap(int maxHeapSize)
    {
        items = new List<T>(maxHeapSize);
    }

    public void Add(T item)
    {
        item.HeapIndex = Count;
        items.Add(item);
        Count++;
        HeapifyUp(item);
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        Count--;
        items[0] = items[Count];
        items[0].HeapIndex = 0;
        HeapifyDown(items[0]);
        items.RemoveAt(Count);
        return firstItem;
    }

    public void UpdateItem(T item)
    {
        HeapifyUp(item);
    }

    private void HeapifyUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (item.HeapIndex > 0 && item.CompareTo(items[parentIndex]) > 0)
        {
            Swap(item, items[parentIndex]);
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    private void HeapifyDown(T item)
    {
        while (true)
        {
            int leftChildIndex = item.HeapIndex * 2 + 1;
            int rightChildIndex = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            if (leftChildIndex < Count)
            {
                swapIndex = leftChildIndex;
                if (rightChildIndex < Count && items[leftChildIndex].CompareTo(items[rightChildIndex]) < 0)
                {
                    swapIndex = rightChildIndex;
                }
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int tempIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = tempIndex;
    }
}
