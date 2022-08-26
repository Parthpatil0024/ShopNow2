using ShopNowBL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Configuration;
using System.Data.SqlClient;
using System.Xml.Linq;


namespace ShopNowBL.Repo
{
    public class UserRepo
    {


        public List<tblUser> listUsers()
        {
            List<tblUser> lstUsers = new List<tblUser>();
            using (DBTContext context = new DBTContext())
            {
                lstUsers = context.tblUsers.ToList();
               
               
            }
            return lstUsers;

        }

        public tblUser authenticateUser(string emailId, string password)
        {
            tblUser user;
            password = encrypt(password); 

            using (DBTContext context=new DBTContext())
            {
                 user = context.tblUsers.Where(x => x.EmailId == emailId && x.Password == password).FirstOrDefault();
            }
            return user;
        }


        public string encrypt(string encryptString)
        {
            string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (System.IO.MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }


        public tblUser getUserById(int Id)
        {
            tblUser objUser=new tblUser(); ;

            string conStr = ConfigurationManager.ConnectionStrings["DBTContext"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(conStr))
            {
                string query = "Select * from tblUsers where Id=" + Id;

                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    objUser.Id = Convert.ToInt32(reader["Id"]);
                    objUser.UserName = Convert.ToString(reader["UserName"]);
                    objUser.EmailId = Convert.ToString(reader["EmailId"]);
                    objUser.MobileNo = Convert.ToString(reader["MobileNo"]);
                    objUser.Password = Convert.ToString(reader["Password"]);
                    objUser.RoleId = Convert.ToInt32(reader["RoleId"]);
                    objUser.City = Convert.ToString(reader["City"]);
                    objUser.StoreId = Convert.ToInt32(reader["StoreId"]);
                    objUser.CreatedBy = Convert.ToInt32(reader["CreatedBy"]);
                    objUser.CreatedDate = Convert.ToDateTime(reader["CreatedDate"]);




                }

            }

            return objUser;
        }

        public bool addUser(tblUser objUser)
        {
            bool result = false;
            using (DBTContext context = new DBTContext())
            {

                objUser.CreatedDate = DateTime.Now;
                objUser.Password = encrypt(objUser.Password);



                context.tblUsers.AddOrUpdate(objUser);
                context.SaveChanges();
                result = true;
            }
            return result;
        }

        public bool saveUserAfterEdit(tblUser objUser)
        {
            bool result = false;
            string conStr = ConfigurationManager.ConnectionStrings["DBTContext"].ConnectionString;

            SqlConnection conn = new SqlConnection(conStr);
            string query = "update tblUsers set UserName= '" + objUser.UserName +
                "', EmailId='" +objUser.EmailId +
                "' , MobileNo='" + objUser.MobileNo +
                "', Password= '" + Convert.ToString(objUser.Password) +
                "', RoleId=" + objUser.RoleId +
                ", City= '" + objUser.City +
                "', StoreId=" + objUser.StoreId +
                ", CreatedBy= '" + objUser.CreatedBy +
                "', CreatedDate= '" + objUser.CreatedDate +
                "' where Id =" + objUser.Id;

            SqlCommand comm = new SqlCommand(query, conn);  
             conn.Open();
            try
            {
                comm.ExecuteNonQuery();
                result= true;
            }
            catch (Exception)
            {
                result = false;
            }
            finally
            {
                conn.Close();
            }
            return result;
        }

        public bool deleteUser(int id)
        {
            bool result = false;
            using (DBTContext context = new DBTContext())
            {
                tblUser user = context.tblUsers.Where(x => x.Id == id).FirstOrDefault();
                context.tblUsers.Remove(user);
                context.SaveChanges();
                result = true;
            }
            return result;

        }

        public tblUser VerifyEmail(string EmailId)
        {
            tblUser user;
            using (DBTContext context = new DBTContext())
            {
                user = context.tblUsers.Where(u => u.EmailId == EmailId).FirstOrDefault();
            }
            return user;
        }

        public bool SaveOTP(tblOTP newOtp)
        {
            bool result = false;
            using (DBTContext context = new DBTContext())
            {
                context.tblOTPs.AddOrUpdate(newOtp);
                context.SaveChanges();
                result = true;
            }
            return (result);

        }
        public tblCustomer findCustomerById(int id)
        {

            tblCustomer customer;
            using (DBTContext context = new DBTContext())
            {
                context.Configuration.LazyLoadingEnabled = false;
                customer = context.tblCustomers.Where(x => x.Id == id).FirstOrDefault();
            }
            return customer;
        }

        public tblOTP getOtpByEmail(string EmailId)
        {
            tblOTP newOtp;
            using (DBTContext context = new DBTContext())
            {
                newOtp = context.tblOTPs.Where(x => x.EmailId == EmailId).OrderByDescending(x => x.Created_DateTime).FirstOrDefault();
            }

            return newOtp;
        }

    }
}
