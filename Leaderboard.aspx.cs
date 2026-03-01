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
    public partial class Leaderboard : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["_ID"] != null)
            {
                string ID = Session["_ID"].ToString();

                string CS = WebConfigurationManager
                                .ConnectionStrings["OnlineExamConnectionString"]
                                .ConnectionString;

                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();

                    string query = "SELECT semester FROM userInfo WHERE id=@id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", ID);

                    SqlDataReader dr = cmd.ExecuteReader();

                    string year = "";

                    if (dr.Read())
                    {
                        year = dr["semester"].ToString();
                    }

                    Session["_Year"] = year;
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
            Response.Redirect("UserProfile.aspx");
        }

        protected void LeaderboardB_Click(object sender, EventArgs e)
        {

        }

        protected void logoutB_Click(object sender, EventArgs e)
        {
            Session["_ID"] = "not";
            Response.Redirect("LoginPage.aspx");
        }
    }
}
