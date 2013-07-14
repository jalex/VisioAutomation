﻿using System.Collections.Generic;
using System.Linq;
using VisioAutomation.ShapeSheet;
using VisioAutomation.ShapeSheet.Query;
using VA = VisioAutomation;
using IVisio = Microsoft.Office.Interop.Visio;
using VisioAutomation.Extensions;

namespace VisioAutomation.Layout
{
    public class XFormCells : VA.ShapeSheet.CellGroups.CellGroup
    {
        public VA.ShapeSheet.CellData<double> PinX { get; set; }
        public VA.ShapeSheet.CellData<double> PinY { get; set; }
        public VA.ShapeSheet.CellData<double> LocPinX { get; set; }
        public VA.ShapeSheet.CellData<double> LocPinY { get; set; }
        public VA.ShapeSheet.CellData<double> Width { get; set; }
        public VA.ShapeSheet.CellData<double> Height { get; set; }
        public VA.ShapeSheet.CellData<double> Angle { get; set; }


        public override void ApplyFormulas(ApplyFormula func)
        {
            func(ShapeSheet.SRCConstants.PinX, this.PinX.Formula);
            func(ShapeSheet.SRCConstants.PinY, this.PinY.Formula);
            func(ShapeSheet.SRCConstants.LocPinX, this.LocPinX.Formula);
            func(ShapeSheet.SRCConstants.LocPinY, this.LocPinY.Formula);
            func(ShapeSheet.SRCConstants.Width, this.Width.Formula);
            func(ShapeSheet.SRCConstants.Height, this.Height.Formula);
            func(ShapeSheet.SRCConstants.Angle, this.Angle.Formula);
        }

        public static IList<XFormCells> GetCells(IVisio.Page page, IList<int> shapeids)
        {
            var query = get_query();
            return VA.ShapeSheet.CellGroups.CellGroup._GetCells(page, shapeids, query, query.GetCells);
        }

        public static XFormCells GetCells(IVisio.Shape shape)
        {
            var query = get_query();
            return VA.ShapeSheet.CellGroups.CellGroup._GetCells(shape, query, query.GetCells);
        }

        private static XFormQuery m_query;
        private static XFormQuery get_query()
        {
            m_query = m_query ?? new XFormQuery();
            return m_query;
        }

        class XFormQuery : VA.ShapeSheet.Query.QueryEx
        {
            public QueryColumn Width { get; set; }
            public QueryColumn Height { get; set; }
            public QueryColumn PinX { get; set; }
            public QueryColumn PinY { get; set; }
            public QueryColumn LocPinX { get; set; }
            public QueryColumn LocPinY { get; set; }
            public QueryColumn Angle { get; set; }

            public XFormQuery()
            {
                PinX = this.AddColumn(VA.ShapeSheet.SRCConstants.PinX, "PinX");
                PinY = this.AddColumn(VA.ShapeSheet.SRCConstants.PinY, "PinY");
                LocPinX = this.AddColumn(VA.ShapeSheet.SRCConstants.LocPinX, "LocPinX");
                LocPinY = this.AddColumn(VA.ShapeSheet.SRCConstants.LocPinY, "LocPinY");
                Width = this.AddColumn(VA.ShapeSheet.SRCConstants.Width, "Width");
                Height = this.AddColumn(VA.ShapeSheet.SRCConstants.Height, "Height");
                Angle = this.AddColumn(VA.ShapeSheet.SRCConstants.Angle, "Angle");
            }

            public  XFormCells GetCells(QueryResult<CellData<double>> data_for_shape)
            {
                var row = data_for_shape.Cells;

                var cells = new XFormCells
                {
                    PinX = row[this.PinX.Ordinal],
                    PinY = row[this.PinY.Ordinal],
                    LocPinX = row[this.LocPinX.Ordinal],
                    LocPinY = row[this.LocPinY.Ordinal],
                    Width = row[this.Width.Ordinal],
                    Height = row[this.Height.Ordinal],
                    Angle = row[this.Angle.Ordinal]
                };
                return cells;
            }
        }
    }
}