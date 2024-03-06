using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace FTP
{
    public partial class Form1 : Form
    {
        FtpWebRequest _request;
        string _username;
        string _password;
        string _ftpServer;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetFileList_Click(object sender, EventArgs e)
        {
            string path = "";
            var ncList = GetProgramList(path);
        }

        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            string path = "";
            var ncList = GetProgramList(path);
            if (ncList.Count > 0)
            {
                try
                {
                    foreach (var prog in ncList)
                    {
                        string deletePath = "ftp://" + _ftpServer + "//" + prog;
                        _request = (FtpWebRequest)WebRequest.Create(deletePath);
                        _request.Credentials = new NetworkCredential(_username, _password);
                        _request.UseBinary = true;
                        _request.KeepAlive = false;
                        _request.Method = WebRequestMethods.Ftp.DeleteFile;
                        var response = (FtpWebResponse)_request.GetResponse();
                        response.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("刪除檔案出錯：" + ex.Message);
                }
            }
        }
        private void btnUploadFile_Click(object sender, EventArgs e)
        {
            string filePath = "";
            UploadProgram(filePath);
        }
        List<string> GetProgramList(string path)
        {
            _ftpServer = txtIP.Text;
            List<string> ncList = new List<string>();
            string filePath = "ftp://" + _ftpServer + "//";
            _request = (FtpWebRequest)WebRequest.Create(filePath);
            _request.Credentials = new NetworkCredential(_username, _password);
            _request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            using (WebResponse response = _request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {

                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        if (!line.Contains("<DIR>"))
                        {
                            string msg = line.Substring(39).Trim();
                            ncList.Add(msg);
                        }
                        line = reader.ReadLine();
                    }
                }
            }
            return ncList;
        }
        void UploadProgram(string filePath)
        {
            string progName = Path.GetFileName(filePath);
            string path = "ftp://" + _ftpServer + "//" + progName;
            _request = (FtpWebRequest)WebRequest.Create(path);
            _request.Credentials = new NetworkCredential(_username, _password);
            _request.Method = WebRequestMethods.Ftp.UploadFile;
            StreamReader sourceStream = new StreamReader(filePath);
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            _request.ContentLength = fileContents.Length;
            Stream requestStream = _request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();
            using (var response = (FtpWebResponse)_request.GetResponse())
            {
                response.Close();
            }
        }

        
    }
}
