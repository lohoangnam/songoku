using System;
using System.Windows.Forms;

namespace StreetFighterGame
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(CreateCenteredForm(new FormStart()));
        }

        private static Form CreateCenteredForm(Form form)
        {
            form.StartPosition = FormStartPosition.CenterScreen;
            return form;
        }
    }
}