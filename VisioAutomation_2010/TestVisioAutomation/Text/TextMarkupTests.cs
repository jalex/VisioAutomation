using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using VisioAutomation.Extensions;
using VA = VisioAutomation;
using IVisio = Microsoft.Office.Interop.Visio;

namespace TestVisioAutomation
{
    [TestClass]
    public class TextMarkupTests : VisioAutomationTest
    {
        [TestMethod]
        public void Markup1()
        {
            // Validate that setting text with no values works
            var el0 = new VA.Text.Markup.TextElement("HELLO");

            var page1 = GetNewPage();
            var s1 = page1.DrawRectangle(0, 0, 4, 4);
            el0.SetText(s1);

            var fmts = VA.Text.TextFormat.GetFormat(s1);
            Assert.AreEqual(1, fmts.CharacterFormats.Count);
            Assert.AreEqual(1, fmts.ParagraphFormats.Count);

            page1.Delete(0);
        }

        [TestMethod]
        public void Markup2()
        {
            // Validate that setting text with no values works
            var el0 = new VA.Text.Markup.TextElement("HELLO");
            el0.CharacterFormat.Color = new VA.Drawing.ColorRGB(0xff,0,0);

            var page1 = GetNewPage();
            var s1 = page1.DrawRectangle(0, 0, 4, 4);
            el0.SetText(s1);

            var fmts = VA.Text.TextFormat.GetFormat(s1);
            Assert.AreEqual(1, fmts.CharacterFormats.Count);
            Assert.AreEqual(1, fmts.ParagraphFormats.Count);

            Assert.AreEqual("RGB(255,0,0)", fmts.CharacterFormats[0].Color.Formula);

            page1.Delete(0);
        }


        [TestMethod]
        public void ValidateFormattingRegions()
        {
            // Check that the formatting regions are correctly
            // mapped given a number of Text structures

            var el1 = new VA.Text.Markup.TextElement();
            var markup1 = el1.GetMarkupInfo();
            var regions1 = markup1.FormatRegions;
            Assert.AreEqual(1, markup1.FormatRegions.Count);
            Assert.AreEqual(0, regions1[0].TextLength);
            Assert.AreEqual(0, regions1[0].TextStartPos);
            Assert.AreEqual(0, regions1[0].TextEndPos);


            var el2 = new VA.Text.Markup.TextElement("HELLO");
            var markup2 = el2.GetMarkupInfo();
            var regions2 = markup2.FormatRegions;
            Assert.AreEqual(1, markup2.FormatRegions.Count);
            Assert.AreEqual(5, regions2[0].TextLength);
            Assert.AreEqual(0, regions2[0].TextStartPos);
            Assert.AreEqual(5, regions2[0].TextEndPos);

            var el3 = new VA.Text.Markup.TextElement("HELLO");
            el3.AppendText(" WORLD");
            var markup3 = el3.GetMarkupInfo();
            var regions3 = markup3.FormatRegions;
            Assert.AreEqual(1, markup3.FormatRegions.Count);
            Assert.AreEqual(11, regions3[0].TextLength);
            Assert.AreEqual(0, regions3[0].TextStartPos);
            Assert.AreEqual(11, regions3[0].TextEndPos);

            var el4 = new VA.Text.Markup.TextElement();
            el4.AppendElement("HELLO");
            el4.AppendElement(" WORLD");
            var markup4 = el4.GetMarkupInfo();
            var regions4 = markup4.FormatRegions;
            Assert.AreEqual(3, markup4.FormatRegions.Count);
            Assert.AreEqual(11, regions4[0].TextLength);
            Assert.AreEqual(0, regions4[0].TextStartPos);
            Assert.AreEqual(11, regions4[0].TextEndPos);
            Assert.AreEqual(5, regions4[1].TextLength);
            Assert.AreEqual(0, regions4[1].TextStartPos);
            Assert.AreEqual(5, regions4[1].TextEndPos);
            Assert.AreEqual(6, regions4[2].TextLength);
            Assert.AreEqual(5, regions4[2].TextStartPos);
            Assert.AreEqual(11, regions4[2].TextEndPos);


            var el5 = new VA.Text.Markup.TextElement();
            var el5_a = el5.AppendElement("HELLO");
            var el5_b = el5_a.AppendElement(" WORLD");

            var markup5 = el5.GetMarkupInfo();
            var regions5 = markup5.FormatRegions;
            Assert.AreEqual(3, markup5.FormatRegions.Count);
            Assert.AreEqual(11, regions5[0].TextLength);
            Assert.AreEqual(0, regions5[0].TextStartPos);
            Assert.AreEqual(11, regions5[0].TextEndPos);
            Assert.AreEqual(11, regions5[1].TextLength);
            Assert.AreEqual(0, regions5[1].TextStartPos);
            Assert.AreEqual(11, regions5[1].TextEndPos);
            Assert.AreEqual(6, regions5[2].TextLength);
            Assert.AreEqual(5, regions5[2].TextStartPos);
            Assert.AreEqual(11, regions5[2].TextEndPos);

        }


