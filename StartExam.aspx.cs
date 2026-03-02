// FIX APPLIED:
//   BEFORE: private string CS = @"Data Source=DESKTOP-I3CE4PO\SQLEXPRESS;..."; (hardcoded)
//   AFTER:  Read from Web.config
//   Also: Session["_qsN"] = 1 preserved — TheoryExam reads this correctly (see TheoryExam fix)

using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace OnlineExamSystem
{
    public partial class StartExam : System.Web.UI.Page
    {
        // FIX: Read from Web.config — NOT hardcoded
        private string CS => WebConfigurationManager
                                .ConnectionStrings["OnlineExamConnectionString"]
                                .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["_ID"] == null)
            {
                Response.Redirect("LoginPage.aspx");
                return;
            }

            if (!IsPostBack)
            {
                string year = "";
                string ID = Session["_ID"].ToString();

                using (SqlConnection con = new SqlConnection(CS))
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("SELECT semester FROM userInfo WHERE id=@id", con);
                    cmd.Parameters.AddWithValue("@id", ID);

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                        year = dr["semester"].ToString();
                }

                LoadCourses(year);
            }
        }

        private void LoadCourses(string year)
        {
            SelectCourseDropDownList.Items.Clear();
            var items = new List<ListItem>();

            if (year == "1st Year 1st Semseter")
            {
                items.Add(new ListItem("CSE-1101")); items.Add(new ListItem("CSE-1102"));
                items.Add(new ListItem("CSE-1103")); items.Add(new ListItem("CSE-1104"));
            }
            else if (year == "1st Year 2nd Semseter")
            {
                items.Add(new ListItem("CSE-1201")); items.Add(new ListItem("CSE-1202"));
                items.Add(new ListItem("CSE-1203")); items.Add(new ListItem("CSE-1204"));
            }
            else if (year == "2nd Year 1st Semseter")
            {
                items.Add(new ListItem("CSE-2101")); items.Add(new ListItem("CSE-2102"));
                items.Add(new ListItem("CSE-2103")); items.Add(new ListItem("CSE-2104"));
            }
            else if (year == "2nd Year 2nd Semseter")
            {
                items.Add(new ListItem("CSE-2201")); items.Add(new ListItem("CSE-2202"));
                items.Add(new ListItem("CSE-2203")); items.Add(new ListItem("CSE-2204"));
            }
            else if (year == "3rd Year 1st Semseter")
            {
                items.Add(new ListItem("CSE-3101")); items.Add(new ListItem("CSE-3102"));
                items.Add(new ListItem("CSE-3103")); items.Add(new ListItem("CSE-3104"));
            }
            else if (year == "3rd Year 2nd Semseter")
            {
                items.Add(new ListItem("CSE-3201")); items.Add(new ListItem("CSE-3202"));
                items.Add(new ListItem("CSE-3203")); items.Add(new ListItem("CSE-3204"));
            }
            else if (year == "4th Year 1st Semseter")
            {
                items.Add(new ListItem("CSE-4101")); items.Add(new ListItem("CSE-4102"));
                items.Add(new ListItem("CSE-4103")); items.Add(new ListItem("CSE-4104"));
            }
            else if (year == "4th Year 2nd Semseter")
            {
                items.Add(new ListItem("CSE-4201")); items.Add(new ListItem("CSE-4202"));
                items.Add(new ListItem("CSE-4203")); items.Add(new ListItem("CSE-4204"));
            }
            else
            {
                items.Add(new ListItem("Select Your Course"));
            }

            SelectCourseDropDownList.Items.AddRange(items.ToArray());
        }

        protected void startB_Click(object sender, EventArgs e)
        {
            string examType = selectExamType.SelectedItem.Text;
            string course = SelectCourseDropDownList.SelectedValue;

            Session["_Course"] = course;
            // FIX: Set _qsN = 1 here. TheoryExam.aspx.cs reads Session["_qsN"] (was broken "_qNo")
            Session["_qsN"] = 1;

            string tableName = (examType == "Theory") ? "theoryQS" : "mcqQS";

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                // Safe: tableName is only ever "theoryQS" or "mcqQS" — not user input
                SqlCommand cmd = new SqlCommand(
                    $"SELECT COUNT(*) FROM {tableName} WHERE course=@course", con);
                cmd.Parameters.AddWithValue("@course", course);

                int count = (int)cmd.ExecuteScalar();

                if (count >= 1)
                {
                    if (examType == "Theory")
                        Response.Redirect("TheoryExam.aspx");
                    else
                        Response.Redirect("MCQExam.aspx");
                }
                else
                {
                    Response.Write("<script>alert('No Exam found for this course!');</script>");
                }
            }
        }

        protected void homeB_Click(object sender, EventArgs e) { Response.Redirect("Dashboard.aspx"); }
        protected void profileB_Click(object sender, EventArgs e) { Response.Redirect("UserProfile.aspx"); }
        protected void LeaderboardB_Click(object sender, EventArgs e) { Response.Redirect("Leaderboard.aspx"); }
        protected void logoutB_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("LoginPage.aspx");
        }
    }
}
