using System.Collections.Generic;
using VisioAutomation.Shapes;
using VisioAutomation.ShapeSheet;
using VisioAutomation.ShapeSheet.Writers;

namespace VisioScripting.Commands
{
    public class ArrangeCommands : CommandSet
    {
        internal ArrangeCommands(Client client) :
            base(client)
        {

        }

        public void Nudge(VisioScripting.Models.TargetShapes targets, double dx, double dy)
        {
            if (dx == 0.0 && dy == 0.0)
            {
                return;
            }

            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            int shape_count = targets.SetSelectionGetSelectedCount(this._client);
            if (shape_count < 1)
            {
                return;
            }

            using (var undoscope = this._client.Application.NewUndoScope("Nudge"))
            {
                var selection = this._client.Selection.Get();
                var unitcode = Microsoft.Office.Interop.Visio.VisUnitCodes.visInches;

                // Move method: http://msdn.microsoft.com/en-us/library/ms367549.aspx   
                selection.Move(dx, dy, unitcode);
            }
        }

        private static void SendShapes(Microsoft.Office.Interop.Visio.Selection selection, Models.ShapeSendDirection dir)
        {

            if (dir == Models.ShapeSendDirection.ToBack)
            {
                selection.SendToBack();
            }
            else if (dir == Models.ShapeSendDirection.Backward)
            {
                selection.SendBackward();
            }
            else if (dir == Models.ShapeSendDirection.Forward)
            {
                selection.BringForward();
            }
            else if (dir == Models.ShapeSendDirection.ToFront)
            {
                selection.BringToFront();
            }
        }


        public void Send(VisioScripting.Models.TargetShapes targets, Models.ShapeSendDirection dir)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            int shape_count = targets.SetSelectionGetSelectedCount(this._client);
            if (shape_count < 1)
            {
                return;
            }

            var selection = this._client.Selection.Get();
            ArrangeCommands.SendShapes(selection, dir);
        }

        public void SetLock(VisioScripting.Models.TargetShapes targets, LockCells lockcells)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            targets = targets.ResolveShapes(this._client);
            if (targets.Shapes.Count < 1)
            {
                return;
            }

            var page = this._client.Page.Get();
            var target_shapeids = targets.ToShapeIDs();
            var writer = new SidSrcWriter();

            foreach (int shapeid in target_shapeids.ShapeIDs)
            {
                lockcells.SetFormulas(writer, (short)shapeid);
            }

            using (var undoscope = this._client.Application.NewUndoScope("Set Lock Properties"))
            {
                writer.Commit(page);
            }
        }


        public Dictionary<int,LockCells> GetLock(VisioScripting.Models.TargetShapes targets)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            targets = targets.ResolveShapes(this._client);
            if (targets.Shapes.Count < 1)
            {
                return new Dictionary<int, LockCells>();
            }

            var dic = new Dictionary<int, LockCells>();

            var page = this._client.Page.Get();
            var target_shapeids = targets.ToShapeIDs();

            var cells = VisioAutomation.Shapes.LockCells.GetCells(page, target_shapeids.ShapeIDs, CellValueType.Formula);

            for (int i = 0; i < target_shapeids.ShapeIDs.Count; i++)
            {
                var shapeid = target_shapeids.ShapeIDs[i];
                var cur_cells = cells[i];
                dic[shapeid] = cur_cells;
            }

            return dic;
        }

        public void SetSize(VisioScripting.Models.TargetShapes targets, double? w, double? h)
        {
            this._client.Application.AssertApplicationAvailable();
            this._client.Document.AssertDocumentAvailable();

            targets = targets.ResolveShapes(this._client);
            if (targets.Shapes.Count < 1)
            {
                return;
            }

            var active_page = this._client.Page.Get();
            var shapeids = targets.ToShapeIDs();
            var writer = new SidSrcWriter();
            foreach (int shapeid in shapeids.ShapeIDs)
            {
                if (w.HasValue && w.Value>=0)
                {
                    writer.SetFormula((short)shapeid, VisioAutomation.ShapeSheet.SrcConstants.XFormWidth, w.Value);
                }
                if (h.HasValue && h.Value >= 0)
                {
                    writer.SetFormula((short)shapeid, VisioAutomation.ShapeSheet.SrcConstants.XFormHeight, h.Value);                    
                }
            }

            using (var undoscope = this._client.Application.NewUndoScope("Set Shape Size"))
            {
                writer.Commit(active_page);
            }
        }
    }
}