        [TestMethod]
        public void TextElement_with_multiple_text_nodes()
        {
            // Validate that multiple text elements in the structure
            // all make it into a real visio shep when the text is render

            var el0 = new VA.Text.Markup.TextElement();
            var el1 = el0.AppendElement("HELLO");
            var el2 = el0.AppendElement(" WORLD");

            var page1 = GetNewPage();

            var s1 = page1.DrawRectangle(0, 0, 4, 4);

            el0.SetText(s1);

            Assert.AreEqual("HELLO WORLD", s1.Text);

            page1.Delete(0);
        }

        [TestMethod]
        public void Element_with_bold_and_italic_text()
        {
            // Validate that basic formatting works when rendering

            var el0 = new VA.Text.Markup.TextElement();
            var el1 = el0.AppendElement("HELLO");
            var el2 = el0.AppendElement(" WORLD");

            el1.CharacterFormat.CharStyle = VA.Text.CharStyle.Bold;
            el2.CharacterFormat.CharStyle = VA.Text.CharStyle.Italic;

            var page1 = GetNewPage();

            var s1 = page1.DrawRectangle(0, 0, 4, 4);

            el0.SetText(s1);

            var fmts = VA.Text.TextFormat.GetFormat(s1);
            Assert.AreEqual(3, fmts.CharacterFormats.Count);
            Assert.AreEqual((int)VA.Text.CharStyle.Bold, fmts.CharacterFormats[0].Style.Result);
            Assert.AreEqual((int)VA.Text.CharStyle.Italic, fmts.CharacterFormats[1].Style.Result);
            Assert.AreEqual((int)VA.Text.CharStyle.None, fmts.CharacterFormats[2].Style.Result);

            page1.Delete(0);
        }

        [TestMethod]
        public void Style_inheritance()
        {
            // Validate that sub elements inherit the formatting of parent elements
            var page1 = GetNewPage();
            var courier = page1.Document.Fonts["Courier New"];
            var impact = page1.Document.Fonts["Impact"];


            var el0 = new VA.Text.Markup.TextElement();
            var el1 = el0.AppendElement("HELLO");
            var el2 = el1.AppendElement(" WORLD");

            el0.CharacterFormat.FontSize = 14;
            el0.CharacterFormat.FontSize = 7;
            
            el1.CharacterFormat.Font = impact.Name;
            el1.CharacterFormat.CharStyle = VA.Text.CharStyle.Bold;
            
            el2.CharacterFormat.Font = courier.Name;
            el2.CharacterFormat.FontSize = 20;
            el2.CharacterFormat.CharStyle = VA.Text.CharStyle.Italic;

            var markup = el0.GetMarkupInfo();
            var regions = markup.FormatRegions;
            Assert.AreEqual(3, regions.Count);
            Assert.AreEqual(6, regions[2].TextLength);
            Assert.AreEqual(5, regions[2].TextStartPos);
            Assert.AreEqual(11, regions[2].TextEndPos);
            Assert.AreEqual(11, regions[1].TextLength);
            Assert.AreEqual(0, regions[1].TextStartPos);
            Assert.AreEqual(11, regions[1].TextEndPos);
            Assert.AreEqual(11, regions[0].TextLength);
            Assert.AreEqual(0, regions[0].TextStartPos);
            Assert.AreEqual(11, regions[0].TextEndPos);

            Assert.AreEqual(el0, regions[0].Element);
            Assert.AreEqual(el1, regions[1].Element);
            Assert.AreEqual(el2, regions[2].Element);


            var s1 = page1.DrawRectangle(0, 0, 4, 4);

            el0.SetText(s1);

            var fmts = VA.Text.TextFormat.GetFormat(s1);
            Assert.AreEqual(3, fmts.CharacterFormats.Count);

            page1.Delete(0);
        }

