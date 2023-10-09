using System.Windows.Controls;
using System.Windows;

namespace EditCellDataGrid.Extensions
{
    public static class WindowExtensions
    {
        /// <summary>
        /// Define position of the Window Edit
        /// </summary>
        /// <param name="editCell"></param>
        /// <param name="cellSelected"></param>
        public static  void DefinePosition(this EditCell editCell, DataGridCell cellSelected)
        {
            var source = PresentationSource.FromVisual(cellSelected);
            var dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
            var dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;

            var scalingFactorX = dpiX / 96.0;
            var scalingFactorY = dpiY / 96.0;

            var coordinate = cellSelected.PointToScreen(new Point(0, 0));

            editCell.Left = coordinate.X / scalingFactorX;
            editCell.Top = (coordinate.Y / scalingFactorY) - 1;// - 19;//-27; move um pouco pra cima
        }
    }
}