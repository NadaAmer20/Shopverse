using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopverse.Application.Helpers
{
    public static class EmailTemplateBuilder
    {
        public static string BuildNewsletterHtml(string subject, string body)
        {
            return $@"
        <html>
        <head>
            <style>
                body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; color: #333; }}
                .container {{ max-width: 600px; margin: 20px auto; background: #fff; padding: 20px; border-radius: 8px; }}
                .header {{ font-size: 24px; font-weight: bold; margin-bottom: 10px; }}
                .footer {{ font-size: 12px; color: #888; margin-top: 20px; text-align: center; }}
                .content {{ margin-top: 10px; line-height: 1.5; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>{subject}</div>
                <div class='content'>{body}</div>
                <div class='footer'>© 2025 PNU Student Portal. All rights reserved.</div>
            </div>
        </body>
        </html>";
        }
    }

}
