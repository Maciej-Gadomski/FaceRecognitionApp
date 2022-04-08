using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Services;
namespace FaceRecognitonApp
{
    public partial class Rejestracja : System.Web.UI.Page
    {
        string Polaczenie;
        public static string wynik = "";
        public static String temp="";
        protected void Page_Load(object sender, EventArgs e)
        {
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            loginValidator.Validate();
            passwordValidator.Validate();
            emailValidator.Validate();
            ComparePasswords.Validate();
            loginValidator.ErrorMessage = "Login musi składać się z minimum 4 znaków";
            string Password = password.Text;
            password.Attributes.Add("value", Password);
            string PasswordConfirm = passwordConfirm.Text;
            passwordConfirm.Attributes.Add("value", PasswordConfirm);
            uploadImage.Attributes["onchange"] = "UploadFile(this)";
            validation();
        }
        [WebMethod]
        public static bool SaveCapturedImage(string data)
        {
            try
            {
                string fileName = (string)HttpContext.Current.Session["user"];
                //Convert Base64 Encoded string to Byte Array.
                byte[] imageBytes = Convert.FromBase64String(data.Split(',')[1]);

                //Save the Byte Array as Image File.
                string filePath = HttpContext.Current.Server.MapPath(string.Format("/Captures/{0}.jpg", fileName));
                File.WriteAllBytes(filePath, imageBytes);
                return true;
            }
            catch
            {
                return false;
            }
        }
        protected void confirmButton_Click(object sender, EventArgs e)
        {
                string path;
            if (imageSource.SelectedValue == "file")
            {
                if (FaceApi.CheckFileType(uploadImage.FileName))
                {
                    byte[] imageDatabase;
                    Stream s = uploadImage.PostedFile.InputStream;
                    BinaryReader br = new BinaryReader(s);
                    imageDatabase = br.ReadBytes((int)s.Length);
                    Task.Run(async () => { wynik = await FaceApi.FaceDetection(imageDatabase); }).Wait();
                    temp = Convert.ToBase64String(imageDatabase);
                    checkRes();
                }
                else
                {
                    pictureCheckResult.Text = "Nieprawidłowy format pliku!";
                    pictureCheckResult.Attributes.CssStyle.Add("color", "#D00909");
                }
            }
            else
            {
                path = HttpContext.Current.Server.MapPath("~/Captures/") + login.Text + ".jpg";
                Task.Run(async () => { wynik = await FaceApi.FaceDetection(FaceApi.ReadAllBytes(path)); }).Wait();
                temp = Convert.ToBase64String(FaceApi.ReadAllBytes(path));
                deleteImage();
                checkRes();
            }
                changeView("confirmButton");
        }

        private void checkRes()
        {
            if (wynik == "[]")
            {
                pictureCheckResult.Text = "Twoje zdjęcie nie zawiera twarzy";
                pictureCheckResult.Attributes.CssStyle.Add("color", "#D00909");
            }
            else
            {
                pictureCheckResult.Text = "Zdjęcie jest prawidłowe - naciśnij przycisk zarejestruj";
                zarejestruj.Enabled = true;
                zarejestruj.Visible = true;
                pictureCheckResult.Attributes.CssStyle.Add("color", "#0E3819");
            }
        }

        protected void zarejestruj_Click(object sender, EventArgs e)
        {
            if (wynik == "[]")
            {
                pictureCheckResult.Text = "Twoje zdjęcie nie zawiera twarzy";
            }
            else
            {
                if (validation())
                {
                    addToDatabase();
                    Response.Redirect("~/Homepage.aspx/");
                }
            }
            
        }
        /// <summary>
        /// check the availability of the login 
        /// </summary>
        /// <param name="login">user login</param>
        /// <returns>true when login is available </returns>
        bool checkLogin()
        {
            string name = login.Text;
            try
            {
                Polaczenie = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
                SqlConnection sql = new SqlConnection(Polaczenie);
                sql.Open();
                SqlCommand cmd = new SqlCommand("select [login] from [users]");
                cmd.Connection = sql;
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    if (rd[0].ToString() == name)
                        return false;
                }
                sql.Close();
            }
            catch { }
            return true;
        }

