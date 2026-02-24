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
    public partial class SetExam1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void homeB_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminPanel.aspx");
        }
        protected void profileB_Click(object sender, EventArgs e)
        {

        }
        protected void LeaderboardB_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminLeaderboard.aspx");
        }
        protected void theoryB_Click(object sender, EventArgs e)
        {
            //Session["_qsN"] = 1;
            Response.Redirect("TheorySet.aspx");
        }

        protected void mcqB_Click(object sender, EventArgs e)
        {
            //Session["_qsN"] = 1;
            Response.Redirect("MCQSet.aspx");
        }

        protected void logoutB_Click(object sender, EventArgs e)
        {
            Response.Redirect("LoginPage.aspx");
        }
    }
}