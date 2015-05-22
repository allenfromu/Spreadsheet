//Written by Jack Stafford for CS 3500, October 2014

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace PS5TestProject
{
    [TestClass]
    public class SpreadsheetTester
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestCellNameInvalid1()
        {
            AbstractSpreadsheet s = new Spreadsheet();
            s.GetCellContents("a_8");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestCellNameInvalid2()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("aasklj3i");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestCellNameInvalid3()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("aaa888*");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestCellNameInvalid4()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("a 8");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestCellNameInvalid5()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("a08");
        }

        [TestMethod]
        public void TestSetContentsOfCellDouble()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("a1", "5.43");
            Assert.AreEqual(5.43, s.GetCellContents("a1"));
            List<string> l = new List<string> { "a1" };
            List<string> contents = s.GetNamesOfAllNonemptyCells() as List<string>;
            Assert.IsTrue(Enumerable.SequenceEqual<string>(l, contents));
        }

        [TestMethod]
        public void TestSetContentsOfCellString()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("b4", "badonkadonk");
            Assert.AreEqual("badonkadonk", s.GetCellContents("b4"));

            s.SetContentsOfCell("b4", "_this_random_string_");
            Assert.AreEqual("_this_random_string_", s.GetCellContents("b4"));

            List<string> l = new List<string> { "b4" };
            List<string> contents = s.GetNamesOfAllNonemptyCells() as List<string>;
            Assert.IsTrue(Enumerable.SequenceEqual<string>(l, contents));
        }

        [TestMethod]
        public void TestSetContentsOfCellFormula()
        {
            Spreadsheet sp = new Spreadsheet(s => true, s => s.ToUpper(), "one");
            sp.SetContentsOfCell("indubitably1", "=1+5-3");
            Assert.AreEqual(new Formula("1.00+5-3"), sp.GetCellContents("indubitably1"));

            sp.SetContentsOfCell("rub2", "=a4+a3");
            Assert.AreEqual(new Formula("A4 + A3"), sp.GetCellContents("rub2"));

            HashSet<string> l = new HashSet<string> { "INDUBITABLY1", "RUB2" };
            List<string> contents = sp.GetNamesOfAllNonemptyCells() as List<string>;
            Assert.IsTrue(Enumerable.SequenceEqual<string>(l, contents));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsNullName()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsInvalidName()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellContents("111");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetContentsOfCellDoubleNullName()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell(null, "1");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetContentsOfCellDoubleInvalidName()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("111", "2342.2");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetContentsOfCellStringNullName()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell(null, "prolix");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetContentsOfCellStringInvalidName()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1*1", "enigma");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetContentsOfCellStringNullText()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("asdf", null as string);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetContentsOfCellFormulaNullName()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell(null, "=a1_+3");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetContentsOfCellFormulaInvalidName()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("asoithbworiiitj_11~!", "=3e10");
        }

        //[TestMethod]
        //[ExpectedException(typeof(ArgumentNullException))]
        //public void TestSetContentsOfCellFormulaNullFormula()
        //{
        //    Spreadsheet ss = new Spreadsheet();
        //    ss.SetContentsOfCell("asdf", null as Formula);
        //}

        [TestMethod]
        public void TestGetContentsCellWithoutSettingContents()
        {
            Spreadsheet ss = new Spreadsheet();
            Assert.AreEqual("", ss.GetCellContents("a123"));

            List<string> contents = ss.GetNamesOfAllNonemptyCells() as List<string>;
            Assert.AreEqual(0, contents.Capacity);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCircular()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "=a1");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCircular2()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a2", " = b3");
            ss.SetContentsOfCell("b3", "=c2");
            ss.SetContentsOfCell("c2", "=d33");
            ss.SetContentsOfCell("d33", "=a2");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCircular3()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("a1", "=b2+1");
            ss.SetContentsOfCell("b2", "=c3");
            ss.SetContentsOfCell("c3", "=d11+3+e12+f13+g31");
            ss.SetContentsOfCell("d11", "=a1");
        }

        [TestMethod]
        public void TestCorrectRecalculateFormula()
        {
            Spreadsheet ss = new Spreadsheet();
            PrivateObject po = new PrivateObject(ss);

            ss.SetContentsOfCell("a1", "=b1+1");
            ss.SetContentsOfCell("b1", "=c1");
            HashSet<string> returned = ss.SetContentsOfCell("c1", "=d1+3+e1+f1+g1") as HashSet<string>;
            HashSet<string> shouldBe = new HashSet<string> { "c1", "b1", "a1" };
            Assert.IsTrue(Enumerable.SequenceEqual<string>(shouldBe, returned));
        }

        [TestMethod]
        public void TestCellsToRecalculateDouble()
        {
            Spreadsheet ss = new Spreadsheet();
            PrivateObject po = new PrivateObject(ss);
            HashSet<string> returned = ss.SetContentsOfCell("c3", "2.0") as HashSet<string>;
            List<string> expected = new List<string> { "c3" };
            Assert.IsTrue(Enumerable.SequenceEqual<string>(expected, returned));

        }

        [TestMethod]
        public void TestCellsToRecalculateString()
        {
            Spreadsheet ss = new Spreadsheet();
            PrivateObject po = new PrivateObject(ss);
            HashSet<string> returned = ss.SetContentsOfCell("c4", "2.0") as HashSet<string>;
            List<string> expected = new List<string> { "c4" };

            Assert.IsTrue(Enumerable.SequenceEqual<string>(expected, returned));

        }

        [TestMethod]
        public void TestGetDirectDependents()
        {
            AbstractSpreadsheet ss = new Spreadsheet();
            PrivateObject po = new PrivateObject(ss);

            ss.SetContentsOfCell("a1", "=b1+1");
            ss.SetContentsOfCell("b1", "=c1");
            ss.SetContentsOfCell("c1", "=d1+3+e1+f1+g1");
            HashSet<string> shouldBe = new HashSet<string> { "b1" };
            List<string> returned = po.Invoke("GetDirectDependents", "c1") as List<string>;
            Assert.IsTrue(Enumerable.SequenceEqual<string>(shouldBe, returned));

            ss.SetContentsOfCell("d1", "=h1");
            ss.SetContentsOfCell("e1", "=h1");
            ss.SetContentsOfCell("f1", "=h1");
            ss.SetContentsOfCell("g1", "=h1");
            shouldBe = new HashSet<string> { "d1", "e1", "f1", "g1" };
            returned = po.Invoke("GetDirectDependents", "h1") as List<string>;
            Assert.IsTrue(Enumerable.SequenceEqual<string>(shouldBe, returned));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetDirectDependentsNullName()
        {
            Spreadsheet ss = new Spreadsheet();
            PrivateObject po = new PrivateObject(ss);
            po.Invoke("GetDirectDependents", null as string);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetDirectDependentsInvalidName()
        {
            Spreadsheet ss = new Spreadsheet();
            PrivateObject po = new PrivateObject(ss);
            po.Invoke("GetDirectDependents", "a123$");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaFormat()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("asdf1234", "=12=");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestFormulaFormat2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("asdf1234", "=(13");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestFormulaValidator()
        {
            Spreadsheet st = new Spreadsheet(s => false, s => s, "sdlkjfs");
            st.SetContentsOfCell("asdfjkl12350987", "=a23+5");
        }

        [TestMethod]
        public void TestGetCellValue()
        {
            Spreadsheet sp = new Spreadsheet();

            sp.SetContentsOfCell("a10", "=1+1");
            Assert.AreEqual(2, (double)sp.GetCellValue("a10"));
        }

        [TestMethod]
        public void TestFormulaError()
        {
            Spreadsheet sp = new Spreadsheet();

            sp.SetContentsOfCell("A3", "hello");
            sp.SetContentsOfCell("A4", "=A3 + 5");
            Assert.IsTrue(sp.GetCellValue("A4") is FormulaError);
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetValueNullName()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.GetCellValue(null as string);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetValueInvalidCellName()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.GetCellValue("12kl43");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetValueInvalidCellName2()
        {
            Spreadsheet sp = new Spreadsheet(s => false, s => s, "versy-own-ninny");
            sp.GetCellValue("as14");
        }

        [TestMethod]
        public void TestGetValueCellNotCreatedYet()
        {
            Spreadsheet sp = new Spreadsheet();
            Assert.AreEqual("", sp.GetCellValue("rtgiuhfjkhfd74378339229"));
        }

        [TestMethod]
        public void TestDependentRemoval()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetContentsOfCell("beaver23", "23");
            sp.SetContentsOfCell("a24", "=beaver23+1");

            sp.SetContentsOfCell("a24", "2");
            PrivateObject po = new PrivateObject(sp);
            Assert.IsTrue(new HashSet<string>().SetEquals(po.Invoke("GetAllDependees", "a24") as HashSet<string>));

            sp.SetContentsOfCell("a24", "=beaver23+1");
            sp.SetContentsOfCell("a24", "string");
            Assert.IsTrue(new HashSet<string>().SetEquals(po.Invoke("GetAllDependees", "a24") as HashSet<string>));
        }

        // saving removed for CS 3505
        //[TestMethod]
        //public void TestSave()
        //{
        //    string version = "soupsicle";
        //    string filename = "testSave1.xml";
        //    Spreadsheet sp = new Spreadsheet(s => true, s => s, version);
        //    sp.SetContentsOfCell("beaver23", "23");
        //    sp.SetContentsOfCell("a24", "=beaver23+1");
        //    sp.SetContentsOfCell("aa44", "snickerdoodles");

        //    Assert.IsTrue(sp.Changed);
        //    sp.Save(filename);
        //    Assert.IsFalse(sp.Changed);

        //    Assert.AreEqual(version, sp.GetSavedVersion(filename));
        //}

        // removed for CS 3505
        //[TestMethod]
        //public void TestGetVersionNotSpreadsheet()
        //{
        //    string filemane = "notASpreadsheet.xml";
        //    using(XmlWriter writer = XmlWriter.Create(filemane))
        //    {
        //        writer.WriteStartDocument();
        //        writer.WriteStartElement("spreadshet");
        //        writer.WriteAttributeString("version", "Dracarys");

        //        writer.WriteStartElement("cell");
        //        writer.WriteElementString("name", "swarley");
        //        writer.WriteElementString("contents", "hufflepuff");
        //        writer.WriteEndElement();

        //        writer.WriteEndElement();
        //        writer.WriteEndDocument();
        //    }

        //    Spreadsheet sp = new Spreadsheet();
        //    try { sp.GetSavedVersion(filemane); }
        //    catch (SpreadsheetReadWriteException e)
        //    {
        //        Assert.AreEqual("Only spreadsheets can be read", e.Message);
        //    }
        //}

        // removed for CS 3505
        //[TestMethod]
        //public void TestSaveNullFilename()
        //{
        //    Spreadsheet sp = new Spreadsheet();
        //    try
        //    {
        //        sp.Save(null);
        //    }
        //    catch (SpreadsheetReadWriteException e)
        //    {
        //        Assert.AreEqual("Filename cannot be null", e.Message);
        //    }
        //}

        // removed for CS 3505
        //[TestMethod]
        //public void TestGetVersionNullFilename()
        //{
        //    Spreadsheet sp = new Spreadsheet();
        //    try
        //    {
        //        sp.GetSavedVersion(null);
        //    }
        //    catch (SpreadsheetReadWriteException e)
        //    {
        //        Assert.AreEqual("Filename cannot be null.", e.Message);
        //    }
        //}

        // removed for CS 3505
        //[TestMethod]
        //public void TestReadingFile()
        //{
        //    AbstractSpreadsheet sp = new Spreadsheet("testSave1.xml", s => true, s => s, "soupsicle");
        //    List<string> expected = new List<string> { "beaver23", "a24", "aa44" };
        //    List<string> returned = sp.GetNamesOfAllNonemptyCells() as List<string>;
        //    Assert.IsTrue(Enumerable.SequenceEqual<string>(expected, returned));
        //    Assert.AreEqual(23, (double)sp.GetCellValue("beaver23"));
        //    Assert.AreEqual(new Formula("beaver23+ 1"), (Formula)sp.GetCellContents("a24"));
        //    Assert.AreEqual("snickerdoodles", (string)sp.GetCellContents("aa44"));
        //}

        // removed for CS 3505
        //[TestMethod]
        //public void TestWrongVersion()
        //{
        //    try { Spreadsheet sp = new Spreadsheet("testSave1.xml", s => true, s => s, "pickle sickle"); }
        //    catch (SpreadsheetReadWriteException e)
        //    {
        //        Assert.AreEqual("Version provided does not match saved version.", e.Message);
        //    }
        //}

        // removed for CS 3505
        //[TestMethod]
        //[ExpectedException(typeof(SpreadsheetReadWriteException))]
        //public void TestNullFilepath()
        //{
        //    Spreadsheet sp = new Spreadsheet(null, s => true, s => s, "das Boot");
        //}

        [TestMethod]
        public void TestInvalidDependency()
        {
            Spreadsheet sp = new Spreadsheet();
            sp.SetContentsOfCell("nsync5", "bye bye bye");
            sp.SetContentsOfCell("sdf890", "=nsync5+8.23");
            Assert.IsTrue(sp.GetCellValue("sdf890") is FormulaError);
        }


        [TestMethod]
        public void TestVariablesShouldBeUppercase()
        {
            Spreadsheet sp = new Spreadsheet(s => true, s => s.ToUpper(), "blerg");
            sp.SetContentsOfCell("a5", "=b5+c5");
            sp.SetContentsOfCell("b5", "5");
            sp.SetContentsOfCell("c5", "1234.56");
            HashSet<string> expected = new HashSet<string> { "A5", "B5", "C5" };
            List<string> returned = sp.GetNamesOfAllNonemptyCells() as List<string>;
            Assert.IsTrue(Enumerable.SequenceEqual<string>(expected, returned));
            Assert.AreEqual(1239.56, sp.GetCellValue("a5"));
        }
    }
}
