using DevExpress.Utils.Text;
using oms.DataAccessLayer;
using oms.Forms;
namespace oms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            ApplicationVariables.ConfigFilePath = (args.Length > 0) ? args[0] : Application.StartupPath;
            ApplicationVariables.ClientId = (args.Length > 1) ? args[1] : "allcare";

            if (string.IsNullOrEmpty(ApplicationVariables.ConfigFilePath.Trim()))
                ApplicationVariables.ConfigFilePath = Application.StartupPath;

            try
            {
                if (!ApplicationVariables.SetApplicationVariables())
                {
                    return;
                }

                bool authenticate = Clickones.ClickOnes.VerifyClientAndMachine(ApplicationVariables.ClientId, "test");
                if (!authenticate)
                {
                    MessageBox.Show("Unable to verify client.", "Unknow Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LoginForm loginForm = new LoginForm();
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    StaticListDL.BuildStaticList();
                    Application.Run(new DashboardForm());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}