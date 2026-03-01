using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
namespace OnlineExamSystem
{
    public partial class UserProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["_ID"] != null)
            {
                string ID = Session["_ID"].ToString();
                string CS = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;

                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();

                    string query = "SELECT * FROM userInfo WHERE id=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", ID);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        NameTB.Text = dr["name"].ToString();
                        idTB.Text = dr["id"].ToString();
                        deptTB.Text = dr["department"].ToString();
                        semesterTB.Text = dr["semester"].ToString();
                        genderTB.Text = dr["gender"].ToString();
                        emailTB.Text = dr["email"].ToString();
                        fatherNameTB.Text = dr["fatherName"].ToString();
                        hallTB.Text = dr["hall"].ToString();
                    }
                }
            }
            else
            {
                Response.Write("<script>alert('You are not login!');</script>");
                Response.Redirect("LoginPage.aspx");
            }
        }
        protected void homeB_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }

        protected void profileB_Click(object sender, EventArgs e)
        {
           // Response.Redirect("UserProfile.aspx");
        }

        protected void LeaderboardB_Click(object sender, EventArgs e)
        {
            Response.Redirect("Leaderboard.aspx");
        }

        protected void NameTB_TextChanged(object sender, EventArgs e)
        {

        }

        protected void logoutB_Click(object sender, EventArgs e)
        {
            Response.Redirect("LoginPage.aspx");
        }

        protected void coursesB_Click(object sender, EventArgs e)
        {
            Response.Redirect("TakenCourses.aspx");
        }
    }
}