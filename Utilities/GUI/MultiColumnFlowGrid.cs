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
                    ColumnDefinitions.Add(cd);
                }

                {
                    ColumnDefinition cd = new ColumnDefinition();
                    cd.Width = new GridLength(1, GridUnitType.Star);
                    ColumnDefinitions.Add(cd);
                }
            }

            // Add the first row
            RowDefinitions.Add(new RowDefinition());
        }

        public void Add(UIElement element)
        {
            // If we have filled the current row
            if (current_column >= num_columns)
            {
                RowDefinitions.Add(new RowDefinition());
                ++current_row;
                current_column = 0;
            }

            // Add the element
            Grid.SetColumn(element, 2 * current_column);
            Grid.SetRow(element, current_row);
            Children.Add(element);

            ++current_column;
        }
    }
}
