using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using ZeekoUtilsPack.BCLExt;

namespace ZeekoUtilsPack.AspNetCore.Jwt
{
    public static class RsaUtils
    {
        /// <summary>
        /// 从本地文件中读取用来签发 Token 的 RSA Key
        /// </summary>
        /// <param name="filePath">存放密钥的文件夹路径</param>
        /// <param name="withPrivate"></param>
        /// <param name="keyParameters"></param>
        /// <returns></returns>
        public static bool TryGetKeyParameters(string filePath, bool withPrivate, out RSAParameters keyParameters)
        {
            string filename = withPrivate ? "key.json" : "key.public.json";
            filePath = Path.Combine(filePath, filename);
            keyParameters = default;
            if (File.Exists(filePath) == false) return false;
            var stored = JsonConvert.DeserializeObject<RsaParameterStorage>(File.ReadAllText(filePath));
            keyParameters = new RSAParameters
            {
                D = stored.D,
                DP = stored.DP,
                Exponent = stored.Exponent,
                DQ = stored.DQ,
                InverseQ = stored.InverseQ,
                Modulus = stored.Modulus,
                P = stored.P,
                Q = stored.Q
            };
            return true;
        }

        /// <summary>
        /// 生成并保存 RSA 公钥与私钥
        /// </summary>
        /// <param name="filePath">存放密钥的文件夹路径</param>
        /// <returns></returns>
        public static RSAParameters GenerateAndSaveKey(string filePath)
        {
            RSAParameters publicKeys, privateKeys;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    privateKeys = rsa.ExportParameters(true);
                    publicKeys = rsa.ExportParameters(false);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            File.WriteAllText(Path.Combine(filePath, "key.json"), privateKeys.ToJsonString());
            File.WriteAllText(Path.Combine(filePath, "key.public.json"), publicKeys.ToJsonString());
            return privateKeys;
        }

        class RsaParameterStorage
        {
            public byte[] D { get; set; }
            public byte[] DP { get; set; }
            public byte[] DQ { get; set; }
            public byte[] Exponent { get; set; }
            public byte[] InverseQ { get; set; }
            public byte[] Modulus { get; set; }
            public byte[] P { get; set; }
            public byte[] Q { get; set; }
        }

        static string ToJsonString(this RSAParameters parameters)
        {
            var content = new RsaParameterStorage
            {
                D = parameters.D,
                DP = parameters.DP,
                Exponent = parameters.Exponent,
                DQ = parameters.DQ,
                InverseQ = parameters.InverseQ,
                Modulus = parameters.Modulus,
                P = parameters.P,
                Q = parameters.Q
            };
            return JsonConvert.SerializeObject(content);
        }
    }
}