        public void addToDatabase()
        {
            Polaczenie = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
            SqlConnection sql = new SqlConnection(Polaczenie);
            sql.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO [users] VALUES (@login,@password,@email,@image)", sql);
            cmd.Parameters.AddWithValue("@login", login.Text);
            cmd.Parameters.AddWithValue("@password", password.Text);
            cmd.Parameters.AddWithValue("@email", email.Text);
            cmd.Parameters.AddWithValue("@image", temp);
            cmd.ExecuteNonQuery();
            sql.Close();
        }

        public void changeView(string view)
        {
            
            if (view=="startFaceRecognition" || view == "stopFaceRecognition" || view == "confirmButton" )
            {
                imageSource.Visible = false;
                login.Enabled = false;

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
                zarejestruj.Enabled = true;
                zarejestruj.Visible = true;
            }
            if (view == "startFaceRecognition")
            {
      
                imageSource.Visible = true;
                imageSource.ClearSelection();
                zarejestruj.Enabled = false;
                zarejestruj.Visible = false;
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
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "changeview", "changePageView()", true);
                imageSource.Visible = true;
                confirmButton.Visible = true;
                uploadImage.Visible = true;
                capturedImage.Visible = false;
                makeImage.Visible = false;
                clearImage.Visible = false;
                pictureCheckResult.Visible = false;
                confirmButton.Attributes.CssStyle.Add("display", "none");

            }
            if (view =="imageFromCamera")
            {
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "changeview", "changePageView()", true);
                confirmButton.Visible = false;
                uploadImage.Visible = false;
                capturedImage.Visible = false;
                clearImage.Visible = false;
                imageSource.Visible = true;
                makeImage.Visible = true;
                pictureCheckResult.Visible = false;
                confirmButton.Attributes.CssStyle.Add("display", "block");
            }

        }

        protected void startFaceRecognition_CheckedChanged(object sender, EventArgs e)
        {
            if (startFaceRecognition.Checked)
            {
                changeView("startFaceRecognition");
                Session["user"] = login.Text;
                Session["password"] = password.Text;
                Session["passwordConfirm"] = passwordConfirm.Text;
            } 
            else
                changeView("stopFaceRecognition");
        }

        protected void imageSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            zarejestruj.Visible = false;
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
       public bool validation()
        {
            if (loginValidator.IsValid == true && emailValidator.IsValid==true && login.Text != "" && password.Text !="" && passwordConfirm.Text!="" && passwordValidator.IsValid==true && ComparePasswords.IsValid == true)
            {
                if (checkLogin())
                {
                    startFaceRecognition.Enabled = true;
                    zarejestruj.Enabled = true;
                    return true;
                }
                else
                {
                    loginValidator.IsValid = false;
                    loginValidator.ErrorMessage = "Podany login jest już zajęty";
                    return false;
                }
            }
            return false;
        }
        
        protected void login_TextChanged(object sender, EventArgs e)
        {
              if(!checkLogin())
            {
                loginValidator.IsValid = false;
                loginValidator.ErrorMessage = "Podany login jest już zajęty";
            }

        }
      
            protected void pomButton_Click(object sender, EventArgs e)
        {
        }

        protected void email_TextChanged(object sender, EventArgs e)
        {
            if(!validation())
            {
                changeView("stopFaceRecognition");
                startFaceRecognition.Checked = false;
                startFaceRecognition.Enabled = false;
            }

        }

        protected void password_TextChanged(object sender, EventArgs e)
        {
     
            if (!validation())
            {
                startFaceRecognition.Checked = false;
                startFaceRecognition.Enabled = false;
                changeView("stopFaceRecognition");
            }
        }

        protected void passwordConfirm_TextChanged(object sender, EventArgs e)
        {
    
            if (!validation())
            {
                changeView("stopFaceRecognition");
                startFaceRecognition.Checked = false;
                startFaceRecognition.Enabled = false;
            }
        }

        protected void log_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Homepage.aspx/");
        }

    }
}