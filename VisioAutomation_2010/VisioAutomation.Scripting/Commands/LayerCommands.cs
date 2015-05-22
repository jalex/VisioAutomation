using System.Collections.Generic;
using System.Linq;
using VisioAutomation.Extensions;
using IVisio = Microsoft.Office.Interop.Visio;
using VA = VisioAutomation;

namespace VisioAutomation.Scripting.Commands
{
    public class LayerCommands : CommandSet
    {
        internal LayerCommands(Client client) :
            base(client)
        {

        }

        public IVisio.Layer Get(string layername)
        {
            this.Client.Application.AssertApplicationAvailable();
            this.Client.Document.AssertDocumentAvailable();

            if (layername == null)
            {
                throw new System.ArgumentNullException("layername");
            }

            if (layername.Length < 1)
            {
                throw new System.ArgumentException("Layer name cannot be empty", "layername");
            }

            var application = this.Client.VisioApplication;
            var page = application.ActivePage;
            IVisio.Layer layer = null;
            try
            {
                this.Client.WriteVerbose("Trying to find Layer named \"{0}\"",layername);
                var layers = page.Layers;
                layer = layers.ItemU[layername];
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                string msg = string.Format("No such layer \"{0}\"", layername);
                throw new VisioOperationException(msg);
            }
            return layer;
        }

        public IList<IVisio.Layer> Get()
        {
            this.Client.Application.AssertApplicationAvailable();
            this.Client.Document.AssertDocumentAvailable();

            var application = this.Client.VisioApplication;
            var page = application.ActivePage;
            return page.Layers.AsEnumerable().ToList();
        }
    }
}