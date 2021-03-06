using System.Collections.Generic;

namespace VisioPowerShell.Models
{
    public abstract class BaseCells
    {
        public abstract IEnumerable<CellTuple> GetCellTuples();

        public void Apply(VisioAutomation.ShapeSheet.Writers.SidSrcWriter writer, short id)
        {
            foreach (var pair in this.GetCellTuples())
            {
                if (pair.Formula != null)
                {
                    writer.SetFormula(id, pair.Src, pair.Formula);
                }
            }
        }

        public static BaseCells CreateCells(CellType type)
        {
            if (type == VisioPowerShell.Models.CellType.Page)
            {
                return new VisioPowerShell.Models.PageCells();
            }
            else if (type == VisioPowerShell.Models.CellType.ShapeFormat)
            {
                return new VisioPowerShell.Models.ShapeFormatCells();
            }
            else if (type == VisioPowerShell.Models.CellType.TextFormat)
            {
                return new VisioPowerShell.Models.TextFormatCells();
            }
            else if (type == VisioPowerShell.Models.CellType.TextBlock)
            {
                return new VisioPowerShell.Models.TextBlockCells();
            }
            else if (type == VisioPowerShell.Models.CellType.ShapeXForm)
            {
                return new VisioPowerShell.Models.ShapeXFormCells();
            }
            else if (type == VisioPowerShell.Models.CellType.Lock)
            {
                return new VisioPowerShell.Models.LockCells();
            }
            else
            {
                throw new System.ArgumentOutOfRangeException();
            }
        }

        public static VisioPowerShell.Models.NamedCellDictionary GetDictionary(CellType type)
        {
            var cells = BaseCells.CreateCells(type);
            var dic = VisioPowerShell.Models.NamedCellDictionary.FromCells(cells);
            return dic;
        }
    }
}