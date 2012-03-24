﻿using System;
using System.Collections.Generic;
using System.Linq;
using VA=VisioAutomation;
using System.Collections;

namespace VisioAutomation.ShapeSheet.Data
{
    public class QueryDataSet<T> : IEnumerable<QueryDataRow<T>>
    {
        internal readonly int ColumnCount;
        internal readonly int RowCount;

        public TableRowGroupList Groups { get; private set; }
        public Table<string> Formulas { get; private set; }
        public Table<T> Results { get; private set; }

        internal QueryDataSet(string[] formulas_array, T[] results_array, IList<int> shapeids, int columncount,
                            int rowcount, TableRowGroupList groups)
        {
            if (formulas_array == null && results_array == null)
            {
                throw new AutomationException("Both formulas and results cannot be null");
            }

            if (formulas_array != null & results_array != null)
            {
                if (formulas_array.Length != results_array.Length)
                {
                    throw new AutomationException("Formula array and Result array must have the same length");
                }
            }

            if (shapeids.Count != groups.Count)
            {
                throw new AutomationException("The number of shapes must be equal to the number of groups");
            }

            int groupcountsum = groups.Select(g=>g.Count).Sum();
            if (rowcount != groupcountsum)
            {
                throw new AutomationException("The total number of rows must be equal to the sum of the group counts");                
            }

            int totalcells = columncount*rowcount;

            if (formulas_array != null)
            {
                if (totalcells != formulas_array.Length)
                {
                    throw new AutomationException("Invalid number of formulas");
                }                
            }

            if (results_array != null)
            {
                if (totalcells != results_array.Length)
                {
                    throw new AutomationException("Invalid number of formulas");
                }
            }

            this.RowCount = rowcount;
            this.ColumnCount = columncount;
            this.Groups = groups;
            this.Formulas = formulas_array != null ? this.BuildTableFromArray(formulas_array) : null;
            this.Results = results_array != null ? this.BuildTableFromArray(results_array) : null;
        }

        private Table<X> BuildTableFromArray<X>(X[] array)
        {
            var table = new Table<X>(this.Count, this.ColumnCount, this.Groups, array);
            return table;
        }

        public QueryDataRow<T> this[int index]
        {
            get { return new QueryDataRow<T>(this, index); }
        }

        public VA.ShapeSheet.CellData<T> GetItem(int row, VA.ShapeSheet.Query.QueryColumn col)
        {
            string formula = this.Formulas[row, col];
            T result = this.Results[row, col];
            var cd = new VA.ShapeSheet.CellData<T>(formula, result);
            return cd;
        }

        public IEnumerator<QueryDataRow<T>> GetEnumerator()
        {
            for (int i = 0; i < this.RowCount; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()     // Explicit implementation
        {                                           // keeps it hidden.
            return GetEnumerator();
        }

        public int Count
        {
            get { return this.RowCount; }
        }

        internal VA.ShapeSheet.Data.Table<CellData<T>> create_merged_table()
        {
            int n = this.Count*this.ColumnCount;
            var array = new VA.ShapeSheet.CellData<T>[n];
            for (int i=0; i<n; i++)
            {
                array[i] = new VA.ShapeSheet.CellData<T>(this.Formulas.RawArray[i], this.Results.RawArray[i]);
            }
            var table = new VA.ShapeSheet.Data.Table<CellData<T>>(this.Count, this.ColumnCount, this.Groups, array);
            return table;
        }
    }
}