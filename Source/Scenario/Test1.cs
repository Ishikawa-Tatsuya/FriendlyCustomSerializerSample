using Codeer.Friendly.Windows;
using NUnit.Framework;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using Driver.InTarget;

namespace Scenario
{
    [TestFixture]
    public class Test1
    {
        [Test]
        public void TestMethod1()
        {
            //!最初呼び出す
            WindowsAppFriend.SetCustomSerializer<CustomSerializer>();

            var targetPath = @"C:\Work\FormsStandardControls.exe";
            var info = new ProcessStartInfo(targetPath) { WorkingDirectory = Path.GetDirectoryName(targetPath) };
            var app = new WindowsAppFriend(Process.Start(info));

            var formControls = app.AttachFormControls();
            formControls.button.EmulateClick();
            formControls.checkBox.EmulateCheck(CheckState.Checked);
            formControls.comboBox.EmulateChangeText("Item-3");
            formControls.comboBox.EmulateChangeSelect(2);
            formControls.radioButton1.EmulateCheck();
            formControls.radioButton1.EmulateCheck();
        }
    }
}
