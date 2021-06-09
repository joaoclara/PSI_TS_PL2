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
            
        }

        private string ComunicarServidor()
        {
            var publicKey = rSA.ToXmlString(false); // chave publica

            var publicK = protocoloSI.Make(ProtocolSICmdType.PUBLIC_KEY, publicKey);

            networkStream.Write(publicK, 0, publicK.Length);

            int bytesRead = networkStream.Read(protocoloSI.Buffer, 0, protocoloSI.Buffer.Length);

            var symetricKey = protocoloSI.GetStringFromData();
                      
            return symetricKey;
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            var symetricKey = ComunicarServidor();

            var pass = txtPassword.Text;
            var user = txtUtilizador.Text;

            var enctypted = Cifrar(pass, user, symetricKey);

            EnviarLogin(enctypted);
        }

        public string Cifrar(string pass, string user, string symetricKey)
        {
            var symetricKeyBytes = Encoding.UTF8.GetBytes(symetricKey);

            RSAParameters RSAParams = new RSAParameters
            {
                Modulus = symetricKeyBytes,
                Exponent = new byte[] { 1, 0, 1 }
            };

            rSA.ImportParameters(RSAParams);

            // 1º - Converter dados para byte[]
            byte[] dados = Encoding.UTF8.GetBytes(pass+"+"+user);

            // 2º - Cifrar os dados e guardá-los numa variável
            byte[] dadosEnc = rSA.Encrypt(dados, true);

            // 3º - Apresentar os dados em Base64
            var encrypted = Convert.ToBase64String(dadosEnc);

            return encrypted;
        }

        private void EnviarLogin(string encrypted)
        {
                byte[] passBytes = protocoloSI.Make(ProtocolSICmdType.USER_OPTION_1, encrypted);

                networkStream.Write(passBytes, 0, passBytes.Length);

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
                            Form1 form1 = new Form1();
                            form1.Show();                           
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
    }
}
