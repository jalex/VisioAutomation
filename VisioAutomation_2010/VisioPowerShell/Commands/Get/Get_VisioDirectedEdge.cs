using System.Collections.Generic;
using System.Management.Automation;
using VACONNECT = VisioAutomation.Shapes.Connections;
using VA = VisioAutomation;

namespace VisioPowerShell.Commands.Get
{
    [Cmdlet(VerbsCommon.Get, VisioPowerShell.Nouns.VisioDirectedEdge)]
    public class Get_VisioDirectedEdge : VisioCmdlet
    {
        [Parameter(Mandatory = false)]
        public SwitchParameter GetShapeObjects { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Raw { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter TreatUndirectedAsBidirectional { get; set; }

        protected override void ProcessRecord()
        {
            var flag = this.get_DirectedEdgeHandling();
            var edges = this.Client.Connection.GetDirectedEdges(flag);

            if (this.GetShapeObjects)
            {
                this.WriteObject(edges, false);
                return;
            }

            this.write_edges_with_shapeids(edges);
                
        }

        private VA.DocumentAnalysis.ConnectorEdgeHandling get_DirectedEdgeHandling()
        {
            var flag = new VA.DocumentAnalysis.ConnectorEdgeHandling();
            flag.ADR =  VA.DocumentAnalysis.ArrowDirectionHandling.NoArrows_Exclude;

            if (this.Raw)
            {
                flag.AR = VA.DocumentAnalysis.ArrowHandling.Raw;
            }
            else
            {
                flag.AR = VA.DocumentAnalysis.ArrowHandling.NonRow;
                flag.ADR = this.TreatUndirectedAsBidirectional ?
                    VA.DocumentAnalysis.ArrowDirectionHandling.NoArrows_Bidirectional 
                    : VA.DocumentAnalysis.ArrowDirectionHandling.NoArrows_Exclude;
            }
            return flag;
        }

        private void write_edges_with_shapeids(IList<VA.DocumentAnalysis.ConnectorEdge> edges)
        {
            foreach (var edge in edges)
            {
                var e = new Model.DirectedEdge(
                    edge.From.ID,
                    edge.To.ID,
                    edge.Connector.ID
                    );
                this.WriteObject(e);
            }
        }
    }
}