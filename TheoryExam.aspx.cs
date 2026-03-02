// FIXES APPLIED:
//   1. BEFORE: Session["_qNo"] — session key never set anywhere => NullReferenceException crash
//      AFTER:  Session["_qsN"] — correct key set in StartExam.aspx.cs
//   2. BEFORE: string CS = "your-database-connection-string"; in submitB_Click (TWO places) — crash on submit
//      AFTER:  Read from Web.config everywhere
//   3. Used parameterized queries for all inserts

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace OnlineExamSystem
{
    public partial class TheoryExam : System.Web.UI.Page
    {
        // FIX: Single Web.config connection string — used everywhere in this file
        private string CS => WebConfigurationManager
                                .ConnectionStrings["OnlineExamConnectionString"]
                                .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["_ID"] == null)
            {
                Response.Write("<script>alert('Please login first!');</script>");
                Response.Redirect("LoginPage.aspx");
                return;
            }

            string ET = "";
            string crs = Session["_Course"].ToString();

            // FIX: Was Session["_qNo"] — that key is never set. Correct key is "_qsN" (set in StartExam)
            int qN = 1;
            if (Session["_qsN"] != null)
                qN = (int)Session["_qsN"];

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                LoadTheoryQuestion(con, crs, qN.ToString(), qs1A, qs1B, m1A, m1B, ref ET);
                LoadTheoryQuestion(con, crs, (qN + 1).ToString(), qs2A, qs2B, m2A, m2B, ref ET);
                LoadTheoryQuestion(con, crs, (qN + 2).ToString(), qs3A, qs3B, m3A, m3B, ref ET);
                LoadTheoryQuestion(con, crs, (qN + 3).ToString(), qs4A, qs4B, m4A, m4B, ref ET);
                LoadTheoryQuestion(con, crs, (qN + 4).ToString(), qs5A, qs5B, m5A, m5B, ref ET);
            }

            if (!IsPostBack)
            {
                int examTime = 30;
                int.TryParse(ET, out examTime);
                if (examTime <= 0) examTime = 30;
                Session["Timer"] = DateTime.Now.AddMinutes(examTime).ToString();
            }
        }

        private void LoadTheoryQuestion(SqlConnection con, string course, string qsNo,
            Label labelA, Label labelB, Label markA, Label markB, ref string ET)
        {
            // Reopen if closed
            if (con.State == System.Data.ConnectionState.Closed) con.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT * FROM theoryQS WHERE course=@course AND qsNo=@qsNo", con);
            cmd.Parameters.AddWithValue("@course", course);
            cmd.Parameters.AddWithValue("@qsNo", qsNo);

            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                labelA.Text = dr["qsA"].ToString();
                labelB.Text = dr["qsB"].ToString();
                markA.Text = dr["markA"].ToString();
                markB.Text = dr["markB"].ToString();
                if (!string.IsNullOrEmpty(dr["eTime"].ToString()))
                    ET = dr["eTime"].ToString();
            }
            dr.Close();
        }

        protected void submitB_Click(object sender, EventArgs e)
        {
            string stID = Session["_ID"].ToString();
            string crsID = Session["_Course"].ToString();

            // FIX: Was "your-database-connection-string" (TWO places) — now Web.config
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();

                // Insert 5 theory answer rows
                InsertTheoryAns(con, stID, crsID, "1", qs1A.Text, ans1ATB.Text, m1A.Text, qs1B.Text, m1B.Text, ans1BTB.Text);
                InsertTheoryAns(con, stID, crsID, "2", qs2A.Text, ans2ATB.Text, m2A.Text, qs2B.Text, m2B.Text, ans2BTB.Text);
                InsertTheoryAns(con, stID, crsID, "3", qs3A.Text, ans3ATB.Text, m3A.Text, qs3B.Text, m3B.Text, ans3BTB.Text);
                InsertTheoryAns(con, stID, crsID, "4", qs4A.Text, ans4ATB.Text, m4A.Text, qs4B.Text, m4B.Text, ans4BTB.Text);
                InsertTheoryAns(con, stID, crsID, "5", qs5A.Text, ans5ATB.Text, m5A.Text, qs5B.Text, m5B.Text, ans5BTB.Text);

                // Add to admin course queue (student pending grading)
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO theoryCourseQueue (student_ID, courseID) VALUES (@sID, @cID)", con);
                cmd.Parameters.AddWithValue("@sID", stID);
                cmd.Parameters.AddWithValue("@cID", crsID);
                cmd.ExecuteNonQuery();

                // Add to admin visible queue
                string courseName = GetCourseName(crsID);
                cmd = new SqlCommand(
                    "INSERT INTO theoryQueue (courseID, courseName) VALUES (@cID, @cName)", con);
                cmd.Parameters.AddWithValue("@cID", crsID);
                cmd.Parameters.AddWithValue("@cName", courseName);
                cmd.ExecuteNonQuery();

                // Record in taken courses
                cmd = new SqlCommand(
                    "INSERT INTO theoryTaken (studentID, courseID, examNo) VALUES (@sID, @cID, @eNo)", con);
                cmd.Parameters.AddWithValue("@sID", stID);
                cmd.Parameters.AddWithValue("@cID", crsID);
                cmd.Parameters.AddWithValue("@eNo", "1");
                cmd.ExecuteNonQuery();
            }

            Response.Write("<script>alert('Your Answer Sheet Submitted!');</script>");
            Response.Redirect("Dashboard.aspx");
        }

        private void InsertTheoryAns(SqlConnection con, string stID, string crsID, string qsNo,
            string qsA, string ansA, string markA, string qsB, string markB, string ansB)
        {
            SqlCommand cmd = new SqlCommand(@"
                INSERT INTO theoryAns (studentID, courseID, qsNo, qsA, ansA, markA, isAprove, qsB, markB, ansB)
                VALUES (@sID, @cID, @qsNo, @qsA, @ansA, @markA, 'No', @qsB, @markB, @ansB)", con);

            cmd.Parameters.AddWithValue("@sID", stID);
            cmd.Parameters.AddWithValue("@cID", crsID);
            cmd.Parameters.AddWithValue("@qsNo", qsNo);
            cmd.Parameters.AddWithValue("@qsA", qsA);
            cmd.Parameters.AddWithValue("@ansA", ansA);
            cmd.Parameters.AddWithValue("@markA", markA);
            cmd.Parameters.AddWithValue("@qsB", qsB);
            cmd.Parameters.AddWithValue("@markB", markB);
            cmd.Parameters.AddWithValue("@ansB", ansB);
            cmd.ExecuteNonQuery();
        }

        private string GetCourseName(string courseID)
        {
            switch (courseID)
            {
                case "CSE-1101": return "Computer Basics & Programming Fundamentals";
                case "CSE-1201": return "Electronic Devices and Circuits";
                case "CSE-2101": return "Object Oriented Programming";
                case "CSE-2201": return "Algorithm Design & Analysis";
                case "CSE-3101": return "Operating System";
                case "CSE-3201": return "Compiler Design";
                case "CSE-4101": return "Artificial Intelligence & Expert System";
                case "CSE-4201": return "Computer Graphics & Animation";
                default: return courseID;
            }
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if (Session["Timer"] == null) return;

            DateTime endTime = DateTime.Parse(Session["Timer"].ToString());
            if (DateTime.Compare(DateTime.Now, endTime) < 0)
            {
                TimeSpan remaining = endTime.Subtract(DateTime.Now);
                Label3.Text = "Time Left: " + (int)remaining.TotalMinutes + " min " + remaining.Seconds + " sec";
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
