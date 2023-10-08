using EditCellDataGrid.EventsArgs;

namespace EditCellDataGrid.Delegates
{
    public delegate void DataGridlValueChangedEventHanddler<T>(object sender, DataGridlValueChangedEventArgs<T> e) where T : class, new();
}