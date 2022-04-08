using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.Services;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace FaceRecognitonApp
{
    public partial class Homepage : System.Web.UI.Page
    {
        string Polaczenie;
        public static string wynik = "";
        public static string imageFromDb="";
        public static string keyFromDb = "";
        public static string pom = "";
        protected void Page_Load(object sender, EventArgs e)
        {

            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            string Password = password.Text;
            password.Attributes.Add("value", Password);
            uploadImage.Attributes["onchange"] = "UploadFile(this)";
            startFaceRecognition.Attributes.CssStyle.Add("visibility", "collapse");
            informations.Attributes.CssStyle.Add("color", "#D00909");
        }

        protected void confirmButton_Click(object sender, EventArgs e)
        {
            string path;
            bool p=true;
            if (imageSource.SelectedValue == "file")
            {
                if (FaceApi.CheckFileType(uploadImage.FileName))
                {
                    byte[] imageDatabase;
                    Stream s = uploadImage.PostedFile.InputStream;
                    BinaryReader br = new BinaryReader(s);
                    imageDatabase = br.ReadBytes((int)s.Length);
                    Task.Run(async () => { wynik = await FaceApi.FaceDetection(imageDatabase); }).Wait();

                    if (wynik == "[]")
                    {
                        addToStats("Niepomyślne", "Brak twarzy na zdjęciu");
                        pictureCheckResult.Text = "Twoje zdjęcie nie zawiera twarzy";
                        pictureCheckResult.Attributes.CssStyle.Add("color", "#D00909");
                    }
                    else
                    {
                        Task.Run(async () => { pom = await FaceApi.imageCompare(keyFromDb, wynik); }).Wait();
                        if (pom == "True")
                        {
                            Session["user"] = login.Text;
                            addToStats("Pomyślne", "Nie dotyczy");
                            auth();
                        }
                        if (pom == "False")
                        {
                            pictureCheckResult.Attributes.CssStyle.Add("color", "#D00909");
                            pictureCheckResult.Text = "Nie rozpoznano twarzy!";
                            addToStats("Niepomyślne", "Nie rozpoznano twarzy");
                        }
                    }
                }
                else
                {
                    pictureCheckResult.Text = "Nieprawidłowy format pliku!";
                    pictureCheckResult.Attributes.CssStyle.Add("color", "#D00909");
                    addToStats("Niepomyślne", "Nieprawidłowy format pliku");
                    p = false;
                }

            }
            else
            {
                path = HttpContext.Current.Server.MapPath("~/Captures/") + login.Text + ".jpg";
                Task.Run(async () => { wynik = await FaceApi.FaceDetection(FaceApi.ReadAllBytes(path)); }).Wait();
                deleteImage();
                if (wynik == "[]")
                {
                    addToStats("Niepomyślne", "Brak twarzy na zdjęciu");
                    pictureCheckResult.Text = "Twoje zdjęcie nie zawiera twarzy";
                    pictureCheckResult.Attributes.CssStyle.Add("color", "#D00909");
                }
                else
                {
                    Task.Run(async () => { pom = await FaceApi.imageCompare(keyFromDb, wynik); }).Wait();
                    if (pom == "True")
                    {
                        Session["user"] = login.Text;
                        addToStats("Pomyślne", "Nie dotyczy");
                        auth();
                    }
                    if (pom == "False")
                    {
                        pictureCheckResult.Attributes.CssStyle.Add("color", "#D00909");
                        pictureCheckResult.Text = "Nie rozpoznano twarzy!";
                        addToStats("Niepomyślne", "Nie rozpoznano twarzy");
                    }
                }
            }
            changeView("confirmButton");
        }

        protected void zarejestruj_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Rejestracja.aspx/");
        }
        public void changeView(string view)
        {
            if (view == "startFaceRecognition" || view == "stopFaceRecognition" || view == "confirmButton")
            {
                imageSource.Visible = false;
                login.Enabled = false;
                login.Enabled = false;
                password.Enabled = false;
                imageSource.Visible = false;
                uploadImage.Visible = false;
                capturedImage.Visible = false;
                makeImage.Visible = false;
                clearImage.Visible = false;
                confirmButton.Visible = false;
                pictureCheckResult.Visible = false;
            }
            if (view == "stopFaceRecognition")
            {
                login.Enabled = true;
                login.Enabled = true;
                password.Enabled = true;
            }
            if (view == "startFaceRecognition")
            {
                imageSource.Visible = true;
                imageSource.ClearSelection();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "changeview", "changePageView()", true);

            }
            if (view == "confirmButton")
            {
                imageSource.Visible = true;
                imageSource.ClearSelection();
                pictureCheckResult.Visible = true;
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "changeview", "changePageView()", true);
            }
            if (view == "imageFromFile")
            {
                imageSource.Visible = true;
                confirmButton.Visible = true;
                uploadImage.Visible = true;
                capturedImage.Visible = false;
                makeImage.Visible = false;
                clearImage.Visible = false;
                pictureCheckResult.Visible = false;
                confirmButton.Attributes.CssStyle.Add("display", "none");
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "changeview", "changePageView()", true);
            }
            if (view == "imageFromCamera")
            {
                confirmButton.Visible = false;
                uploadImage.Visible = false;
                capturedImage.Visible = false;
                clearImage.Visible = false;
                imageSource.Visible = true;
                makeImage.Visible = true;
                pictureCheckResult.Visible = false;
                confirmButton.Attributes.CssStyle.Add("display", "block");
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "changeview", "changePageView()", true);
            }
        }

        protected void startFaceRecognition_CheckedChanged(object sender, EventArgs e)
        {  
        }

        protected void imageSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (imageSource.SelectedValue == "file")
                changeView("imageFromFile");
            else
            {
                changeView("imageFromCamera");
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "text", "runcamera()", true);
            }
        }

        protected void makeImage_Click(object sender, EventArgs e)
        {
            capturedImage.Visible = true;
            clearImage.Visible = true;
            confirmButton.Visible = true;
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "changeview", "changePageView()", true);
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "save", "saveImg()", true);
        }

        public void deleteImage()
        {
            string path = HttpContext.Current.Server.MapPath("~/Captures/") + login.Text + ".jpg";
            File.Delete(path);
        }

        protected void clearImage_Click(object sender, EventArgs e)
        {
            changeView("imageFromCamera");
            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "text", "runcamera()", true);
        }

        protected void login_TextChanged(object sender, EventArgs e)
        {
        }
        protected void pomButton_Click(object sender, EventArgs e)
        {
        }
        public void addToStats(string status,string przyczyna)
        {
            try
            {
                Polaczenie = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
                SqlConnection sqln = new SqlConnection(Polaczenie);
                sqln.Open();
                DateTime dateTime = DateTime.Now;
                SqlCommand cmdn = new SqlCommand("INSERT INTO [stats] VALUES (@login,@status,@data,@przyczyna)", sqln);
                cmdn.Parameters.AddWithValue("@login", login.Text);
                cmdn.Parameters.AddWithValue("@status", status);
                cmdn.Parameters.AddWithValue("@data", dateTime);
                cmdn.Parameters.AddWithValue("@przyczyna", przyczyna);
                cmdn.ExecuteNonQuery();
                sqln.Close();
            }
            catch
            {
            }
        }
        protected void password_TextChanged(object sender, EventArgs e)
        {

        }
        protected void zaloguj_Click(object sender, EventArgs e)
        {
            Session["user"] = login.Text;
            Logowanie();
        }
        public void Logowanie()
        {
            bool flag = false;
                String Polaczenie;
                Polaczenie = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
                SqlConnection sql = new SqlConnection(Polaczenie);
                sql.Open();
                SqlCommand cmd = new SqlCommand("select * from [users] WHERE login=@login", sql);
                cmd.Parameters.AddWithValue("@login", login.Text.Trim());
                SqlDataReader mdr = cmd.ExecuteReader();
                if(mdr.Read())
                {
                flag = true;
                if (mdr[1].ToString()==password.Text)
                {
                    if(mdr[3].ToString()!="")
                    {
                        informations.Text = "Wypełnij pozostałe pola";
                        informations.Visible = true;
                        startFaceRecognition.Checked = true;
                        zaloguj.Visible = false;
                        imageFromDb = mdr[3].ToString();
                        sql.Close();
                        Task.Run(async () => { keyFromDb = await FaceApi.FaceDetection(Convert.FromBase64String(imageFromDb)); }).Wait();
                        changeView("startFaceRecognition");
                    }
                    else
                    {
                        Session["user"] = login.Text;
                        sql.Close();
                        addToStats("Pomyślne", "Nie dotyczy");
                        auth();
                    }
                }
                else
                {
                    sql.Close();
                    informations.Visible = true;
                    informations.Text = "Nieprawidłowy login lub hasło!";
                    addToStats("Niepomyślne", "Błędne hasło");
                }
                }
                if(!flag)
                {
                informations.Visible = true;
                informations.Text = "Nieprawidłowy login lub hasło!";
                }
        }
        public void auth ()
        {
                HttpCookie mycookie = new HttpCookie("Login");
                mycookie["login"] = login.Text;
                mycookie.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(mycookie);
                Response.Redirect("~/Statystyki.aspx/");
        }
    }
}