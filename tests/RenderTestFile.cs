using System.IO;
using bootstrap_email;
using NUnit.Framework;

namespace BootstrapEmailTests
{
    public class RenderTestFile
    {
        /// <summary>
        /// Test the length of the resulting File and save it. This requires you to manually check the file if it looks good
        /// </summary>
        [Test]
        public void Alert1()
        {
            var tmpResult=BootstrapEmail.Parse(FullTextSource);

            File.WriteAllText("C:/temp/BootstramEmailTestresult.html", tmpResult);

            Assert.AreEqual(tmpResult.Length, 24299);
        }


        public static string FullTextSource = @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html>
 <head>
   <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
 </head>
 <!-- Edit the code below this line -->
 <body class=""bg-light"">
<preview>Thank you for riding with Lyft!</preview>
<div class=""container"">
  <img class=""mx-auto mt-4 mb-3"" width=""42"" height=""30"" src=""https://s3.amazonaws.com/lyft.zimride.com/images/emails/logo/v2/logo_44@2x.png"" />

  <div class=""card mb-4"" style=""border-top: 5px solid #ff00bf;"">
    <div class=""card-body"">
      <img class=""mx-auto"" width=""100"" height=""100"" src=""https://d3rlubkewr7oe0.cloudfront.net/production/photos/320x200/809278619209872118_driver.jpg?w=100&h=100&dpr=2&fit=crop&mask=ellipse&crop=faces&faceindex=1&fm=png"" />
      <h4 class=""text-center"">Thanks for riding with Damon!</h4>
      <h5 class=""text-muted text-center"">September 21, 2017 at 6:54 PM</h5>

      <hr/>

      <h5 class=""text-center""><strong>Ride Details</strong></h5>
      <table class=""table"">
        <tbody>
          <tr>
            <td style=""border-top: 0;"">Lyft fare (2.62mi, 13m 10s)</td>
            <td style=""border-top: 0;"" class=""text-right"">$11.87</td>
          </tr>
          <tr>
            <td>
              <img class=""pr-2 float-left"" height=""20"" width=""32"" src=""https://s3.amazonaws.com/lyft.zimride.com/images/emails/credit_icons/cc_apple_pay.png""/>Apple Pay (MasterCard)
            </td>
            <td>
              <h4 class=""text-right""><strong>$11.87</strong></h4>
            </td>
          </tr>
        </tbody>
      </table>

      <img width=""556"" height=""322"" class=""img-fluid"" src=""https://api.lyft.com/v1/staticmap/1046813999679082200?hash=a7d395982bcc1f3b653ac0e6db15573e5c7e9da30bf62d2f83d77fb7d9426214&template=receipt&version=v2""/>
    </div>
  </div>

  <div class=""card w-100 mb-4"">
    <div class=""card-body"">
      <img width=""50"" height=""50"" class=""mx-auto"" src=""https://s3.amazonaws.com/lyft.zimride.com/images/emails/enterprise/briefcase_dark_large.png"">
      <h4 class=""text-center"">Make expensing business rides easy</h4>
      <p class=""text-center"">Enable business profile on Lyft to make expensing rides quick and easy.</p>
      <a class=""btn btn-primary btn-lg mx-auto mt-2"" href=""https://lyft.com"">Get Business Profile</a>
    </div>
  </div>

  <div class=""text-center text-muted"">Pricing FAQ � Help Center</div>
  <div class=""text-center text-muted"">Receipt #qwertyuiopasdfghjklzxcvbnm</div>
  <div class=""text-center text-muted mb-4"">Map data � OpenStreetMap contributors</div>

  <table class=""table-unstyled text-muted mb-4"">
    <tbody>
      <tr>
        <td>� Lyft 2017</td>
        <td>
          <img class=""float-right pl-2"" width=""20"" height=""20"" src=""https://s3.amazonaws.com/lyft.zimride.com/images/emails/social/v2/facebook@2x.png""/>
          <img class=""float-right pl-2"" width=""20"" height=""20"" src=""https://s3.amazonaws.com/lyft.zimride.com/images/emails/social/v2/twitter@2x.png""/>
          <img class=""float-right"" width=""20"" height=""20"" src=""https://s3.amazonaws.com/lyft.zimride.com/images/emails/social/v2/instagram@2x.png""/>
        </td>
      </tr>
      <tr>
        <td>548 Market St #68514</td>
        <td class=""text-right"">Work at Lyft</td>
      </tr>
      <tr>
        <td>San Francisco, CA 94104</td>
        <td class=""text-right"">Become a Driver</td>
      </tr>
    </tbody>
  </table>

</div>

 </body>
</html>";
    }
}