using System.Management.Automation;
using VisioAutomation.Drawing.Layout;
using VA = VisioAutomation;
using IVisio = Microsoft.Office.Interop.Visio;

namespace VisioPowerShell.Commands.Format
{
    [Cmdlet(VerbsCommon.Format, VisioPowerShell.Nouns.VisioShape)]
    public class Format_VisioShape : VisioCmdlet
    {
        [Parameter(Mandatory = false)]
        public double NudgeX { get; set; }

        [Parameter(Mandatory = false)]
        public double NudgeY { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DistributeHorizontal { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter DistributeVertical { get; set; }

        [Parameter(Mandatory = false)]
        public Model.VerticalAlignment AlignVertical = Model.VerticalAlignment.None;

        [Parameter(Mandatory = false)]
        public Model.HorizontalAlignment AlignHorizontal = Model.HorizontalAlignment.None;

        [Parameter(Mandatory = false)]
        public IVisio.Shape[] Shapes;

        protected override void ProcessRecord()
        {
            if (this.NudgeX != 0.0 || this.NudgeY != 0.0)
            {
                this.Client.Arrange.Nudge(this.Shapes, this.NudgeX, this.NudgeY);                
            }

            if (this.DistributeHorizontal)
            {
                this.Client.Arrange.Distribute(this.Shapes, Axis.XAxis);
            }

            if (this.DistributeVertical)
            {
                this.Client.Arrange.Distribute(this.Shapes, Axis.YAxis);
            }

            if (this.AlignVertical != Model.VerticalAlignment.None)
            {
                this.Client.Arrange.Align(this.Shapes, (AlignmentVertical)this.AlignVertical);
            }

            if (this.AlignHorizontal != Model.HorizontalAlignment.None)
            {
                this.Client.Arrange.Align(this.Shapes, (AlignmentHorizontal)this.AlignHorizontal);
            }

        }
    }
}