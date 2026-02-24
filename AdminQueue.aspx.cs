using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OnlineExamSystem
{
    public partial class AdminCourseQueue : System.Web.UI.Page
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

        protected void logoutB_Click(object sender, EventArgs e)
        {
            Response.Redirect("LoginPage.aspx");
        }

        protected void GridView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = GridView2.SelectedRow;
            Session["_crsID1"] = row.Cells[1].Text;

            Response.Redirect("AdminCourseQueue.aspx");
        }
    }
}