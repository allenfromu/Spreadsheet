using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SpreadsheetUITests
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class CodedUITest1
    {
        public CodedUITest1()
        {
        }

        [TestMethod]
        public void Test()
        {
            TestOpen();
            TestSave();
            TestNew();
            TestDivideByZero();
            TestHelp();
            TestClose();
        }

        private void TestClose()
        {
            this.UIMap.TestClose();
        }

        private void TestHelp()
        {
            this.UIMap.SetupHelpTest();
            this.UIMap.TearDownHelp();
        }

        private void TestDivideByZero()
        {
            this.UIMap.BeginZeroTest();
            this.UIMap.ZeroAssert1();
            this.UIMap.ZeroMiddle();
            this.UIMap.ZeroAssert2();
            this.UIMap.EndZeroTest();
        }

        private void TestNew()
        {
            this.UIMap.StartNewTest();
            this.UIMap.NewAssert();
            this.UIMap.EndNewTest();
        }

        private void TestSave()
        {
            this.UIMap.BeginSaveTest();
            this.UIMap.SaveAssert();
            this.UIMap.EndSaveTest();
        }

        private void TestOpen()
        {
            this.UIMap.BeginOpenTest();
            this.UIMap.OpenAssert();
            this.UIMap.EndOpenTest();
        }
       

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public UIMap UIMap
        {
            get
            {
                if ((this.map == null))
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
