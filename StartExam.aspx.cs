using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

namespace OnlineExamSystem
{
    public partial class StartExam : System.Web.UI.Page
    {
        // ✅ Global connection string
        private string CS = @"Data Source=DESKTOP-I3CE4PO\SQLEXPRESS;Initial Catalog=OnlineExam;Integrated Security=True;";

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
                    string query = "SELECT semester FROM userInfo WHERE id=@id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", ID);

                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        year = dr["semester"].ToString();
                    }
                }

                LoadCourses(year);
            }
        }

        // ✅ Load courses based on semester
        private void LoadCourses(string year)
        {
            SelectCourseDropDownList.Items.Clear();
            List<ListItem> items = new List<ListItem>();

            if (year == "1st Year 1st Semseter")
            {
                items.Add(new ListItem("CSE-1101"));
                items.Add(new ListItem("CSE-1102"));
                items.Add(new ListItem("CSE-1103"));
                items.Add(new ListItem("CSE-1104"));
            }
            else if (year == "1st Year 2nd Semseter")
            {
                items.Add(new ListItem("CSE-1201"));
                items.Add(new ListItem("CSE-1202"));
                items.Add(new ListItem("CSE-1203"));
                items.Add(new ListItem("CSE-1204"));
            }
            else if (year == "2nd Year 1st Semseter")
            {
                items.Add(new ListItem("CSE-2101"));
                items.Add(new ListItem("CSE-2102"));
                items.Add(new ListItem("CSE-2103"));
                items.Add(new ListItem("CSE-2104"));
            }
            else if (year == "2nd Year 2nd Semseter")
            {
                items.Add(new ListItem("CSE-2201"));
                items.Add(new ListItem("CSE-2202"));
                items.Add(new ListItem("CSE-2203"));
                items.Add(new ListItem("CSE-2204"));
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
            string course = SelectCourseDropDownList.Text;

            if (examType == "Select Exam Type")
            {
                Response.Write("<script>alert('Please select exam type!');</script>");
                return;
            }

            Session["_Course"] = course;
            Session["_qsN"] = 1;

            string tableName = examType == "Theory" ? "theoryQS" : "mcqQS";

            using (SqlConnection con = new SqlConnection(CS))
            {
                con.Open();
                string query = $"SELECT COUNT(*) FROM {tableName} WHERE course=@course";
                SqlCommand cmd = new SqlCommand(query, con);
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
                    Response.Write("<script>alert('No Exam Found!');</script>");
                }
            }
        }

        protected void logoutB_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("LoginPage.aspx");
        }

        protected void homeB_Click(object sender, EventArgs e)
        {
            Response.Redirect("Dashboard.aspx");
        }

        protected void profileB_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserProfile.aspx");
        }

        protected void LeaderboardB_Click(object sender, EventArgs e)
        {
            Response.Redirect("Leaderboard.aspx");
        }
    }
}