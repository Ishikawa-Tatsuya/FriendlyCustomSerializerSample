using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using Codeer.TestAssistant.GeneratorToolKit;
using Ong.Friendly.FormsStandardControls;

namespace Scenario
{
    [WindowDriver(TypeFullName = "FormsStandardControls.FormControls")]
    public class FormControlsDriver
    {
        public WindowControl Core { get; }
        public FormsButton button => Core.Dynamic().button; 
        public FormsCheckBox checkBox => Core.Dynamic().checkBox; 
        public FormsComboBox comboBox => Core.Dynamic().comboBox; 
        public FormsRadioButton radioButton1 => Core.Dynamic().radioButton1; 

        public FormControlsDriver(WindowControl core)
        {
            Core = core;
        }

        public FormControlsDriver(AppVar core)
        {
            Core = new WindowControl(core);
        }
    }

    public static class FormControlsDriverExtensions
    {
        [WindowDriverIdentify(TypeFullName = "FormsStandardControls.FormControls")]
        public static FormControlsDriver AttachFormControls(this WindowsAppFriend app)
            => app.WaitForIdentifyFromTypeFullName("FormsStandardControls.FormControls").Dynamic();
    }
}