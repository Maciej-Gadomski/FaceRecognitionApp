using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FaceRecognitonApp
{
    public partial class Statystyki : System.Web.UI.Page
    {
        String Polaczenie;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Cookies["login"] == null )
            {
                Response.Redirect("~/Homepage.aspx/");
            }
            else
            {
                string pom = Request.Cookies["login"].Value;
                string log = pom.Substring(pom.IndexOf('=') + 1);
                Polaczenie = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
                using (SqlConnection sql = new SqlConnection(Polaczenie))
                {
                    sql.Open();
                    SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT status,data,przyczyna FROM [stats] WHERE login='" + log + "'", sql);
                    DataTable datatbl = new DataTable();
                    sqlDa.Fill(datatbl);
                    gridView.DataSource = datatbl;
                    ViewState["dt1"] = datatbl;
                    gridView.DataBind();
                }
            }
        }

        protected void gridView_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void gridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            if(ViewState["dt1"]!=null)
            {
                DataTable dt = (DataTable)ViewState["dt1"];
                DataView dv = new DataView(dt);
                dv.Sort = e.SortExpression;
                gridView.DataSource = dv;
                gridView.DataBind();
            }
        }

        protected void gridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            for(int i=0;i<gridView.Rows.Count;i++)
            {
                string tekst = Convert.ToString(gridView.Rows[i].Cells[0].Text);
                if (tekst == "Pomyślne")
                    gridView.Rows[i].BackColor = ColorTranslator.FromHtml("#27a929");
                else
                    gridView.Rows[i].BackColor = ColorTranslator.FromHtml("#D00909");
            }

        }

        protected void reg_Click(object sender, EventArgs e)
        {
        }

        protected void log_Click(object sender, EventArgs e)
        {
            HttpCookie mycookie = new HttpCookie("Login");
            mycookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(mycookie);

            Response.Redirect("~/Homepage.aspx/");
        }
    }
}