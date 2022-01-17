using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace ClientDetails
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] skipSD = Convert.ToString(ConfigurationManager.AppSettings["skip"]).Split(',');
            string conSqlMain = string.Empty;
            //string conSqlCentral = string.Empty;
            string subDomain = string.Empty;
            for (int i = 1; i < 10; i++)
            {

                try
                {
                    conSqlMain = Convert.ToString(ConfigurationManager.ConnectionStrings["ConnectionString" + i]);
                    DataSet clientsData = new DataSet();
                    using (SqlConnection cnMain = new SqlConnection(conSqlMain))
                    {
                        SqlDataAdapter da = new SqlDataAdapter("select * from clientdetails", cnMain);
                        da.Fill(clientsData);
                    }
                    SqlConnection connection = new SqlConnection("uid=sa;pwd=Ide@123;database=AB;server=AJAYBHARATH\\SQLEXPRESS");
                    SqlCommand sqlCommand = new SqlCommand("proc_ClientData", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter Name, subdomain, Domain, APIListener, MQTTListenerTopic, portalUrl;
                    for (int cd = 0; cd < clientsData.Tables[0].Rows.Count; cd++)
                    {
                        //url:https://(subdomain)MarkEn01.(domain)subzeroiot.com
                        connection.Open();
                        sqlCommand.Parameters.Clear();
                        subDomain = Convert.ToString(clientsData.Tables[0].Rows[cd]["DomainName"]);
                        if (Array.IndexOf(skipSD, subDomain) == -1)
                        {
                            Name = new SqlParameter("Name", SqlDbType.VarChar);
                            Name.Value = clientsData.Tables[0].Rows[cd]["ClientName"];
                            sqlCommand.Parameters.Add(Name);
                            subdomain = new SqlParameter("Subdomain", SqlDbType.VarChar);
                            //subdomain.Value = clientsData.Tables[0].Rows[cd]["DomainName"];
                            //sqlCommand.Parameters.Add(subdomain);
                            if (subDomain.ToString() == "vignaninstruments")
                            {
                                subdomain.Value = "web";
                                sqlCommand.Parameters.Add(subdomain);
                            }
                            else
                            {
                                subdomain.Value = clientsData.Tables[0].Rows[cd]["DomainName"];
                                sqlCommand.Parameters.Add(subdomain);
                            }
                            Domain = new SqlParameter("domain", SqlDbType.VarChar);
                            Domain.Value = clientsData.Tables[0].Rows[cd]["IoTDomain"];
                            sqlCommand.Parameters.Add(Domain);
                            APIListener = new SqlParameter("APIListener", SqlDbType.VarChar);
                            APIListener.Value = clientsData.Tables[0].Rows[cd]["ListenerURL"];
                            sqlCommand.Parameters.Add(APIListener);
                            MQTTListenerTopic = new SqlParameter("MQTTListenerTopic", SqlDbType.VarChar);
                            MQTTListenerTopic.Value = clientsData.Tables[0].Rows[cd]["mqtt_topic"];
                            sqlCommand.Parameters.Add(MQTTListenerTopic);
                            portalUrl = new SqlParameter("portalurl", SqlDbType.VarChar);
                            portalUrl.Value = clientsData.Tables[0].Rows[cd]["DomainURL"];
                            sqlCommand.Parameters.Add(portalUrl);
                            sqlCommand.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                }
                catch (Exception ex)
                {
                    ex = null;
                    
                    break;
                }
            }
        }
    }
}
