using System.Text;

namespace WebTests;

public static class ReportHtmlGenerator
{
    public static string Generate(SuiteReport report)
    {
        var sb = new StringBuilder();

        sb.Append(@"<html><head>
            <style>
                body { font-family: Arial; padding: 20px; }
                table { width: 100%; border-collapse: collapse; }
                th, td { padding: 8px; border: 1px solid #ccc; }
                .pass { background: #d4edda; }
                .fail { background: #f8d7da;}
                .skip { background: #fff3cd;}
            </style>
        </head><body>");

        sb.Append($"<h1>Test Execution Report</h1>");
        sb.Append($"<p><b>Timestamp:</b> {report.Timestamp}</p>");
        sb.Append($"<p><b>Total Time:</b> {report.TotalTimeSeconds} sec</p>");
        sb.Append($"<p><b>OS:</b> {report.OS}</p>");
        sb.Append($"<p><b>.NET:</b> {report.DotNetVersion}</p>");

        sb.Append("<h2>Test Results</h2>");
        sb.Append("<table><tr><th>Name</th><th>Status</th><th>Duration (s)</th><th>Message</th></tr>");

        foreach (var r in report.TestResults)
        {
            // Normalize CSS class name so that Skipped maps to .skip
            var statusClass = r.Status.Equals("Skipped", StringComparison.OrdinalIgnoreCase)
                ? "skip"
                : r.Status.ToLower();

            sb.Append($@"
                <tr class='{statusClass}'>
                    <td>{r.Name}</td>
                    <td>{r.Status}</td>
                    <td>{r.Duration}</td>
                    <td>{r.Message}</td>
                </tr>");
        }

        sb.Append("</table></body></html>");
        return sb.ToString();
    }
}