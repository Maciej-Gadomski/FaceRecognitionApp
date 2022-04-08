<%@ Page Async="true" Language="C#" AutoEventWireup="true" CodeBehind="Homepage.aspx.cs" Inherits="FaceRecognitonApp.Homepage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
     <link href="/Style/StyleGadomski.css" rel="stylesheet" type="text/css"/>
    <title></title>
</head>
<body>
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script src="/js/WebCam.js" type="text/javascript"></script>
    <script type="text/javascript">
        function runcamera() {
            var counter = 0;
            console.log(counter);
            navigator.mediaDevices.getUserMedia({ video: true });
            navigator.permissions.query({
                name: 'camera'
            }).then(function (result) {
                if (result.state === 'granted' && counter < 1) {
                    //permission has already been granted, no prompt is shown
                    Webcam.set({
                        width: 440,
                        height: 330,
                        image_format: 'jpeg',
                        jpeg_quality: 90
                    });
                    Webcam.attach('#idwebcam');
                    document.getElementById("makeImage").style.visibility = "visible";
                    counter = 1;
                }
                else if (result.state === 'prompt') {
                    document.getElementById("makeImage").style.visibility = "hidden";
                    var now = new Date().getTime();
                    while (new Date().getTime() < now + 2000) { /* Do nothing */ }
                    if (counter === 0)
                        runcamera();
                }
                else if (result.state === 'denied') {
                    //permission has been denied
                    document.getElementById("startFaceRecognition").checked = false;
                    document.getElementById("imageSource").style.visibility = "hidden";
                    document.getElementById("makeImage").style.visibility = "hidden";
                    alert("Aby korzystać z tej funkcji musisz zezwolić na używanie kamery");
                    document.getElementById("pomButton").click();
                }
            });
        };

        function makeImage() {
            Webcam.snap(function (data_uri) {
                $("#capturedImage")[0].src = data_uri;
            })
        };
        function saveImg() {
            Webcam.snap(function (data_uri) {
                $("#capturedImage")[0].src = data_uri;
            })
            //"/Rejestracja.aspx/SaveCapturedImage",
            $.ajax({
                type: "POST",
                url: '<%=ResolveClientUrl("~/Rejestracja.aspx/SaveCapturedImage")%>',
               data: "{data: '" + $("#capturedImage")[0].src + "'}",
               contentType: "application/json; charset=utf-8",
               dataType: "json",
               success: function (r) {
               }
           });
        }

        function UploadFile(fileUpload) {
            if (fileUpload.value != '') {
                //document.getElementById("<%=pomButton.ClientID %>").click();
                //document.getElementById("<%=confirmButton.ClientID %>").setAttribute('display','');
                document.getElementById('confirmButton').style.display = 'block';
            }
        }
        function changePageView() {
            document.getElementById("divpom").className = "columnRegister"
        }
    </script>
    
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True">
                                </asp:ScriptManager>
        <div class="footer">
            <div class="menu">
           <asp:Button ID="zarejestruj" runat="server" Text="Zarejestruj"  Font-Size="Large" ForeColor="White"  CssClass="buttongreen" BorderStyle="Solid" OnClick="zarejestruj_Click" Font-Bold="True" />
                </div>
        </div>
    <div class="row">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
          <ContentTemplate>
    <div class="startRegister" id="divpom">
        <table>
            <tr>
                <td class="registerBoxes">
                    <asp:TextBox ID="login" runat="server" placeholder="Login" CssClass="ceil" BorderStyle="Solid" AutoPostBack="True" MaxLength="49" OnTextChanged="login_TextChanged" ToolTip="Login musi składać się z minimum 4 znaków"></asp:TextBox>
                </td>
                <td class="others">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="registerBoxes">
                    <asp:TextBox ID="password" runat="server" placeholder="Hasło" CssClass="ceil" BorderStyle="Solid" AutoPostBack="True" ToolTip="Hasło musi składać się z min. 8 znaków, zawierać małe i duże litery, cyfrę oraz znak specjalny" TextMode="Password" MaxLength="49" OnTextChanged="password_TextChanged"></asp:TextBox>
                </td>
                <td class="others">
                    &nbsp;</td>
            </tr>
         
            </tr>
            <td class="others" colspan="2">
                <asp:Label ID="informations" runat="server" Font-Size="Large" Visible="False"></asp:Label>
            </td>
            <tr>
                <td class="others" colspan="2">
                                        <asp:Button ID="zaloguj" runat="server" Text="Zaloguj" Font-Bold="True" Font-Size="X-Large" ForeColor="White" Height="70px" Width="200px" CssClass="buttongreen" BorderStyle="Solid" OnClick="zaloguj_Click" />
                    </td>

            </tr>
            <tr>
                <td class="others" colspan="2">
                      <asp:CheckBox ID="startFaceRecognition" runat="server" Text="Dodaj rozpoznawanie twarzy" AutoPostBack="True" OnCheckedChanged="startFaceRecognition_CheckedChanged" Enabled="False" Font-Bold="True" ForeColor="#0E3819" Width="100%" />
                </td>
            </tr>
        </table>
    </div>
    <div class="columnFace">
        <table>
            <tr>
                <td>
<asp:RadioButtonList ID="imageSource" runat="server" RepeatDirection="Horizontal" AutoPostBack="True" OnSelectedIndexChanged="imageSource_SelectedIndexChanged" Visible="False" Font-Bold="True" >
            <asp:ListItem Value="file">Wybierz zdjęcie z pliku</asp:ListItem>
            <asp:ListItem Value="camera">Użyj kamery</asp:ListItem>
        </asp:RadioButtonList>
                </td>
            </tr>
        <tr>
            <td>
<asp:FileUpload ID="uploadImage" runat="server" Visible="False" CssClass="margin" />
     <asp:Label ID="pictureCheckResult" runat="server" Visible="False"></asp:Label>
            </td>
        </tr>
<tr>
    <td>
 <div class="treebuttons">
                <div class="left">
                <asp:Button ID="makeImage" runat="server" Text="Zrób zdjęcie" OnClick="makeImage_Click" Visible="False" CssClass="buttongreen" Font-Size="Small" />
                </div>
                <div class="left">
                    <asp:Button ID="clearImage" runat="server" Text="Wyczyść" Visible="False" OnClick="clearImage_Click" CssClass="buttonred" Font-Size="Small" />
                </div>
                <div class="left">
                    <asp:Button ID="confirmButton" runat="server" Text="Zatwierdź i Zaloguj" Visible="False" OnClick="confirmButton_Click" CssClass="buttongreen" Font-Size="Small" />
                </div>
           </div>   
    </td>
</tr>
 <tr>
     <td>
<asp:Image ID="capturedImage" runat="server" Visible="False" Height="330px" Width="440px" />
            <div id="idwebcam">
            </div>
     </td>
 </tr>   
            </table>
    </div>
         <asp:Button ID="pomButton" runat="server" Text="Button" OnClick="pomButton_Click" />
          </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="confirmButton" />
            <asp:PostBackTrigger ControlID="pomButton" />
        </Triggers>
    </asp:UpdatePanel>

    </div>
    </form>
     
</body>
</html>
