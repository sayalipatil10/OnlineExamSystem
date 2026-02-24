using System;
using System.Web;
using System.Web.UI;

namespace OnlineExamSystem
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["_ID"] == null)
            {
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
            Response.Redirect("Leaderboard.aspx");
        }

        protected void sExamB_Click(object sender, EventArgs e)
        {
            Response.Redirect("StartExam.aspx");
        }

        protected void logoutB_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("LoginPage.aspx");
        }
    }
}