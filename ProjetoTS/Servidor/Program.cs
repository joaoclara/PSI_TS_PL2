using EI.SI;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Security.Cryptography;


namespace Servidor
{
    class Program
    {
        private const int SALTSIZE = 8;
        private const int PORT = 10000;

        private static ProtocolSI protocolSI = new ProtocolSI();
        private static RSACryptoServiceProvider rSA = new RSACryptoServiceProvider();

        private static string Decifrar(string message)
        {
            // 1º - Converter dados de Base64 para byte[]
            byte[] dados = Convert.FromBase64String(message);
            // 2º - Decifrar os dados e guardá-los numa variável
            byte[] dadosDec = rSA.Decrypt(dados, true);
            // 3º - Apresentar os dados
            var decrypted = Encoding.UTF8.GetString(dadosDec);

            return decrypted;
        }

        public static void Main(string[] args)
        {         
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            TcpListener tcpListener = new TcpListener(endPoint);

            tcpListener.Start();
            Console.WriteLine("Server ready");
            int clientCounter = 0;

            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();

                clientCounter++;

                Console.WriteLine("Client {0} conected ", clientCounter);

                ClientHandler clientHandler = new ClientHandler(tcpClient, clientCounter);
                clientHandler.Handle();
            }

        }
      
        class ClientHandler
        {
            TcpClient tcpClient;

            private int clientID;

            public ClientHandler(TcpClient tcpClient, int clientID)
            {
                this.tcpClient = tcpClient;                                         //Construtor da Classe
                this.clientID = clientID;
            }

            public void Handle()
            {
                Thread thread = new Thread(threadHandler);                      //Instanciar e Inicar a thread
                thread.Start();
            }

            private void threadHandler()                                            //Metodo que permite controlar o envio de mensagens ou sair da session 
            {
                NetworkStream networkStream = this.tcpClient.GetStream();

                ProtocolSI protocoloSI = new ProtocolSI();

                while (protocoloSI.GetCmdType() != ProtocolSICmdType.EOT)
                {
                    int bytesRead = networkStream.Read(protocoloSI.Buffer, 0, protocoloSI.Buffer.Length);

                    byte[] ack;

                    switch (protocoloSI.GetCmdType())
                    {
                        case ProtocolSICmdType.PUBLIC_KEY:
                            Console.WriteLine("Public key");
                            var clientPublicKey = protocoloSI.GetStringFromData();
                            break;

                        case ProtocolSICmdType.DATA:
                            Console.WriteLine("Client" + clientID + ": " + protocoloSI.GetStringFromData());                //Enviar as mensagens
                            ack = protocoloSI.Make(ProtocolSICmdType.ACK);

                            var x = Convert.ToBase64String(ack);
                            Console.WriteLine(x);
                            networkStream.Write(ack, 0, ack.Length);
                            break;

                        case ProtocolSICmdType.EOT:
                            Console.WriteLine("Client {0} terminated session ", clientID);
                            ack = protocoloSI.Make(ProtocolSICmdType.ACK);

                            //Sair da sesion
                            networkStream.Write(ack, 0, ack.Length);
                            break;

                        case ProtocolSICmdType.USER_OPTION_1:

                            Console.WriteLine("User option 1");
                            var userPass = protocoloSI.GetStringFromData();
                            var userName = protocoloSI.GetStringFromData();

                            User user = new User(userPass, userName);
                            var login = user.VerifyLogin(userPass, userName);

                            
                            break;
                        case ProtocolSICmdType.USER_OPTION_2:
                            Console.WriteLine("User option 2");
                            
                            break;
                    }
                }
                networkStream.Close();
                tcpClient.Close();
            }
        }
    }

    
    class User
    {
        private const int NUMBER_OF_ITERATIONS = 1000;
        private const int SALTSIZE = 8;
        private string pass;
        private string user;

        public User(string pass, string user )
        {
            this.user = user;
            this.pass = pass;
        }
        public bool VerifyLogin(string username, string password)
        {
            SqlConnection conn = null;
            try
            {
                // Configurar ligação à Base de Dados
                conn = new SqlConnection();
                conn.ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\IPL\OneDrive - IPLeiria\TP\VS\ProjetoTS\ProjetoTS\Users.mdf;Integrated Security=True");

                // Abrir ligação à Base de Dados
                conn.Open();

                // Declaração do comando SQL
                String sql = "SELECT * FROM Users WHERE Username = @username";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;

                // Declaração dos parâmetros do comando SQL
                SqlParameter param = new SqlParameter("@username", username);

                // Introduzir valor ao parâmentro registado no comando SQL
                cmd.Parameters.Add(param);

                // Associar ligação à Base de Dados ao comando a ser executado
                cmd.Connection = conn;

                // Executar comando SQL
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    throw new Exception("Error while trying to access an user");
                }

                // Ler resultado da pesquisa
                reader.Read();

                // Obter Hash (password + salt)
                byte[] saltedPasswordHashStored = (byte[])reader["SaltedPasswordHash"];

                // Obter salt
                byte[] saltStored = (byte[])reader["Salt"];

                conn.Close();

                //TODO: verificar se a password na base de dados 

                byte[] hash = GenerateSaltedHash(Encoding.UTF8.GetBytes(password), saltStored);

                return saltedPasswordHashStored.SequenceEqual(hash);
            }
            catch (Exception e)
            {
                //MessageBox.Show("An error occurred: " + e.Message);
                return false;
            }
        }
        private void Register(string username, byte[] saltedPasswordHash, byte[] salt)
        {
            SqlConnection conn = null;
            try
            {
                // Configurar ligação à Base de Dados
                conn = new SqlConnection();
                conn.ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\IPL\OneDrive - IPLeiria\TP\VS\ProjetoTS\ProjetoTS\Users.mdf;Integrated Security=True");

                // Abrir ligação à Base de Dados
                conn.Open();

                // Declaração dos parâmetros do comando SQL
                SqlParameter paramUsername = new SqlParameter("@username", username);
                SqlParameter paramPassHash = new SqlParameter("@saltedPasswordHash", saltedPasswordHash);
                SqlParameter paramSalt = new SqlParameter("@salt", salt);

                // Declaração do comando SQL
                String sql = "INSERT INTO Users (Username, SaltedPasswordHash, Salt) VALUES (@username,@saltedPasswordHash,@salt)";

                // Prepara comando SQL para ser executado na Base de Dados
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Introduzir valores aos parâmentros registados no comando SQL
                cmd.Parameters.Add(paramUsername);
                cmd.Parameters.Add(paramPassHash);
                cmd.Parameters.Add(paramSalt);

                // Executar comando SQL
                int lines = cmd.ExecuteNonQuery();

                // Fechar ligação
                conn.Close();

            }
            catch (Exception e)
            {
                throw new Exception("Error while inserting an user:" + e.Message);
            }
        }

        private static byte[] GenerateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return buff;
        }

        private static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(plainText, salt, NUMBER_OF_ITERATIONS);//repete 1000x
            return rfc2898.GetBytes(32);
        }
    }
       
}

