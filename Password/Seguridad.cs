using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Password
{
    public class Seguridad
    {
        //public string cn = Configuracion.CadenaConexion;
        public static string PassKey = "AB";
        public string UserNameWindows = Environment.UserName;
        public string MachineName = Environment.MachineName;
        public string IpMachine = GetLocalIPAddress();
        public byte[] Encryption(string plainText)
        {
            TripleDES des = CreateDES(PassKey);
            ICryptoTransform ct = des.CreateEncryptor();
            byte[] input = Encoding.Unicode.GetBytes(plainText);
            return ct.TransformFinalBlock(input, 0, input.Length);
        }
        public static string Decryption(string cypherText)
        {
            byte[] b = Convert.FromBase64String(cypherText);
            TripleDES des = CreateDES(PassKey);
            ICryptoTransform ct = des.CreateDecryptor();
            byte[] output = ct.TransformFinalBlock(b, 0, b.Length);
            return Encoding.Unicode.GetString(output);
        }
        public static TripleDES CreateDES(string key)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            TripleDES des = new TripleDESCryptoServiceProvider();
            des.Key = md5.ComputeHash(Encoding.Unicode.GetBytes(key));
            des.IV = new byte[des.BlockSize / 8];
            return des;
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        //public void NewUser(string _Alias, string _Pass, string _Name, int _Group, string _Tag = "", string _Tag1 = "", string _Tag2 = "", string _Img = "0", string screenIni = "0", bool ChangePW = false, int UserId = 0)
        //{
        //    try
        //    {
        //        string cn = Configuracion.CadenaConexion;
        //        if (screenIni == "") screenIni = "0";
        //        if (_Img == "") _Img = "0";
        //        byte[] bufferEmailID = Encryption(_Alias.Trim() + _Pass.Trim());
        //        string _PassEcrip = Convert.ToBase64String(bufferEmailID);
        //        using (SqlConnection connection = new SqlConnection(cn))
        //        {
        //            connection.Open();
        //            SqlCommand cmd2 = new SqlCommand("Insert into Seg_user (UserName,UserAlias,GroupId,UserKey,UserStatus,Tag,Tag1,Tag2,ImageId,UserIniScreen,IsRDP,changepwinit) values ('" + _Name.Trim() + "','" + _Alias + "'," + _Group.ToString() + " ,'" + _PassEcrip + "',1,'" + _Tag.Trim() + "','" + _Tag1 + "','" + _Tag2 + "'," + _Img.ToString() + "," + screenIni.ToString() + ",0,'" + ChangePW.ToString() + "')", connection);
        //            cmd2.ExecuteNonQuery();
        //            connection.Close();
        //            Auditor(0, ((Inicio)Application.Current.MainWindow)._ProyectId, UserId, _Group, 0, -9, -1, -9, "Creo usuario:" + _Alias, "");
        //        }
        //    }
        //    catch (System.Exception _error)
        //    {
        //        MessageBox.Show(_error.Message.ToString());
        //    }
        //}
        //public void EditUser(string _Alias, string _Name, int _Group, bool _estado, string _Tag, string _Tag1, string _Tag2, string _Img, string screenIni, bool IsRDP, bool ChangePW, int UserId)
        //{
        //    try
        //    {
        //        string cn = Configuracion.CadenaConexion;
        //        using (SqlConnection connection = new SqlConnection(cn))
        //        {
        //            connection.Open();
        //            SqlCommand cmd3 = new SqlCommand("Update Seg_user set UserAlias='" + _Alias + "',UserName='" + _Name + "',GroupId=" + _Group.ToString() + ",UserStatus='" + _estado.ToString() + "',Tag='" + _Tag + "',Tag1='" + _Tag1 + "',Tag2='" + _Tag2 + "',ImageId=" + _Img.ToString() + ",UserIniScreen=" + screenIni.ToString() + ",IsRDP='" + IsRDP.ToString() + "',ChangePWInit='" + ChangePW.ToString() + "' where UserAlias='" + _Alias + "'", connection);
        //            cmd3.ExecuteNonQuery();
        //            connection.Close();
        //            Auditor(0, ((Inicio)Application.Current.MainWindow)._ProyectId, UserId, _Group, 0, -9, -1, -9, "Modifico usuario:" + _Alias, "");
        //        }
        //    }
        //    catch (System.Exception _error)
        //    {
        //        MessageBox.Show(_error.Message.ToString());
        //    }
        //}
        //public void Auditor(int Id_RowParent, int ProjectId, int UserId, int GroupId, int BusinessId, int ModulesId, int AccessId, int Id_RowReference, string Event, string EventError)
        //{
        //    //MessageBox.Show("Event:" + Event);
        //    string sqlAudcab = @"SET QUOTED_IDENTIFIER OFF;  Insert into AuditLog (Id_RowParent,ProyectId,UserId,GroupId,BusinessId,ModulesId,AccessId,Id_RowReference,UserWindows,MachineName) values (" + Id_RowParent.ToString() + "," + ProjectId.ToString() + "," + UserId.ToString().Trim() + "," + GroupId.ToString() + "," + BusinessId.ToString() + "," + ModulesId.ToString() + "," + AccessId.ToString() + "," + Id_RowReference.ToString() + ",'" + UserNameWindows + "','" + MachineName + "-" + IpMachine + "');DECLARE @NewIDco INT;SELECT @NewIDco = SCOPE_IDENTITY();";
        //    string sqlAudcue = null;
        //    StringReader xx1 = new StringReader(Event.ToString());
        //    //StringReader xx1Error = new StringReader(EventError.ToString());
        //    //sqlAudcue = sqlAudcue + @"insert into AuditLogCue (id_rowcab,Event,EventError) values (@NewIdco,'" + Event + "','" + EventError + "'); ";
        //    sqlAudcue = sqlAudcue + @"insert into AuditLogCue (id_rowcab,Event,EventError) values (@NewIdco,@Event,@EventError ); ";

        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(Configuracion.CadenaConexion))
        //        {
        //            using (SqlCommand cmd = connection.CreateCommand())
        //            {

        //                cmd.CommandText = sqlAudcab + sqlAudcue;
        //                cmd.Parameters.AddWithValue("@Event", Event.ToString().Trim());
        //                cmd.Parameters.AddWithValue("@EventError", EventError.ToString().Trim());
        //                connection.Open();
        //                cmd.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (System.Exception _error)
        //    {
        //        MessageBox.Show(_error.Message.ToString(), "Auditor", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //    return;

        //    //string aLine = null;
        //    //  while (true)
        //    //{
        //    //    aLine = xx1.ReadLine();
        //    //  if (aLine != null)
        //    //{
        //    //  sqlAudcue = sqlAudcue + @"insert into AuditLogCue (id_rowcab,Event,EventError) values (@NewIdco,'" + aLine + "','"+ EventError+"'); ";
        //    // }
        //    //else
        //    //{
        //    //  break;
        //    //}
        //    //}


        //    try
        //    {
        //        string cn = Configuracion.CadenaConexion;
        //        using (SqlConnection connection = new SqlConnection(cn))
        //        {
        //            connection.Open();
        //            SqlCommand cmd2 = new SqlCommand(sqlAudcab + sqlAudcue, connection);
        //            //SqlCommand cmd2 = new SqlCommand(@"SET QUOTED_IDENTIFIER OFF;  Insert into AuditLog (Id_RowParent,ProyectId,UserId,GroupId,BusinessId,ModulesId,AccessId,Id_RowReference,Event,EventError,UserWindows,MachineName) values (" + Id_RowParent.ToString() + "," + ProjectId.ToString() + "," + UserId.ToString().Trim() + "," + GroupId.ToString() + "," + BusinessId.ToString() + "," + ModulesId.ToString() + "," + AccessId.ToString() + "," + Id_RowReference.ToString() + ",'" + Event.Trim() + "','" + EventError.Trim() +  "','"+ UserNameWindows+"','"+MachineName+"-"+IpMachine+"')", connection);
        //            //MessageBox.Show(cmd2.CommandText.ToString());
        //            cmd2.ExecuteNonQuery();
        //            connection.Close();
        //        }
        //    }
        //    catch (System.Exception _error)
        //    {
        //        MessageBox.Show(_error.Message.ToString(), "Auditor", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}
        //public void ErrorLog(string tipo, string evento)
        //{
        //    string path = @"ErrorLog.Log";
        //    // This text is added only once to the file.
        //    if (!File.Exists(path))
        //    {
        //        // Create a file to write to.
        //        using (StreamWriter sw = File.CreateText(path))
        //        {
        //            //0 - Error/1 Auditoria /2=Evento;
        //            sw.WriteLine(Guid.NewGuid() + "|Evento |" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|" + ((Inicio)Application.Current.MainWindow)._UserAlias + "|" + UserNameWindows + "|" + MachineName + "|" + IpMachine + "|" + "Creo Archivo de Eventos" + "|");
        //        }
        //    }
        //    else
        //    {
        //        using (StreamWriter sw = File.AppendText(path))
        //        {
        //            //0 - Error/1 Auditoria /2=Evento;
        //            sw.WriteLine(Guid.NewGuid() + "|" + tipo + "|" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "|" + ((Inicio)Application.Current.MainWindow)._UserAlias + "|" + UserNameWindows + "|" + MachineName + "|" + IpMachine + "|" + evento + "|");
        //        }
        //    }

        //}


    }
}
