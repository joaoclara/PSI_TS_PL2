using Cliente;
using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjetoTS
{
    public partial class Form1 : Form
    {
        private const int PORT = 10000;
        NetworkStream networkStream;
        ProtocolSI protocoloSI;                    
        TcpClient tcpClient;
        RSACryptoServiceProvider rSA;

        public Form1()
        {
            InitializeComponent();            
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            tcpClient = new TcpClient();
            tcpClient.Connect(endPoint);                                            //Instaciar as propriedades
            networkStream = tcpClient.GetStream();
            protocoloSI = new ProtocolSI();
            rSA = new RSACryptoServiceProvider();

        }

        // FEITO gerar chave assimetrica (chave publica + chave privada) 
        // FEITO enviar a chave publica do cliente 
        // FEITO servidor recebe a chave pulibca

        // FEITO servidor gera a chave simetrica 
        // FEITO servidor cifrar com a chave publica do cliente a chave simetrica e envia para o cliente
        // o cliente guarda a chave simetrica 


        //cifrar simetrica

        //decifrar chave privada

        //decifrar chave simetrica

        public string Cifrar(string message)
        {                
            // 1º - Converter dados para byte[]
            byte[] dados = Encoding.UTF8.GetBytes(message);
            // 2º - Cifrar os dados e guardá-los numa variável
            byte[] dadosEnc = rSA.Encrypt(dados, true);
            // 3º - Apresentar os dados em Base64
            var encrypted = Convert.ToBase64String(dadosEnc);

            return encrypted;
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            //string msg = Cifrar(rtxtMessage.Text);                                                      //Mensagem escrita pelo User
            string msg = rtxtMessage.Text;                                                      //Mensagem escrita pelo User
            rtxtMessage.Clear();

            byte[] packet = protocoloSI.Make(ProtocolSICmdType.DATA, msg);                     //Button que permite enviar mensagem, através do ProtocolSICmdType.DATA

            networkStream.Write(packet, 0, packet.Length);
        }

        private void btnLeaveSession_Click(object sender, EventArgs e)
        {
            byte[] eot = protocoloSI.Make(ProtocolSICmdType.EOT);

            networkStream.Write(eot, 0, eot.Length);

            networkStream.Read(protocoloSI.Buffer, 0, protocoloSI.Buffer.Length);               //Button que permite sair da session, através do ProtocolSICmdType.EOT

            networkStream.Close();

            tcpClient.Close();                          

            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
