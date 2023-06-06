using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Common
{
    public class Helper
    {
        public static string CountQuery(string queryString, string tableName, string columnName)
        {
            List<string> stringsList = new List<string>(queryString.Split(' '));

            int element = 0;
            while (element < stringsList.Count())
            {
                if (stringsList[element].Equals("FROM", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                else
                {
                    if (!stringsList[element].Equals("SELECT", StringComparison.OrdinalIgnoreCase))
                    {
                        stringsList.Remove(stringsList[element]);
                    }
                    else
                    {
                        element++;
                    }
                }
            }

            string newQuery = string.Join(" ", stringsList);
            string finalQuery = newQuery.Replace("SELECT ", $"SELECT COUNT(\"{tableName}\".\"{columnName}\") ");

            return finalQuery;
        }
    }
}
