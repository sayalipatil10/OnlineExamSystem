using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OnlineExamSystem
{
    public partial class AdminPanel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void homeB_Click(object sender, EventArgs e)
        {

        }

        protected void profileB_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminQueue.aspx");
        }

        protected void LeaderboardB_Click(object sender, EventArgs e)
        {
           Response.Redirect("AdminLeaderboard.aspx");
        }

        protected void sExamB_Click(object sender, EventArgs e)
        {
            Response.Redirect("SetExam.aspx");
        }
        protected void eExamB_Click(object sender, EventArgs e)
        {
            Response.Redirect("EditExam.aspx");
        }
        protected void logoutB_Click(object sender, EventArgs e)
        {
            Response.Redirect("LoginPage.aspx");
        }

       
    }
}