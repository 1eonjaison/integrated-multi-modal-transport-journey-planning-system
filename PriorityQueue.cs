using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neanea
{
    public class PriorityQueue
    {
        private double maxnumofstops = 10000000000000000000;
        private List<(double priority, long node)> queue = new List<(double, long)>();

        public bool IsEmpty() //checks if queue is empty 
        {
            if (queue.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsFull() //checks if queue is full
        {
            if (queue.Count == maxnumofstops)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Enqueue(double priority, long node) //used to add items to queue and resorts the list after addition
        {
            SortingAlgorithms sorts = new SortingAlgorithms();
            if (IsFull() == true) 
            {
                Console.WriteLine("Queue is full, too many stops");
                return false;
            }
            else
            {
                queue.Add((priority, node));

                queue = sorts.MergeSort(queue);
                return true;
            }
        }

        public bool Dequeue(out double priority, out long item) //used to remove item from queue which sorts the list and then gets the first item
        {
            SortingAlgorithms sorts = new SortingAlgorithms();
            if (IsEmpty())
            {
                Console.WriteLine("Queue is empty, there are no stops.");
                priority = 10000000000000000000;
                item = -1;
                return false;
            }
            else
            {
                (double priorityValue, long itemValue) = sorts.MergeSort(queue).First(); 
                priority = priorityValue;
                item = itemValue;
                queue.Remove((priorityValue, itemValue));
                return true;
            }
        }
    }
}
