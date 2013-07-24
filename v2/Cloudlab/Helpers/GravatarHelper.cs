namespace Cloudlab.Helpers
{
    using System.Text;
    using System.Web.Mvc;

    public static class GravatarHelper
    {
        public static string GravatarAvatarUrl(this HtmlHelper helper, string email, int size)
        {
            string md5 = email.ToLowerInvariant().MD5();
            return string.Format(
                "https://secure.gravatar.com/avatar/{0}?s={1}&r=pg&d=mm",
                md5.ToLowerInvariant(),
                size
            );
        }

        public static string GravatarProfileUrl(this HtmlHelper helper, string email)
        {
            string md5 = email.ToLowerInvariant().MD5();
            return string.Format(
                "http://www.gravatar.com/{0}",
                md5.ToLowerInvariant()
            );
        }
        //

        public static string MD5(this string value)
        {
            System.Security.Cryptography.MD5 algorithm =
                System.Security.Cryptography.MD5.Create();

            byte[] data = Encoding.ASCII.GetBytes(value);
            data = algorithm.ComputeHash(data);
            string md5 = "";
            for (int i = 0; i < data.Length; i++)
            {
                md5 += data[i].ToString("x2").ToLower();
            }

            return md5;
        }
    }
}