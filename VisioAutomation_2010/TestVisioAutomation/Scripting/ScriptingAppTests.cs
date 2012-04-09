using Microsoft.VisualStudio.TestTools.UnitTesting;
using IVisio = Microsoft.Office.Interop.Visio;
using VA = VisioAutomation;
using VisioAutomation.Extensions;

namespace TestVisioAutomation
{
    [TestClass]
    public class ScriptingSessionTests : VisioAutomationTest
    {
        [TestMethod]
        public void Scripting_DevDocumentation()
        {
            var ss = GetScriptingSession();
            var doc= ss.Developer.DrawScriptingDocumentation();
            doc.Close(true);
        }

        [TestMethod]
        public void Scripting_DevDocumentation2()
        {
            var ss = GetScriptingSession();
            var doc = ss.Developer.DrawNamespaces();
            //doc.Close(true);
        }
    }
}