using System;
using System.Linq;
using System.Collections.Generic;

namespace PrometheusCore
{
    public class Labels : List<ILabel>, ILabels
    {
        public static ILabels Empty => new Labels(new string[0], new string[0]);
        

        public Labels(string[] names, string[] values)
        {
            if (names.Length != values.Length)
            {
                throw new ArgumentOutOfRangeException("There should be a value for every Label.");
            }

            for (int i = 0; i < names.Length; i++)
            {
                Add(new Label(names[i], values[i]));
            }
        }
       
        public Labels(ILabels labels)
        {
            for (int i = 0; i < labels.Count; i++)
            {
                Add(new Label(labels[i].Name, labels[i].Value));
            }
        }
        public string Serialize()
        {
            return string.Join(",", this);
        }
        public bool Equals(ILabels x, ILabels y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }
            else
            { 
                for(int i = 0; i < x.Count; i++)
                {
                    if(x[i] != y[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public int GetHashCode(ILabels obj)
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return string.Join(",", this);
        }
    }
}