// FIX APPLIED:
//   BEFORE: SelectCourseDropDownList_SelectedIndexChanged used "your-database-connection-string" — crash
//   AFTER:  Read from Web.config

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;

namespace OnlineExamSystem
{
    public partial class SetExam : System.Web.UI.Page
    {
        private string CS => WebConfigurationManager
                                .ConnectionStrings["OnlineExamConnectionString"]
                                .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void homeB_Click(object sender, EventArgs e) { Response.Redirect("AdminPanel.aspx"); }
        protected void profileB_Click(object sender, EventArgs e) { Response.Redirect("AdminQueue.aspx"); }
        protected void LeaderboardB_Click(object sender, EventArgs e) { Response.Redirect("AdminLeaderboard.aspx"); }
        protected void logoutB_Click(object sender, EventArgs e) { Response.Redirect("LoginPage.aspx"); }

        protected void clearB_Click(object sender, EventArgs e)
        {
            QTBox.Text = "";
            Op1TBox.Text = "";
            Op2TBox.Text = "";
            Op3TBox.Text = "";
            Op4TBox.Text = "";
            AnsTBox.Text = "";
            qsNoTBox.Text = "";
            qsTagTB.Text = "";
        }

        protected void AddQB_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    // FIX: Parameterized insert
                    SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO mcqQS (course, qsId, qsNo, qs, op1, op2, op3, op4, ans, tag, etime)
                        VALUES (@course, NULL, @qsNo, @qs, @op1, @op2, @op3, @op4, @ans, @tag, @etime)", con);
                    cmd.Parameters.AddWithValue("@course", SelectCourseDropDownList.SelectedValue);
                    cmd.Parameters.AddWithValue("@qsNo", qsNoTBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@qs", QTBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@op1", Op1TBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@op2", Op2TBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@op3", Op3TBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@op4", Op4TBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@ans", AnsTBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@tag", qsTagTB.Text.Trim());
                    cmd.Parameters.AddWithValue("@etime", timeTB.Text.Trim());
                    cmd.ExecuteNonQuery();
                }

                // Update the question number display
                UpdateQuestionNumber();
                Response.Write("<script>alert('Question Added!');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error adding question: " + ex.Message.Replace("'", "\\'") + "');</script>");
            }
        }

        private void UpdateQuestionNumber()
        {
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM mcqQS WHERE course=@course", con);
                cmd.Parameters.AddWithValue("@course", SelectCourseDropDownList.SelectedValue);
                int count = (int)cmd.ExecuteScalar();
                qsNoTBox.Text = (count + 1).ToString();
            }
        }

        protected void setB_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM mcqCourseDetail WHERE courseID=@cID", con);
                cmd.Parameters.AddWithValue("@cID", SelectCourseDropDownList.SelectedValue);
                int count = (int)cmd.ExecuteScalar();
                string examNo = (count + 1).ToString();

                cmd = new SqlCommand(
                    "INSERT INTO mcqCourseDetail (courseID, examNo) VALUES (@cID, @eNo)", con);
                cmd.Parameters.AddWithValue("@cID", SelectCourseDropDownList.SelectedValue);
                cmd.Parameters.AddWithValue("@eNo", examNo);
                cmd.ExecuteNonQuery();
            }

            Response.Write("<script>alert('Exam Set Successfully!');</script>");
            Response.Redirect("AdminPanel.aspx");
        }

        // FIX: Was "your-database-connection-string" here — crashes when dropdown changes
        protected void SelectCourseDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateQuestionNumber();
        }

        protected void selectB_Click(object sender, EventArgs e) { LoadCoursesByYear(); }
        protected void selectSemesterDropDownList_SelectedIndexChanged(object sender, EventArgs e) { LoadCoursesByYear(); }

        private void LoadCoursesByYear()
        {
            SelectCourseDropDownList.Items.Clear();
            var items = new List<ListItem>();
            items.Add(new ListItem("-- Select --", "Select"));

            string sem = selectSemesterDropDownList.SelectedItem.Text;
            if (sem == "1st Year 1st Semseter") { items.Add(new ListItem("CSE-1101")); items.Add(new ListItem("CSE-1102")); items.Add(new ListItem("CSE-1103")); items.Add(new ListItem("CSE-1104")); }
            else if (sem == "1st Year 2nd Semseter") { items.Add(new ListItem("CSE-1201")); items.Add(new ListItem("CSE-1202")); items.Add(new ListItem("CSE-1203")); items.Add(new ListItem("CSE-1204")); }
            else if (sem == "2nd Year 1st Semseter") { items.Add(new ListItem("CSE-2101")); items.Add(new ListItem("CSE-2102")); items.Add(new ListItem("CSE-2103")); items.Add(new ListItem("CSE-2104")); }
            else if (sem == "2nd Year 2nd Semseter") { items.Add(new ListItem("CSE-2201")); items.Add(new ListItem("CSE-2202")); items.Add(new ListItem("CSE-2203")); items.Add(new ListItem("CSE-2204")); }
            else if (sem == "3rd Year 1st Semseter") { items.Add(new ListItem("CSE-3101")); items.Add(new ListItem("CSE-3102")); items.Add(new ListItem("CSE-3103")); items.Add(new ListItem("CSE-3104")); }
            else if (sem == "3rd Year 2nd Semseter") { items.Add(new ListItem("CSE-3201")); items.Add(new ListItem("CSE-3202")); items.Add(new ListItem("CSE-3203")); items.Add(new ListItem("CSE-3204")); }
            else if (sem == "4th Year 1st Semseter") { items.Add(new ListItem("CSE-4101")); items.Add(new ListItem("CSE-4102")); items.Add(new ListItem("CSE-4103")); items.Add(new ListItem("CSE-4104")); }
            else if (sem == "4th Year 2nd Semseter") { items.Add(new ListItem("CSE-4201")); items.Add(new ListItem("CSE-4202")); items.Add(new ListItem("CSE-4203")); items.Add(new ListItem("CSE-4204")); }

            SelectCourseDropDownList.Items.AddRange(items.ToArray());
        }
    }
}
