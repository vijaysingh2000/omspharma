using oms.DataAccessLayer;
using oms.Model;
namespace oms.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            this.Text = CommonFunctions.GetDialogText("Login");
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            User user = UsersDL.Get(txtLoginId.Text);

            if (user != null)
            {
                if (user.Password.IsStringEqual(txtPassword.Text))
                {
                    ApplicationVariables.LoggedInUser = user;
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    CommonFunctions.ShowErrorMessage("Invalid Password.");
                }
            }
            else
            {
                CommonFunctions.ShowErrorMessage("Invalid login Id. User not found.");
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void linkNewUsers_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            UserDetailsForm userDetails = new UserDetailsForm();
            userDetails.ShowDialog();
        }
    }
}