using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace neanea
{
    public class SortingAlgorithms
    {
        //Merge sort algorthm has had inspiration taken from youtube video to support coding it so some parts are the same or very similar.
        public List<(double priority, long item)> Merge(List<(double priority, long item)> left, List<(double priority, long item)> right) //Merges split lists back together
        {
            List<(double priority, long item)> newlist = new List<(double priority, long item)>();
            int i = 0, j = 0;
            while (i < left.Count && j < right.Count)
            {
                if (left[i].priority <= right[j].priority) 
                {
                    newlist.Add(left[i]);
                    i++;
                }
                else
                {
                    newlist.Add(right[j]);
                    j++;
                }
            }
            while (i < left.Count)
            {
                newlist.Add(left[i]);
                i++;
            }
            while (j < right.Count)
            {
                newlist.Add(right[j]);
                j++;
            }
            return newlist; 
        }

        public List<(double priority, long item)> MergeSort(List<(double priority, long item)> queue) //Recursively splits lists up 
        {
            int length = queue.Count;
            if (length <= 1) 
            {
                return queue;
            }
            int middle = length / 2;
            List<(double priority, long item)> left = new List<(double priority, long item)>();
            List<(double priority, long item)> right = new List<(double priority, long item)>();
            for (int i = 0; i < middle; i++) 
            {
                left.Add(queue[i]);
            }
            for (int j = middle; j < length; j++)
            {
                right.Add(queue[j]);
            }

            left = MergeSort(left); 
            right = MergeSort(right);
            return Merge(left, right); 
        }

        public void BubbleSortAscending(DataGridView datagridview, int columnindex) //sorts datagridview ascending 
        {
            int rowcount = datagridview.Rows.Count - 1;
            for (int passnum = 0; passnum < rowcount; passnum++)
            {
                for (int index = 0; index < rowcount - passnum; index++)
                {
                    string value1 = datagridview.Rows[index].Cells[columnindex].Value?.ToString();
                    string value2 = datagridview.Rows[index + 1].Cells[columnindex].Value?.ToString();

                    if (value1 != null && value2 != null && value1.CompareTo(value2) > 0) //returns 1 if value 1 is > value 2
                    {
                        Swap(datagridview, index, index + 1);
                    }
                }
            }
        }

        public void BubbleSortDescending(DataGridView datagridview, int columnindex) //sorts datagrid view descending
        {
            int rowcount = datagridview.Rows.Count - 1;
            for (int passnum = 0; passnum < rowcount; passnum++)
            {
                for (int index = 0; index < rowcount - passnum; index++)
                {
                    string value1 = datagridview.Rows[index].Cells[columnindex].Value?.ToString(); 
                    string value2 = datagridview.Rows[index + 1].Cells[columnindex].Value?.ToString(); 

                    if (value1 != null && value2 != null && value1.CompareTo(value2) < 0) //returns -1 or 0 if value 1 < value 2
                    {
                        Swap(datagridview, index, index + 1);
                    }
                }
            }
        }

        public void Swap(DataGridView datagridview, int index1, int index2) //swaps the rows around 
        {
            for (int i = 0; i < datagridview.ColumnCount; i++)
            {
                var temp = datagridview.Rows[index1].Cells[i].Value;
                datagridview.Rows[index1].Cells[i].Value = datagridview.Rows[index2].Cells[i].Value;
                datagridview.Rows[index2].Cells[i].Value = temp;
            }
        }
    }
}
