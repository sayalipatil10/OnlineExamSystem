// No critical bugs — cleaned up to use parameterized queries and Web.config connection

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;

namespace OnlineExamSystem
{
    public partial class TheorySet : System.Web.UI.Page
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
            QATBox.Text = "";
            markATB.Text = "";
            QBTBox.Text = "";
            markBTB.Text = "";
        }

        protected void AddQB_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO theoryQS (course, qsId, qsNo, qsA, markA, qsB, markB, eTime)
                        VALUES (@course, NULL, @qsNo, @qsA, @markA, @qsB, @markB, @eTime)", con);
                    cmd.Parameters.AddWithValue("@course", SelectCourseDropDownList.SelectedValue);
                    cmd.Parameters.AddWithValue("@qsNo", qsNoTB.Text.Trim());
                    cmd.Parameters.AddWithValue("@qsA", QATBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@markA", markATB.Text.Trim());
                    cmd.Parameters.AddWithValue("@qsB", QBTBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@markB", markBTB.Text.Trim());
                    cmd.Parameters.AddWithValue("@eTime", timeTB.Text.Trim());
                    cmd.ExecuteNonQuery();
                }

                UpdateQuestionNumber();
                Response.Write("<script>alert('Question Added!');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Error: " + ex.Message.Replace("'", "\\'") + "');</script>");
            }
        }

        private void UpdateQuestionNumber()
        {
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM theoryQS WHERE course=@course", con);
                cmd.Parameters.AddWithValue("@course", SelectCourseDropDownList.SelectedValue);
                int count = (int)cmd.ExecuteScalar();
                qsNoTB.Text = (count + 1).ToString();
            }
        }

        protected void setB_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM theoryCourseDetail WHERE courseID=@cID", con);
                cmd.Parameters.AddWithValue("@cID", SelectCourseDropDownList.SelectedValue);
                int count = (int)cmd.ExecuteScalar();
                string examNo = (count + 1).ToString();

                cmd = new SqlCommand(
                    "INSERT INTO theoryCourseDetail (courseID, examNo) VALUES (@cID, @eNo)", con);
                cmd.Parameters.AddWithValue("@cID", SelectCourseDropDownList.SelectedValue);
                cmd.Parameters.AddWithValue("@eNo", examNo);
                cmd.ExecuteNonQuery();
            }

            Response.Write("<script>alert('Theory Exam Set Successfully!');</script>");
            Response.Redirect("AdminPanel.aspx");
        }

        protected void selectB_Click(object sender, EventArgs e) { LoadCoursesByYear(); }
        protected void selectSemesterDropDownList_SelectedIndexChanged(object sender, EventArgs e) { LoadCoursesByYear(); }
        protected void SelectCourseDropDownList_SelectedIndexChanged(object sender, EventArgs e) { UpdateQuestionNumber(); }

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
