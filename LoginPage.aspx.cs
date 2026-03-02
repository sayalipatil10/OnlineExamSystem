// FIX APPLIED:
//   BEFORE: private string CS = @"Data Source=DESKTOP-I3CE4PO\SQLEXPRESS;..."; (hardcoded)
//   AFTER:  Read from Web.config using WebConfigurationManager (works on any machine)

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace OnlineExamSystem
{
    public partial class LoginPage : System.Web.UI.Page
    {
        // FIX: Read connection string from Web.config — NOT hardcoded
        private string CS => WebConfigurationManager
                                .ConnectionStrings["OnlineExamConnectionString"]
                                .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void signupB(object sender, EventArgs e)
        {
            Response.Redirect("SignUpPage.aspx");
        }

        protected void loginButton_Click(object sender, EventArgs e)
        {
            if (AccountTypeDB.SelectedItem.Text == "Student")
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(CS))
                    {
                        con.Open();

                        SqlCommand cmd = new SqlCommand(
                            "SELECT COUNT(*) FROM userInfo WHERE id=@id AND password=@pass", con);

                        cmd.Parameters.AddWithValue("@id", userTextBox.Text.Trim());
                        cmd.Parameters.AddWithValue("@pass", passTextBox.Text);

                        int result = (int)cmd.ExecuteScalar();

                        if (result == 1)
                        {
                            Session["_ID"] = userTextBox.Text.Trim();
                            Response.Redirect("Dashboard.aspx");
                        }
                        else
                        {
                            Response.Write("<script>alert('User ID or Password do not match!');</script>");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('Database Error: " + ex.Message.Replace("'", "\\'") + "');</script>");
                }
            }
            else if (AccountTypeDB.SelectedItem.Text == "Teacher")
            {
                // Admin login — hardcoded credentials (unchanged)
                if (userTextBox.Text == "Admin" && passTextBox.Text == "Admin")
                {
                    Response.Redirect("AdminPanel.aspx");
                }
                else
                {
                    Response.Write("<script>alert('User ID or Password do not match!');</script>");
                }
            }
        }
    }
}
