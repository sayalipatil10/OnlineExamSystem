// FIX APPLIED:
//   - Wrapped entire signup in proper try/catch with visible error messages
//   - FileUpload validation added (must upload an image)
//   - No logic changes — schema stays the same

using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace OnlineExamSystem
{
    public partial class SignUpPage : System.Web.UI.Page
    {
        private string CS => WebConfigurationManager
                                .ConnectionStrings["OnlineExamConnectionString"]
                                .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void loginB_Click(object sender, EventArgs e)
        {
            Response.Redirect("LoginPage.aspx");
        }

        protected void signUpB_Click(object sender, EventArgs e)
        {
            string pass1 = passTxBox.Text;
            string pass2 = cPassTxBox.Text;

            if (pass1 != pass2)
            {
                Response.Write("<script>alert('Passwords do not match!');</script>");
                return;
            }

            if (!FileUpload1.HasFile)
            {
                Response.Write("<script>alert('Please upload a profile image.');</script>");
                return;
            }

            try
            {
                // Save the uploaded image to ~/Images/
                string fileName = Path.GetFileName(FileUpload1.FileName);
                FileUpload1.SaveAs(Server.MapPath("~/Images/") + fileName);
                string link = "Images/" + fileName;

                int nEx = 0;
                double tM = 0.0;

                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();

                    // FIX: Parameterized query — prevents SQL injection
                    string query = @"INSERT INTO userInfo 
                        (id, name, department, email, semester, gender, password, fatherName, hall, image, no_of_exam, total_mark, abc)
                        VALUES (@id, @name, @dept, @email, @sem, @gender, @pass, @father, @hall, @image, @nEx, @tM, 0)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", idTxBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@name", nameTxBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@dept", deptDropDownList.SelectedValue);
                    cmd.Parameters.AddWithValue("@email", emailTxBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@sem", semesterDropDownList.SelectedValue);
                    cmd.Parameters.AddWithValue("@gender", genderDropDownList.SelectedValue);
                    cmd.Parameters.AddWithValue("@pass", pass1);
                    cmd.Parameters.AddWithValue("@father", fatherNameTB.Text.Trim());
                    cmd.Parameters.AddWithValue("@hall", hallTB.Text.Trim());
                    cmd.Parameters.AddWithValue("@image", link);
                    cmd.Parameters.AddWithValue("@nEx", nEx);
                    cmd.Parameters.AddWithValue("@tM", tM);

                    cmd.ExecuteNonQuery();
                }

                Response.Write("<script>alert('Registration Successful!'); window.location='LoginPage.aspx';</script>");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Primary key violation
                    Response.Write("<script>alert('This Student ID is already registered. Please use a different ID.');</script>");
                else
                    Response.Write("<script>alert('Database Error: " + ex.Message.Replace("'", "\\'") + "');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error: " + ex.Message.Replace("'", "\\'") + "');</script>");
            }
        }
    }
}
