using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Utilities.GUI
{
    public class MultiColumnFlowGrid : Grid
    {
        private int num_columns;
        private double spacing;

        private int current_column = 0;
        private int current_row = 0;

        public MultiColumnFlowGrid(int num_columns, double spacing)
        {
            this.num_columns = num_columns;
            this.spacing = spacing;

            // Add the column definitions
            for (int i = 0; i < num_columns; ++i)
            {
                if (0 != i && 0 != spacing)
                {
                    ColumnDefinition cd = new ColumnDefinition();
                    cd.Width = new GridLength(spacing);
                    this.ColumnDefinitions.Add(cd);
                }

                {
                    ColumnDefinition cd = new ColumnDefinition();
                    cd.Width = new GridLength(1, GridUnitType.Star);
                    this.ColumnDefinitions.Add(cd);
                }
            }

            // Add the first row
            this.RowDefinitions.Add(new RowDefinition());
        }

        public void Add(UIElement element)
        {            
            // If we have filled the current row
            if (current_column >= num_columns)
            {
                this.RowDefinitions.Add(new RowDefinition());
                ++current_row;
                current_column = 0;
            }

            // Add the element
            Grid.SetColumn(element, 2 * current_column);
            Grid.SetRow(element, current_row);
            this.Children.Add(element);

            ++current_column;
        }
    }
}
