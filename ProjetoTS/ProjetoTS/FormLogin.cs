using ProjetoTS;
using System;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using EI.SI;
using System.Security.Cryptography;

namespace Cliente
{
    public partial class FormLogin : Form
    {
        private const int PORT = 10000;

        NetworkStream networkStream;

        ProtocolSI protocoloSI;                     //Inicializar as propriedades

        TcpClient tcpClient;

        RSACryptoServiceProvider rSA;

        public FormLogin()
        {
            InitializeComponent();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            protocoloSI = new ProtocolSI();
            rSA = new RSACryptoServiceProvider();
            tcpClient = new TcpClient();
            tcpClient.Connect(endPoint);                                            //Instaciar as propriedades
            networkStream = tcpClient.GetStream();
        }
        private void FormLogin_Load(object sender, EventArgs e)
        {
            var publicKey = rSA.ToXmlString(false); // chave publica

            var publicK = protocoloSI.Make(ProtocolSICmdType.PUBLIC_KEY, publicKey);

            networkStream.Write(publicK, 0, publicK.Length);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            var pass = txtPassword.Text;
            var user = txtUtilizador.Text;
          
            EnviarLogin(pass, user);

        }
        private void EnviarLogin(string pass, string user)
        {
           
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);

                tcpClient = new TcpClient();

                tcpClient.Connect(endPoint);

                networkStream = tcpClient.GetStream();

                byte[] passBytes = protocoloSI.Make(ProtocolSICmdType.USER_OPTION_1, pass);

                byte[] userBytes = protocoloSI.Make(ProtocolSICmdType.USER_OPTION_1, user);

                networkStream.Write(passBytes, 0, passBytes.Length);

                networkStream.Write(userBytes, 0, userBytes.Length);

                while (protocoloSI.GetCmdType() != ProtocolSICmdType.EOT)
                {
                    int bytesRead = networkStream.Read(protocoloSI.Buffer, 0, protocoloSI.Buffer.Length);

                    byte[] ack;

                    switch (protocoloSI.GetCmdType())
                    {

                        case ProtocolSICmdType.USER_OPTION_3:
                            var msg = protocoloSI.GetStringFromData();
                            var login = Convert.ToBoolean(msg);
                            if (login == true)
                                MessageBox.Show("Login com Sucesso");
                            break;

                        case ProtocolSICmdType.SECRET_KEY:

                            break;


                    }
                }

                if (networkStream != null)
                {
                    networkStream.Close();
                }
                if (tcpClient != null)
                {
                    tcpClient.Close();
                }            
            
        }

        private void btnRegistar_Click(object sender, EventArgs e)
        {
            string pass = txtPassword.Text;
            string user = txtUtilizador.Text;

            protocoloSI.Make(ProtocolSICmdType.USER_OPTION_2, pass);                     //Button que permite enviar mensagem, através do ProtocolSICmdType.DATA
            protocoloSI.Make(ProtocolSICmdType.USER_OPTION_2, user);

            /*
            var pass = Encoding.UTF8.GetBytes(txtPassword.Text);

            var user = txtUtilizador.Text;

            var salt = GenerateSalt(SALTSIZE);

            var saltedHash = GenerateSaltedHash(pass, salt);

            Register(user, saltedHash, salt);*/
        }


        /*
        if (verifylogin(txtutilizador.text, txtpassword.text) == true)
        {
            MessageBox.Show("login com sucesso");

            Form1 formchat = new Form1();

            formchat.Show();
        }
        else
        {
            MessageBox.Show("login errado");
        }
        */
    }
    //Enviar as mensagens



    /*
     * 
     *       try
{            int bytesRead = 0;
    byte[] ack = new byte[tcpClient.ReceiveBufferSize];

    string response = Encoding.UTF8.GetString(ack, 0, bytesRead);


    if (response == "")
    {
        return "ERRO";
    }

    return null;

}
catch
    {
        return "ERRO - Excepção";
    }

finally
{
    */
}
