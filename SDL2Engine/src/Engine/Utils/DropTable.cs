using SDL2Engine.Rand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDL2Engine.Utils
{

    /*
     * | DropTable
     * | - List<DropTableItem>
     * | - TotalWeight
     * | - AddItem
     * | - Roll(n) -> List<Item>
     * 
     * 
     * | IDropTableItem
     * | - GetWeight
     * | - Roll(n) -> List<Item>
     */

    public interface IDropTableItem<T>
    {
        int GetWeight();
        List<T> Roll(int rolls);
    }

    public class BasicDropTableItem<T> : IDropTableItem<T>
    {
        public int Weight { get; set; }
        public T Item { get; set; }

        public BasicDropTableItem(int weight, T item)
        {
            Weight = weight;
            Item = item;
        }

        public int GetWeight()
        {
            return Weight;
        }

        public List<T> Roll(int rolls)
        {
            List<T> items = new List<T>();
            for (int i = 0; i < rolls; i++)
            {
                items.Add(Item);
            }
            return items;
        }
    }

    /* A generic class that represents a drop table. 
     * A drop table is a list of items that can be dropped by a monster or a chest. 
     * Each item has a weight, which determines the probability of it being dropped. 
     */
    public class DropTable<T>
    {

        private List<IDropTableItem<T>> Items { get; set; }
        private int TotalWeight { get; set; }
        private StableRandom random = new StableRandom();

        public DropTable()
        {
            Items = new List<IDropTableItem<T>>();
            TotalWeight = 0;
        }

        public void AddItem(IDropTableItem<T> item)
        {
            Items.Add(item);
            TotalWeight += item.GetWeight();
        }

        public List<T> Roll(int rolls)
        {
            List<T> items = new List<T>();
            for (int i = 0; i < rolls; i++)
            {
                int roll = random.Next(0, TotalWeight);
                int currentWeight = 0;
                foreach (IDropTableItem<T> item in Items)
                {
                    currentWeight += item.GetWeight();
                    if (roll < currentWeight)
                    {
                        items.AddRange(item.Roll(1));
                        break;
                    }
                }
            }
            return items;
        }

    }

    public class SubDropTableItem<T> : IDropTableItem<T>
    {
        public int Weight { get; set; }
        public DropTable<T> SubTable { get; set; }

        public SubDropTableItem(int weight, DropTable<T> subTable)
        {
            Weight = weight;
            SubTable = subTable;
        }

        public int GetWeight()
        {
            return Weight;
        }

        public List<T> Roll(int rolls)
        {
            return SubTable.Roll(rolls);
        }
    }

}
