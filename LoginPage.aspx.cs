using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;

namespace OnlineExamSystem
{
    public partial class LoginPage : System.Web.UI.Page
    {
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
                string CS = @"Data Source=DESKTOP-I3CE4PO\SQLEXPRESS;Initial Catalog=OnlineExam;Integrated Security=True;";

                try
                {
                    using (SqlConnection con = new SqlConnection(CS))
                    {
                        con.Open();

                        SqlCommand cmd = new SqlCommand(
                            "SELECT COUNT(*) FROM userInfo WHERE id=@id AND password=@pass", con);

                        cmd.Parameters.AddWithValue("@id", userTextBox.Text);
                        cmd.Parameters.AddWithValue("@pass", passTextBox.Text);

                        int result = (int)cmd.ExecuteScalar();

                        if (result == 1)
                        {
                            Session["_ID"] = userTextBox.Text;
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
                    Response.Write("<script>alert('" + ex.Message + "');</script>");
                }
            }
            else if (AccountTypeDB.SelectedItem.Text == "Teacher")
            {
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
