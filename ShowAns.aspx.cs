// FIXES APPLIED:
//   1. BEFORE: double total = a+b+c+d+ee; then immediately total = a1+b1+c1+d1+ee1;
//      The second line OVERWROTE the first — A-part marks were completely lost!
//      AFTER:  double total = (a+b+c+d+ee) + (a1+b1+c1+d1+ee1);  — sums ALL marks correctly
//   2. Used parameterized queries for all DB operations
//   3. Used ExecuteNonQuery() consistently (not ExecuteReader on UPDATE/DELETE)

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace OnlineExamSystem
{
    public partial class ShowAns : System.Web.UI.Page
    {
        private string CS => WebConfigurationManager
                                .ConnectionStrings["OnlineExamConnectionString"]
                                .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["_stID"] == null || Session["_crsID"] == null)
            {
                Response.Write("<script>alert('Some problem finding your data');</script>");
                Response.Redirect("AdminPanel.aspx");
                return;
            }

            string sID = Session["_stID"].ToString();
            string cID = Session["_crsID"].ToString();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                LoadAnswer(con, sID, cID, "1", qs1A, m1A, ans1ATB, qs1B, m1B, ans1BTB);
                LoadAnswer(con, sID, cID, "2", qs2A, m2A, ans2ATB, qs2B, m2B, ans2BTB);
                LoadAnswer(con, sID, cID, "3", qs3A, m3A, ans3ATB, qs3B, m3B, ans3BTB);
                LoadAnswer(con, sID, cID, "4", qs4A, m4A, ans4ATB, qs4B, m4B, ans4BTB);
                LoadAnswer(con, sID, cID, "5", qs5A, m5A, ans5ATB, qs5B, m5B, ans5BTB);
            }
        }

        private void LoadAnswer(SqlConnection con, string sID, string cID, string qsNo,
            Label qsLabelA, Label markLabelA, TextBox ansA,
            Label qsLabelB, Label markLabelB, TextBox ansB)
        {
            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM theoryAns WHERE studentID=@sID AND courseID=@cID AND qsNo=@qsNo", con);
            cmd.Parameters.AddWithValue("@sID", sID);
            cmd.Parameters.AddWithValue("@cID", cID);
            cmd.Parameters.AddWithValue("@qsNo", qsNo);

            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                qsLabelA.Text = dr["qsA"].ToString();
                markLabelA.Text = dr["markA"].ToString();
                ansA.Text = dr["ansA"].ToString();
                qsLabelB.Text = dr["qsB"].ToString();
                markLabelB.Text = dr["markB"].ToString();
                ansB.Text = dr["ansB"].ToString();
            }
            dr.Close();
        }

        protected void submitB_Click(object sender, EventArgs e)
        {
            // Parse all marks entered by admin
            int a = 0, b = 0, c = 0, d = 0, ee = 0;
            int a1 = 0, b1 = 0, c1 = 0, d1 = 0, ee1 = 0;

            int.TryParse(mark1A.Text, out a);
            int.TryParse(mark2A.Text, out b);
            int.TryParse(mark3A.Text, out c);
            int.TryParse(mark4A.Text, out d);
            int.TryParse(mark5A.Text, out ee);

            int.TryParse(mark1B.Text, out a1);
            int.TryParse(mark2B.Text, out b1);
            int.TryParse(mark3B.Text, out c1);
            int.TryParse(mark4B.Text, out d1);
            int.TryParse(mark5B.Text, out ee1);

            // FIX: BEFORE — total was assigned TWICE, second assignment wiped out A-part marks
            // AFTER: Single correct calculation summing all parts
            double total = (a + b + c + d + ee) + (a1 + b1 + c1 + d1 + ee1);

            string sID = Session["_stID"].ToString();
            string cID = Session["_crsID"].ToString();

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();

                // Update theory answers with admin-assigned mark and approval
                SqlCommand cmd = new SqlCommand(
                    "UPDATE theoryAns SET mark=@mark, isAprove='Yes' WHERE studentID=@sID AND courseID=@cID", con);
                cmd.Parameters.AddWithValue("@mark", total);
                cmd.Parameters.AddWithValue("@sID", sID);
                cmd.Parameters.AddWithValue("@cID", cID);
                cmd.ExecuteNonQuery(); // FIX: was ExecuteReader on an UPDATE

                // Remove student from pending queue
                SqlCommand cmd1 = new SqlCommand(
                    "DELETE FROM theoryCourseQueue WHERE student_ID=@sID AND courseID=@cID", con);
                cmd1.Parameters.AddWithValue("@sID", sID);
                cmd1.Parameters.AddWithValue("@cID", cID);
                cmd1.ExecuteNonQuery(); // FIX: was ExecuteReader on a DELETE
            }

            Response.Write("<script>alert('Mark Submitted!');</script>");
            Session["_checkCID"] = cID;
            Response.Redirect("AdminCourseQueue.aspx");
        }

        protected void homeB_Click(object sender, EventArgs e) { Response.Redirect("AdminPanel.aspx"); }
        protected void profileB_Click(object sender, EventArgs e) { Response.Redirect("AdminQueue.aspx"); }
        protected void LeaderboardB_Click(object sender, EventArgs e) { Response.Redirect("AdminLeaderboard.aspx"); }
        protected void logoutB_Click(object sender, EventArgs e) { Response.Redirect("LoginPage.aspx"); }
    }
}
