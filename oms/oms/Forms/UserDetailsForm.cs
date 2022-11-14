using oms.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using oms.Model;
namespace oms.Forms
{
    public partial class UserDetailsForm : Form
    {
        private User WorkingUser;
        private bool newUser = false;

        public UserDetailsForm(int userId)
        {
            InitializeComponent();

            this.WorkingUser = UsersDL.Get(userId);

            if (this.WorkingUser != null)
            {
                this.newUser = false;
            }
            else
            {
                this.WorkingUser = new User();
                this.newUser = true;
            }
        }

        public UserDetailsForm()
        {
            InitializeComponent();

            this.newUser = true;
            this.WorkingUser = new User();
        }

        private void UserDetails_Load(object sender, EventArgs e)
        {
            this.Text = CommonFunctions.GetDialogTextWithUser("User Details");

            txtLoginId.Text = this.WorkingUser.LoginId;
            txtPassword.Text = this.WorkingUser.Password;
            txtConfirmPassword.Text = this.WorkingUser.Password;
            txtFirstName.Text = this.WorkingUser.FirstName;
            txtLastName.Text = this.WorkingUser.LastName;
            txtEmail.Text = this.WorkingUser.Email; 
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(!txtPassword.Text.IsStringEqual(txtConfirmPassword.Text))
            {
                CommonFunctions.ShowErrorMessage("Password and confirm password don't match.");
                return;
            }

            User loginExists = UsersDL.Get(txtLoginId.Text);

            if(loginExists != null)
            {
                if (newUser || !loginExists.Id.Equals(this.WorkingUser.Id))
                {
                    CommonFunctions.ShowErrorMessage("Login Id already in use.");
                    return;
                }
            }

            this.WorkingUser.LoginId = txtLoginId.Text.Trim();
            this.WorkingUser.Password = txtPassword.Text.Trim();
            this.WorkingUser.FirstName = txtFirstName.Text.Trim();
            this.WorkingUser.LastName = txtLastName.Text.Trim();
            this.WorkingUser.Email = txtEmail.Text.Trim();

            int returnValue = 0;
            if (newUser)
            {
                returnValue = UsersDL.Add(this.WorkingUser);
                if (returnValue > 0)
                {
                    CommonFunctions.ShowInfomationMessage("Congratulations !!! User is registered successfully.");
                }
            }
            else
            {
                returnValue = UsersDL.Update(this.WorkingUser);
                if (returnValue > 0)
                {
                    CommonFunctions.ShowInfomationMessage("User updated successfully.");
                }
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
