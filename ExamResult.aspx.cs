// FIXES APPLIED:
//   1. BEFORE: cmd1.ExecuteReader() used for an UPDATE query — should be ExecuteNonQuery()
//      AFTER:  cmd1.ExecuteNonQuery()
//   2. Added null checks for all Session values before accessing them
//   3. Wrapped DB operations in try/catch

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace OnlineExamSystem
{
    public partial class ExamResult : System.Web.UI.Page
    {
        private string CS => WebConfigurationManager
                                .ConnectionStrings["OnlineExamConnectionString"]
                                .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["_ID"] == null || Session["_tMark"] == null)
            {
                Response.Write("<script>alert('Session expired. Please login again.');</script>");
                Response.Redirect("LoginPage.aspx");
                return;
            }

            int mark = (int)Session["_tMark"];
            markLabel.Text = mark.ToString();

            string ID = Session["_ID"].ToString();

            try
            {
                int noOfExam = 0;
                double totalMark = 0.0;

                // Step 1: Read existing stats
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT no_of_exam, total_mark FROM userInfo WHERE id=@id", con);
                    cmd.Parameters.AddWithValue("@id", ID);

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        noOfExam = Convert.ToInt32(dr["no_of_exam"]);
                        totalMark = Convert.ToDouble(dr["total_mark"]);
                    }
                }

                // Step 2: Calculate new stats
                totalMark = totalMark + (double)mark;
                noOfExam = noOfExam + 1;
                double avg = totalMark / (double)noOfExam;

                // FIX: Was SqlDataReader on UPDATE — must be ExecuteNonQuery()
                using (SqlConnection con1 = new SqlConnection(CS))
                {
                    con1.Open();
                    SqlCommand cmd1 = new SqlCommand(
                        "UPDATE userInfo SET no_of_exam=@nEx, total_mark=@tM, abc=@avg WHERE id=@id", con1);
                    cmd1.Parameters.AddWithValue("@nEx", noOfExam);
                    cmd1.Parameters.AddWithValue("@tM", totalMark);
                    cmd1.Parameters.AddWithValue("@avg", avg);
                    cmd1.Parameters.AddWithValue("@id", ID);
                    cmd1.ExecuteNonQuery(); // FIX: was cmd1.ExecuteReader()
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>console.error('Stats update error: " + ex.Message.Replace("'", "") + "');</script>");
            }

            // Show question results (from session)
            if (Session["_qs1"] != null) qs1.Text = Session["_qs1"].ToString();
            if (Session["_qs2"] != null) qs2.Text = Session["_qs2"].ToString();
            if (Session["_qs3"] != null) qs3.Text = Session["_qs3"].ToString();
            if (Session["_qs4"] != null) qs4.Text = Session["_qs4"].ToString();
            if (Session["_qs5"] != null) qs5.Text = Session["_qs5"].ToString();

            if (Session["_tag1"] != null) tag1.Text = "(" + Session["_tag1"].ToString() + ")";
            if (Session["_tag2"] != null) tag2.Text = "(" + Session["_tag2"].ToString() + ")";
            if (Session["_tag3"] != null) tag3.Text = "(" + Session["_tag3"].ToString() + ")";
            if (Session["_tag4"] != null) tag4.Text = "(" + Session["_tag4"].ToString() + ")";
            if (Session["_tag5"] != null) tag5.Text = "(" + Session["_tag5"].ToString() + ")";

            if (Session["_ans1"] != null) ans1.Text = Session["_ans1"].ToString();
            if (Session["_ans2"] != null) ans2.Text = Session["_ans2"].ToString();
            if (Session["_ans3"] != null) ans3.Text = Session["_ans3"].ToString();
            if (Session["_ans4"] != null) ans4.Text = Session["_ans4"].ToString();
            if (Session["_ans5"] != null) ans5.Text = Session["_ans5"].ToString();
        }

        protected void homeB_Click(object sender, EventArgs e) { Response.Redirect("Dashboard.aspx"); }
        protected void profileB_Click(object sender, EventArgs e) { Response.Redirect("UserProfile.aspx"); }
        protected void LeaderboardB_Click(object sender, EventArgs e) { Response.Redirect("Leaderboard.aspx"); }
        protected void logoutB_Click(object sender, EventArgs e) { Response.Redirect("LoginPage.aspx"); }
    }
}
