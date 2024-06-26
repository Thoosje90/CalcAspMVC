using System.Collections.Concurrent;

namespace CalcAspMVC.Models
{
    public class MemoryModel
    {
        public MemoryModel()
        {
            Memory = new List<float>();
        }

        public List<float> Memory { get; set; }

        //public void AddOrder()
        //{
        //    OrdersById.TryAdd(...);
        //}
    }

    public interface IIntegerDataStore
    {
        void AddItem(int item);
        List<int> GetAllItems();
        void ClearItems();
    }

    public class IntegerDataStore : IIntegerDataStore
    {
        private readonly List<int> _items = new List<int>();

        public void AddItem(int item)
        {
            lock (_items)
            {
                _items.Add(item);
            }
        }

        public List<int> GetAllItems()
        {
            lock (_items)
            {
                return new List<int>(_items);
            }
        }

        public void ClearItems()
        {
            lock (_items)
            {
                _items.Clear();
            }
        }
    }
}
