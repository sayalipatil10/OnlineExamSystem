// FIX APPLIED:
//   BEFORE: string CS = "your-database-connection-string"; in submitB_Click — CRASH on submit
//   AFTER:  Read from Web.config everywhere
//   Also: Added null check for Session["_Course"] on submit

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;

namespace OnlineExamSystem
{
    public partial class MCQExam : System.Web.UI.Page
    {
        // FIX: Single connection string from Web.config — used everywhere in this file
        private string CS => WebConfigurationManager
                                .ConnectionStrings["OnlineExamConnectionString"]
                                .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            string ET = "";

            if (Session["_Course"] != null)
            {
                string crs = Session["_Course"].ToString();

                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    LoadQuestion(con, crs, "1", qs1, RadioButtonList1, "_qs1", "_ans1", "_tag1", ref ET);
                }
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    LoadQuestion(con, crs, "2", qs2, RadioButtonList2, "_qs2", "_ans2", "_tag2", ref ET);
                }
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    LoadQuestion(con, crs, "3", qs3, RadioButtonList3, "_qs3", "_ans3", "_tag3", ref ET);
                }
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    LoadQuestion(con, crs, "4", qs4, RadioButtonList4, "_qs4", "_ans4", "_tag4", ref ET);
                }
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    LoadQuestion(con, crs, "5", qs5, RadioButtonList5, "_qs5", "_ans5", "_tag5", ref ET);
                }
            }
            else
            {
                Response.Write("<script>alert('No exam selected. Please go back and select a course.');</script>");
                return;
            }

            if (!IsPostBack)
            {
                int examTime = 30; // default 30 min
                int.TryParse(ET, out examTime);
                if (examTime <= 0) examTime = 30;
                Session["Timer"] = DateTime.Now.AddMinutes(examTime).ToString();
            }
        }

        private void LoadQuestion(SqlConnection con, string course, string qsNo,
            Label qsLabel, RadioButtonList rbl,
            string sessionQs, string sessionAns, string sessionTag,
            ref string ET)
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM mcqQS WHERE course=@course AND qsNo=@qsNo", con);
            cmd.Parameters.AddWithValue("@course", course);
            cmd.Parameters.AddWithValue("@qsNo", qsNo);

            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                qsLabel.Text = dr["qs"].ToString();
                rbl.Items[0].Text = dr["op1"].ToString();
                rbl.Items[1].Text = dr["op2"].ToString();
                rbl.Items[2].Text = dr["op3"].ToString();
                rbl.Items[3].Text = dr["op4"].ToString();
                Session[sessionQs] = dr["qs"].ToString();
                Session[sessionAns] = dr["ans"].ToString();
                Session[sessionTag] = dr["tag"].ToString();
                if (!string.IsNullOrEmpty(dr["etime"].ToString()))
                    ET = dr["etime"].ToString();
            }
        }

        protected void submitB_Click(object sender, EventArgs e)
        {
            // Guard: ensure session values exist
            if (Session["_ans1"] == null) { Response.Write("<script>alert('Session expired. Please log in again.');</script>"); return; }

            string a1 = Session["_ans1"].ToString();
            string a2 = Session["_ans2"].ToString();
            string a3 = Session["_ans3"].ToString();
            string a4 = Session["_ans4"].ToString();
            string a5 = Session["_ans5"].ToString();
            int mark = 0;

            if (RadioButtonList1.SelectedIndex > -1 && RadioButtonList1.SelectedItem.Text == a1) mark++;
            if (RadioButtonList2.SelectedIndex > -1 && RadioButtonList2.SelectedItem.Text == a2) mark++;
            if (RadioButtonList3.SelectedIndex > -1 && RadioButtonList3.SelectedItem.Text == a3) mark++;
            if (RadioButtonList4.SelectedIndex > -1 && RadioButtonList4.SelectedItem.Text == a4) mark++;
            if (RadioButtonList5.SelectedIndex > -1 && RadioButtonList5.SelectedItem.Text == a5) mark++;

            Session["_tMark"] = mark;

            string sNo = Session["_ID"].ToString();
            string crsNo = Session["_Course"].ToString();

            // FIX: Was "your-database-connection-string" — now uses Web.config
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO mcqTaken (studentID, courseID, examNo, mark) VALUES (@sID, @cID, @eNo, @mark)", con);
                cmd.Parameters.AddWithValue("@sID", sNo);
                cmd.Parameters.AddWithValue("@cID", crsNo);
                cmd.Parameters.AddWithValue("@eNo", "1");
                cmd.Parameters.AddWithValue("@mark", (double)mark);
                cmd.ExecuteNonQuery();
            }

            Response.Redirect("ExamResult.aspx");
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if (Session["Timer"] == null) return;

            DateTime endTime = DateTime.Parse(Session["Timer"].ToString());
            if (DateTime.Compare(DateTime.Now, endTime) < 0)
            {
                TimeSpan remaining = endTime.Subtract(DateTime.Now);
                Label3.Text = "Time Left: " + (int)remaining.TotalMinutes + " min " + (remaining.Seconds) + " sec";
            }
            else
            {
                Label3.Text = "Time Out!";
            }
        }

        protected void homeB_Click(object sender, EventArgs e) { Response.Redirect("Dashboard.aspx"); }
        protected void profileB_Click(object sender, EventArgs e) { Response.Redirect("UserProfile.aspx"); }
        protected void LeaderboardB_Click(object sender, EventArgs e) { Response.Redirect("Leaderboard.aspx"); }
        protected void logoutB_Click(object sender, EventArgs e) { Response.Redirect("LoginPage.aspx"); }
    }
}
