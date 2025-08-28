using Autoclicker.Core.Services;
using Autoclicker.Infrastructure;
using Autoclicker.Interop;
using Autoclicker.UI;

namespace Autoclicker
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // DI Composition Root
            var keySender = new KeySender();              // IKeyboard + IKeyParser
            var builder = new SequenceBuilder(keySender);
            var runner = new SequenceRunner(keySender);
            var store = new SequencePersistence();
            var hotkeys = new HotkeyServiceFactory();   // hotkey fabric

            Application.Run(new MainForm(builder, runner, store, hotkeys));
        }
    }
}