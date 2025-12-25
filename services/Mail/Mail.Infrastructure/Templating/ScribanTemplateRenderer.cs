using Mail.Application.Abstractions.Templating;
using Scriban;

namespace Mail.Infrastructure.Templating;

public sealed class ScribanTemplateRenderer : ITemplateRenderer
{
    public string Render(string layoutHtml, string contentHtml, object model)
    {
        var merged = layoutHtml.Replace("{{ content }}", contentHtml);

        var template = Template.Parse(merged);
        if (template.HasErrors)
            throw new InvalidOperationException(string.Join("\n", template.Messages.Select(m => m.Message)));

        return template.Render(model, member => member.Name);
    }
}