        [TestMethod]
        public void Test_Format_Text_field()
        {
            // Now account for field insertion

            var el0 = new VA.Text.Markup.TextElement();
            el0.AppendText("HELLO ");
            el0.AppendField(VA.Text.Markup.FieldConstants.Height);
            el0.AppendText(" WORLD");

            var page1 = GetNewPage();

            var s1 = page1.DrawRectangle(0, 0, 4, 4);

            string it = el0.GetInnerText();
            Assert.AreEqual("HELLO " + VA.Text.Markup.FieldConstants.Height.PlaceholderText + " WORLD", it);
            el0.SetText(s1);

            var shape_size = VisioAutomationTest.GetSize(s1);

            string s = string.Format("HELLO {0} WORLD", shape_size.Height);
            var s1_characters = s1.Characters;
            Assert.AreEqual(s, s1_characters.Text);

            page1.Delete(0);
        }


        [TestMethod]
        public void TextBlockFormatCells_Check_SetFormat_1()
        {
            var page1 = GetNewPage();
            var s1 = page1.DrawRectangle(0, 0, 4, 4);
            var s2 = page1.DrawRectangle(5, 5, 7, 7);

            var tf0 = VA.Text.TextFormat.GetFormat(s1);
            Assert.AreEqual("4 pt",tf0.TextBlocks.BottomMargin.Formula);

            var tb1 = new VA.Text.TextBlockFormatCells();
            tb1.BottomMargin = "8 pt";

            var update = new VA.ShapeSheet.Update.SIDSRCUpdate();
            tb1.Apply(update,s1.ID16);
            update.Execute(page1);

            var tf2 = VA.Text.TextFormat.GetFormat(s1);
            Assert.AreEqual("8 pt", tf2.TextBlocks.BottomMargin.Formula);

            var tfs = VA.Text.TextFormat.GetFormat(page1, new[] { s1.ID, s2.ID });
            Assert.AreEqual("8 pt", tfs[0].TextBlocks.BottomMargin.Formula);
            Assert.AreEqual("4 pt", tfs[1].TextBlocks.BottomMargin.Formula);

            page1.Delete(0);
        }

        [TestMethod]
        public void Test_Fields1()
        {
            var page1 = GetNewPage();
            var shape = page1.DrawRectangle(0, 0, 4, 2);


            // case 1 - markup is just a single field element
            var markup_1 = new VA.Text.Markup.TextElement();
            markup_1.AppendField(VA.Text.Markup.FieldConstants.Height);
            markup_1.SetText(shape);
            Assert.AreEqual("2", shape.Characters.Text);

            // case 2 - markup contains a single field surrounded by literal text
            var markup2 = new VA.Text.Markup.TextElement();
            markup2.AppendText("HELLO ");
            markup2.AppendField(VA.Text.Markup.FieldConstants.Height);
            markup2.AppendText(" WORLD");
            markup2.SetText(shape);
            Assert.AreEqual("HELLO 2 WORLD", shape.Characters.Text);

            // case 3 - markup contains a single literal surrounded by two fields
            var markup3 = new VA.Text.Markup.TextElement();
            markup3.AppendField(VA.Text.Markup.FieldConstants.Height);
            markup3.AppendText(" HELLO ");
            markup3.AppendField(VA.Text.Markup.FieldConstants.Width);
            markup3.SetText(shape);
            Assert.AreEqual("2 HELLO 4", shape.Characters.Text);

            var markup4 = new VA.Text.Markup.TextElement();
            markup4.AppendField(VA.Text.Markup.FieldConstants.Height);
            markup4.AppendText(" HELLO ");
            markup4.AppendField(VA.Text.Markup.FieldConstants.Width);
            markup4.AppendField(VA.Text.Markup.FieldConstants.Width);
            markup4.SetText(shape);
            Assert.AreEqual("2 HELLO 44", shape.Characters.Text);

            page1.Delete(0);
        }

    }
}